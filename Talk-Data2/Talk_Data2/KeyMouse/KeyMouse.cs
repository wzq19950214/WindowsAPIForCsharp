using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace GTR
{
    public class KeyMouse : KmHelper
    {
        /// <summary>
        /// 加载驱动
        /// </summary>
        /// <returns></returns>
        public static bool InitKeyMouse()
        {
            int  nRes = InitKeyMouseDriver();
            if (nRes != (int)(KmError.KE_Ok))
            {
                MessageBox.Show(string.Format("加载驱动失败. \r\n返回值[{0}]\r\n系统错误代码[{1}]",
                    nRes, WinApi.GetLastError()), "加载驱动", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 关闭驱动
        /// </summary>
        public static void CloseKeyMouse()
        {
            CloseKeyMouseDriver();
        }

        public static void Sendkey(char ch)
        {
            short sk = KmGetCharScanCode(ch);
            bool bShift = ((sk & 0x0100) == 0x0100);

            if (bShift)
            {
                KmSendExtentKey(WinApi.VK_SHIFT, true);
                Sleep(10);
            }

            KmSendKey((byte)sk, true);
            Sleep(20);
            KmSendKey((byte)sk, false);

            if (bShift)
            {
                Sleep(10);
                KmSendExtentKey(WinApi.VK_SHIFT, false);
            }
        }

        public static void SendKeys(string strText, int delayTime)
        {
            try
            {
                foreach (char ch in strText)
                {
                    Sendkey(ch);
                    Thread.Sleep(delayTime);
                }
            }
            catch
            { 
            }
        }

        public static void SendFunKey(char ch, bool ctrl, bool alt, bool shift, bool funKey)
        {
            if (ctrl) { KmSendExtentKey(WinApi.VK_CONTROL, true); Sleep(20); }
            if (alt) { KmSendExtentKey(WinApi.VK_MENU, true); Sleep(20); }
            if (shift) { KmSendExtentKey(WinApi.VK_SHIFT, true); Sleep(20); }
            if (!funKey) //输入字符
            {
                //获取字符(此处应过滤汉字)
                short sCode = KmGetCharScanCode(ch);
                KmSendKey((byte)sCode, true);
		        Sleep( 20 );
                KmSendKey((byte)sCode, false);
		        Sleep( 20 );
            }
            else
            {
                KmSendExtentKey(ch, true);
                Sleep(20);
                KmSendExtentKey(ch, false);
                Sleep(20);
            }
            if (ctrl) { KmSendExtentKey(WinApi.VK_CONTROL, false); Sleep(20); }
            if (alt) { KmSendExtentKey(WinApi.VK_MENU, false); Sleep(20); }
            if (shift) { KmSendExtentKey(WinApi.VK_SHIFT, false); Sleep(20); }
        }

        public static void SendTabKey()
        {
            KmSendExtentKey(WinApi.VK_TAB, true);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_TAB, false);
            Sleep(20);
        }

        public static void SendEnterKey()
        {
            KmSendExtentKey(WinApi.VK_RETURN, true);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_RETURN, false);
            Sleep(20);
        }

        public static void SendEscKey()
        {
            KmSendExtentKey(WinApi.VK_ESCAPE, true);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_ESCAPE, false);
            Sleep(20);
        }
        public static void SendDownKey() {
            KmSendExtentKey(WinApi.VK_DOWN, true);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_DOWN, false);
            Sleep(20);
        }
        public static void SendCtrlC()
        {
	        short	sCode	= KmGetCharScanCode( 'c' );

            KmSendExtentKey(WinApi.VK_CONTROL, true);
	        Sleep(20);
	        KmSendKey( (byte)sCode, true );
	        Sleep(20);
            KmSendKey((byte)sCode, false);
	        Sleep(20);
            KmSendExtentKey(WinApi.VK_CONTROL, false);
	        Sleep(20);
        }

        public static void SendCtrlPlus(char ch)
        {
            short sCode = KmGetCharScanCode(ch);

            KmSendExtentKey(WinApi.VK_CONTROL, true);
            Sleep(20);
            KmSendKey((byte)sCode, true);
            Sleep(20);
            KmSendKey((byte)sCode, false);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_CONTROL, false);
            Sleep(20);
        }

       

        public static void SendCtrlV()
        {
	        short	sCode	= KmGetCharScanCode( 'v' );

            KmSendExtentKey(WinApi.VK_CONTROL, true);
	        Sleep(20);
            KmSendKey((byte)sCode, true);
	        Sleep(20);
            KmSendKey((byte)sCode, false);
	        Sleep(20);
            KmSendExtentKey(WinApi.VK_CONTROL, false);
	        Sleep(20);
        }

        public static void SendCtrlA()
        {
            short sCode = KmGetCharScanCode('a');

            KmSendExtentKey(WinApi.VK_CONTROL, true);
            Sleep(20);
            KmSendKey((byte)sCode, true);
            Sleep(20);
            KmSendKey((byte)sCode, false);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_CONTROL, false);
            Sleep(20);
        }
         public static void SendAltAny(char a)
        {
            short sCode = KmGetCharScanCode(a);

            KmSendExtentKey(WinApi.VK_MENU, true);
            Sleep(20);
            KmSendKey((byte)sCode, true);
            Sleep(20);
            KmSendKey((byte)sCode, false);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_MENU, false);
            Sleep(20);
        }
        public static void SendWinR()
        {
            short sCode = KmGetCharScanCode('r');

            KmSendExtentKey(WinApi.VK_LWIN, true);
            Sleep(20);
            KmSendKey((byte)sCode, true);
            Sleep(20);
            KmSendKey((byte)sCode, false);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_LWIN, false);
            Sleep(20);
        }

        public static void SendBackSpaceKey(int times)
        {
            for (int i = 0; i < times; i++)
            {
                SendBackSpaceKey();
                Sleep(30);
            }
        }

        public static void SendBackSpaceKey()
        {
            KmSendExtentKey(WinApi.VK_BACK, true);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_BACK, false);
            Sleep(20);
        }
        public static void SendF5Key()
        {
            KmSendExtentKey(WinApi.VK_F5, true);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_F5, false);
            Sleep(20);
        }

        public static void SendDeleteKey()
        {
	        //KmSendKeyEx( 0x53, IS0xE0 );
	        KmSendExtentKey( WinApi.VK_DELETE, true );
	        Sleep(20);
            KmSendExtentKey(WinApi.VK_DELETE, false);
	        Sleep(20);
        }

        private static void Sleep(int time)
        {
            Thread.Sleep(time);
        }
        

        public static void MouseMove(IntPtr hWnd, Point pt)
        {
            if (hWnd != IntPtr.Zero)
            {
                WinApi.RECT rect;
                WinApi.GetWindowRect(hWnd, out rect);
                pt.X += rect.left;
                pt.Y += rect.top;
            }
            KmMouseMove(pt.X, pt.Y, true, true);
            //KmMouseMove((uint)pt.X, (uint)pt.Y, true, true);
            Sleep(50);
        }

        public static void MouseMove(RECT rect,int x, int y)
        {
            x += rect.left;
            y += rect.top;
            KmMouseMove(x, y, true, true);
            //KmMouseMove((uint)x, (uint)y, true, true);
            Sleep(50);
        }
        public static void MouseMove(int x,int y)
        {
            KmMouseMove(x, y, true, true);
            //KmMouseMove((uint)x, (uint)y, true, true);
            Sleep(50);
        }

        public static void MouseMove(IntPtr hWnd, int x, int y)
        {
            if (hWnd != IntPtr.Zero)
            {
                WinApi.RECT rect;
                WinApi.GetWindowRect(hWnd, out rect);
                x += rect.left;
                y += rect.top;
            }
            KmMouseMove(x, y, true, true);
            //KmMouseMove((uint)x, (uint)y, true, true);
            Sleep(50);
        }

        public static void MouseMove(int x,int y,bool a,bool b)
        {
            KmMouseMove(x, y, a, b);
            //KmMouseMove((uint)x, (uint)y, a, b);
        }


        public static void MouseClick(int x,int y,uint tb,uint tc,int time)
        {
            MouseClick(IntPtr.Zero, x, y, tb, tc);
            Sleep(time);
        }

        public static void MouseClick(IntPtr hWnd, Point pt, uint tb, uint tc)
        {
            MouseClick(hWnd, pt, (TYPE_BUTTON)tb, (TYPE_CLICK)tc);
        }

        public static void MouseLeftDown()
        {
            KmMouseKey(true, true);
        }

        public static void MouseLeftUp()
        {
            KmMouseKey(true, false);
        }
        public static void MouseDrag(IntPtr hWnd,int b,int p,int m,int f)
        {
            MouseMove(b, p);
            Sleep(1000);
            KmMouseKey(true ,true);
            Sleep(100);
            MouseMove(m, f);
            Sleep(100);
            KmMouseKey(true, false);
        }
        public static void MouseClick(IntPtr hWnd, Point pt, TYPE_BUTTON tb, TYPE_CLICK tc)
        {
            int x = pt.X;
            int y = pt.Y;
            if (hWnd != IntPtr.Zero)
            {
                WinApi.RECT rect;
                WinApi.GetWindowRect(hWnd, out rect);
                //x += rect.left;
                //y += rect.top;
            }
            KmMouseMove(x, y, true, true);
            Sleep(100);
            MouseClick(tb, tc);

        }
        public static void MouseClick(TYPE_BUTTON tb, TYPE_CLICK tc)
        {
            bool flagLb = true;
           
            if (tb == TYPE_BUTTON.R_BUTTON)
                flagLb = false;
         
            //KmMouseMove(x, y, true, true);
            //Sleep(500);
            switch (tc)
            {
                case TYPE_CLICK.BUTTON_CLICK:
                    
                    KmMouseKey(flagLb, true);
                    Sleep(100);
                    KmMouseKey(flagLb, false);
                    Sleep(10);
                    break;
                case TYPE_CLICK.BUTTON_DOUBLE_CLICK:
                    KmMouseKey(flagLb, true);
                    Sleep(10);
                    KmMouseKey(flagLb, false);
                    Sleep(50);
                    KmMouseKey(flagLb, true);
                    Sleep(10);
                    KmMouseKey(flagLb, false);
                    Sleep(10);
                    break;
                case TYPE_CLICK.BUTTON_DOWN:
                    KmMouseKey(flagLb, true);
                    Sleep(10);
                    break;
                case TYPE_CLICK.BUTTON_UP:
                    KmMouseKey(flagLb, false);
                    Sleep(10);
                    break;
                default:
                    break;
            }
        }
        public static void MouseClick(IntPtr hWnd, int x, int y, TYPE_BUTTON tb, TYPE_CLICK tc)
        {
            MouseClick(hWnd, new Point(x, y), tb, tc);
        }
        public static void MouseClick(IntPtr hWnd, int x, int y, uint tb, uint tc,int time)
        {
            MouseClick(hWnd, new Point(x, y), tb, tc);
            Sleep(time);
        }
        public static void MouseClick(IntPtr hWnd, int x, int y, uint tb, uint tc)
        {
            MouseClick(hWnd, new Point(x, y), (TYPE_BUTTON)tb, (TYPE_CLICK)tc);
        }
        public static void PressMouseKey(IntPtr hWnd, Point pt)
        {
            MouseMove(hWnd, pt);
            KmMouseKey(true, true);
            Sleep(50);
            KmMouseKey(true, false);
            Sleep(50);
        }
        public static void PressMouseKey(IntPtr hWnd, int x, int y)
        {
            MouseMove(hWnd, x,y);
            KmMouseKey(true, true);
            Sleep(50);
            KmMouseKey(true, false);
            Sleep(50);
        }
        //左键单击
        public static void PressMouseKey()
        {
            KmMouseKey(true, true);
            Sleep(50);
            KmMouseKey(true, false);
            Sleep(50);
        }
        public static void PressMousekeyDouble(int times)
        {
            KmMouseKey(true, true);
            Sleep(50);
            KmMouseKey(true, false);
            Sleep(100);
            KmMouseKey(true, true);
            Sleep(50);
            KmMouseKey(true, false);
            Sleep(50);
        }
        public static void WenDaoMouse(IntPtr hwnd, int x, int y)
        {
            IntPtr desk = User32API.GetDesktopWindow();
            //User32API.SetForegroundWindow(desk);
            //User32API.ShowWindow(hwnd, ShowWindowCmd.SW_MINIMIZE);
            //User32API.ShowWindow(desk, ShowWindowCmd.SW_SHOWNORMAL);
            KeyMouse.SendWin();
            Sleep(200);
            KeyMouse.MouseMove(x, y);
            Sleep(200);
            //User32API.SetForegroundWindow(hwnd);
            //User32API.ShowWindow(hwnd, ShowWindowCmd.SW_SHOWNORMAL);
            KeyMouse.SendWin();
            Sleep(500);
            KeyMouse.MouseLeftDown();
            Sleep(50);
            KeyMouse.MouseLeftUp();
            Sleep(500);
        }
        public static void SendWin()
        {
            KmSendExtentKey(WinApi.VK_LWIN, true);
            Sleep(20);
            KmSendExtentKey(WinApi.VK_LWIN, false);
            Sleep(20);
        }
        public static void PressMousekeyDouble(IntPtr hWnd, int x, int y)
        {
            MouseMove(hWnd, x, y);
            KmMouseKey(true, true);
            Sleep(50);
            KmMouseKey(true, false);
            Sleep(100);
            KmMouseKey(true, true);
            Sleep(50);
            KmMouseKey(true, false);
            Sleep(50);
        }

        public enum TYPE_CLICK : uint
        {
            BUTTON_CLICK = 1,		//单击
            BUTTON_DOUBLE_CLICK,		//双击
            BUTTON_DOWN,				//按下
            BUTTON_UP					//弹起
        }
        public enum TYPE_BUTTON : uint
        {
            L_BUTTON = 1,			//左键
            R_BUTTON,					//右键
            M_BUTTON					//中键
        }
        
    }
    public class WinApi
    {
        public static UInt32 VK_ESCAPE = 0x1B;
        public static UInt32 VK_SHIFT = 0x10;
        public static UInt32 VK_CONTROL = 0x11;
        public static UInt32 VK_MENU = 0x12;
        public static UInt32 VK_LEFT = 0x25;
        public static UInt32 VK_UP = 0x26;
        public static UInt32 VK_RIGHT = 0x27;
        public static UInt32 VK_RETURN = 0x0D;
        public static UInt32 VK_BACK = 0x08;
        public static UInt32 VK_TAB = 0x09;
        public static UInt32 VK_DOWN = 0x28;
        public static UInt32 VK_PRINT = 0x2A;
        public static UInt32 VK_DELETE = 0x2E;
        public static UInt32 VK_LWIN = 0x5B;
        public static UInt32 VK_F5 = 0x74;

        [DllImport("Kernel32.dll", EntryPoint = "GetLastError", CallingConvention = CallingConvention.Winapi)]
        public static extern UInt32 GetLastError();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pt);

        [DllImport("user32.dll", EntryPoint = "GetWindowRect")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
    }
}
