// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Components.Server;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.RazorComponents
{
    public abstract class MvcRazorComponentPrerrendererFactory
    {
        public abstract IComponentPrerrenderer CreatePrerrenderer();
    }
}
