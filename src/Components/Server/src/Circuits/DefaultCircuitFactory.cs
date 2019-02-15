// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.AspNetCore.Components.Browser;
using Microsoft.AspNetCore.Components.Browser.Rendering;
using Microsoft.AspNetCore.Components.Environment;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Server.Abstractions.Browser;
using Microsoft.AspNetCore.Components.Server.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Components.Server.Circuits
{
    internal class DefaultCircuitFactory : CircuitFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILoggerFactory _loggerFactory;

        public DefaultCircuitFactory(
            IServiceScopeFactory scopeFactory,
            ILoggerFactory loggerFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _loggerFactory = loggerFactory;
        }

        public override CircuitHost CreateCircuitHost(HttpContext httpContext, IClientProxy client)
        {
            var scope = _scopeFactory.CreateScope();
            var options = scope.ServiceProvider.GetRequiredService<IOptions<RazorComponentsOptions>>();
            var jsRuntimeBase = scope.ServiceProvider.GetRequiredService<ServerJSRuntimeBase>();
            var jSRuntime = new RemoteJSRuntime(client);
            jsRuntimeBase.Initialize(jSRuntime);
            var uriHelper = new CircuitUriHelper(jSRuntime);

            var rendererRegistry = new RendererRegistry();
            var dispatcher = Renderer.CreateDefaultDispatcher();
            var renderer = new RemoteRenderer(
                scope.ServiceProvider,
                rendererRegistry,
                jSRuntime,
                client,
                dispatcher,
                _loggerFactory.CreateLogger<RemoteRenderer>());

            var circuitHandlers = scope.ServiceProvider.GetServices<CircuitHandler>()
                .OrderBy(h => h.Order)
                .ToArray();

            var circuitHost = new CircuitHost(
                scope,
                client,
                rendererRegistry,
                renderer,
                options.Value,
                jsRuntimeBase,
                circuitHandlers);

            // Initialize per-circuit data that services need
#pragma warning disable CS0618 // Type or member is obsolete
            (circuitHost.Services.GetRequiredService<IJSRuntimeAccessor>() as DefaultJSRuntimeAccessor).JSRuntime = jsRuntimeBase;
#pragma warning restore CS0618 // Type or member is obsolete
            (circuitHost.Services.GetRequiredService<ICircuitAccessor>() as DefaultCircuitAccessor).Circuit = circuitHost.Circuit;

            return circuitHost;
        }
    }
}
