using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Interop;
using static EMVLogger.KernelCalls;

namespace EMVLogger
{
    public class MainManager
    {
        private ILogger<MainManager> logger;
        private readonly IConfiguration config;
        private readonly ExConnHandler exConnHandler;
        private bool isInit = false;
        private KernelCalls.LogCallbackDelegate logCallbackDelegate = new KernelCalls.LogCallbackDelegate(LogCallback);
        private KernelCalls.PinCallbackDelegate pinCallbackDelegate = new KernelCalls.PinCallbackDelegate(PinCallback);
        private static SemaphoreSlim pinLock = new SemaphoreSlim(0, 1);
        private static string pinEntered = "";

        public MainManager(IConfiguration config, ILogger<MainManager> logger, ExConnHandler exConnHandler)
        {
            this.logger = logger;
            this.config = config;
            this.exConnHandler = exConnHandler;
        }

        public void InitKernel()
        {
            if (!isInit)
            {
                string json = System.IO.File.ReadAllText("config.json");
                var config = JsonConvert.DeserializeObject<ConfigRoot>(json);

                MainWindow.instance.log($"Emv Kernel Core Lib Version: {KernelCalls.emvl2Version()}\n");
                KernelCalls.emvl2Init(config.CardReaderName, logCallbackDelegate, pinCallbackDelegate);

                config.PublicKeys.ForEach(key =>
                {
                    KernelCalls.emvl2AddCaKey(key.Rid, Convert.ToByte(key.Index, 16), key.Modulus, key.Exponent);
                });

                config.Contact.ForEach(app =>
                {
                    string aid = "00";
                    string data = "";
                    app.Item.ForEach(it =>
                    {
                        if (it.Tag.Equals("9F06", StringComparison.OrdinalIgnoreCase))
                        {
                            aid = it.Value;
                        }
                        else
                        {
                            data += it.Tag;
                            data += (it.Value.Length / 2).ToString("X2");
                            data += it.Value;
                        }
                    });
                    KernelCalls.emvl2AddAidPrms(aid, data);
                });


                isInit = true;
            }
        }
        public void OpenConfigEdit()
        {
            Process.Start("notepad.exe", "config.json");
            isInit = false;
        }
        public ResponseData MakeSale(string amount)
        {
            InitKernel();

            byte termDecision = 0;
            byte cardDecision = 0;

            string data = "";
            int rv = 0xff;
            rv = KernelCalls.emvl2CardReset();
            MainWindow.instance.log($"emvl2CardReset:{rv}\n");
            if (rv == 0)
            {
                ShowMessage("Application Selection");
                rv = KernelCalls.emvl2ApplicationSelection();
                MainWindow.instance.log($"emvl2ApplicationSelection:{rv}\n");
                if (rv == 0)
                {
                    ShowMessage("Get Processing Options");
                    rv = KernelCalls.emvl2Gpo(0, 0, amount, "0");
                    MainWindow.instance.log($"emvl2Gpo:{rv}\n");
                    if (rv == 0)
                    {
                        ShowMessage("Read Application Data");
                        rv = KernelCalls.emvl2ReadAppData();
                        MainWindow.instance.log($"emvl2ReadAppData:{rv}\n");
                        if (rv == 0)
                        {
                            ShowMessage("Offline Data Authentication");
                            rv = KernelCalls.emvl2OfflineDataAuth();
                            MainWindow.instance.log($"emvl2OfflineDataAuth:{rv}\n");
                            if (rv == 0)
                            {
                                ShowMessage("Process Restriction");
                                rv = KernelCalls.emvl2ProcessRestrict();
                                MainWindow.instance.log($"emvl2ProcessRestrict:{rv}\n");
                                if (rv == 0)
                                {
                                    ShowMessage("Card Holder Verification");
                                    rv = KernelCalls.emvl2ProcessCVM();
                                    MainWindow.instance.log($"emvl2ProcessCVM:{rv}\n");
                                    if (rv == 0)
                                    {
                                        ShowMessage("Terminal Risk Management");
                                        rv = KernelCalls.emvl2TerminalRiskMng();
                                        MainWindow.instance.log($"emvl2TerminalRiskMng:{rv}\n");
                                        if (rv == 0)
                                        {
                                            ShowMessage("Terminal Action Analysis");
                                            rv = KernelCalls.emvl2TermActionAnalysis(ref termDecision);
                                            MainWindow.instance.log($"emvl2TermActionAnalysis:{rv}\n");
                                            if (rv == 0)
                                            {
                                                ShowMessage("Generate First Application Cryptogram");
                                                rv = KernelCalls.emvl2GenAC1(termDecision, ref cardDecision);
                                                MainWindow.instance.log($"emvl2GenAC1:{rv}\n");
                                                if (rv == 0)
                                                {
                                                    MainWindow.instance.log("Cryptogram is generated successfully\n");

                                                    switch (cardDecision)
                                                    {
                                                        case 0x00:
                                                            data = "Offline Declined";
                                                            MainWindow.instance.log("Offline Declined\n");
                                                            break;
                                                        case 0x40:
                                                            data = "Offline Approved";
                                                            MainWindow.instance.log("Offline Approved\n");
                                                            break;
                                                        case 0x80:
                                                            data = "Online Processing";
                                                            MainWindow.instance.log("Online Processing\n");
                                                            break;
                                                        case 0xC0:
                                                            data = "Referral";
                                                            MainWindow.instance.log("Referral\n");
                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            LogTags();

            return new ResponseData() { code = rv.ToString(), data = data };
        }

        private void LogTags()
        {
            int rv = 0;
            byte[] buffer = new byte[1024];
            rv = KernelCalls.emvl2GetTag(0x95, buffer, 1024);
            if(rv > 0)
            {
                MainWindow.instance.log("TVR(95): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x98, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Transaction Certificate (TC) Hash Value(98): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x82, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("AIP(82): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9A, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Transaction Date(9A): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9C, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Transaction Type(9C): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x5F2A, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Transaction Currency Code(5F2A): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F02, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Amount Authorised(9F02): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F03, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Amount Other(9F03): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F37, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Unpredictable Number(9F37): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F1A, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Terminal Country Code(9F1A): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F33, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Terminal Capabilities(9F33): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x5F34, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Card seq number(5F34): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F09, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Terminal app version number(9F09): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F1E, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("IFD serial number(9F1E): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F34, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Cardholder verification method result(9F34): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F35, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Terminal type(9F35): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F41, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Trx sequence counter(9F41): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F26, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Application Cryptogram(9F26): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F36, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Application Transaction Counter(9F36): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F10, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Issuer Application Data(9F10): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }

            rv = KernelCalls.emvl2GetTag(0x9F27, buffer, 1024);
            if (rv > 0)
            {
                MainWindow.instance.log("Cryptogram Information Data(9F27): " + BitConverter.ToString(buffer, 0, rv).Replace("-", "") + "\n");
            }
        }

        public void PinEntered(string pin)
        {
            pinEntered = pin;
            pinLock.Release();
        }
        private void ShowMessage(string msg)
        {
            Helpers.RunSync(() => exConnHandler.Publish(JsonConvert.SerializeObject(new { msg = msg })));
        }
        public static void LogCallback(string log)
        {
            MainWindow.instance.log(log);
        }
        public static IntPtr PinCallback()
        {
            var exConnHandler = Startup.GetService<ExConnHandler>();
            Helpers.RunSync(() => exConnHandler.Publish(JsonConvert.SerializeObject(new { msg = "PIN" })));

            pinLock.Wait();

            return Marshal.StringToHGlobalAnsi(pinEntered);
        }
    }
}
