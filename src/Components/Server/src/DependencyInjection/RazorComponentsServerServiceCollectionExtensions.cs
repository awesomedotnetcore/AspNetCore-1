using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components.Server.Builder;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to configure an <see cref="IServiceCollection"/> for interactive components.
    /// </summary>
    public static class RazorComponentsServerServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Razor Component app services to the service collection.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static RazorComponentsBuilder AddRazorComponents(this IServiceCollection services)
        {
            // We've discussed with the SignalR team and believe it's OK to have repeated
            // calls to AddSignalR (making the nonfirst ones no-ops). If we want to change
            // this in the future, we could change AddComponents to be an extension
            // method on ISignalRServerBuilder so the developer always has to chain it onto
            // their own AddSignalR call. For now we're keeping it like this because it's
            // simpler for developers in common cases.
            services.AddSignalR().AddMessagePackProtocol();

            var builder = services.AddRazorComponentsCore();

            // Here we add a bunch of services that don't vary in any way based on the
            // user's configuration. So even if the user has multiple independent server-side
            // Components entrypoints, this lot is the same and repeated registrations are a no-op.
            services.TryAddSingleton<CircuitFactory, DefaultCircuitFactory>();
            services.TryAddScoped(s => s.GetRequiredService<ICircuitAccessor>().Circuit);
            services.TryAddScoped<ICircuitAccessor, DefaultCircuitAccessor>();

            return builder;
        }
    }
}
