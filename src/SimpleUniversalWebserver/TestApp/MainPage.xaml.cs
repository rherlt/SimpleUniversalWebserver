using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Devices.PointOfService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimpleUniversalWebserver.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            StartServer();

        }

        private const string Html = "<html><head></head><body>this works!</body></html>";

        public void StartServer()
        {
            HttpServer server = new HttpServer(8000, MessageHandler)
            {
                ErrorWithStackTrace = true,
                EnforceHtmlResponseBody = true
            };
            server.StartServer();

        }

        private static HttpResponseMessage MessageHandler(HttpRequestMessage httpRequestMessage)
        {
            HttpResponseMessage response;
            response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(Html, Encoding.UTF8, "text/html"),
                RequestMessage = httpRequestMessage
            };

            return response;
        }
    }
}
