// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures.RazorComponents
{
    internal class DefaultAspNetCorePrerrenderingFactoryConfiguration
        : IPostConfigureOptions<MvcRazorComponentPrerrenderFactoryOptions>
    {
        private readonly HtmlEncoder _encoder;
        private readonly IEnumerable<IComponentPrerrenderer> _prerrenderers;
        private readonly ILogger<DefaultAspNetCorePrerrenderingFactoryConfiguration> _logger;

        public DefaultAspNetCorePrerrenderingFactoryConfiguration(
            HtmlEncoder encoder,
            IEnumerable<IComponentPrerrenderer> prerrenderers,
            ILogger<DefaultAspNetCorePrerrenderingFactoryConfiguration> logger)
        {
            _encoder = encoder;
            _prerrenderers = prerrenderers;
            _logger = logger;
        }

        /// <inheritdoc />
        public void PostConfigure(string name, MvcRazorComponentPrerrenderFactoryOptions options)
        {
            if (name != Options.DefaultName)
            {
                return;
            }

            // We are configuring a component prerrenderer for MVC in case no one was configured explicitly by the user.
            var componentPrerrendererTypeName = typeof(IComponentPrerrenderer).Name;
            if (options.ComponentPrerrenderer != null)
            {
                _logger.LogDebug($"'{componentPrerrendererTypeName}' already configured. No action taken.");
                // Prerrenderer already configured.
                return;
            }

            var list = _prerrenderers.ToArray();
            if (list.Length == 0)
            {
                // No prerrenderer configured, so adding our default implementation. We don't register
                // this implementation in DI and it's internal.
                _logger.LogDebug(
                    $"'{componentPrerrendererTypeName}' not configured and no registered '{componentPrerrendererTypeName}' found. Using the default implementation.");
                options.ComponentPrerrenderer = new MvcRazorComponentPrerrenderer(_encoder);
            }
            else if (list.Length == 1)
            {
                // Some prerrender was registered, so using that one. This can be for example when
                // Components.Server registers itself as a prerrenderer. (This allows it to take over
                // at a later stage).
                var implementationTypeName = list[0].GetType().FullName;
                _logger.LogDebug($"'{componentPrerrendererTypeName}' not configured and registerd implementation '{implementationTypeName}' found." +
                    $" Using the registered implementation.");
                options.ComponentPrerrenderer = list[0];
            }
            else
            {
                // We can't decide on a default, so we throw.
                throw new InvalidOperationException(
                    $"More than one '{componentPrerrendererTypeName}' instances have been registered. " +
                    "Manual configuration is required to use MVC with prerrendering in this scenario. " +
                    "Call 'services.ConfigureOptions<MvcRazorComponentPrerrenderFactoryOptions> to set the " +
                    $"'{componentPrerrendererTypeName}' you want MVC to use for prerrendering components.");
            }
        }
    }
}
