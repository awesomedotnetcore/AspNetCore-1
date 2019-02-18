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
    internal class RemoteUriHelper : UriHelperBase
    {
        private IJSRuntime _jsRuntime;

        public RemoteUriHelper()
        {
        }

        /// <summary>
        /// Creates a new <see cref="RemoteUriHelper"/>.
        /// </summary>
        /// <param name="jsRuntime"></param>
        public RemoteUriHelper(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Initializes the <see cref="RemoteUriHelper"/>.
        /// </summary>
        /// <param name="uriAbsolute">The absolute URI of the current page.</param>
        /// <param name="baseUriAbsolute">The absolute base URI of the current page.</param>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/> to use for interoperability.</param>
        public void Initialize(string uriAbsolute, string baseUriAbsolute)
        {
            SetAbsoluteBaseUri(baseUriAbsolute);
            SetAbsoluteUri(uriAbsolute);
            TriggerOnLocationChanged();
        }

        /// <summary>
        /// Initializes the <see cref="RemoteUriHelper"/>.
        /// </summary>
        /// <param name="uriAbsolute">The absolute URI of the current page.</param>
        /// <param name="baseUriAbsolute">The absolute base URI of the current page.</param>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/> to use for interoperability.</param>
        public void Initialize(string uriAbsolute, string baseUriAbsolute, IJSRuntime jsRuntime)
        {
            if (jsRuntime != null && _jsRuntime != jsRuntime)
            {
                throw new InvalidOperationException("JavaScript runtime already initialized.");
            }

            _jsRuntime = jsRuntime;

            Initialize(uriAbsolute, baseUriAbsolute);

            try
            {
                _jsRuntime.InvokeAsync<object>(
                    Interop.EnableNavigationInterception,
                    typeof(RemoteUriHelper).Assembly.GetName().Name,
                    nameof(NotifyLocationChanged));
            }
            catch
            {
            }
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

            var uriHelper = (RemoteUriHelper)circuit.Services.GetRequiredService<IUriHelper>();

            uriHelper.SetAbsoluteUri(uriAbsolute);
            uriHelper.TriggerOnLocationChanged();
        }

        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            _jsRuntime.InvokeAsync<object>(Interop.NavigateTo, uri, forceLoad);
        }
    }
}
