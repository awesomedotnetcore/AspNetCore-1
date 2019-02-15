using System;
using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Components.Server
{
    public class ComponentPrerrenderingContext
    {
        public Type ComponentType { get; set; }
        public ParameterCollection Parameters { get; set; }
        public HttpContext Context { get; set; }
    }
}
