using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing;
using Windows.Security.Cryptography.Core;

namespace SimpleUniversalWebserver.Errors
{
    public static class SimpleErrorPage
    {
        public static string Html(int code, string message)
        {
            return
                "<html>\r\n" +
                "  <head>\r\n" +
                "    <title>\r\n" +
                $"      {code} - {message}\r\n" +
                "    </title>\r\n" +
                "	</head>\r\n" +
                "	<body bgcolor=\"#000033\" text=\"#FFFFFF\">\r\n" +
                "		<table align =\"center\" cellpadding=\"5\" width=\"650\">\r\n" +
                "			<tr>\r\n" +
                "			  <td align=\"center\">\r\n" +
                "                    <h1> SimpleUniversalWebserver </h1>\r\n" +
                $"                    <h2> HTTP Status {code}<br>{message}</h2>\r\n" +
                "					<a href = \"https://github.com/rherlt/SimpleUniversalWebserver\">\r\n" +
                $"                    <img src=\"https://http.cat/{code}\" alt=\"{code} - {message}\" border=\"0\" width=\"450\">\r\n" +
                "					<br>https://github.com/rherlt/SimpleUniversalWebserver</a>\r\n" +
                "					<br>\r\n" +
                "					<p>&nbsp;</p>\r\n" +
                "					<p>{0}</p>\r\n" + //stacktrace is printed here!
                "					<hr size = \"1\" width=\"75%\" color=\"#FFFFFF\">By Rico Herlt<br>\r\n" +
                "  			   </td>\r\n" +
                "			</tr>\r\n" +
                "		</table>\r\n" +
                "	</body>\r\n" +
                "</html>\r\n";
        }

    }
}
