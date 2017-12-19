using System;
using System.Runtime;
using System.Runtime.InteropServices;

// 使用者在代码中引用此命名空间就可以
namespace GTR
{
	// 调用此类的静态成员函数
	public class KmHelper
	{
        protected enum KmError
		{
			KE_Ok	= 0,
			KE_NotDriverService				= -1,	// 未安装驱动
			KE_StartDriverFailure			= -2,	// 启动驱动服务失败
			KE_OutOfMemory					= -3,	// 程序内部错误. ( 如: 申请内存失败之类 )
			KE_InvalidParameter				= -4,	// 参数错误
			KE_LoadResourceDriverFailure	= -5,	// 加载资源驱动失败
			KE_NotSupportCurrentOS			= -6,	// 驱动不支持当前操作系统
			KE_SaveDriverFileFailure		= -7,	// 保存驱动至当前DLL所在目录时失败
			KE_FileNotExists				= -8,	// 指定的驱动文件不存在
			KE_InstallDriverFailure			= -9,	// 安装驱动失败
		};

        protected static UInt32 KEY_MAKE = 0;
        protected static UInt32 KEY_BREAK = 1;
        protected static UInt32 KEY_E0 = 2;
        protected static UInt32 KEY_E1 = 4;

		[DllImport( "KeyMouseHelper.dll", EntryPoint="InitKeyMouseDriver", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Winapi )]
        protected static extern Int32 InitKeyMouseDriver();

		[DllImport( "KeyMouseHelper.dll", EntryPoint="CloseKeyMouseDriver", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Winapi )]
        protected static extern void CloseKeyMouseDriver();

		[DllImport( "KeyMouseHelper.dll", EntryPoint="KmSendKey", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Winapi )]
        protected static extern bool KmSendKey(byte ScanCode, bool bKeyDown);

		[DllImport( "KeyMouseHelper.dll", EntryPoint="KmSendExtentKey", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Winapi )]
        protected static extern bool KmSendExtentKey(UInt32 uFuncKey, bool bKeyDown);

		[DllImport( "KeyMouseHelper.dll", EntryPoint="KmMouseKey", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Winapi )]
        protected static extern bool KmMouseKey(bool bLeftButton, bool bButtonDown);

		[DllImport( "KeyMouseHelper.dll", EntryPoint="KmMouseMove", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Winapi )]
        protected static extern bool KmMouseMove(int x, int y, bool bIsAbsolute, bool bScreenCoordinate);

		[DllImport( "KeyMouseHelper.dll", EntryPoint="WiReadPort", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Winapi )]
        protected unsafe static extern bool WiReadPort(UInt16 PortAddr, UInt32* pPortValue, byte bSize);
       // protected static extern bool WiReadPort(UInt16 PortAddr, IntPtr pPortValue, byte bSize);

		[DllImport( "KeyMouseHelper.dll", EntryPoint="WiWritePort", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Winapi )]
        protected static extern bool WiReadPort(UInt16 PortAddr, UInt32 PortValue, byte bSize);

		[DllImport( "KeyMouseHelper.dll", EntryPoint="KmGetCharScanCode", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Winapi )]
        protected static extern Int16 KmGetCharScanCode(Char chKey);

		[DllImport( "KeyMouseHelper.dll", EntryPoint="KmGetExtentScanCode", CharSet=CharSet.Ansi, CallingConvention=CallingConvention.Winapi )]
        protected static extern UInt32 WiReadPort(UInt32 uFuncKey);
	}
}