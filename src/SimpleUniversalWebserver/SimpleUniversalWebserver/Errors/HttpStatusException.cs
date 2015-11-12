using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUniversalWebserver.Errors
{
    public class HttpStatusException : Exception
    {
        public HttpStatusException(HttpStatusCode httpStatusCode) : base()
        {
            HttpStatusCode = httpStatusCode;
        }

        public HttpStatusException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public HttpStatusException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }

        public HttpStatusCode HttpStatusCode { get; }
    }
}
