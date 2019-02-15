// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.RazorComponents
{
    internal class DefaultMvcRazorComponentPrerrendererFactory : MvcRazorComponentPrerrendererFactory
    {
        private readonly MvcRazorComponentPrerrenderFactoryOptions _options;

        public DefaultMvcRazorComponentPrerrendererFactory(IOptions<MvcRazorComponentPrerrenderFactoryOptions> options)
        {
            _options = options.Value;
        }

        public override IComponentPrerrenderer CreatePrerrenderer()
        {
            return _options.ComponentPrerrenderer;
        }
    }
}
