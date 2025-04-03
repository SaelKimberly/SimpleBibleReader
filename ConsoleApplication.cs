using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;
using Microsoft.Win32;
using Microsoft.Web.WebView2.Core;

namespace Simple_Bible_Reader
{
    public class ConsoleApplication
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        [STAThread]
        public static void Main(string[] args)
        {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FormatUtil.Init();

            if (args.Length > 0)
            {
                Themes.MessageBox("Please use multi-platform sbrcli from trumpet-call.org/sbrcli for console based operations.", "Simple Bible Reader", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;

                /*

                GlobalMemory.getInstance().ConsoleMode = true;

                //sbr.exe TYPE N1 a.bblx N2 a.bbl
                // if N1 is 0, then, autodetect input format.
                string format_type = args[0];

                if (args.Length>0 && args.Length!=7)
                {
                    //Path.GetFileName(Application.ExecutablePath) + " <Format-Type> <Input-Format> <Input-File> <Output-Format> <Output-File> [Clean]
                    Themes.DarkMessageBoxShow("Please refer to " + GlobalMemory.AUTHOR_WEBSITE + " for console based parameters help.", "Simple Bible Reader - Parameters help",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    return;
                }

                int in_type = -1;
                if (!int.TryParse(args[1], out in_type))
                    ConsoleUtil.addLog("Err: input type " + args[1] + " invalid");

                string in_File=args[2];
                int out_type = -1;
                if (!int.TryParse(args[3], out out_type))
                    ConsoleUtil.addLog("Err: output type " + args[3] + " invalid");

                string out_File = args[4];
                bool clean = true;
                if(!Boolean.TryParse(args[5], out clean))
                    clean = true;

                if (!File.Exists(in_File))
                    ConsoleUtil.addLog("Input file '"+in_File+"' doesn't exist");

                string log_file = args[6];
                if (ConsoleUtil.errors.Count == 0)
                {
                    ConsoleUtil.Convert(format_type, in_type, in_File, out_type, out_File, clean);
                    if (ConsoleUtil.errors.Count != 0)
                    {
                        if(log_file!="nolog")
                            ConsoleUtil.writeLog(log_file);
                    }
                }
                else
                {
                    if (log_file != "nolog")
                        ConsoleUtil.writeLog(log_file);
                }
                */
            }
            else
            {

                int res = -1;
                try
                {
                    res = (int)Registry.GetValue("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize", "AppsUseLightTheme", -1);
                }
                catch
                {
                    //Exception Handling     
                }
                switch (res)
                {
                    case 0: // dark theme
                        GlobalMemory.getInstance().Theme = DarkMode.DarkMode.Theme.DARK;
                        GlobalMemory.getInstance().SelectedTheme = "Dark";
                        break;
                    case 1: // light theme
                        GlobalMemory.getInstance().Theme = DarkMode.DarkMode.Theme.LIGHT;
                        break;
                    default:
                        GlobalMemory.getInstance().Theme = DarkMode.DarkMode.Theme.SYSTEM;
                        break;
                }
                DarkMode.DarkMode.SetAppTheme(GlobalMemory.getInstance().Theme);
                SwordLookup.checkAndSetSwordLibrary();
                
                ////// Load preferences from file (if save enabled)
                GlobalMemory.loadPrefFromFile();
                //////

                // Use WebControl if the environment is wine
                if (Win32NativeCalls.isRunningOnWine())
                {
                    GlobalMemory.getInstance().useRenderer = "WEBCONTROL";
                }
                else
                {

                    // Use WebControl if OS is Windows 7 (or) WebView2 not installed.
                    try
                    {
                        string webviewver = CoreWebView2Environment.GetAvailableBrowserVersionString(null);
                    }
                    catch (Exception)
                    {
                        // WebView2 not installed.
                        GlobalMemory.getInstance().useRenderer = "WEBCONTROL";
                    }
                }

                switch (GlobalMemory.getInstance().useRenderer)
                {
                    case "WEBCONTROL":
                        // WebControl
                        Application.Run(new RendererWebBrowserControl());
                        break;
                    case "WEBVIEW2":
                        // WebView2
                        Application.Run(new RendererWebView2());
                        break;
                    default:
                        // default
                        Application.Run(new RendererWebView2());
                        break;
                }
            }
        }

        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            DialogResult result = DialogResult.Abort;
            try
            {
                result = Themes.MessageBox("An error has occured:\n\n"
                + e.Exception.Message /*+ e.Exception.StackTrace*/, "Application Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
            }
            finally
            {
                if (result == DialogResult.Abort)
                {
                    Application.Exit();
                }
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;

                Themes.MessageBox("An error has occured:\n\n" + ex.Message
                    /*+ ex.StackTrace*/, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                Application.Exit();
            }
        }
    }
}
