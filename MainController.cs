using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EMVLogger
{
    [ApiController]
    public class MainController : Controller
    {
        private IHttpContextAccessor contextAccessor;
        private ILogger<MainController> _logger;

        private readonly MainManager mainManager;
        public MainController(ILogger<MainController> logger, IHttpContextAccessor contextAccessor, MainManager mainManager)
        {
            this.contextAccessor = contextAccessor;
            this._logger = logger;
            this.mainManager = mainManager;
        }

        [Route("/api/{cmd}"), AcceptVerbs("POST"), HttpPost]
        public Object RunCommandAsync(string cmd, [FromBody] RequestData req)
        {
            try
            {
                switch (cmd)
                {
                    case "version":
                        return new ResponseData() { code = App.VERSION };
                    case "processing":
                        return mainManager.MakeSale(req.amount);
                    case "pinentered":
                        mainManager.PinEntered(req.pin);
                        return new ResponseData() { code = "0" };
                    case "savelog":
                        MainWindow.instance.savelog();
                        return new ResponseData() { code = "0" };
                    case "editconfig":                        
                        mainManager.OpenConfigEdit();
                        return new ResponseData() { code = "0" };
                    case "help":
                        MainWindow.instance.log("© 2024 Ufuk Varol. All rights reserved.\n" +
                            "Version " + App.VERSION + "\n");
                        return new ResponseData() { code = "0" };

                }
                return new ResponseData() { code = "Unknown Error" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new ResponseData() { code = "Error: " + ex.Message };
            }
        }
    }

    public class RequestData
    {
        public string type { get; set; }
        public string amount { get; set; }
        public string pin { get; set; }
    }

    public class ResponseData
    {
        public string code { get; set; }
        public string data { get; set; }
    }
}
