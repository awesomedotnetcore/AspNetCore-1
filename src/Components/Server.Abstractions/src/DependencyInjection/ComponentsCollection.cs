// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;

namespace Microsoft.AspNetCore.Components.Server.Builder
{
    /// <summary>
    /// A collection of components registered for rendering on the server side renderer.
    /// </summary>
    public class ComponentsCollection : Collection<ComponentDescriptor>
    {
        /// <summary>
        /// Adds the component <typeparamref name="TComponent"/> to the list
        /// of registered components with the type name in lowercase letters
        /// as the selector.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <example>
        /// For a component whose full type name is Namespace.App the component
        /// selector will be "app".
        /// </example>
        public void AddComponent<TComponent>() where TComponent : IComponent
        {
            AddComponent(typeof(TComponent), nameof(TComponent).ToLowerInvariant());
        }

        /// <summary>
        /// Adds the component <typeparamref name="TComponent"/> to the list
        /// of registered components with the selector specified in <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TComponent">The type of the component.</typeparam>
        /// <param name="selector">The selector for the component.</param>
        public void AddComponent<TComponent>(string selector) where TComponent : IComponent
        {
            AddComponent(typeof(TComponent), selector);
        }

        /// <summary>
        /// Adds the component <typeparamref name="TComponent"/> to the list
        /// of registered components with the selector specified in <paramref name="selector"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for the component.</param>
        /// <param name="selector">The selector for the component.</param>
        public void AddComponent(Type type)
        {
            AddComponent(type, type.Name.ToLowerInvariant());
        }

        /// <summary>
        /// Adds the component <typeparamref name="TComponent"/> to the list
        /// of registered components with the selector specified in <paramref name="selector"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for the component.</param>
        /// <param name="selector">The selector for the component.</param>
        public void AddComponent(Type type, string selector)
        {
            Add(new ComponentDescriptor
            {
                ComponentType = type,
                Selector = selector
            });
        }
    }
}
