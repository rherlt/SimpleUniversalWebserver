using System;
using System.Collections.Generic;
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
                    req.Headers.Add(key, value);
                }
            });
           
            string body = requestParts.Last().TrimEnd('\0');
            req.Content = new StringContent(body);
            return req;
        }

        public static string ToHeaderString(this HttpResponseMessage response)
        {
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.AppendLine($"HTTP/{response.Version} {response.StatusCode} {response.ReasonPhrase}");
            //headerBuilder.AppendLine($"Content-Length: {contentLength}");
            foreach (var header in response.Headers)
            {
                headerBuilder.AppendLine($"{header.Key}: {header.Value}");
            }
            foreach (var header in response.Content.Headers)
            {
                headerBuilder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            //if (response.Headers.Connection.Count == 0)
            //    headerBuilder.AppendLine("Connection: close\r\n");
            headerBuilder.AppendLine();
            return headerBuilder.ToString();
        }
    }
}
