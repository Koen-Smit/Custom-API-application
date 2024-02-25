using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CustomApiServer
{
    /// <summary>
    /// Leest tekst uit de views in de Views map. Zorg dat die 'Embedded Resource' als Build Action hebben.
    /// </summary>
    internal class ViewFormatter
    {
        // Bron: https://stackoverflow.com/a/3314213
        public static string ReadView(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(fileName));

            using Stream stream = assembly.GetManifestResourceStream(resourcePath);
            using StreamReader reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }

        public static string ReplaceTemplateVariables(string html, Dictionary<string, object> templateVariables)
        {
            var newHtml = html;

            foreach (var variable in templateVariables)
            {
                newHtml = newHtml.Replace(variable.Key, variable.Value.ToString());
            }

            return newHtml;
        }
    }
}
