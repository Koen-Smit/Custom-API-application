using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomApiServer
{
    /// <summary>
    /// Helpt in code een methode markeren, zodat die bereikbaar is via de aangegeven route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RouteAttribute : Attribute
    {
        public string Url { get; set; }
        public string[] Parameters { get; set; }

        public RouteAttribute(string url, params string[] parameters)
        {
            this.Url = url;
            this.Parameters = parameters;
        }
    }
}
