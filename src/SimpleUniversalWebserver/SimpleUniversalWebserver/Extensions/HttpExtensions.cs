using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUniversalWebserver.Extensions
{
    public static class HttpExtensions
    {
        public static HttpRequestMessage ToHttpRequest(this string request)
        {
            string[] requestParts = request.Split('\n');
            string[] requestProlog = requestParts[0].Split(' ');
            string method = requestProlog[0].Trim();
            string ressource = requestProlog[1].Trim();
            string[] protocolVersion = requestProlog[2].Trim().Split('/');
            string protocol = protocolVersion[0].Trim();
            if (!protocol.ToLower().Contains("http"))
                throw new NotSupportedException("Just HTTP requests are supported!");
            string version = protocolVersion[1].Trim();
            HttpRequestMessage req = new HttpRequestMessage
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(ressource, UriKind.Relative),
                Version = Version.Parse(version)
            };

            requestParts.Skip(1).TakeWhile(x => x != "\r").ToList().ForEach(x =>
            {
                int seperatorIndex = x.IndexOf(':');
                if (seperatorIndex > -1)
                {
                    string key = x.Substring(0, seperatorIndex).Trim();
                    string value = x.Substring(seperatorIndex + 1, x.Length - seperatorIndex - 1).Trim();
                    try
                    {
                        req.Headers.Add(key, value);
                    }
                    catch (Exception ex)
                    {
                          Debug.WriteLine($"Header {key}: {value} could not be added to incoming HttpRequestMessage, reason:\r\n {ex.Message}");
                    }
                }
            });
           
            string body = requestParts.Last().TrimEnd('\0');
            req.Content = new StringContent(body);
            return req;
        }

        public static string ToHeaderString(this HttpResponseMessage response)
        {
            StringBuilder headerBuilder = new StringBuilder();
            try
            {
                headerBuilder.AppendLine($"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");
                foreach (var header in response.Headers)
                {
                    headerBuilder.AppendLine($"{header.Key}: {header.Value}");
                }
                if (response.Content != null)
                {
                    foreach (var header in response.Content.Headers)
                    {
                        headerBuilder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                    }
                    headerBuilder.AppendLine($"Content-Length: {response.Content.Headers.ContentLength}");
                }
                else
                    headerBuilder.AppendLine("Content-Length: 0");
                
                if(response.Headers.Connection.Count == 0)
                    headerBuilder.AppendLine("Connection: close");

                headerBuilder.AppendLine();
            }
            catch (Exception ex)
            {
                    
                Debug.WriteLine(ex);
            }
           
            
            return headerBuilder.ToString();
        }
    }
}
