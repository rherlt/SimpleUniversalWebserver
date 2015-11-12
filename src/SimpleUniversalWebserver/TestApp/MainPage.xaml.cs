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
using SimpleUniversalWebserver.Errors;
using SimpleUniversalWebserver.Net;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WebserverTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            _server = new HttpServer(8000, MessageHandler)
            {
                ErrorWithStackTrace = true,
                EnforceHtmlResponseBody = true
            };

            startButton_Click(null, null);
        }

        private const string Html = "<html><head></head><body>this works! Date:{0}</br>RequestUri: {1}</body></html>";
        private HttpServer _server;

        

        private static bool MessageHandler(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (request.RequestUri.OriginalString.Contains("status"))
            {
                try
                {
                    int code =
                        Convert.ToInt32(
                            request.RequestUri.OriginalString.Substring(request.RequestUri.OriginalString.Length - 3, 3));
                    HttpStatusCode status = (HttpStatusCode) code;
                    throw new HttpStatusException(status);
                }
                catch (HttpStatusException)
                {
                    throw;
                }
                catch (Exception)
                {

                    throw new HttpStatusException(HttpStatusCode.BadRequest);
                }
            }
            else
                response.Content = new StringContent(string.Format(Html, DateTime.Now.ToString("s"), request.RequestUri.OriginalString), Encoding.UTF8, "text/html");
            return true;
        }

       
        private async void startButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Starting Server...";
            await _server.StartServer();
            StatusTextBlock.Text = "Server running.";


        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Stopping Server...";
            _server.StopServer();
            StatusTextBlock.Text = "Server stopped.";
        }
    }
}
