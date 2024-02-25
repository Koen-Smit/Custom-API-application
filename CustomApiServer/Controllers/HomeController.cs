using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CustomApiServer.Controllers
{
    internal class HomeController
    {
        [Route("/")]
        public static string Index(HttpListenerRequest request, HttpListenerResponse response)
        {
            string responseString = ViewFormatter.ReadView("home.html");

            responseString = ViewFormatter.ReplaceTemplateVariables(responseString, new()
            {
                { "{{CURRENT_TIME}}", DateTime.Now }
            });

            response.ContentType = "text/html";
            return responseString;
        }
    }
}
