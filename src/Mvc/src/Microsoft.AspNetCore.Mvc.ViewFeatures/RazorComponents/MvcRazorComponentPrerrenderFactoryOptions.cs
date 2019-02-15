// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Components.Server;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.RazorComponents
{
    /// <summary>
    /// Options for prerrendering components as part of MVC applications.
    /// </summary>
    public class MvcRazorComponentPrerrenderFactoryOptions
    {
        /// <summary>
        /// Gets or sets the <see cref="IComponentPrerrenderer"/> returned by the default component
        /// prerrenderer factory.
        /// </summary>
        public IComponentPrerrenderer ComponentPrerrenderer { get; set; }
    }
}
