using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using GTR;

namespace RC2APP
{
    public partial class SplashTip : Form
    {
        private string strTip;
        private uint outTime;
        private Color colorFont;
        private string fontName;
        private int fontSize;

        public SplashTip(string strTip, Color colorFont, uint outTime, string fontName, int fontSize)
        {
            this.strTip = strTip;
            this.colorFont = colorFont;
            this.outTime = outTime;
            this.fontName = fontName;
            this.fontSize = fontSize;

            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ShowInTaskbar = false;

            InitializeComponent();
        }

        //重写消息循环
        //protected override void WndProc(ref Message m)
        //{
        //    const int WM_PAINT = 0x000F;
        //    switch (m.Msg)
        //    {
        //        case WM_PAINT:
        //            User32.SetForegroundWindow(this.Handle);
        //            break;
        //    }
        //    base.WndProc(ref m);
        //}

        //延时，单位(毫秒)
        private static bool Delay(uint ms)
        {
            uint start = GetTickCount();
            while (GetTickCount() - start < ms)
            {
                Application.DoEvents();
            }
            return true;
        }

        private void SetText(string strTip, int fontSize, string fontName)
        {
            IntPtr dc = GetDC(this.Handle);
            //创建字体
            IntPtr m_Font = CreateFont(fontSize, 0, 0, 0, FW_DONTCARE, 0, 0, 0, ANSI_CHARSET,
                OUT_DEFAULT_PRECIS, CLIP_DEFAULT_PRECIS, DEFAULT_QUALITY, DEFAULT_PITCH | FF_SWISS, fontName);
            //开始记录窗体轮廓路径
            BeginPath(dc);
            //设置背景为透明模式,这是必须有的
            SetBkMode(dc, TRANSPARENT);
            IntPtr m_OldFont = SelectObject(dc, m_Font);
            TextOut(dc, 0, 0, strTip, strTip.Length * 2);
            SelectObject(dc, m_OldFont);
            //结束记录窗体轮廓路径
            EndPath(dc);
            //把所记录的路径转化为窗体轮廓句柄
            IntPtr m_wndRgn = PathToRegion(dc);
            //赋予窗体指定的轮廓形状
            SetWindowRgn(this.Handle, m_wndRgn, true);
            ReleaseDC(this.Handle, dc);
        }
        
        private void SetWindows()
        {
            //this.strTip = "游戏截图已保存";
            //this.fontSize = 100;
            //this.fontName = "宋体";
            //this.colorFont = Color.FromArgb(0, 255, 0);
            SetText(strTip, fontSize, fontName);
            this.Width = fontSize * strTip.Length;
            this.Height = fontSize;
            //设置窗体的背景颜色
            this.BackColor = this.colorFont;
            int x = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            int y = Screen.PrimaryScreen.WorkingArea.Height - this.Height;
            this.Location = new Point(x / 2, y / 2);
        }

        #region [WinAPI Handler]
        [DllImport("gdi32")]
        private static extern IntPtr CreateFont(int H, int W, int E, int O, int FW, int I, int u, int S, int C, int OP, int CP, int Q, int PAF, string F);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32")]
        private static extern IntPtr BeginPath(IntPtr hdc);
        [DllImport("gdi32")]
        private static extern IntPtr EndPath(IntPtr hdc);
        [DllImport("user32.dll")]
        public extern static IntPtr GetDesktopWindow();
        [DllImport("gdi32")]
        private static extern IntPtr PathToRegion(IntPtr hdc);
        [DllImport("gdi32")]
        private static extern int SetBkMode(IntPtr hdc, int nBkMode);
        [DllImport("gdi32")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);
        [DllImport("gdi32")]
        private static extern int TextOut(IntPtr hdc, int x, int y, string lpString, int nCount);
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowRgn(IntPtr hwnd, IntPtr hRgn, bool bRedraw);

        private const int FW_DONTCARE = 0;
        private const int FW_BOLD = 700;
        private const int FW_HEAVY = 900;
        private const int ANSI_CHARSET = 0;
        private const int OUT_DEFAULT_PRECIS = 0;
        private const int CLIP_DEFAULT_PRECIS = 0;
        private const int DEFAULT_QUALITY = 0;
        private const int DEFAULT_PITCH = 0;
        private const int FF_SWISS = 32;
        private const int TRANSPARENT = 1;

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern uint GetTickCount();
        #endregion

        private void SplashTip_Load(object sender, EventArgs e)
        {
            GTR.User32API.SetForegroundWindow(this.Handle);
            SetWindows();
        }

        private void SplashTip_Shown(object sender, EventArgs e)
        {
            Delay(this.outTime);
            this.Close();
        }
    }
}
