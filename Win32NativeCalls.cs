using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Simple_Bible_Reader
{
    public class Win32NativeCalls
    {
        [FlagsAttribute]
        public enum EXECUTION_STATE : uint
        {
            ES_AWAYMODE_REQUIRED = 0x00000040,
            ES_CONTINUOUS = 0x80000000,
            ES_DISPLAY_REQUIRED = 0x00000002,
            ES_SYSTEM_REQUIRED = 0x00000001
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern EXECUTION_STATE SetThreadExecutionState(EXECUTION_STATE esFlags);

        [DllImport("ntdll.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr WineGetVersion();

        // Adpated from https://stackoverflow.com/questions/38282770/stop-screensaver-programmatically

        public static void preventScreensaver()
        {
            // To stop screen saver and monitor power off event
            SetThreadExecutionState(EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_CONTINUOUS);
        }

        public static void restoreScreensaver()
        {
            //To reset or allow those event again you have to call this API with only ES_CONTINUOUS
            SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);//This will reset as normal
        }

        // adapted from C++ code below URL
        //https://stackoverflow.com/questions/7372388/determine-whether-a-program-is-running-under-wine-at-runtime
        /*
        public static bool isRunningOnWine()
        {
            try
            {
                WineGetVersion pwine_get_version;
                IntPtr hntdll = GetModuleHandle("ntdll.dll");
                if (hntdll == IntPtr.Zero)
                {
                    //Console.WriteLine("Not running on NT.");
                    return false;
                }

                IntPtr functionPtr = GetProcAddress(hntdll, "wine_get_version");
                if (functionPtr != IntPtr.Zero)
                {
                    pwine_get_version = Marshal.GetDelegateForFunctionPointer<WineGetVersion>(functionPtr);
                    //Console.WriteLine("Running on Wine... " + Marshal.PtrToStringAnsi(pwine_get_version()));
                    return true;
                }
                else
                {
                    //Console.WriteLine("Did not detect Wine.");
                    return false;
                }
            }
            catch (Exception)
            {
                //some exception. try registry method
                return isRunningOnWineRegistry();
            }
        }
        */

        public static bool isRunningOnWine()
        {
            RegistryKey rkSubKey = Registry.CurrentUser.OpenSubKey(@"Software\Wine", false);
            if (rkSubKey != null)
                return true;
            else
                return false;
        }
    }
}
