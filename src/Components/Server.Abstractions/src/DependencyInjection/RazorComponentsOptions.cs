// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Builder;

namespace Microsoft.AspNetCore.Components.Server.Builder
{
    /// <summary>
    /// Specifies options to configure <see cref="RazorComponentsApplicationBuilderExtensions.UseRazorComponents{TStartup}(IApplicationBuilder)"/>
    /// </summary>
    public class RazorComponentsOptions
    {
        /// <summary>
        /// Gets the collection of registered components.
        /// </summary>
        public ComponentsCollection Components { get; } = new ComponentsCollection();
    }
}
