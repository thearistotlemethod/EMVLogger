using System.Windows;

namespace EMVLogger
{
    public class MainViewModel
    {
        public bool IsInstalled => WebView2Install.GetInfo().InstallType != InstallType.NotInstalled;
        public bool IsNotInstalled => !IsInstalled;

        public string source => "http://localhost:" + App.httpport;
    }
}