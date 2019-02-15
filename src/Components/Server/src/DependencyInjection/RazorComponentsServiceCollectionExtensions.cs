// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Components.Environment;
using Microsoft.AspNetCore.Components.Server.Builder;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to configure an <see cref="IServiceCollection"/> for interactive components.
    /// </summary>
    public static class RazorComponentsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Razor Component app services to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static RazorComponentsBuilder AddRazorComponents(
            this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return AddRazorComponentsCore(services);
        }

        private static RazorComponentsBuilder AddRazorComponentsCore(
            IServiceCollection services)
        {
            AddStandardRazorComponentsServices(services);
            return new RazorComponentsBuilder(services);
        }

        private static void AddStandardRazorComponentsServices(IServiceCollection services)
        {
            // Here we add a bunch of services that don't vary in any way based on the
            // user's configuration. So even if the user has multiple independent server-side
            // Components entrypoints, this lot is the same and repeated registrations are a no-op.
            services.TryAddSingleton<CircuitFactory, DefaultCircuitFactory>();
            services.TryAddScoped(s => s.GetRequiredService<ICircuitAccessor>().Circuit);
            services.TryAddScoped<ICircuitAccessor, DefaultCircuitAccessor>();

#pragma warning disable CS0618 // Type or member is obsolete
            services.TryAddScoped<IJSRuntimeAccessor, DefaultJSRuntimeAccessor>();
#pragma warning restore CS0618 // Type or member is obsolete
            services.TryAddScoped<ComponentEnvironment>();
            services.TryAddScoped(s => s.GetRequiredService<ComponentEnvironment>().JSRuntime);
            services.TryAddScoped(s => s.GetRequiredService<ComponentEnvironment>().UriHelper);

            // We've discussed with the SignalR team and believe it's OK to have repeated
            // calls to AddSignalR (making the nonfirst ones no-ops). If we want to change
            // this in the future, we could change AddComponents to be an extension
            // method on ISignalRServerBuilder so the developer always has to chain it onto
            // their own AddSignalR call. For now we're keeping it like this because it's
            // simpler for developers in common cases.
            services.AddSignalR().AddMessagePackProtocol();
        }
    }
}
