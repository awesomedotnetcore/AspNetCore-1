using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Components.Server.Builder
{
    public class RazorComponentsBuilder
    {
        internal RazorComponentsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
