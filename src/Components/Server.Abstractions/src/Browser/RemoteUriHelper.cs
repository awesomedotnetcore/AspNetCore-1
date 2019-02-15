// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Components.Services;
using Microsoft.JSInterop;
using Interop = Microsoft.AspNetCore.Components.Browser.BrowserUriHelperInterop;

namespace Microsoft.AspNetCore.Components.Server.Circuits
{
    /// <summary>
    /// A Server-Side Blazor implementation of <see cref="IUriHelper"/>.
    /// </summary>
    public class RemoteUriHelper : UriHelperBase
    {
        /// <summary>
        /// Creates a new <see cref="RemoteUriHelper"/>.
        /// </summary>
        public RemoteUriHelper()
        {
        }

        /// <summary>
        /// Creates a new <see cref="RemoteUriHelper"/>.
        /// </summary>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/> to use for interoperability.</param>
        public RemoteUriHelper(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        protected IJSRuntime JsRuntime { get; private set; }

        /// <summary>
        /// Initializes the <see cref="RemoteUriHelper"/>.
        /// </summary>
        /// <param name="uriAbsolute">The absolute URI of the current page.</param>
        /// <param name="baseUriAbsolute">The absolute base URI of the current page.</param>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/> to use for interoperability.</param>
        public void Initialize(string uriAbsolute, string baseUriAbsolute, IJSRuntime jSRuntime)
        {
            if (jSRuntime != null)
            {
                throw new InvalidOperationException("JavaScript runtime already initialized.");
            }

            JsRuntime = jSRuntime;

            Initialize(uriAbsolute, baseUriAbsolute);
        }

        /// <summary>
        /// Initializes the <see cref="RemoteUriHelper"/>.
        /// </summary>
        /// <param name="uriAbsolute">The absolute URI of the current page.</param>
        /// <param name="baseUriAbsolute">The absolute base URI of the current page.</param>
        public virtual void Initialize(string uriAbsolute, string baseUriAbsolute)
        {
            if (JsRuntime == null)
            {
                throw new InvalidOperationException("JavaScript runtime not initialized.");
            }

            SetAbsoluteBaseUri(baseUriAbsolute);
            SetAbsoluteUri(uriAbsolute);
            TriggerOnLocationChanged();
        }

        /// <inheritdoc />
        protected override void NavigateToCore(string uri, bool forceLoad)
        {
            if (JsRuntime == null)
            {
                throw new InvalidOperationException("JavaScript runtime not initialized.");
            }

            JsRuntime.InvokeAsync<object>(Interop.NavigateTo, uri, forceLoad);
        }
    }
}
