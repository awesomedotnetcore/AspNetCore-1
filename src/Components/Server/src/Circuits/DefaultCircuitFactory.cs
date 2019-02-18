// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components.Browser;
using Microsoft.AspNetCore.Components.Browser.Rendering;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Server.Builder;
using Microsoft.AspNetCore.Components.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace Microsoft.AspNetCore.Components.Server.Circuits
{
    internal class DefaultCircuitFactory : CircuitFactory
    {
        private readonly ConcurrentDictionary<string, CircuitHost> _trackedHosts =
            new ConcurrentDictionary<string, CircuitHost>();

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILoggerFactory _loggerFactory;

        public DefaultCircuitFactory(
            IServiceScopeFactory scopeFactory,
            ILoggerFactory loggerFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _loggerFactory = loggerFactory;
        }

        public override CircuitHost CreateCircuitHost(
            HttpContext httpContext,
            IClientProxy client,
            string uriAbsolute,
            string baseUriAbsolute)
        {
            var circuitId = GetCircuitId(httpContext);
            IJSRuntime jsRuntime;
            CircuitHost circuitHost;
            if (circuitId != null)
            {
                circuitHost = Untrack(circuitId);
                UpdateHost(circuitHost, client, uriAbsolute, baseUriAbsolute);
                jsRuntime = circuitHost.JSRuntime;
            }
            else
            {
                (jsRuntime, circuitHost) = CreateNewHost(client, uriAbsolute, baseUriAbsolute);
                SetCircuitContext(jsRuntime, circuitHost);

                Track(circuitHost);
                httpContext.Response.Cookies.Append("Circuits.ConnectionId", circuitHost.CircuitId);
            }

            return circuitHost;
        }

        private static void SetCircuitContext(IJSRuntime jsRuntime, CircuitHost circuitHost)
        {
            // Initialize per-circuit data that services need
#pragma warning disable CS0618 // Type or member is obsolete
            (circuitHost.Services.GetRequiredService<IJSRuntimeAccessor>() as DefaultJSRuntimeAccessor).JSRuntime = jsRuntime;
#pragma warning restore CS0618 // Type or member is obsolete
            (circuitHost.Services.GetRequiredService<ICircuitAccessor>() as DefaultCircuitAccessor).Circuit = circuitHost.Circuit;
        }

        private void UpdateHost(CircuitHost circuitHost, IClientProxy client, string uriAbsolute, string baseUriAbsolute)
        {
            circuitHost.Renderer.Invoke(() =>
            {
                var services = circuitHost.Services;
                var jsRuntime = GetJavaScriptRuntime(client, services);
                _ = GetUriHelper(jsRuntime, uriAbsolute, baseUriAbsolute, services);
                circuitHost.Client = client;
                SetCircuitContext(jsRuntime, circuitHost);
                circuitHost.Renderer.ResumeRendering(client);
            });
        }

        private (IJSRuntime, CircuitHost) CreateNewHost(IClientProxy client, string uriAbsolute, string baseUriAbsolute)
        {
            var scope = _scopeFactory.CreateScope();
            var services = scope.ServiceProvider;
            var jsRuntime = GetJavaScriptRuntime(client, services);
            var uriHelper = GetUriHelper(jsRuntime, uriAbsolute, baseUriAbsolute, services);
            var encoder = services.GetRequiredService<HtmlEncoder>();
            var options = services.GetRequiredService<IOptions<RazorComponentsOptions>>();
            var rendererRegistry = new RendererRegistry();
            var dispatcher = Renderer.CreateDefaultDispatcher();
            var renderer = new RemoteRenderer(
                services,
                rendererRegistry,
                jsRuntime,
                client,
                dispatcher,
                encoder,
                _loggerFactory.CreateLogger<RemoteRenderer>());

            var circuitHandlers = scope.ServiceProvider.GetServices<CircuitHandler>()
                .OrderBy(h => h.Order)
                .ToArray();

            var circuitHost = new CircuitHost(
                scope,
                client,
                rendererRegistry,
                renderer,
                dispatcher,
                options.Value,
                jsRuntime,
                circuitHandlers);

            return (jsRuntime, circuitHost);
        }

        private string GetCircuitId(HttpContext httpContext)
        {
            return httpContext.Request.Cookies["Circuits.ConnectionId"];
        }

        private void Track(CircuitHost circuitHost)
        {
            _trackedHosts.GetOrAdd(circuitHost.CircuitId, circuitHost);
        }

        private CircuitHost Untrack(string circuitId)
        {
            if (_trackedHosts.TryRemove(circuitId, out var circuitHost))
            {
                return circuitHost;
            }

            return null;
        }

        private static IUriHelper GetUriHelper(RemoteJSRuntime jsRuntime, string uriAbsolute, string baseUriAbsolute, IServiceProvider scope)
        {
            var helper = scope.GetRequiredService<IUriHelper>();
            var remoteUriHelper = helper as RemoteUriHelper;
            if (helper != null && remoteUriHelper == null)
            {
                throw new InvalidOperationException($"The '{typeof(IUriHelper).Name}' has been overrided with an unsuitable implementation.");
            }
            else
            {
                remoteUriHelper.Initialize(uriAbsolute, baseUriAbsolute, jsRuntime);
                return remoteUriHelper;
            }
        }

        private static RemoteJSRuntime GetJavaScriptRuntime(IClientProxy client, IServiceProvider scope)
        {
            var runtime = scope.GetRequiredService<IJSRuntime>();
            var jsRuntime = runtime as RemoteJSRuntime;
            if (runtime != null && jsRuntime == null)
            {
                throw new InvalidOperationException($"The '{typeof(IJSRuntime).Name}' has been overrided with an unsuitable implementation.");
            }
            else if (client != null)
            {
                jsRuntime.Initialize(client);
                return jsRuntime;
            }
            else
            {
                return jsRuntime;
            }
        }
    }
}
