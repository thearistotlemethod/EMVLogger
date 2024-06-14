using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace EMVLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;
        private System.Windows.Point startPos;

        public MainWindow()
        {
            instance = this;

            InitializeComponent();

            DataContext = new MainViewModel();
            webView.CoreWebView2InitializationCompleted += WebView_CoreWebView2Ready;

            this.Title = this.Title + " v" + App.VERSION;

            if (System.Windows.SystemParameters.PrimaryScreenHeight < 800)
            {
                this.Height = (this.Height * 0.75);
                this.Width = (this.Width * 0.75);
            }
        }

        private void WebView_CoreWebView2Ready(object sender, EventArgs e)
        {
            webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            webView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
            webView.ZoomFactor = 0.95;
            webView.CoreWebView2.ServerCertificateErrorDetected += WebView_ServerCertificateErrorDetected;
        }

        void WebView_ServerCertificateErrorDetected(object sender, CoreWebView2ServerCertificateErrorDetectedEventArgs e)
        {
            CoreWebView2Certificate certificate = e.ServerCertificate;
            e.Action = CoreWebView2ServerCertificateErrorAction.AlwaysAllow;
        }
        private void Hyperlink_OnClick()
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://go.microsoft.com/fwlink/p/?LinkId=2124703",
                UseShellExecute = true
            });
        }
        public void log(string s)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    AppConsole.AppendText(s);
                    AppConsole.ScrollToEnd();
                });
            }
            catch { }
        }

        public void savelog()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {

                    SaveFileDialog sfDialog = new SaveFileDialog()
                    {
                        DefaultExt = ".log",
                        Filter = "Log files (*.log) |*.log",
                        AddExtension = true,
                        FileName = "kernel_console_log_" + DateTime.Now.ToString(@"yyyyMMdd_hhmmss", DateTimeFormatInfo.InvariantInfo),
                    };


                    if (sfDialog.ShowDialog() == true)
                    {

                        if (File.Exists(sfDialog.FileName))
                            File.Delete(sfDialog.FileName);


                        File.WriteAllText(sfDialog.FileName, AppConsole.Text);
                    }
                });
            }
            catch { }            
        }

        public void BtnTogglePopUp(object sender, RoutedEventArgs e)
        {
            try
            {
                log("pop up test");
            }
            catch { }
        }
        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://go.microsoft.com/fwlink/p/?LinkId=2124703",
                UseShellExecute = true
            });
        }
        public string getAppSettings()
        {
            try
            {
                string fileName = "";
#if DEBUG
                fileName = "appsettings.development.json";
#else
                fileName = "appsettings.json";
                
#endif
                var jsonRead = File.ReadAllText(fileName);
                return jsonRead;
            }
            catch (Exception e)
            {
                MainWindow.instance.log(e.Message);
                return "";
            }
        }

        public string getDirectoryAndNameOfFile(string filter)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.CheckFileExists = true;
                ofd.Filter = filter;
                ofd.Multiselect = false;

                if ((bool)ofd.ShowDialog())
                {
                    return ofd.FileName;
                }

                return "";
            }
            catch (Exception e)
            {
                MainWindow.instance.log(e.Message);
                return "";
            }
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
        }

        private void System_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount >= 2)
                {
                    this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
                }
                else
                {
                    startPos = e.GetPosition(null);
                }
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                var pos = PointToScreen(e.GetPosition(this));
                IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                IntPtr hMenu = GetSystemMenu(hWnd, false);
                int cmd = TrackPopupMenu(hMenu, 0x100, (int)pos.X, (int)pos.Y, 0, hWnd, IntPtr.Zero);
                if (cmd > 0) SendMessage(hWnd, 0x112, (IntPtr)cmd, IntPtr.Zero);
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

        private void System_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.WindowState == WindowState.Maximized && Math.Abs(startPos.Y - e.GetPosition(null).Y) > 2)
                {
                    var point = PointToScreen(e.GetPosition(null));

                    this.WindowState = WindowState.Normal;

                    this.Left = point.X - this.ActualWidth / 2;
                    this.Top = point.Y - border.ActualHeight / 2;
                }
                DragMove();
            }
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            } 
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Mimimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                main.BorderThickness = new Thickness(0);
                main.Margin = new Thickness(7, 7, 7, 0);
                rectMax.Visibility = Visibility.Hidden;
                rectMin.Visibility = Visibility.Visible;
            }
            else
            {
                main.BorderThickness = new Thickness(1);
                main.Margin = new Thickness(0);
                rectMax.Visibility = Visibility.Visible;
                rectMin.Visibility = Visibility.Hidden;
            }
        }
    }
}
