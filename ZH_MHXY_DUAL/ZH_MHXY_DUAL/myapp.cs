using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace RC_ZH_LOL
{
    class myapp
    {

        [DllImport("user32.dll")]
        public static extern void ClientToScreen(IntPtr hWnd, ref Point p);
        private delegate bool WNDENUMPROC(IntPtr hWnd, int lParam);
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, int lParam);
        [DllImport("user32.dll")]
        private static extern int GetWindowTextW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        private static extern int GetClassNameW(IntPtr hWnd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpString, int nMaxCount);
        [DllImport("user32")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        public struct WindowInfo
        {
            public IntPtr hWnd;
            public string szWindowName;
            public string szClassName;
        }
        /// <summary>
        /// 遍历方法 - 返回一个List<WindowInfo> 集合
        /// </summary>
        /// <returns></returns>
        public static List<WindowInfo> GetAllDesktopWindows()
        {
            List<WindowInfo> wndList = new List<WindowInfo>();

            //enum all desktop windows
            EnumWindows(delegate(IntPtr hWnd, int lParam)
            {
                WindowInfo wnd = new WindowInfo();
                StringBuilder sb = new StringBuilder(256);
                wnd.hWnd = hWnd;
                GetWindowTextW(hWnd, sb, sb.Capacity);
                wnd.szWindowName = sb.ToString();
                GetClassNameW(hWnd, sb, sb.Capacity);
                wnd.szClassName = sb.ToString();
                wndList.Add(wnd);
                return true;
            }, 0);

            return wndList;
        }
        /// <summary>
        /// 塞选方法 - 返回满足条件的句柄集合
        /// </summary>
        /// <param name="textName">窗口标题 - 为空不匹配标题( 模糊匹配，不区分大小写)</param>
        /// <param name="textClass">窗口类名 - 为空不匹配类名( 模糊匹配，不区分大小写)</param>
        /// <returns></returns>
        public static List<int> EnumWindow(string textName, string textClass)
        {
            CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;
            List<int> gethwnd = new List<int>();
            //先取得所有句柄
            List<WindowInfo> Listwindows = GetAllDesktopWindows();
            //
            foreach (WindowInfo wdf in Listwindows)
            {
                if (textName != "" && textClass != "")
                {
                    if (Compare.IndexOf(wdf.szClassName.ToUpper(), textClass.ToUpper()) > -1 && Compare.IndexOf(wdf.szWindowName.ToUpper(), textName.ToUpper()) > -1)
                    {
                        gethwnd.Add((int)wdf.hWnd);
                    }
                }
                else if (textName == "" && textClass == "")
                {
                    gethwnd.Add((int)wdf.hWnd);
                    //都返回
                }
                else if (textName == "")
                {
                    //只匹配类名
                    if (Compare.IndexOf(wdf.szClassName.ToUpper(), textClass.ToUpper()) > -1)
                    {
                        gethwnd.Add((int)wdf.hWnd);
                    }
                }
                else if (textClass == "")
                {
                    //只匹配标题
                    if (Compare.IndexOf(wdf.szWindowName.ToUpper(), textName.ToUpper()) > -1)
                    {
                        gethwnd.Add((int)wdf.hWnd);
                    }
                }
            }
            return gethwnd;
        }
        public static void RunBat(string batPath)
        {
            Process pro = new Process();
            FileInfo file = new FileInfo(batPath);
            pro.StartInfo.WorkingDirectory = file.Directory.FullName;
            pro.StartInfo.FileName = batPath;
            pro.StartInfo.CreateNoWindow = false;
            pro.Start();
        }
        public delegate bool EnumDesktopWindowsDelegate(IntPtr hWnd, uint lParam);
        [DllImport("user32.dll", EntryPoint = "EnumDesktopWindows", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]

        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDesktopWindowsDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.Dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr parentHandle, EnumChildWindowsDelegate callback, IntPtr lParam);
        public delegate bool EnumChildWindowsDelegate(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        /// <summary>
        /// 已知主窗口句柄查找指定子窗口句柄
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="className"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static List<IntPtr> FindControl(IntPtr hwnd, string className, string title = null)
        {
            List<IntPtr> controls = new List<IntPtr>();
            IntPtr handle = IntPtr.Zero;
            while (true)
            {
                IntPtr tmp = handle;
                handle = FindWindowEx(hwnd, tmp, className, title);
                if (handle != IntPtr.Zero)
                {
                    controls.Add(handle);
                }
                else
                    break;
            }
            return controls;
        }
      
    }
}
