using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUniversalWebserver.Errors
{ 
    public class DefaultErrorPages
    {
        public Dictionary<int, string> Pages { get; }

        public DefaultErrorPages()
        {
            Pages = new Dictionary<int, string>
            {
                //200er
                { 200, SimpleErrorPage.Html(200, "OK")},
                { 201, SimpleErrorPage.Html(201, "Created")},
                { 202, SimpleErrorPage.Html(202, "Accepted")},
                { 204, SimpleErrorPage.Html(204, "No Content")},

                //300er
                { 301, SimpleErrorPage.Html(301, "Moved Permanently")},
                { 302, SimpleErrorPage.Html(302, "Found")},
                { 304, SimpleErrorPage.Html(304, "Not Modified")},
                { 307, SimpleErrorPage.Html(307, "Temporary Redirect")},
                { 309, SimpleErrorPage.Html(309, "Permanent Redirect")},

                //400er
                { 400, SimpleErrorPage.Html(400, "Bad Request")},
                { 401, SimpleErrorPage.Html(401, "Unauthorized")},
                { 403, SimpleErrorPage.Html(403, "Forbidden")}, 
                { 404, SimpleErrorPage.Html(404, "Not Found")}, 
                { 405, SimpleErrorPage.Html(405, "Method Not Allowed")},

                //500er
                { 500, SimpleErrorPage.Html(500, "Internal Server Error")},
                { 501, SimpleErrorPage.Html(501, "Not Implemented")},
                { 503, SimpleErrorPage.Html(503, "Service Not Availible")},

                //to be continued... ;)
            };
        }


    }
}
