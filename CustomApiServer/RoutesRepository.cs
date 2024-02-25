using System.Net;
using System.Reflection;

namespace CustomApiServer
{
    internal class RoutesRepository
    {
        private static Dictionary<RouteAttribute, MethodInfo> routes = new();

        public static void DiscoverRoutes()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

                foreach (var method in methods)
                {
                    var routeAttribute = method.GetCustomAttribute<RouteAttribute>();

                    if (routeAttribute != null)
                    {
                        routes.Add(routeAttribute, method);
                    }
                }
            }
        }

        public static void ExecuteRoute(HttpListenerRequest request, HttpListenerResponse response)
        {
            string routeName;

            // request.Url!.Segments bevat bij /user/1:
            // [0] = /
            // [1] = user/
            // [2] = 1
            if (request.Url!.Segments.Length > 1)
            {
                routeName = request.Url!.Segments[1].TrimEnd('/');
            }
            else
            {
                routeName = request.Url!.Segments[0];
            }

            string? responseString = null;
            bool foundMethod = false;

            foreach (var route in routes)
            {
                RouteAttribute routeAttribute = route.Key;
                MethodInfo routeMethod = route.Value;

                if (routeAttribute.Url == routeName)
                {
                    var parameters = new Dictionary<string, string>(routeAttribute.Parameters.Length);

                    if (routeAttribute.Parameters.Length > 0)
                    {
                        // De methode is alleen gevonden, als ook het aantal parameters overeenkomt.
                        for (int i = 0; i < routeAttribute.Parameters.Length; i++)
                        {
                            var segmentIndex = i + 2;

                            if (request.Url!.Segments.Length > segmentIndex)
                            {
                                var parameterName = routeAttribute.Parameters[i].Trim('{', '}');
                                parameters[parameterName] = request.Url!.Segments[i + 2];
                                foundMethod = true;
                            }
                        }
                    } 
                    else
                    {
                        foundMethod = true;
                    }

                    if (foundMethod)
                    {
                        responseString = CallRouteMethod(routeMethod, request, response, parameters);
                    }
                }
            }

            if (responseString == null)
            {
                responseString = "<html><body><h1>404 - Page Not Found</h1></body></html>";
                response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            // Zet de string om naar rauwe bytes
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

            // We moeten in het antwoordpakket aangeven hoe lang ons antwoord is
            response.ContentLength64 = buffer.Length;

            // Schrijf je antwoord naar de 'OutputStream' in het antwoordpakket. Dit is een
            // soort tunnel waardoor je het antwoord kunt schrijven naar de client.
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            // Klaar met schrijven? Sluit dan de tunnel.
            output.Close();
        }

        // Bron: https://chat.openai.com/share/f13c8e77-5f18-4e5a-bc53-a15c7337bf14
        private static string? CallRouteMethod(
            MethodInfo routeMethod, 
            HttpListenerRequest request, 
            HttpListenerResponse response,
            Dictionary<string, string> parameters
        )
        {
            // Get the parameters expected by the routeMethod
            ParameterInfo[] methodParams = routeMethod.GetParameters();
            object?[] args = new object[methodParams.Length];

            for (int i = 0; i < methodParams.Length; i++)
            {
                ParameterInfo param = methodParams[i];

                // If the parameter is HttpListenerRequest or HttpListenerResponse, pass them directly
                if (param.ParameterType == typeof(HttpListenerRequest))
                {
                    args[i] = request;
                }
                else if (param.ParameterType == typeof(HttpListenerResponse))
                {
                    args[i] = response;
                }
                else if (parameters.ContainsKey(param.Name))
                {
                    args[i] = ConvertTypeFromUrl(param, parameters[param.Name]);
                }
                else
                {
                    args[i] = null;
                }
            }

            // Invoke the method
            object? result = routeMethod.Invoke(null, args);

            // Convert the result to a string if not null, else return null
            return result?.ToString();
        }

        private static object? ConvertTypeFromUrl(ParameterInfo param, string value)
        {
            // Convert the string value to the expected type
            Type targetType = Nullable.GetUnderlyingType(param.ParameterType) ?? param.ParameterType;

            // Check if the target type is a numeric type and use the appropriate TryParse method
            object paramValue = null;

            // Handling for nullable types
            bool isNullableType = Nullable.GetUnderlyingType(targetType) != null;
            Type nonNullableTargetType = Nullable.GetUnderlyingType(targetType) ?? targetType; // Ensure we have the non-nullable type for TryParse methods

            if (nonNullableTargetType == typeof(int))
            {
                if (int.TryParse(value, out int intValue))
                {
                    paramValue = intValue;
                }
            }
            else if (nonNullableTargetType == typeof(long))
            {
                if (long.TryParse(value, out long longValue))
                {
                    paramValue = longValue;
                }
            }
            else if (nonNullableTargetType == typeof(float))
            {
                if (float.TryParse(value, out float floatValue))
                {
                    paramValue = floatValue;
                }
            }
            else if (nonNullableTargetType == typeof(double))
            {
                if (double.TryParse(value, out double doubleValue))
                {
                    paramValue = doubleValue;
                }
            }
            else if (nonNullableTargetType == typeof(decimal))
            {
                if (decimal.TryParse(value, out decimal decimalValue))
                {
                    paramValue = decimalValue;
                }
            }
            else if (nonNullableTargetType == typeof(bool))
            {
                if (bool.TryParse(value, out bool boolValue))
                {
                    paramValue = boolValue;
                }
            }
            else
            {
                // For non-numeric types, fall back to Convert.ChangeType
                paramValue = Convert.ChangeType(value, targetType);
            }

            // Assign the converted value or a default if parsing failed and the type is nullable
            if (paramValue != null || isNullableType)
            {
                return paramValue;
            }
            else
            {
                // Handle parsing failure for non-nullable types, maybe throw an exception or assign a default value
                // For this example, assign default value for value types, null for reference types
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
        }
    }
}
