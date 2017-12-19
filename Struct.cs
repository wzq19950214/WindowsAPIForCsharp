﻿using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GTR
{
    #region INPUT
#pragma warning disable 649
    /// <summary>
    /// The INPUT structure is used by SendInput to store information for synthesizing input events such as keystrokes, mouse movement, and mouse clicks.
    /// (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    /// <remarks>
    /// This structure contains information identical to that used in the parameter list of the keybd_event or mouse_event function.
    /// Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard input methods, such as handwriting recognition or voice recognition,
    /// as if it were text input by using the KEYEVENTF_UNICODE flag.
    /// For more information, see the remarks section of KEYBDINPUT.
    /// </remarks>
    public struct INPUT
    {
        /// <summary>
        /// Specifies the type of the input event. This member can be one of the following values. 
        /// <see cref="InputType.Mouse"/> - The event is a mouse event. Use the mi structure of the union.
        /// <see cref="InputType.Keyboard"/> - The event is a keyboard event. Use the ki structure of the union.
        /// <see cref="InputType.Hardware"/> - Windows 95/98/Me: The event is from input hardware other than a keyboard or mouse. Use the hi structure of the union.
        /// </summary>
        public UInt32 Type;
        /// <summary>
        /// The data structure that contains information about the simulated Mouse, Keyboard or Hardware event.
        /// </summary>
        public MOUSEKEYBDHARDWAREINPUT Data;
    }

    /// <summary>
    /// The combined/overlayed structure that includes Mouse, Keyboard and Hardware Input message data 
    /// (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MOUSEKEYBDHARDWAREINPUT
    {
        /// <summary>
        /// The <see cref="MOUSEINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public MOUSEINPUT mi;

        /// <summary>
        /// The <see cref="KEYBDINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public KEYBDINPUT ki;

        /// <summary>
        /// The <see cref="HARDWAREINPUT"/> definition.
        /// </summary>
        [FieldOffset(0)]
        public HARDWAREINPUT hi;
    }
    /// <summary>
    /// 此结构包含了一个模拟键盘事件的信息
    /// </summary>
    public struct KEYBDINPUT
    {
        /// <summary>
        /// 指定虚拟键值码(1-254).如果dwFlags指定了KEYEVENTF_UNICODE,此值必须为0
        /// </summary>
        public UInt16 wVk;

        /// <summary>
        /// 指定扫描码,如果dwFlags指定了KEYEVENTF_UNICODE,此参数指定一个unicode字符,来发送给 foreground application(前台应用程序).
        /// </summary>
        public UInt16 wScan;

        /// <summary>
        /// 可选值,可以组合:
        /// KEYEVENTF_EXTENDEDKEY 如果指定了此标志,扫描码将被加上一个前缀字节0xE0(224).
        /// KEYEVENTF_KEYUP 如果指定了此标志,该键正在被释放,如果没有指定,该键正在被按下.
        /// KEYEVENTF_SCANCODE 如果指定了此标志,由wScan指定键,忽略wVK. 
        /// KEYEVENTF_UNICODE 如果指定了此标志,系统将合成一个VK_PACKET按键,wVK必须为0,此标志只能与KEYEVENTF_KEYUP标志组合. 
        /// </summary>
        public UInt32 dwFlags;

        /// <summary>
        /// 毫秒单位的时间戳.如果置0,系统将提供. 
        /// </summary>
        public UInt32 time;

        /// <summary>
        /// 附加信息. 
        /// </summary>
        public IntPtr dwExtraInfo;
    }
    /// <summary>
    /// 此结构包含了一个模拟鼠标事件的信息
    /// </summary>
    public struct MOUSEINPUT
    {
        /// <summary>
        /// 指定x轴绝对坐标或相对坐标(相对于最后一次鼠标事件的坐标)
        /// </summary>
        public Int32 dx;

        /// <summary>
        /// 指定y轴绝对坐标或相对坐标(相对于最后一次鼠标事件的坐标)
        /// </summary>
        public Int32 dy;

        /// <summary>
        /// 如果指定了MOUSEEVENTF_WHEEL,此参数包含一个正数或负数,描述了滚轮向前或向后滚动的距离;
        /// 如果指定了MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP,此参数将指示哪一个x button被按下或被释放,可选值:
        /// XBUTTON1:第一个x button被按下或释放
        /// XBUTTON2:第二个x button被按下或释放
        /// <see cref="WinAPI.Enums.XButton"/>
        /// </summary>
        public UInt32 mouseData;

        /// <summary>
        /// 可以是下列值的组合(MOUSEEVENTF_WHEEL不能和MOUSEEVENTF_XDOWN或MOUSEEVENTF_XUP组合)
        /// The <see cref="WinAPI.Enums.MouseEventType"/> definition.
        /// </summary>
        public UInt32 dwFlags;

        /// <summary>
        /// 毫秒单位的事件时间戳,如果置0将由系统提供. 
        /// </summary>
        public UInt32 time;

        /// <summary>
        /// 附加信息
        /// 如果指定了MOUSEEVENTF_ABSOLUTE,dx,dy将包含0-65535的绝对坐标,它将被映射为显示器的坐标(0,0)表示左上角坐标,(65535,65535)表示右下角坐标
        ///如果没有指定MOUSEEVENTF_ABSOLUTE,dx,dy将包含相对坐标,正数表示鼠标向右或向下移动,负数表示鼠标向上或向左移动
        /// </summary>
        public IntPtr dwExtraInfo;
    }

    /// <summary>
    /// The HARDWAREINPUT structure contains information about a simulated message generated by an input device other than a keyboard or mouse. 
    /// (see: http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx)
    /// Declared in Winuser.h, include Windows.h
    /// </summary>
    public struct HARDWAREINPUT
    {
        /// <summary>
        /// Value specifying the message generated by the input hardware. 
        /// </summary>
        public UInt32 uMsg;
        /// <summary>
        /// Specifies the low-order word of the lParam parameter for uMsg. 
        /// </summary>
        public UInt16 wParamL;
        /// <summary>
        /// Specifies the high-order word of the lParam parameter for uMsg. 
        /// </summary>
        public UInt16 wParamH;
    }
#pragma warning restore 649
    #endregion

    public struct tagPoint
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public  RECT(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
        /// <summary>
        /// 转换到Rectangle类.
        /// </summary>
        /// <returns></returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle(left, top, right - left, bottom - top);
        }
        public int Width
        {
            get { return right - left; }
        }

        public int Height
        {
            get { return bottom - top; }
        }

    }

    /// <summary>
    ///  结构中包含了有关窗口在屏幕上位置的信息.
    /// </summary>
    public struct WINDOWPLACEMENT
    {

        #region 常量
        /// <summary>
        /// 指定窗口最小化时的x位置和y位置.
        /// </summary>
        public const uint WPF_SETMINPOSITION = 0x0001;
        /// <summary>
        /// 指定窗口以最大化方式还原，尽管可能窗口并不是在最大化时最小化的。不改变窗口的缺省还原方式.
        /// </summary>
        public const uint WPF_RESTORETOMAXIMIZED = 0x0002;
        #endregion
        /// <summary>
        /// 指定了结构的长度，以字节为单位.
        /// </summary>
        public uint length;
        /// <summary>
        /// 指定了控制最小化窗口的位置的标志以及复原窗口的方法.
        /// WPF_SETMINPOSITION和
        /// WPF_RESTORETOMAXIMIZED.
        /// </summary>
        public uint flags;
        /// <summary>
        /// 指定窗口的当前显示状态.
        /// <see cref="WinAPI.Enums.ShowWindowCmd"/>
        /// </summary>
        public uint showCmd;
        /// <summary>
        /// 指定窗口最小化时的左上角坐标.
        /// </summary>
        public tagPoint ptMinPosition;
        /// <summary>
        /// 指定窗口最大化时的左上角坐标.
        /// </summary>
        public tagPoint ptMaxPosition;
        /// <summary>
        /// 指定窗口在还原时的坐标.
        /// </summary>
        public RECT rcNormalPosition;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct tagCURSORINFO
    {

        /// DWORD->unsigned int
        public uint cbSize;

        /// DWORD->unsigned int
        public uint flags;

        /// HCURSOR->HICON->HICON__*
        public System.IntPtr hCursor;

        /// POINT->tagPOINT
        public tagPoint ptScreenPos;
    }

    #region GDI+
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct tagRGBQUAD
    {
        /// BYTE->unsigned char
        public byte rgbBlue;
        /// BYTE->unsigned char
        public byte rgbGreen;
        /// BYTE->unsigned char
        public byte rgbRed;
        /// BYTE->unsigned char
        public byte rgbReserved;
    }
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct tagBITMAPINFOHEADER
    {

        /// DWORD->unsigned int
        public uint biSize;

        /// LONG->int
        public int biWidth;

        /// LONG->int
        public int biHeight;

        /// WORD->unsigned short
        public ushort biPlanes;

        /// WORD->unsigned short
        public ushort biBitCount;

        /// DWORD->unsigned int
        public uint biCompression;

        /// DWORD->unsigned int
        public uint biSizeImage;

        /// LONG->int
        public int biXPelsPerMeter;

        /// LONG->int
        public int biYPelsPerMeter;

        /// DWORD->unsigned int
        public uint biClrUsed;

        /// DWORD->unsigned int
        public uint biClrImportant;
    }
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct tagBITMAPINFO
    {
        /// BITMAPINFOHEADER->tagBITMAPINFOHEADER
        public tagBITMAPINFOHEADER bmiHeader;
        /// RGBQUAD[1]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct)]
        public tagRGBQUAD[] bmiColors;
    }
    #endregion

    [StructLayout(LayoutKind.Sequential)]
    public struct ProcessEntry32
    {
        /// <summary>
        /// 结构的大小
        /// </summary>
        public uint dwSize;
        /// <summary>
        /// 此进程的引用计数
        /// </summary>
        public uint cntUsage;
        /// <summary>
        /// 进程的PID
        /// </summary>
        public uint th32ProcessID;
        /// <summary>
        /// 0
        /// </summary>
        public IntPtr th32DefaultHeapID;
        /// <summary>
        /// 0
        /// </summary>
        public uint th32ModuleID;
        /// <summary>
        /// 进程开启的线程计数(这个成员执行线程开始的进程)
        /// </summary>
        public uint cntThreads;
        /// <summary>
        /// 父进程的PID
        /// </summary>
        public uint th32ParentProcessID;
        /// <summary>
        /// 当前进程创建的任何一个线程的基础优先级，即在当前进程内创建线程的话，其基本优先级的值
        /// </summary>
        public int pcPriClassBase;
        /// <summary>
        /// 0
        /// </summary>
        public uint dwFlags;
        /// <summary>
        /// （进程的可执行文件名称。
        /// 要获得可执行文件的完整路径，应调用Module32First函数，再检查其返回的MODULEENTRY32结构的szExePath成员。
        /// 但是，如果被调用进程是一个64位程序，您必须调用QueryFullProcessImageName函数去获取64位进程的可执行文件完整路径名。）
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SHFILEINFO
    {

        /// HICON->HICON__*
        public System.IntPtr hIcon;

        /// int
        public int iIcon;

        /// DWORD->unsigned int
        public uint dwAttributes;

        /// WCHAR[260]
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;

        /// WCHAR[80]
        [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

}
