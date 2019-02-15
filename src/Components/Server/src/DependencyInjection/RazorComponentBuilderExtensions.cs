using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.Builder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RazorComponentBuilderExtensions
    {
        /// <summary>
        /// Adds the component <typeparamref name="TComponent"/> to the list
        /// of registered components with the type name in lowercase letters
        /// as the selector.
        /// </summary>
        /// <param name="builder">The <see cref="RazorComponentsBuilder"/>.</param>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <example>
        /// For a component whose full type name is Namespace.App the component
        /// selector will be "app".
        /// </example>
        public static RazorComponentsBuilder AddComponent<TComponent>(this RazorComponentsBuilder builder) where TComponent : IComponent =>
            builder.AddComponent(typeof(TComponent), nameof(TComponent).ToLowerInvariant());

        /// <summary>
        /// Adds the component <typeparamref name="TComponent"/> to the list
        /// of registered components with the selector specified in <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <param name="selector">The selector for the component.</param>
        public static RazorComponentsBuilder AddComponent<TComponent>(this RazorComponentsBuilder builder, string selector) where TComponent : IComponent =>
            builder.AddComponent(typeof(TComponent), selector);

        /// <summary>
        /// Adds the component <typeparamref name="TComponent"/> to the list
        /// of registered components with the selector specified in <paramref name="selector"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for the component.</param>
        /// <param name="selector">The selector for the component.</param>
        public static RazorComponentsBuilder AddComponent(this RazorComponentsBuilder builder, Type type) =>
            builder.AddComponent(type, type.Name.ToLowerInvariant());

        /// <summary>
        /// Adds the component <typeparamref name="TComponent"/> to the list
        /// of registered components with the selector specified in <paramref name="selector"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for the component.</param>
        /// <param name="selector">The selector for the component.</param>
        public static RazorComponentsBuilder AddComponent(this RazorComponentsBuilder builder, Type type, string selector)
        {
            builder.Services.Configure<RazorComponentsOptions>(options =>
            {
                options.Components.AddComponent(type, selector);
            });

            return builder;
        }
    }
}
