using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LoadLibrary
{
    class Program
    {

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "LoadLibrary")]
        static extern IntPtr LoadLibrary(string dllName);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, EntryPoint = "FreeLibrary")]
        static extern IntPtr FreeLibrary(IntPtr intPtr);

        delegate string AppMain(string a, string b);
        static void Main(string[] args)
        {
            //AppaDomin方式
            AppaDominLoadlibrary();
            //LoadLibrary方式
            AppLoadLibrary();
        }
        private static void AppLoadLibrary()
        {
            string path = $"{System.Environment.CurrentDirectory}\\AppDllExport.dll";
            if (File.Exists(path))
            {
                IntPtr intPtr = LoadLibrary(path);
                if (intPtr != null)
                {
                    IntPtr intPtr1 = GetProcAddress(intPtr, "AppMain");
                    AppMain appMain = (AppMain)Marshal.GetDelegateForFunctionPointer(intPtr1, typeof(AppMain));
                    string ret = appMain("你好", "LoadLibrary");
                    //对于使用dllexport的无法正常卸载
                    // FreeLibrary(intPtr);
                }
            }
        }
        private static void AppaDominLoadlibrary()
        {
            string path = $"{System.Environment.CurrentDirectory}\\AppDllExport.dll";
            if (File.Exists(path))
            {
                AppDomain appDomain = AppDomain.CreateDomain(path);
                Assembly assembly = appDomain.Load(new AssemblyName("AppDllExport"));
                object obj = assembly.CreateInstance("AppDllExport.AppRun");
                Type t = obj.GetType();
                string ret =  (string)t.InvokeMember("AppMain", BindingFlags.InvokeMethod, null, obj, new object[] { "你好", "AppDomain" });
                AppDomain.Unload(appDomain);
            }
        }
    }

    //本例子调用的dll内容参考
    //namespace AppDllExport
    //{
    //    public class AppRun
    //    {
    //        [DllExport(CallingConvention = CallingConvention.StdCall)]
    //        public static string AppMain(string a, string b)
    //        {
    //            return "return" + a + b;
    //        }
    //    }
    //}
}
