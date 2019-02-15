// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Components.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Interop = Microsoft.AspNetCore.Components.Browser.BrowserUriHelperInterop;

namespace Microsoft.AspNetCore.Components.Server.Circuits
{
    /// <summary>
    /// A Server-Side Blazor implementation of <see cref="IUriHelper"/>.
    /// </summary>
    public class CircuitUriHelper : RemoteUriHelper
    {
        public CircuitUriHelper()
        {
        }

        /// <summary>
        /// Creates a new <see cref="CircuitUriHelper"/>.
        /// </summary>
        /// <param name="jsRuntime"></param>
        public CircuitUriHelper(IJSRuntime jsRuntime) : base(jsRuntime)
        {
        }

        public override void Initialize(string uriAbsolute, string baseUriAbsolute)
        {
            base.Initialize(uriAbsolute, baseUriAbsolute);
            JsRuntime.InvokeAsync<object>(
                Interop.EnableNavigationInterception,
                typeof(RemoteUriHelper).Assembly.GetName().Name,
                nameof(NotifyLocationChanged));
        }

        /// <summary>
        /// For framework use only.
        /// </summary>
        [JSInvokable(nameof(NotifyLocationChanged))]
        public static void NotifyLocationChanged(string uriAbsolute)
        {
            var circuit = CircuitHost.Current;
            if (circuit == null)
            {
                var message = $"{nameof(NotifyLocationChanged)} called without a circuit.";
                throw new InvalidOperationException(message);
            }

            var uriHelper = (CircuitUriHelper)circuit.Services.GetRequiredService<IUriHelper>();

            uriHelper.SetAbsoluteUri(uriAbsolute);
            uriHelper.TriggerOnLocationChanged();
        }
    }
}
