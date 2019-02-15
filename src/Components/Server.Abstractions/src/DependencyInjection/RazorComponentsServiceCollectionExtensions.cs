// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Components.Server.Abstractions.Browser;
using Microsoft.AspNetCore.Components.Server.Builder;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to configure an <see cref="IServiceCollection"/> for components.
    /// </summary>
    public static class RazorComponentsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the core Razor Component app services to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static RazorComponentsBuilder AddRazorComponentsCore(
            this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            AddStandardRazorComponentsServices(services);
            return new RazorComponentsBuilder(services);
        }

        private static void AddStandardRazorComponentsServices(IServiceCollection services)
        {
            services.TryAddScoped<IUriHelper, RemoteUriHelper>();
            services.TryAddScoped<IJSRuntime, UnsupportedJSRuntime>();
        }
    }
}
