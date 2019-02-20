﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.RazorComponents
{
    internal class MvcRazorComponentPrerrenderer : IComponentPrerrenderer
    {
        private HtmlEncoder _encoder;

        public MvcRazorComponentPrerrenderer(HtmlEncoder encoder)
        {
            _encoder = encoder;
        }

        public async Task<IEnumerable<string>> PrerrenderComponentAsync(ComponentPrerrenderingContext context)
        {
            var dispatcher = Renderer.CreateDefaultDispatcher();
            var parameters = context.Parameters;
            var helper = (HttpUriHelper)context.Context.RequestServices.GetRequiredService<IUriHelper>();
            helper.InitializeState(context.Context);
            using (var htmlRenderer = new HtmlRenderer(context.Context.RequestServices, _encoder.Encode, dispatcher))
            {
                return await dispatcher.InvokeAsync(() => htmlRenderer.RenderComponentAsync(
                    context.ComponentType,
                    parameters));
            }
        }
    }
}
