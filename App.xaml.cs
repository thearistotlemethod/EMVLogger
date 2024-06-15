using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace EMVLogger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string VERSION = "1.0.01";
        public static IConfiguration Configuration { get; set; }
        public static string httpport = "80";
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        void AppStartup(object sender, StartupEventArgs args)
        {
            try
            {
                System.Windows.Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

                System.Text.EncodingProvider ppp = System.Text.CodePagesEncodingProvider.Instance;
                Encoding.RegisterProvider(ppp);

                if (!Directory.Exists("Logs"))
                    Directory.CreateDirectory("Logs");

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
                Configuration = builder.Build();

                //httpport = Configuration["HttpPort"] ?? "80";
                httpport = "8089";
                CreateWebHostBuilder(args.Args)
               .UseKestrel(c =>
               {
                   c.AddServerHeader = false;
                   c.Limits.MaxRequestBodySize = int.MaxValue;
                   c.Limits.MaxRequestBufferSize = int.MaxValue;
                   c.Limits.MaxResponseBufferSize = int.MaxValue;
               })
               .UseUrls("http://*:" + httpport)
               .Build().RunAsync(tokenSource.Token);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            tokenSource.Cancel();
            Environment.Exit(0);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
        .UseStartup<Startup>();
    }
}
