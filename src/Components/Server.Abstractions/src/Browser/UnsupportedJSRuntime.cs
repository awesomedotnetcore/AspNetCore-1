// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Microsoft.AspNetCore.Components.Server.Abstractions.Browser
{
    internal class UnsupportedJSRuntime : IJSRuntime
    {
        public Task<T> InvokeAsync<T>(string identifier, params object[] args)
        {
            throw new InvalidOperationException("Javascript interoperability is not supported.");
        }

        public void UntrackObjectRef(DotNetObjectRef dotNetObjectRef)
        {
            throw new InvalidOperationException("Javascript interoperability is not supported.");
        }
    }
}
