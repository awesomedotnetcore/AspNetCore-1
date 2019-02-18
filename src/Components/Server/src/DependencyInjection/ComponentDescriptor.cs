// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Components.Server.Builder
{
    /// <summary>
    /// Describes a <see cref="IComponent"/>.
    /// </summary>
    public class ComponentDescriptor
    {
        /// <summary>
        /// Gets or sets the <see cref="Type"/> of the <see cref="IComponent"/>.
        /// </summary>
        public Type ComponentType { get; set; }

        /// <summary>
        /// Gets or sets dom element selector to be replaced by the component.
        /// </summary>
        public string Selector { get; internal set; }

        public void Deconstruct(out Type componentType, out string domElementSelector)
        {
            componentType = ComponentType;
            domElementSelector = Selector;
        }
    }
}
