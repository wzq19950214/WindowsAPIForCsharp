using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging; //ImageFormat.Jpeg
using System.Web;
using System.Web.Security;
using System.Net;
using System.Net.Sockets;
using System.Management;
using System.Text.RegularExpressions;//正则
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;

namespace GTR
{
    class QJSJ2
    {

        public static int m_sign;
        //订单数据
        public static string m_strOrderData;
        //人工干预返回RC2数据
        public static string  m_strRC2Data;
        //人工干预操作人
        public static string m_strDeliverId="";
        //允许人工干预
        public static bool m_bHumanAgree = false;
        //确认人工干预
        public static bool m_bHuman = false;
        //udpsockets
        public static udpSockets udpdd;
        //验证码返回
        public static string mouseP = "";
        //验证码回答次数
        public int yzmTimes = 0;
        //udp端口
        public static int m_UDPPORT = 6902;
        //脚本端口
        public static int the_nRC2Port = 0;
        //订单类型-交易单/发布单
        public static string m_strOrderType;
        //订单号
        public static string OrdNo = "测试订单"; //"MZH-160607000000001";
        //订单状态
        public int Status;
        //日志
        public static string strAllog = "...";

        //截图默认尺寸
        public const int Lwidth = 1280;
        public const int Lheight = 1200;
        public const int nZHPicWidth = 880;
        //拼图是否已满
        public bool bPicFull = false;
        public Point ptBigPic;
        //装备截图尺寸
        public Point ptMAX;
        static int PicNum = 1;
        static string strLastPicID = "";


        //同意人工干预
        public static bool m_bHumanFinish = false;
        //答题标志
        public static bool IsAnswer =false;
        //邮寄标志
        public static bool IsAskMail = false;

        public static bool IntoOrder = false;
        //移交标志
        public static bool bYiJiao = false;
        //进入游戏标志
        public static bool IfEnter = false;
        //M站点订单标志
        public static bool MZH = false;

        //窗口句柄
        public static IntPtr m_hGameWnd;
        //程序所在路径
        private string m_strProgPath = System.Windows.Forms.Application.StartupPath;
        //匹配图片路径
        public static string m_strPicPath = System.Windows.Forms.Application.StartupPath + @"\pic\";
        //异常截图保存路径
        public static string LocalPicPath ="E:\\piccQQSJ\\";

        //订单详细数据

        public string m_GameTitle;
        public string m_strGameStartFile = @"D:\game\FullClient\FullClient\SUN2.exe";//@"E:\FullClient\SUN2.exe"; 
        public string m_strAccount = "";//账号
        public string m_strPassword;//密码
        public string m_strGameName = "奇迹世界2";
        public string m_strArea;//区
        public string m_strServer;//服
        public string m_strSellerRoleName;//卖家角色名
        public string m_strSecondPwd;//二级密码
        public string m_strSellerItemId;//交易物品
        public string m_strSellerItemNum;//数目
        public string m_strBuyerRoleName;//买家角色名
        public string m_strBuyerItemNum;//交易数目
        public long BackPackNum;//背包金币数量
        public bool Totransfer = false;//是否传送了
        public int  ToTrNum = 0;//传送金币数量
        public bool isSeller = false;
        public string m_strCapturePath = System.Windows.Forms.Application.StartupPath + @"\pic\";

        public static StringBuilder strb = new StringBuilder();

        public static string ServerAddr = "";
        //是否进入游戏标识
        public static bool InGame = false;
        //判断是否在沃德
        public static bool IfTrading = false;
        /// <summary>
        ///主函数入口
        /// </summary>
        public void StartToWork()
        {

            try
            {
                Status = GameProc();
            }
            catch (System.Exception ex)
            {
                WriteToFile(ex.ToString());
                Status = 3120;
            }
            if (Status > 1000)
            {
                try
                { 
                    CaptureBmpInRect("失败");
                    int Sta = 0;
                    Sta = Offline();
                    if (Sta > 1)
                    {
                        Status = Sta;
                    }
                }
                catch (Exception e)
                {
                    WriteToFile(e.ToString());
                    Status = 3120;
                }
                
            }
            
            string tmp;
            tmp = string.Format("移交状态={0}\r\n", Status);
            WriteToFile(tmp);
            tmp = string.Format("FStatus={0}\r\n", Status);
            if (the_nRC2Port != 0)
            {
                for (int j = 0; j < 2; j++)
                {
                    udpdd.theUDPSend((int)TRANSTYPE.TRANS_ORDER_END, tmp, OrdNo);
                    for (int i = 0; i < 10; i++)
                    {
                        if (bYiJiao)
                        {
                            WriteToFile("移交成功");
                            break;
                        }
                        Sleep(300);
                    }
                    if (bYiJiao)
                        break;
                    WriteToFile("移交失败");
                }
            }
            else
                WriteToFile("端口为0");

            CloseGames();
            string adslpath = System.Windows.Forms.Application.StartupPath + @"\adsl.ini";
            StringBuilder retVal = new StringBuilder(256);
            User32API.GetPrivateProfileString("记录参数", "ADSL本次做单", "", retVal, 256, adslpath);
            int num = 0;
            if (retVal.ToString() != "")
                num = int.Parse(retVal.ToString());
            if (num > 100)
            {
                User32API.WritePrivateProfileString("记录参数", "ADSL本次做单", "0", adslpath);
                Sleep(2500);
            }
            else
            {
                if ((Status > 1000 && Status < 3000) || Status > 4000)
                {
                    string strNum = string.Format("{0}", num + 18);
                    User32API.WritePrivateProfileString("记录参数", "ADSL本次做单", strNum, adslpath);
                }
            }

            StringBuilder retVal1 = new StringBuilder(256);
            User32API.GetPrivateProfileString("记录参数", "连续失败", "", retVal1, 255, adslpath);
            int num1 = int.Parse(retVal1.ToString());
            if (Status==3120)
            {
                if (num1 == 3)
                {
                    User32API.WritePrivateProfileString("记录参数", "连续失败", "0", adslpath);
                    RestartPC();//重启电脑
                }
                else
                {
                    string strNum1 = string.Format("{0}", num1 + 1);
                    User32API.WritePrivateProfileString("记录参数", "连续失败", strNum1, adslpath);
                }
            }
            else
            {
                User32API.WritePrivateProfileString("记录参数", "连续失败", "0", adslpath);
            }

            m_hGameWnd = User32API.FindWindow(null, "奇迹世界SUN");
            if (m_hGameWnd != IntPtr.Zero)
            {
                RestartPC();
            }

            return;

        }
        /// <summary>
        /// 主函数
        /// </summary>
        /// <returns>订单状态</returns>
        public int GameProc()
        {
            if (!KeyMouse.InitKeyMouse())
            {
                WriteToFile("驱动加载失败");
                return 3120;
            }
            int n = OrdNo.IndexOf("-");
            if (n > 0 || OrdNo == "测试订单" || MZH)
                m_strOrderType = "发布单";
            else
                m_strOrderType = "交易单";
            if (!RequestOrderData())
                return 3120;
            if (!ReadOrderDetail())
                return 3120;
            if (Regex.IsMatch(m_strAccount, @"[\u4e00-\u9fa5]"))
            {
                WriteToFile("账号含有中文");
                return 3000;
            }
            if (Regex.IsMatch(m_strPassword, @"[\u4e00-\u9fa5]"))
            {
                WriteToFile("密码含有中文");
                return 3000;
            }
            Sleep(2000);
            AppInit();
            //清空日志文件
            System.IO.File.WriteAllText(m_strProgPath + @"\输出日志.log", string.Empty);
            //显示区服
            WriteToFile("订单区服：" + m_strArea + "   卖家角色名：" + m_strSellerRoleName);
            for (int i = 0; i < 5; i++)
            {
                if (i == 3)
                {
                    WriteToFile("无法连接游戏服务器");
                    WriteToFile("网络连接错误或游戏维护中");
                    return 3120;
                }
                CloseGames();
                Status = RunGame();
                if (Status > 1000)
                    return Status;

                Status = Login();
                if (Status > 1000)
                    return Status;
                if (Status == 123)
                    continue;
                if (Status == 1)
                    break;
            }


            Status = Role();
            if (Status > 1000)
                return Status;

            Status = EnterGame();
            if (Status > 1000)
                return Status;

            Status = BackPack();
            if (Status > 1000)
                return Status;


            Status = GoTrading();
            if (Status > 1000)
                return Status;

            Status = SetUp();
            if (Status > 1000)
                return Status;

            Status = Whisper();
            if (Status > 1000)
                return Status;

            Status = Trading();
            if (Status > 1000)
                return Status;
           
           
            return Status;
        }
        /// <summary>
        /// 判断顶号
        /// </summary>
        /// <returns></returns>
        public int Offline() {
            if(isSeller){
                return 4010;
            }
            Point at = new Point(-1,-2);
            for (int a = 0; a < 3;a++ ) {
                at = ImageTool.fPic(m_strPicPath+"顶号.bmp",380,320,650,450);
                if(at.X>0){
                    WriteToFile("被顶号");
                    CaptureBmpInRect("顶号");
                    KeyMouse.MouseClick(at.X+185,at.Y+110,1,1,1000);
                    return 3030;
                }
            }
            return 1;
        }
        /// <summary>
        /// 启动游戏
        /// </summary>
        /// <returns></returns>
        public int RunGame()
        {
            //判断买家角色名是否有大写的o和0 有直接转3930
            if (m_strBuyerRoleName.IndexOf("0") >= 0 || m_strBuyerRoleName.IndexOf("O") >= 0) {
                WriteToFile("买家名:" + m_strBuyerRoleName);
                WriteToFile("无法判断买家角色名");
                WriteToFile("自动交易系统满负荷，转人工");
                return 3930;
            }
            //判断账号位数是否合法
            if (m_strAccount.Length > 10 || m_strAccount.Length < 4)
            {
                WriteToFile("卖家账号:" + m_strAccount);
                WriteToFile("账号不合法");
                WriteToFile("自动交易系统满负荷，转人工");
                return 3930;
            }
            Point pt = new Point(-1, -2);
            Point at = new Point(-1, -2);
            int aS = 0;
            int error = 0;
            Game.RunCmd("taskkill /im  iexplore.exe /F");
            m_hGameWnd = IntPtr.Zero;
            m_GameTitle = "UpdateClient Beta 1.0.8.0";
            WriteToFile("尝试打开客户端");
            strAllog = "等待进入游戏";
            string[] Sever = new string[] {"艾斯莫拉","天空之城","布拉齐恩"};
            for (int i = 0; i < 100; i++)
            {
                m_hGameWnd = User32API.FindWindow(null, m_GameTitle);
                if (m_hGameWnd == IntPtr.Zero)
                {
                    //打开游戏
                    Game.StartProcess(m_strGameStartFile, "start");
                    Sleep(1000*10);
                }
                else
                {
                    WriteToFile("已打开客户端");
                    for (int j = 0; j < 5;j++ ) {
                        pt = ImageTool.fPic(m_strPicPath + "同意.bmp", 0, 0, 0, 0);
                        if (pt.X > 0)
                        {
                            //点击同意按钮
                            KeyMouse.MouseClick(pt.X-15,pt.Y+8,1,1,500);
                            //点击选择区服
                            for (int Se = 0; Se < Sever.Length;Se++ ) {
                                if (m_strArea == Sever[Se])
                                {
                                    aS = Se;
                                    KeyMouse.MouseClick(pt.X-130*Se,pt.Y-30,1,1,500);
                                    break;
                                }
                            }
                            //点击开始游戏
                            for (int a = 0; a < 10;a++ ) {
                                at = ImageTool.fPic(m_strPicPath + "开始游戏.bmp", 0, 0, 0, 0);
                                if (at.X > 0)
                                {
                                    KeyMouse.MouseClick(at.X + 5, at.Y + 3, 1, 1, 1000);
                                     at = ImageTool.fPic(m_strPicPath + "开始游戏.bmp", 0, 0, 0, 0);
                                     if (at.X > 0)
                                     {
                                         KeyMouse.MouseClick(pt.X - 15, pt.Y + 8, 1, 1, 500);
                                         KeyMouse.MouseClick(pt.X - 15, pt.Y + 8, 1, 1, 500);
                                         KeyMouse.MouseClick(at.X + 5, at.Y + 3, 1, 1, 1000);
                                     }
                                    return 1;
                                }
                                else {
                                    WriteToFile("正在更新");
                                    Sleep(2000);
                                }
                            }
                            break;
                        }
                        Sleep(1000);
                   }
                   if (error < 3)
                   {

                       at = ImageTool.fPic(m_strPicPath + "更新错误.bmp", 0, 0, 0, 0);
                       if (at.X > 0)
                       {
                           KeyMouse.MouseClick(650, 552, 1, 1, 1000);
                           Game.RunCmd("taskkill /im  iexplore.exe /F");
                       }
                       Game.RunCmd("taskkill /im  SUN2.exe /F");
                       WriteToFile("未找到打开游戏标识,重开游戏");
                       Sleep(2000);
                       error++;
                       continue;
                   }
                   WriteToFile("进入游戏失败");
                   return 3120;
                }
            }

            WriteToFile("开启游戏失败");
            return 3120;
        }
        /// <summary>
        /// 账号密码
        /// </summary>
        /// <returns></returns>
        public int Login() {
            strAllog = "等待进入游戏";
            for (int i = 0; i < 40;i++ ) {
                m_hGameWnd = User32API.FindWindow(null, "奇迹世界SUN");
                if(m_hGameWnd!=IntPtr.Zero){
                    Sleep(5000);
                    RECT rt = new RECT(0, 0, 0, 0);
                    User32API.GetWindowRect(m_hGameWnd, out rt);
                    User32API.MoveWindow(m_hGameWnd, 0, 0, rt.Width, rt.Height, true);
                    Sleep(5000);
                    break;
                }
                Sleep(2000);
                if(i>10){
                    m_hGameWnd = User32API.FindWindow(null, "Soul of the Ultimate Nation Online - System Notice");
                    if (m_hGameWnd != IntPtr.Zero)
                    {
                        WriteToFile("服务器未响应,重新开启游戏");
                        KeyMouse.MouseClick(882,462,1,1,500);
                        KeyMouse.MouseClick(882, 462, 1, 1, 500);

                        KeyMouse.MouseClick(830,465,1,1,500);
                        KeyMouse.MouseClick(830, 465, 1, 1, 500);
                        KeyMouse.MouseClick(670, 585, 1, 1, 500);
                        KeyMouse.MouseClick(670, 585, 1, 1, 500);
                        return 123;
                    }
                }
                if(i==39){
                    WriteToFile("进入账号界面失败");
                    return 3120;
                }
            }
            strAllog = "等待账号输入";
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            for (int a = 0; a < 40;a++ ) {
                at = ImageTool.fPic(m_strPicPath+"登录游戏.bmp",0,0,0,0);
                if(at.X>0){
                    Sleep(1000);
                    for (int a1 = 0; a1 < 3;a1++ ) {
                        //输入账号
                        WriteToFile("输入账号:" + m_strAccount);
                        KeyMouse.MouseClick(at.X + 132, at.Y - 60, 1, 1, 200);
                        KeyMouse.MouseClick(at.X + 132, at.Y - 60, 1, 1, 200);
                        KeyMouse.SendBackSpaceKey(15);
                        KeyMouse.SendKeys(m_strAccount, 200);

                        Sleep(500);
                        //输入密码
                        WriteToFile("输入密码:" + m_strPassword.Length + "位");
                        KeyMouse.MouseClick(at.X + 132, at.Y - 27, 1, 1, 200);
                        KeyMouse.MouseClick(at.X + 132, at.Y - 27, 1, 1, 200);
                        KeyMouse.SendBackSpaceKey(15);
                        KeyMouse.SendKeys(m_strPassword, 200);

                        Sleep(500);
                        //截图
                        CaptureBmpInRect("账号密码");
                        KeyMouse.MouseClick(at.X+5,at.Y+3,1,1,1000);
                        //判断封停
                        bt = ImageTool.fPic(m_strPicPath + "封停.bmp", 0, 0, 0, 0);
                        if(bt.X>0){
                            WriteToFile("账号已封停");
                            CaptureBmpInRect("封停");
                            KeyMouse.MouseClick(bt.X + 71, bt.Y + 108, 1, 1, 1000);
                            return 2300;
                        }

                        bool judge = LoginError(a1);
                        if (judge)
                        {
                            Sleep(2000);
                            for (int a2 = 0; a2 < 5;a2++ ) {
                                at = ImageTool.fPic(m_strPicPath +"服务器列表.bmp", 0, 0, 0, 0);
                                if(at.X>0){
                                    //点击服务器
                                    KeyMouse.MouseClick(at.X-114,at.Y+46,1,1,200);
                                    KeyMouse.MouseClick(at.X - 114, at.Y + 46, 1, 2, 500);

                                    //点击选择二线
                                    KeyMouse.MouseClick(at.X + 157, at.Y + 93, 1, 1, 500);
                                    for (int i = 0; i < 5;i++ ) {
                                        at = ImageTool.fPic(m_strPicPath  + "进入服务器.bmp",375,320,655,490);
                                        if(at.X>0){
                                            WriteToFile("登录完成");
                                            KeyMouse.MouseClick(at.X+71,at.Y+108,1,1,500);
                                            Sleep(2000);
                                            break;
                                        }
                                        Sleep(1000);
                                    }
                                    return 1;
                                }
                                Sleep(1000);
                            } 
                        }
                        if (!judge&&a1==2)
                        {
                            WriteToFile("账号密码错误");
                            return 2000;
                        }
                        if(!judge){
                            continue;
                        }

                    }

                }
                Sleep(2000);
            }
            WriteToFile("进入账号界面失败");
            return 3120;
        }
        /// <summary>
        /// 判断账号密码情况
        /// </summary>
        /// <returns></returns>
        public bool LoginError(int e) {
            Point at = new Point(-1,-2);

            for (int a = 0; a < 3;a++ ) {
                at = ImageTool.fPic(m_strPicPath+"密码错误.bmp",0,0,0,0);
                if(at.X>0){
                    WriteToFile("密码错误,再次输入");
                    CaptureBmpInRect("错误" + e);
                    KeyMouse.MouseClick(at.X+71,at.Y+108,1,1,1000);
                    Sleep(4000);
                    return false;
                }
                at = ImageTool.fPic(m_strPicPath + "账号不存在.bmp", 0, 0, 0, 0);
                if (at.X > 0)
                {
                    WriteToFile("用户不存在,再次输入");
                    CaptureBmpInRect("错误" + e);
                    KeyMouse.MouseClick(at.X + 71, at.Y + 108, 1, 1, 1000);
                    Sleep(4000);
                    return false;
                }
                at = ImageTool.fPic(m_strPicPath + "服务器未响应.bmp", 0, 0, 0, 0);
                if (at.X > 0)
                {
                    WriteToFile("服务器未响应,再次输入");
                    CaptureBmpInRect("错误" + e);
                    KeyMouse.MouseClick(at.X + 127, at.Y + 107, 1, 1, 1000);
                    Sleep(4000);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 选择角色
        /// </summary>
        /// <returns></returns>
        public int Role() {
            strAllog = "选择角色";
            Sleep(8000);
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            for (int a = 0; a < 40;a++ ) {

                at = ImageTool.fPic(m_strPicPath + "选择角色.bmp", 0, 110, 300, 180);
                if (at.X > 0)
                {
                    CaptureBmpInRect("角色");
                    bt = ImageTool.fPic(m_strPicPath + "创建角色.bmp", 0, 0, 0, 0);
                    if(bt.X<=0){
                        bt = ImageTool.fPic(m_strPicPath + "创建角色1.bmp", 0, 0, 0, 0);
                    }
                    if (bt.Y > at.Y + 125)
                    {
                        WriteToFile("账号有多个角色");
                        CaptureBmpInRect1("角色列表",at.X-109,at.Y+20,at.X+178,bt.Y-20);
                        PinTu("角色,换行,角色列表,角色序号", "角色名", true);
                        //发送答题选择角色
                        string Path = m_strProgPath + "\\答题\\角色名.bmp";
                        string strResult = "";
                        strResult = RequestSafeCardInfo(1, Path,"", 60);
                        //判断答题结果
                        WriteToFile("答题返回:"+strResult);
                        if (strResult == "error")
                        {
                            WriteToFile("答题员放弃答题");
                            return 3230;
                        }
                        if (strResult == "")
                        {
                            WriteToFile("答题员放弃答题");
                            return 3230;
                        }
                        else {
                            int num = Int32.Parse(strResult);
                            if (num == 0)
                            {
                                WriteToFile("答题错误");
                                return 3230;
                            }
                            else {
                                WriteToFile("选择角色"+num);
                                KeyMouse.MouseClick(at.X, at.Y + 50*num, 1, 1, 500);
                                CaptureBmpInRect("角色");
                                KeyMouse.MouseClick(at.X, at.Y + 50 * num, 1, 2, 500);
                                return 1;
                            }
                        }
                    }
                    else {
                        WriteToFile("账号只有唯一角色");
                        KeyMouse.MouseClick(at.X,at.Y+45,1,1,500);
                        KeyMouse.MouseClick(at.X, at.Y + 50, 1, 2, 200);
                        KeyMouse.MouseClick(at.X, at.Y + 50, 1, 2, 500);
                        return 1;
                    }
                    
                }
                Sleep(2000);
            }
            WriteToFile("进入角色选择页面失败");
            return 3120;
        }
        /// <summary>
        /// 进入游戏的判断
        /// </summary>
        /// <returns></returns>
        public int EnterGame() {
            strAllog = "正在进入游戏";
            Point at = new Point(-1,-2);
            Sleep(1000*10);
            for (int j = 0; j < 40; j++)
            {
                at = ImageTool.fPic(m_strPicPath + "进入游戏.bmp", 0, 0, 0, 0);
                if (at.X > 0)
                {
                    WriteToFile("登陆成功");
                    Sleep(3000);
                    TanC();
                    CaptureBmpInRect("登陆成功");
                    return 1;
                }
                at = ImageTool.fPic(m_strPicPath + "进入游戏1.bmp", 0, 0, 0, 0);
                if (at.X > 0)
                {
                    WriteToFile("登陆成功");
                    Sleep(3000);
                    TanC();
                    CaptureBmpInRect("登陆成功");
                    return 1;
                }
                at = ImageTool.fPic(m_strPicPath + "进入游戏2.bmp", 0, 0, 0, 0);
                if (at.X > 0)
                {
                    WriteToFile("登陆成功");
                    Sleep(3000);
                    TanC();
                    CaptureBmpInRect("登陆成功");
                    return 1;
                }
                if(j==25||j==26){
                    TanC();
                }
                Sleep(1000);
            }

            
            
            WriteToFile("进入游戏失败");
            return 3120;
        }
        public void TanC() {
            Point at = new Point(-1, -2);
            for (int i = 0; i < 10;i++ ) {
                KeyMouse.SendEscKey();
                Sleep(500);
                at = ImageTool.fPic(m_strPicPath + "退出游戏.bmp", 425, 215, 650, 445);
                if(at.X>0){
                    KeyMouse.MouseClick(at.X+120,at.Y-174,1,1,500);
                    break;
                }
            }
        }
        /// <summary>
        /// 打开背包获取金币
        /// </summary>
        /// <returns></returns>
        public int BackPack() {

            strAllog = "获取背包金币数量";
            Point at = new Point(-1,-2);
            RECT rt = new RECT(0, 0, 0, 0);
            string Num = null;//获取的金币
            long num = 0;//金币转换成int
            KeyMouse.Sendkey('i');
            Sleep(2000);
            for (int a = 0; a < 20; a++)
            {
                at = ImageTool.fPic(m_strPicPath+"背包标识.bmp",810,520,1010,580,5);
                if(at.X<0&&a>10){
                    KeyMouse.Sendkey('i');
                    at = ImageTool.fPic(m_strPicPath + "背包标识.bmp", 810, 520, 1010, 580, 5);
                }
                if(at.X>0){
                    Sleep(1000);
                    for (int n = 0; n < 10;n++ ) {
                        Num = CheckPic(at.X + 20, at.Y, at.X + 190, at.Y + 17,1);
                        if (Num == null||Num=="0")
                        {
                            Sleep(1000);
                            continue;
                        }
                        break;
                    }
                    CaptureBmpInRect("背包");
                    if(Num ==null){
                        WriteToFile("读取金币失败");
                        return 2140;
                    }
                    WriteToFile("交易数量:" + m_strBuyerItemNum);

                    num = Int64.Parse(Num);
                    WriteToFile("背包金币:" + Num);
                    if (num >= Int64.Parse(m_strBuyerItemNum))
                    {
                        BackPackNum = num;
                        WriteToFile("金币充足");
                        KeyMouse.Sendkey('i'); 
                    }
                    else {
                        WriteToFile("金币不足"); 
                        KeyMouse.Sendkey('i');

                        //前往仓库取钱
                        int TakeOut = WareHouse(num);
                        if(TakeOut==-1){
                            return 2140;
                        }else if(TakeOut==-2){
                            return 2140;
                        }
                        else if (TakeOut == 1)
                        {
                            KeyMouse.SendEscKey();
                            break;
                        }
                        
                    }

                    break;
                }
                if (a > 15)
                {
                    KeyMouse.SendEscKey();
                }
                Sleep(500);
                if(a==19){
                    WriteToFile("读取金币失败..");
                    return 2140;
                }
            }

            Sleep(2000);
            //判断地点,判断金币是否足够传送
            int dt = 0;
            for (int i = 0; i < 10;i++ ) {
                KeyMouse.MouseMove(100,100);
                at = ImageTool.fPic(m_strPicPath + "背包标识.bmp", 810, 520, 1010, 580, 5);
                if(at.X>0){
                    KeyMouse.Sendkey('i'); 
                }
                at = ImageTool.fPic(m_strPicPath + "沃德.bmp", 740, 0, 1027, 200);
                if (at.X < 0)
                {
                    WriteToFile("不在交易地图.判断金币是否足够传送");

                    int TrNum= TransNum();
                    if (TrNum==3120) {
                        WriteToFile("无法获取传送所需金币");
                        return 3120;
                    }
                    ToTrNum = TrNum;
                    if ((num - TrNum) < Int64.Parse(m_strBuyerItemNum))
                    {
                        WriteToFile("金币不足传送");
                        return 2140;
                    }
                    else
                    {
                        WriteToFile("金币足够传送");
                        Totransfer = true;
                        return 1;
                    }
                }
                else {
                    dt++;
                }
                if(dt>5){
                    WriteToFile("在交易地图");
                    IfTrading = true;
                    return 1;
                }
            }

            WriteToFile("获取背包金币错误");
            return 3120;
           
        }
        /// <summary>
        /// 获取仓库金币
        /// </summary>
        /// <returns></returns>
        public int WareHouse(long MoneyNum) {  
            Sleep(1000);
            Point at = new Point(-1,-2);

            //前往仓库
            #region
            WriteToFile("正在前往仓库");
            at = ImageTool.fPic(m_strPicPath + "沃德.bmp", 740, 0, 1027, 200);
            if (at.X < 0)
            {
                WriteToFile("不在交易地图");
                return -1;
            }
           
            for (int a = 0; a < 10; a++)
            {
                KeyMouse.Sendkey('m');
                Sleep(400);
                at = ImageTool.fPic(m_strPicPath + "地图.bmp", 0, 645, 375, 790);
                if (at.X > 0)
                {
                    Sleep(1000);
                    //点击仓库位置
                    at = ImageTool.fPic(m_strPicPath + "仓库坐标.bmp", 400, 400, 600, 600);
                    if (at.X > 0)
                    {
                        KeyMouse.MouseClick(at.X, at.Y, 1,1, 8000);
                    }
                    else {
                        KeyMouse.MouseClick(503, 475, 1, 1, 8000);
                    }
                    break;
                }
                
                if (a == 19)
                {
                    WriteToFile("打开地图失败");
                    return -1;
                }
            }
            #endregion
            

            //打开仓库
            #region
            for (int b = 0; b < 10; b++)
            {
                at = ImageTool.fPic(m_strPicPath + "仓库标识.bmp", 400, 480, 630, 640);
                if (at.X > 0)
                {
                    WriteToFile("打开仓库成功");
                    break;
                }
                if (b == 9)
                {
                    WriteToFile("未能打开仓库");
                    return -1;
                }
                Sleep(1000);
            }
            #endregion

            //获取仓库金币
            string Num = "";//仓库中的金币
            for (int i = 0; i < 5; i++)
            {
                at = ImageTool.fPic(m_strPicPath + "取出金币.bmp", 400, 470, 600, 590);
                if (at.X > 0)
                {
                    for (int n = 0; n < 5; n++)
                    {
                        //读取仓库中的金币
                        Num = CheckPic(at.X, at.Y + 20, at.X + 200, at.Y + 45, 1);
                        if (Num == "" || Num == "0")
                        {
                            Sleep(500);
                            continue;
                        }
                        if (Num == "")
                        {
                            Sleep(500);
                            continue;
                        }
                        break;
                    }
                    if (Num == "")
                    {
                        WriteToFile("未能正确读取仓库金币");
                        return -1;
                    }
                    WriteToFile("仓库余额:" + Num);
                    CaptureBmpInRect("仓库余额");

                    if ((Int64.Parse(Num) + MoneyNum) >= Int64.Parse(m_strBuyerItemNum))
                    {
                        WriteToFile("金币足够");
                        break;
                    }
                    else {
                        return -1;
                    }
                }
            }
            //判断是否锁定  
            bool Lock = false;//仓库锁定标识
            #region
            for (int i = 0; i < 5; i++)
            {
                at = ImageTool.fPic(m_strPicPath + "仓库锁定.bmp", 400, 170, 630, 270);
                if (at.X > 0)
                {
                    Lock = true;
                    WriteToFile("仓库为锁定状态");
                    KeyMouse.MouseClick(at.X - 50, at.Y + 417, 1, 1, 1000);
                    break;
                }
                at = ImageTool.fPic(m_strPicPath + "仓库开启.bmp", 400, 170, 630, 270);
                if (at.X > 0)
                {
                    WriteToFile("仓库为开启状态");
                    break;
                }
                Sleep(500);
                if (i == 4)
                {
                    return -1;
                }
            }
            #endregion


            if(Lock){
                strAllog = "正在尝试开启仓库锁";
                //开启输入密码
                #region
                for (int c = 0; c < 5; c++)
                {
                    at = ImageTool.fPic(m_strPicPath + "仓库密码1.bmp", 360, 300, 660, 500);
                    if (at.X > 0)
                    {
                        KeyMouse.MouseClick(at.X + 70, at.Y + 108, 1, 1, 1000);

                        break;

                    }
                    if (c == 4)
                    {
                        return -1;
                    }
                    Sleep(500);
                }
                #endregion


                //输入密码界面
                int x=0, y=0;
                #region
                for (int d = 0; d < 5; d++)
                {
                    at = ImageTool.fPic(m_strPicPath + "仓库密码2.bmp", 400, 280, 630, 520);
                    if (at.X > 0)
                    {
                        x = at.X; y = at.Y;
                        break;
                    }
                    if (d == 4)
                    {
                        return -1;
                    }
                    Sleep(500);
                }
                #endregion

                //尝试输入密码
                #region
                RECT rt = new RECT(405, 355, 540, 515);
                int error = 0;
                if (m_strSecondPwd.Length > 8 || m_strSecondPwd.Length<4) {
                    return -1;
                }
                //将仓库密码存入数组
                ArrayList b = new ArrayList();
                foreach (char c in m_strSecondPwd)
                {
                    b.Add(c.ToString());
                }

                m_hGameWnd = User32API.GetDesktopWindow();
                for (int j = 0; j < 20; j++)
                {

                    Sleep(1000);
                    //根据密码点击
                    KeyMouse.MouseMove(rt.left, rt.top - 50);
                    for (int i = 0; i < b.Count; i++)
                    {
                        Sleep(500);
                        if (b[i].ToString()=="1") {
                            List<Point> l1 = ReadStorPaswd("1", 424, 373, 38, 34);
                            if (l1.Count > 1) {
                                at = ImageTool.FindText(m_hGameWnd, "X", Color.FromArgb(217, 119, 0), "宋体", 12, FontStyle.Bold, 0, 0, rt, 30);
                                if (at.X > 0)
                                {
                                    l1.Remove(at);
                                    KeyMouse.MouseClick(l1[0].X, l1[0].Y, 1, 1, 1000);
                                    KeyMouse.MouseMove(rt.left, rt.top-50);
                                    continue;
                                }
                            }
                            
                        }
                        if (b[i].ToString() == "3")
                        {
                            List<Point> l1 = ReadStorPaswd("3", 424, 373, 38, 34);
                            if(l1.Count>1){
                                at = ImageTool.FindText(m_hGameWnd, "8", Color.FromArgb(217, 119, 0), "宋体", 12, FontStyle.Bold, 0, 0, rt, 30);
                                if (at.X > 0)
                                {
                                    l1.Remove(at);
                                    KeyMouse.MouseClick(l1[0].X, l1[0].Y, 1, 1, 1000);
                                    KeyMouse.MouseMove(rt.left, rt.top - 50);
                                    continue;
                                }
                            }
                        }
                        at = ImageTool.FindText(m_hGameWnd, b[i].ToString(), Color.FromArgb(217, 119, 0), "宋体", 12, FontStyle.Bold, 0, 0, rt, 30);
                        if (at.X > 0)
                        {
                            KeyMouse.MouseClick(at.X, at.Y, 1, 1, 1000);
                            KeyMouse.MouseMove(rt.left, rt.top - 50);
                        }

                    }
                    CaptureBmpInRect("仓库密码"+error);
                    KeyMouse.MouseClick(x + 100, y + 185, 1, 1, 2000);

                    //密码错误
                    at = ImageTool.fPic(m_strPicPath + "仓库密码错误.bmp", 380, 325, 660, 885);
                    if(at.X<0){
                        at = ImageTool.fPic(m_strPicPath + "仓库密码错误.bmp", 380, 325, 660, 885);
                    }
                    if(at.X>0){
                        KeyMouse.MouseClick(at.X+130,at.Y+108,1,1,500);
                        CaptureBmpInRect("仓库密码错误" + error);
                        error++;
                    }

                    //判断是否开锁
                    at = ImageTool.fPic(m_strPicPath + "仓库开锁.bmp", 380, 325, 660, 885);
                    if (at.X > 0)
                    {
                        Sleep(2000);
                        KeyMouse.MouseClick(at.X + 80, at.Y + 105, 1, 1, 1000);
                        at = ImageTool.fPic(m_strPicPath + "仓库开启.bmp", 400, 170, 630, 270);
                        if (at.X > 0)
                        {
                            WriteToFile("已开启仓库");
                            break;
                        }
                    }
                    //密码错误次数过多
                    if (error==3) {
                        return -2;
                    }
                    if (j == 19)
                    {
                        return -1;
                    }
                }
                #endregion
            }

            //从仓库取出金币
            string InputNum = "";//输入的金币
            long OutNum = 0;//需要取出的金币
            string BegNum = "";//背包金币
            for (int i = 0; i < 5;i++ ) {
                at = ImageTool.fPic(m_strPicPath + "取出金币.bmp", 400, 470, 600, 590);
                if (at.X > 0) {
                    if ((Int64.Parse(Num) + MoneyNum) >= Int64.Parse(m_strBuyerItemNum))
                    {
                        //取出金币
                        #region
                        for (int j = 0; j < 5; j++)
                        {
                            OutNum = Int64.Parse(m_strBuyerItemNum) - MoneyNum;
                            KeyMouse.MouseClick(at.X + 170, at.Y + 57, 1, 1, 500);
                            KeyMouse.MouseClick(at.X + 170, at.Y + 57, 1, 1, 500);
                            KeyMouse.SendKeys(OutNum.ToString(), 200);
                            KeyMouse.MouseMove(at.X, at.Y - 20);
                            Sleep(500);
                            //读取输入的金币
                            InputNum = CheckPic(at.X, at.Y + 47, at.X + 200, at.Y + 67, 1);
                            if (InputNum == OutNum.ToString())
                            {
                                WriteToFile("取出金币：" + InputNum);

                                for (int m = 0; m < 5; m++)
                                {
                                    KeyMouse.MouseClick(at.X + 155, at.Y + 85, 1, 1, 500);
                                    BegNum = CheckPic(at.X + 430, at.Y + 45, at.X + 590, at.Y + 67, 1);
                                    if ((Int64.Parse(BegNum)) >= (Int64.Parse(m_strBuyerItemNum)))
                                    {
                                        BackPackNum = Int64.Parse(BegNum);
                                        WriteToFile("背包金币:" + BegNum);
                                        WriteToFile("取钱成功");
                                        CaptureBmpInRect("取钱成功");
                                        return 1;
                                    }
                                    else if (Int64.Parse(BegNum) == MoneyNum)
                                    {
                                        continue;
                                    }
                                }

                                WriteToFile("取钱失败");
                                CaptureBmpInRect("取钱失败");
                                return -1;
                            }
                            else
                            {
                                KeyMouse.MouseClick(at.X + 170, at.Y + 57, 1, 1, 500);
                                KeyMouse.MouseClick(at.X + 170, at.Y + 57, 1, 1, 500);
                                KeyMouse.SendBackSpaceKey(30);
                                Sleep(500);
                            }
                        }    
                        #endregion
                        WriteToFile("取出金币失败");
                        CaptureBmpInRect("取钱失败");
                        return -1;

                    }
                    else
                    {
                        return -1;
                    }

                    
                }
            }

            return -1;
        }
        /// <summary>
        /// 密码列表获取坐标
        /// </summary>
        /// <param name="str">需要的数字坐标</param>
        /// <param name="left">列表左坐标</param>
        /// <param name="top">列表右坐标</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public List<Point> ReadStorPaswd(string str, int left, int top, int width, int height)
        {
            List<Point> l = new List<Point>();
            Point pt = new Point(-1, -2);
            RECT rt;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    rt = new RECT(left + j * width, top + i * height, left + (j+1) * width, top + (i+1) * height);
                    pt = ImageTool.FindText(m_hGameWnd, str, Color.FromArgb(217, 119, 0), "宋体", 12, FontStyle.Bold, 0, 0, rt, 30);
                    if (pt.X > 0)
                    {
                        l.Add(pt);
                    }
                }
            }
            return l;
        }
        /// <summary>
        /// 获取传送费用
        /// </summary>
        /// <returns></returns>
        public int TransNum() {
            RECT rt = new RECT(740, 0, 1027, 200);
            Point at = new Point(-1, -2);
            m_hGameWnd = User32API.FindWindow(null, "奇迹世界SUN");
            string[,] str = new string[,] { 
                            { "特林格", "3000" }, 
                            { "伊瑟琳", "6000" }, 
                            { "奥克拉", "35000" }, 
                            { "精灵湖", "2000" },
                            { "炼金之路", "3000" },
                            { "地下纪念馆", "4000" },
                            { "西部地下水路", "5000" },
                            { "死亡沙漠", "20000" },
                            { "灼热峡谷", "25000" },
                            { "龙火之心", "30000" },
                            { "悲伤墓地", "3000" },
                            { "毁灭之地", "35000" },
                            { "青铜之都", "45000" },
                            { "神殿中层", "55000" }
            };

            for (int j = 0; j < 3;j++) {
                for (int i = 0; i < 14; i++)
                {
                    at = ImageTool.FindText(m_hGameWnd, str[i, 0], Color.FromArgb(197, 178, 137), "宋体", 12, FontStyle.Regular, 0, 0, rt, 20);
                    if (at.X > 0)
                    {
                        int n = int.Parse(str[i, 1]);
                        WriteToFile("当前所在地图:" + str[i, 0]);
                        WriteToFile("传送需要金币:"+str[i, 1]);
                        return n;
                    }
                    Sleep(200);
                }
                Sleep(1000);
            }
            
            return 3120;
        }
        /// <summary>
        /// 匹配获取数字
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="type">1背包,2交易输入</param>
        /// <returns>匹配到的数字</returns>
        public string CheckPic(int left, int top, int right, int bottom,int type)
        {
            
            Point at = new Point(-1, -2);
            m_hGameWnd = User32API.GetDesktopWindow();
            //m_hGameWnd = User32API.FindWindow(null, "奇迹世界SUN");
            RECT rt = new RECT(left, top, right, bottom);
            Point[] ST=new Point[100];
            string num=null;
            int n=0;
            FontStyle FontType = new FontStyle();
            if(type==1){
                FontType = FontStyle.Bold;
            }
            if(type==2){
                FontType = FontStyle.Regular;
            }
            for (int j = 0; j < 10; j++)
            {
                rt = new RECT(left, top, right, bottom);
                string[] a = new string[10] { "8", "0", "4", "2", "3", "1", "5", "6", "7", "9" };
                at = ImageTool.FindText(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "宋体", 12, FontType, 0, 0, rt, 38);
                while(at.X > 0)
                {
                    
                    bool key = false;
                    for (int i = 0; i < n; i++)
                    {
                        if (Math.Abs(at.X - ST[i].X) < 3)
                        {
                            key = true;
                            break;
                        }
                    }
                    if (!key) {
                        ST[n].X = at.X;
                        ST[n].Y = int.Parse(a[j]);
                        n++;
                    }
                    rt = new RECT(at.X + 2, top, right, bottom);
                    at = ImageTool.FindText(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "宋体", 12, FontType, 0, 0, rt, 38);
                }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    if (ST[i].X > ST[j].X)
                    {
                        int a = ST[i].X;
                        int b = ST[i].Y;
                        ST[i].X = ST[j].X;
                        ST[i].Y = ST[j].Y;
                        ST[j].X = a;
                        ST[j].Y = b;
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {

                num += ST[i].Y.ToString();
            }
            return num;
        }
        /// <summary>
        /// 前往交易地点
        /// </summary>
        /// <returns></returns>
        public int GoTrading() {
            strAllog = "前往交易地点";
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            Point ct = new Point(-1, -2);
            bool su = false;
            int cs = 0;
            //不在沃德
            #region
            if (!IfTrading)
            {
                WriteToFile("前往交易地图");
                for (int a = 0; a < 20; a++)
                {
                    KeyMouse.Sendkey('m');
                    Sleep(1000);
                    at = ImageTool.fPic(m_strPicPath + "传送点.bmp", 0, 0, 940, 750);
                    if (at.X > 0)
                    {
                        ct = new Point(at.X,at.Y);
                        KeyMouse.MouseClick(at.X, at.Y, 1, 1, 500);
                        for (int c = 0; c < 3; c++)
                        {
                            Sleep(500);
                            at = ImageTool.fPic(m_strPicPath + "地图.bmp", 0, 645, 375, 790);
                            if (at.X > 0)
                            {
                                KeyMouse.SendEscKey();
                            }
                        }
                        for (int b = 0; b < 20; b++)
                        {
                            Sleep(500);
                            su = Transfer();
                            if (su)
                            {
                                cs++;
                                break;
                                
                            }
                        }
                        if(cs>0){
                            break;
                        }
                    }
                    else
                    {
                        KeyMouse.SendEscKey();
                        su = Transfer();
                        if (su)
                        {
                            break;
                        }
                        for (int i = 0; i < 5;i++ ) {

                            if (i % 2 == 0)
                            {
                                KeyMouse.SendKeyW('a', 2);
                                KeyMouse.SendKeyW('w', 10);
                                KeyMouse.Sendkey('m');
                                Sleep(1000);
                                ct = ImageTool.fPic(m_strPicPath + "传送点.bmp", 0, 0, 940, 750);
                                if (ct.X > 0)
                                {
                                    KeyMouse.MouseClick(ct.X, ct.Y, 1, 1, 500);
                                }
                                else { 
                                    KeyMouse.SendEscKey(); 
                                }
                                
                            }
                            else {
                                KeyMouse.SendKeyW('d', 2);
                                KeyMouse.SendKeyW('s', 10);
                                KeyMouse.Sendkey('m');
                                Sleep(1000);
                                ct = ImageTool.fPic(m_strPicPath + "传送点.bmp", 0, 0, 940, 750);
                                if (ct.X > 0)
                                {
                                    KeyMouse.MouseClick(ct.X, ct.Y, 1, 1, 500);
                                }
                                else
                                {
                                    KeyMouse.SendEscKey();
                                }
                            }

                            su = Transfer();
                            if (su)
                            {
                                break;
                            }
                        }
                        
                        
                    }
                    if (su)
                    {
                        break;
                    }
                    if (a == 19)
                    {
                        WriteToFile("前往交易地点超时");
                        return 3040;
                    }
                }
            }
            #endregion

            //在沃德
            #region
            su = false;
            if (IfTrading)
            {
                WriteToFile("已经在交易地图");
                for (int a = 0; a < 20; a++)
                {
                    KeyMouse.Sendkey('m');
                    Sleep(500);
                    at = ImageTool.fPic(m_strPicPath + "传送点.bmp", 0, 0, 940, 750);
                    if (at.X > 0)
                    {
                        KeyMouse.MouseClick(at.X, at.Y, 1, 1, 500);
                        for (int c = 0; c < 3;c++ ) {
                            Sleep(500);
                            at = ImageTool.fPic(m_strPicPath + "地图.bmp", 0, 645, 375, 790);
                            if (at.X > 0)
                            {
                                KeyMouse.SendEscKey();
                            }
                        }
                        for (int b = 0; b < 10; b++)
                        {
                            Sleep(500);
                            at = ImageTool.fPic(m_strPicPath + "传送点目录.bmp", 0, 0, 0, 0);
                            if (at.X > 0)
                            {
                                KeyMouse.MouseClick(at.X + 108, at.Y + 420, 1, 1, 500);
                                WriteToFile("到达交易地点");
                                CaptureBmpInRect("交易地图");
                                su = true;
                                return 1;
                            }

                        }
                    }
                    else
                    {
                        KeyMouse.SendEscKey();
                        KeyMouse.SendKeyW('w',6);
                    }
                    if (su)
                    {
                        break;
                    }
                    if (a == 19)
                    {
                        WriteToFile("前往交易地点超时");
                        return 3040;
                    }
                }
            }
            #endregion
            

            WriteToFile("无法到达交易地点");
            return 3040;
        }
        /// <summary>
        /// 传送点目录
        /// </summary>
        /// <returns></returns>
        public bool Transfer() {
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            Point ct = new Point(-1, -2);
            for (int a = 0; a < 5;a++ ) {
                at = ImageTool.fPic(m_strPicPath+"传送点目录.bmp",320,180,720,220);
                if(at.X>0){
                    Sleep(1000);
                    at = ImageTool.fPic(m_strPicPath+"传送点目录.bmp",320,180,720,220);
                    if (at.X > 0) {
                        WriteToFile("出现传送点目录");
                        for (int b = 0; b < 5; b++)
                        {
                            KeyMouse.MouseMove(at.X,at.Y);
                            bt = ImageTool.fPic(m_strPicPath + "传送沃德.bmp", at.X - 55, at.Y - 6, at.X + 237, at.Y + 447);
                            if(bt.X<0){
                                bt = ImageTool.fPic(m_strPicPath + "传送沃德1.bmp", at.X - 55, at.Y - 6, at.X + 237, at.Y + 447);
                            }
                            if (bt.X > 0)
                            {
                                WriteToFile("点击传送");
                                CaptureBmpInRect("传送");


                                KeyMouse.MouseClick(bt.X + 3, bt.Y + 3, 1, 1, 200);
                                KeyMouse.MouseClick(bt.X + 3, bt.Y + 3, 1, 2, 500);

                                for (int c = 0; c < 10; c++)
                                {
                                    ct = ImageTool.fPic(m_strPicPath + "确定传送.bmp", 0, 0, 0, 0);
                                    if (ct.X > 0)
                                    {
                                        KeyMouse.MouseClick(ct.X + 43, ct.Y + 90, 1, 1, 500);
                                        for (int d = 0; d < 10; d++)
                                        {
                                            Sleep(3000);
                                            ct = ImageTool.fPic(m_strPicPath + "沃德.bmp", 760, 30, 1027, 200);
                                            if (ct.X > 0)
                                            {
                                                WriteToFile("到达交易地图");
                                                IfTrading = true;
                                                return true;
                                            }
                                        }
                                        return false;
                                    }
                                    KeyMouse.MouseClick(bt.X + 3, bt.Y + 3, 1, 2, 500);
                                    Sleep(2000);
                                }
                                return false;
                                
                            }
                            Sleep(200);
                        }
                    }
                    return false;
                   
                }
                Sleep(200);
            }
            return false;
        }
        /// <summary>
        /// 游戏配置设定
        /// </summary>
        /// <returns></returns>
        public int SetUp() {
            strAllog = "修改系统设置";
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);

            for (int a = 0; a < 10;a++ ) {
                KeyMouse.SendF12();
                Sleep(1000);
                at = ImageTool.fPic(m_strPicPath+"设置.bmp",0,0,0,0);
                if(at.X>0){
                        KeyMouse.MouseClick(at.X - 24, at.Y + 37, 1, 1, 500);
                    for (int b = 0; b < 5;b++ ) {
                        bt = ImageTool.fPic(m_strPicPath + "拒绝交易.bmp",at.X-173,at.Y,at.X+260,at.Y+568,5);
                        if (bt.X > 0)
                        {
                            KeyMouse.MouseClick(bt.X + 314, bt.Y + 10, 1, 1, 1000);
                            KeyMouse.MouseClick(bt.X + 314, bt.Y + 32, 1, 1, 1000);
                            KeyMouse.MouseClick(bt.X + 137, bt.Y + 70, 1, 1, 1000);
                            return 1;
                        }
                        else {
                            KeyMouse.MouseClick(at.X + 164, at.Y + 71, 1, 1, 500);
                        }
                        Sleep(500);
                    }
                }
                Sleep(500);
            }
            return 1;
        }
        /// <summary>
        /// 发送私聊信息
        /// </summary>
        /// <returns></returns>
        public Thread thread_fun;
        public static int waittime = 0;
        void fun()
        {
            while (true)
            {
                waittime++;
                Sleep(1000);
            }
        }
        public int Whisper(){
            strAllog = "等待交易框出现";
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            int k = 0;
            int bz = 0;
            int Time = 180;
            int number = 0;
            int nums = 0;
            thread_fun = new Thread(fun);
            thread_fun.Start();
            //分三次输入聊天信息
            WriteToFile("发送私聊信息");
            WriteToFile("私聊内容:"+m_strBuyerRoleName+" 您好请到2线沃德传送点处向我发送交易申请");
            string[] str = new string[3] {"/悄悄话",m_strBuyerRoleName,"您好请到2线沃德传送点处向我发送交易申请" };
            bool ToCl = false;
            for (int a = 0; a < 200;a++ ) {
                int time = Time - waittime;
                if (time <= 0)
                {
                    WriteToFile("等待买家发起交易超时");
                    thread_fun.Abort();
                    return 3100;
                }
                if(number==5&&nums==0){
                    bool su = Friend();
                    nums++;
                    Sleep(2000);
                    //if(!su){
                    //    WriteToFile("发送私信失败");
                    //    return 3100;
                    //}
                }
                KeyMouse.SendEnterKey();
                at = ImageTool.fPic(m_strPicPath+"聊天框.bmp",0,580,390,755,5);
                if (at.X > 0)
                {
                    number++;
                    if (k == 0)
                    {
                        KeyMouse.MouseClick(at.X + 347, at.Y + 10, 1, 1, 500);
                        k++;
                    }


                    KeyMouse.MouseClick(at.X + 266, at.Y + 10, 1, 1, 500);

                    for (int st = 0; st < str.Length; st++)
                    {
                        ToCl = ToClipboard(str[st]);
                        if (ToCl)
                        {
                            KeyMouse.SendCtrlV();
                            KeyMouse.SendKG();//输入空格
                        }
                        else
                        {
                            return 3120;
                        }
                    }
                    KeyMouse.SendEnterKey();
                    bt = ImageTool.fPic(m_strPicPath + "不在线或不存在.bmp", at.X + 534, at.Y - 214, at.X + 976, at.Y - 31);
                    if(bt.X>0){
                       if(bz==0){
                           WriteToFile("买家不在线,缩短时间");
                           bz++;
                       }
                        Time = 120;
                    }
                    for (int i = 0; i < 32;i++ ) {
                        bt = ImageTool.fPic(m_strPicPath + "交易标识.bmp", 0,0,0,0);
                        if (bt.X > 0)
                        {
                            return 1;
                        }
                        Sleep(500);
                    }
                    int Sta = 0;
                    Sta = Offline();
                    if (Sta > 1)
                    {
                        return Sta;
                    }
                }
                Sleep(300);

            }

            WriteToFile("无法发送私聊信息");
            return 3120;
        
        }
        /// <summary>
        /// 发私信
        /// </summary>
        public bool Friend(){
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            Point ct = new Point(-1, -2);
            for (int a = 0; a < 10;a++ ) {
                
                KeyMouse.Sendkey('f');
                KeyMouse.MouseMove(0,0);
                at = ImageTool.fPic(m_strPicPath+"编辑短消息.bmp",300,400,660,600);
                if(at.X>0){
                    KeyMouse.MouseClick(at.X+3,at.Y+5,1,1,3000);
                    for (int b = 0; b < 3;b++ ) {
                        
                        bt = ImageTool.fPic(m_strPicPath + "发送消息.bmp", 700, 200, 1000, 400);
                        if(bt.X>0){
                            KeyMouse.MouseClick(at.X+220, at.Y-385, 1, 1, 1000);

                            for (int i = 0; i < 20;i++ ) {
                                KeyMouse.MouseClick(bt.X + 104, bt.Y - 158, 1, 1, 200);
                                KeyMouse.MouseClick(bt.X + 104, bt.Y - 158, 1, 1, 500);
                                KeyMouse.SendBackSpaceKey(10);

                                //买家角色名
                                bool su = false;
                                su = ToClipboard(m_strBuyerRoleName);
                                if (su)
                                {
                                    KeyMouse.SendCtrlV();
                                }

                                KeyMouse.MouseClick(bt.X + 84, bt.Y - 60, 1, 1, 200);
                                KeyMouse.MouseClick(bt.X + 84, bt.Y - 60, 1, 1, 500);

                                //内容
                                string content = "您好请到2线沃德传送点处向我发送交易申请";
                                su = ToClipboard(content);
                                if (su)
                                {
                                    KeyMouse.SendCtrlV();
                                }

                                //发送
                                KeyMouse.MouseClick(bt.X, bt.Y, 1, 1, 500);

                                Sleep(1000);
                                ct = ImageTool.fPic(m_strPicPath + "请输入.bmp", 370, 325, 650, 500);
                                if(ct.X>0){
                                    Sleep(1000);
                                    KeyMouse.MouseClick(ct.X+125,ct.Y+120,1,1,500);
                                    continue;
                                }
                                return true;
                            }

                        }
                        Sleep(500);
                    }
                }
                Sleep(500);
            }
            // bt = ImageTool.fPic(m_strPicPath + "发送消息.bmp", 700, 200, 1000, 400);
            // if (bt.X > 0) {
            //     KeyMouse.SendEscKey();
            // }
            //at = ImageTool.fPic(m_strPicPath + "编辑短消息.bmp", 300, 400, 660, 600);
            //if(at.X>0){
            //    KeyMouse.SendEscKey();
            //}
            return false;
        }
        /// <summary>
        /// 内容复制到粘贴板
        /// </summary>
        /// <returns></returns>
        public bool ToClipboard(string str)
        {
            //引用 将内容复制到粘贴板
            for (int BN = 0; BN < 20; BN++)
            {
                Game.StartProcess(m_strProgPath + "\\ZH_Clipboard.exe", str);
                string Bname = Clipboard.GetText();
                if (Bname == str)
                {
                    return true;
                }
                else
                {
                    Game.StartProcess(m_strProgPath + "\\ZH_Clipboard.exe", str);
                    Game.StartProcess(m_strProgPath + "\\ZH_Clipboard.exe", str);
                    Game.StartProcess(m_strProgPath + "\\ZH_Clipboard.exe", str);
                }
                if (BN == 19)
                {
                    WriteToFile("无法正确复制内容");
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 交易
        /// </summary>
        /// <returns></returns>
        public int Trading() {
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            Point ct = new Point(-1, -2);
            Point dt = new Point(-1, -2);
            Point st = new Point(-1, -2);
            WriteToFile("买家角色名:【" + m_strBuyerRoleName + "】");
            WriteToFile("交易金额:【" + m_strBuyerItemNum + "】");
            strAllog = "等待交易";
            string Num = null;//获取的金币
            long num = 0;//金币转换成int
            for (int a = 0; a < 180;a++ ) {
                if(a==179){
                    WriteToFile("等待买家发起交易超时");
                    return 3100;
                }
                at = ImageTool.fPic(m_strPicPath+"交易标识.bmp",0,0,0,0);
                if(at.X>0){
                    WriteToFile("已接收买家发起的交易");
                    //匹配玩家名
                    Sleep(2000);
                    CaptureBmpInRect("交易框");
                    bool PlName = PlayerName(at.X - 82, at.Y - 3, at.X + 148, at.Y + 386);
                    if (PlName)
                    {
                        WriteToFile("匹配玩家名成功,继续交易");
                    }
                    else {
                        WriteToFile("匹配玩家名失败,取消交易");
                        return 3120;
                    }

                    #region
                   
                    for (int b = 0; b < 10;b++ ) {
                        if(b==9){
                            st = ImageTool.fPic(m_strPicPath + "交易取消.bmp", 0, 0, 0, 0);
                            if (st.X > 0)
                            {
                                WriteToFile("交易失败或被取消");
                                return 3120;
                            }
                            WriteToFile("无法输入金币");
                            return 3120;
                        }
                        KeyMouse.MouseClick(at.X-56,at.Y+150,1,1,500);
                        //输入金币
                        bt = ImageTool.fPic(m_strPicPath + "输入金币.bmp", 0, 0, 0, 0);
                        if(bt.X>0){
                            KeyMouse.MouseClick(bt.X+117,bt.Y+90,1,1,500);
                            KeyMouse.SendKeys(m_strBuyerItemNum, 200);
                            for (int n = 0; n < 20; n++)
                            {
                                //读取输入的金币
                                Num = CheckPic(bt.X -36, bt.Y+82, bt.X + 139, bt.Y + 103, 2);
                                if (Num == null || Num == "0")
                                {
                                    Sleep(1000);
                                    continue;
                                }
                                break;
                            }
                            //&&&&&&&&&&&&&&&&&&&&&&&&&
                            CaptureBmpInRect("输入金币");
                            if (Num == null)
                            {
                                WriteToFile("读取金币失败");
                                return 3120;
                            }

                            num = Int64.Parse(Num);
                            WriteToFile("输入:" + Num);
                            if (num == Int64.Parse(m_strBuyerItemNum))
                            {

                                WriteToFile("金币输入正确");
                                KeyMouse.SendEnterKey();
                                //----------------
                                for (int b1 = 0; b1 < 10;b1++ ) {
                                    if (b1 > 8)
                                    {
                                        st = ImageTool.fPic(m_strPicPath + "交易取消.bmp", 0, 0, 0, 0);
                                        if (st.X > 0)
                                        {
                                            WriteToFile("交易失败或被取消");
                                            CaptureBmpInRect("交易未成功");
                                            return 3120;
                                        }
                                        WriteToFile("未找到提确定输入金币按钮");
                                        return 3120;
                                    }
                                    st = ImageTool.fPic(m_strPicPath + "邀请组队.bmp", 360, 310, 650, 470);
                                    if (st.X > 0)
                                    {
                                        KeyMouse.MouseClick(st.X, st.Y + 105, 1, 1, 500);
                                    }
                                    //确定金币的输入
                                    bt = ImageTool.fPic(m_strPicPath + "确定输入.bmp", 0, 0, 0, 0);
                                    if(bt.X>0){
                                        WriteToFile("确定输入");
                                        KeyMouse.MouseClick(bt.X+54,bt.Y+103,1,1,500);

                                        //$$$$$$$$$$$$$$$$$
                                        for (int c = 0; c < 10;c++ ) {
                                            if(c==9){
                                                st = ImageTool.fPic(m_strPicPath + "邀请组队.bmp", 360, 310, 650, 470);
                                                if (st.X > 0)
                                                {
                                                    KeyMouse.MouseClick(st.X, st.Y + 105, 1, 1, 500);
                                                }
                                                st = ImageTool.fPic(m_strPicPath + "更换物品.bmp", 360,310,650,470);
                                                if (st.X > 0)
                                                {
                                                    KeyMouse.MouseClick(st.X+135,st.Y+105,1,1,500);
                                                }
                                                st = ImageTool.fPic(m_strPicPath + "交易取消.bmp", 0, 0, 0, 0);
                                                if (st.X > 0)
                                                {
                                                    WriteToFile("交易失败或被取消");
                                                    return 3120;
                                                }
                                                WriteToFile("未找到提交交易按钮");
                                                return 3120;
                                            }
                                            //准备提交交易
                                            ct = ImageTool.fPic(m_strPicPath + "交易提交.bmp", at.X-82,at.Y-3,at.X+148,at.Y+386);
                                            if(ct.X>0){
                                                //核对输入框金币数量
                                                CaptureBmpInRect("核对金币");
                                                long SureNum = 0;
                                                string SureNums = "";

                                                for (int m = 0; m < 100;m++ ) {
                                                    SureNums = CheckPic1(at.X - 10, at.Y + 140, at.X + 135, at.Y + 161);
                                                    if(m==99){
                                                        WriteToFile("核对输入框金币:" + SureNums);
                                                        WriteToFile("核对金币不正确");
                                                        return 3120;
                                                    }
                                                    if (SureNums != m_strBuyerItemNum)
                                                    {
                                                        Sleep(1000);
                                                        continue;
                                                    }
                                                    else {
                                                        break;
                                                    }
                                                }
                                                SureNum = Int64.Parse(SureNums);
                                                WriteToFile("核对输入框金币:" + SureNum);
                                                if (SureNum == Int64.Parse(m_strBuyerItemNum))
                                                {
                                                    WriteToFile("核对金币正确");
                                                }
                                                else {
                                                    WriteToFile("核对金币不正确");
                                                    return 3120;
                                                }
                                                //提交交易
                                                WriteToFile("提交交易");
                                                KeyMouse.MouseClick(ct.X+3,ct.Y+3,1,1,500);
                                                strAllog = "等待买家提交交易";

                                                for (int d = 0; d < 61;d++ ) {
                                                    if(d>30){
                                                        st = ImageTool.fPic(m_strPicPath + "邀请组队.bmp", 360, 310, 650, 470);
                                                        if (st.X > 0)
                                                        {
                                                            KeyMouse.MouseClick(st.X, st.Y + 105, 1, 1, 500);
                                                        }
                                                        st = ImageTool.fPic(m_strPicPath + "交易取消.bmp", 0, 0, 0, 0);
                                                        if (st.X > 0)
                                                        {
                                                            WriteToFile("交易失败或被取消");
                                                            CaptureBmpInRect("交易未成功");
                                                            return 3120;
                                                        }
                                                        WriteToFile("等待买家提交超时");
                                                        return 3100;
                                                    }
                                                    //买家也提交.出现完成交易按钮
                                                    dt = ImageTool.fPic(m_strPicPath + "完成交易.bmp", at.X - 82, at.Y - 3, at.X + 148, at.Y + 386);
                                                    if(dt.X>0){
                                                        WriteToFile("买家已经提交交易");

                                                        WriteToFile("请求服务器同意交易");
                                                        Sleep(100);
                                                        if (OrdNo != "测试订单" && !IsAskMail)
                                                        {
                                                            udpdd.theUDPSend(35, "", OrdNo);
                                                            for (int j = 0; j < 10; j++)
                                                            {
                                                                Sleep(1000);
                                                                if (IsAskMail)
                                                                {
                                                                    WriteToFile("服务器同意交易");
                                                                    Sleep(500);
                                                                    break;
                                                                }
                                                                if (j >= 9)
                                                                {
                                                                    WriteToFile(OrdNo);
                                                                    WriteToFile(m_sign.ToString());
                                                                    WriteToFile("等待服务器同意交易超时");
                                                                    return 3120;
                                                                }
                                                                Sleep(1000);
                                                            }

                                                        }
                                                        if (!AskArgess())
                                                        {
                                                            return 3120;
                                                        }

                                                        KeyMouse.MouseClick(dt.X+3,dt.Y,1,1,500);
                                                        isSeller = true;
                                                        //++++++++++
                                                        strAllog = "等待买家确定交易完成";
                                                        for (int s = 0; s < 20;s++ ) {
                                                            //判断交易框是否还存在
                                                            at = ImageTool.fPic(m_strPicPath + "交易标识.bmp", 0, 0, 0, 0);
                                                            if(at.X<0){
                                                                WriteToFile("买家已确定交易完成");
                                                                break;
                                                            }
                                                            if(s>15){
                                                                WriteToFile("等待买家确定交易完成超时");
                                                                return 3100;
                                                            }
                                                            Sleep(2000);
                                                        }

                                                        
                                                        //+++++++++++
                                                        //##############
                                                        for (int s = 0; s < 5; s++) {
                                                            st = ImageTool.fPic(m_strPicPath + "交易完成.bmp", 0, 0, 0, 0); 
                                                            if(st.X>0){
                                                                //交易完成判断背包金币余额
                                                                int SNum1 = RemainBackPack();
                                                                if (SNum1 == 1)
                                                                {
                                                                    WriteToFile("交易成功");
                                                                    CaptureBmpInRect("交易成功");
                                                                    return 1000;
                                                                }
                                                                else
                                                                {
                                                                    WriteToFile("未检测到交易成功,人工查对");
                                                                    CaptureBmpInRect("交易未成功");
                                                                    return 4010;
                                                                }
                                                            }
                                                            st = ImageTool.fPic(m_strPicPath + "交易完成1.bmp", 0, 0, 0, 0);
                                                            if (st.X > 0)
                                                            {//交易完成判断背包金币余额
                                                                int SNum2 = RemainBackPack();
                                                                if (SNum2 == 1)
                                                                {
                                                                    WriteToFile("交易成功");
                                                                    CaptureBmpInRect("交易成功");
                                                                    return 1000;
                                                                }
                                                                else
                                                                {
                                                                    WriteToFile("未检测到交易成功,人工查对");
                                                                    CaptureBmpInRect("交易未成功");
                                                                    return 4010;
                                                                }
                                                            }
                                                            Sleep(200);
                                                        }
                                                        //未找到交易完成
                                                        WriteToFile("未找到交易成功标志");
                                                        WriteToFile("用剩余金币判断是否已经交易");
                                                        //未找到交易完成标识,判断背包金币余额
                                                        int SNum = RemainBackPack();
                                                        if (SNum == 1)
                                                        {
                                                            WriteToFile("已经交易");
                                                            CaptureBmpInRect("交易成功");
                                                            return 1000;
                                                        }
                                                        else {
                                                            WriteToFile("未检测到交易成功,人工查对");
                                                            CaptureBmpInRect("交易未成功");
                                                            return 4010;
                                                        }
                                                        
                                                        
                                                        //##############

                                                    }
                                                    Sleep(2000);
                                               }

                                            }
                                            Sleep(300);
                                        }
                                        //$$$$$$$$$$$$$$$$$
                                    }
                                    Sleep(300);
                                }
                                //----------------
                            }
                            else {
                                st = ImageTool.fPic(m_strPicPath + "交易取消.bmp", 0, 0, 0, 0);
                                if (st.X > 0)
                                {
                                    WriteToFile("交易失败或被取消");
                                    return 3120;
                                }
                                WriteToFile("金币输入出错");
                                return 3120;
                            }
                            //&&&&&&&&&&&&&&&&&&&&&&&&&&
                        }
                    }
                    Sleep(300);
                    #endregion
                }
                Sleep(1000);
            }
            WriteToFile("交易失败");
            return 3120;
        }
        /// <summary>
        /// 匹配玩家名
        /// </summary>
        /// <returns></returns>
        public bool PlayerName(int left, int top, int right, int bottom)
        {
            m_hGameWnd = User32API.FindWindow(null, "奇迹世界SUN");
            if (m_hGameWnd == IntPtr.Zero)
            {
                m_hGameWnd = User32API.GetDesktopWindow();
            }

            Point at = new Point(-1,-2);
            RECT rt = new RECT(left,top,right,bottom);
            bool b = false;
            for (int a = 0; a < 10;a++ ) {
                at = ImageTool.FindText(m_hGameWnd,m_strBuyerRoleName+"的物品", Color.FromArgb(255, 133, 50), "宋体", 12, FontStyle.Bold, 1, 1, rt, 38);
                if(at.X>0){
                    b = ImageTool.CheckRGB(at.X-20, at.Y, at.X, at.Y+14, 255, 133, 50, 1);
                    if(b){
                        return false;
                    }
                    return true;
                }
                Sleep(200);
            }
            return false;
        }
        /// <summary>
        /// 交易前匹配金币数量
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string CheckPic1(int left, int top, int right, int bottom)
        {

            Point at = new Point(-1, -2);
            m_hGameWnd = User32API.GetDesktopWindow();
            //m_hGameWnd = User32API.FindWindow(null, "奇迹世界SUN");
            RECT rt = new RECT(left, top, right, bottom);
            Point[] ST = new Point[15];
            string num = null;
            int n = 0;
            FontStyle FontType = new FontStyle();
            for (int j = 0; j < 10; j++)
            {
                rt = new RECT(left, top, right, bottom);
                string[] a = new string[10] { "8", "0", "4", "2", "3", "1", "5", "6", "7", "9" };
                at = ImageTool.FindText1(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "宋体", 12, FontType, 0, 0, rt, 38);
                while (at.X > 0)
                {

                    bool key = false;
                    for (int i = 0; i < n; i++)
                    {
                        if (Math.Abs(at.X - ST[i].X) < 3)
                        {
                            key = true;
                            break;
                        }
                    }
                    if (!key)
                    {
                        ST[n].X = at.X;
                        ST[n].Y = int.Parse(a[j]);
                        n++;
                    }
                    rt = new RECT(at.X + 2, top, right, bottom);
                    at = ImageTool.FindText1(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "宋体", 12, FontType, 0, 0, rt, 38);
                }
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    if (ST[i].X > ST[j].X)
                    {
                        int a = ST[i].X;
                        int b = ST[i].Y;
                        ST[i].X = ST[j].X;
                        ST[i].Y = ST[j].Y;
                        ST[j].X = a;
                        ST[j].Y = b;
                    }
                }
            }
            for (int i = 0; i < n; i++)
            {

                num += ST[i].Y.ToString();
            }
            return num;
        }
        /// <summary>
        /// 打开背包获取金币
        /// </summary>
        /// <returns></returns>
        public int RemainBackPack()
        {

            strAllog = "获取交易后背包金币数量";
            Point at = new Point(-1, -2);
            RECT rt = new RECT(0, 0, 0, 0);
            string Num = null;//获取的金币
            long num = 0;//金币转换成int
            long RemainNum = 0;//剩余金币
            KeyMouse.Sendkey('i');
            for (int a = 0; a < 7; a++)
            {
                at = ImageTool.fPic(m_strPicPath + "背包标识.bmp", 700, 340, 1025, 600);
                if (at.X > 0)
                {
                    for (int n = 0; n < 5; n++)
                    {
                        Num = CheckPic(at.X + 20, at.Y, at.X + 190, at.Y + 17, 1);
                        if (Num == null || Num == "0")
                        {
                            Sleep(500);
                            continue;
                        }
                        break;
                    }
                    CaptureBmpInRect("剩余金币");
                    if (Num == null)
                    {
                        WriteToFile("读取交易后金币失败");
                        return 4010;
                    }


                    RemainNum = BackPackNum - Int64.Parse(m_strBuyerItemNum);
                    if (Totransfer)
                    {
                        RemainNum = RemainNum - ToTrNum;
                        num = Int64.Parse(Num);
                        WriteToFile("传送使用了金币：" + ToTrNum);
                        WriteToFile("应剩余金币:" + RemainNum);
                        WriteToFile("实际剩余金币:" + Num);
                        if (RemainNum == num)
                        {
                            
                            WriteToFile("交易数量正确");
                            KeyMouse.Sendkey('i');
                            return 1;
                        }
                        else
                        {
                            WriteToFile("交易数量不正确");
                            return 4010;
                        } 
                    }
                    else {
                        num = Int64.Parse(Num);
                        WriteToFile("应剩余金币:" + RemainNum);
                        WriteToFile("实际剩余金币:" + Num);
                        if (RemainNum == num)
                        {
                           
                            WriteToFile("交易数量正确");
                            KeyMouse.Sendkey('i');
                            return 1;
                        }
                        else
                        {
                            WriteToFile("交易数量不正确");
                            return 4010;
                        } 
                    }
                   

                   
                }
                Sleep(500);
                if (a == 6)
                {
                    WriteToFile("读取交易后金币失败");
                    return 4010;
                }
            }

            WriteToFile("获取背包金币错误");
            return 3120;

        }
        /// <summary>
        /// StopGTR返回值
        /// </summary>
        /// <returns></returns>
        public bool AskArgess()
        {
            if (OrdNo == "测试订单")
            {
                WriteToFile("测试订单,选择1");
                return true;
            }
            string stradd = System.Windows.Forms.Application.StartupPath;
            int ipos = stradd.LastIndexOf("\\");
            stradd = stradd.Substring(0, ipos);

            stradd = stradd + "\\RCClient1.ini";
            if (!File.Exists(stradd))
            {
                string stp = string.Format("地址[{0}]不存在", stradd);
                WriteToFile(stp);
                return false;
            }
            string strTemp = FileRW.ReadFile(stradd);

            ServerAddr = MyStr.FindStr(strTemp, "ServerAddr=", "\r\n");

            string str = string.Format("服务器地址:[{0}]", ServerAddr);
            WriteToFile(str);

            string strUrl = "";
            strUrl = string.Format("http://192.168.36.16/StopGTR/Service.asmx/AskSendMoney?OrderNum={0}&dbName={1}", OrdNo, ServerAddr);
            string strHTML = "";
            WebClient myWebClient = new WebClient();
            try
            {
                Stream myStream = myWebClient.OpenRead(strUrl);
                StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                strHTML = sr.ReadToEnd();
                myStream.Close();
            }
            catch (Exception err)
            {
                FileRW.WriteError(err);
            }
            string Re = MyStr.FindStr(strHTML, "\">", "<");
            if (Re == "0")
            {
                str = string.Format("StopGTR返回值[{0}]", Re);
                WriteToFile(str);
                return true;
            }
            else
            {
                str = string.Format("StopGTR返回值[{0}]", Re);
                WriteToFile(str);
                return false;
            }
        }
        /// <summary>
        /// 关闭游戏
        /// </summary>
        public void CloseGames()
        {
            ////关闭游戏
            Point at = new Point(-1,-2);
            m_hGameWnd = User32API.FindWindow(null, "奇迹世界SUN");
            if(m_hGameWnd!=IntPtr.Zero){

                for (int i = 0; i < 2; i++)
                {
                    at = ImageTool.fPic(m_strPicPath + "关闭游戏.bmp", 0, 0, 0, 0);
                    if (at.X > 0)
                    {
                        KeyMouse.MouseClick(at.X + 3, at.Y + 3, 1, 1, 500);
                        break;
                    }
                    at = ImageTool.fPic(m_strPicPath + "关闭游戏1.bmp", 0, 0, 0, 0);
                    if (at.X > 0)
                    {
                        KeyMouse.MouseClick(at.X + 3, at.Y + 3, 1, 1, 500);
                        break;
                    }
                    Sleep(500);
                }
               
                for (int j = 0; j <4; j++)
                {
                    KeyMouse.MouseClick(1018, 11, 1, 1, 500);
                    at = ImageTool.fPic(m_strPicPath + "退出游戏.bmp", 425,215,650,445);
                    if (at.X > 0)
                    {
                        KeyMouse.MouseClick(at.X + 3, at.Y + 3, 1, 1, 500);
                        for (int k = 0; k < 3;k++ ) {
                            at = ImageTool.fPic(m_strPicPath + "退出游戏1.bmp", 370,320,655,490);
                            if (at.X > 0)
                            {
                                KeyMouse.MouseClick(at.X + 40, at.Y + 105, 1, 1, 500);
                                Sleep(9000);
                                break;
                            }
                            Sleep(500);
                        }
                        break;
                    }

                    Sleep(500);
                }
                Sleep(3000);
            }

            Game.RunCmd("taskkill /im  SUN2.exe /F");
            Game.RunCmd("taskkill /im  Sungame.exe /F");
            

            WriteToFile("关闭残留窗口");
            Game.RunCmd("taskkill /im  iexplore.exe /F");


        }
        /// <summary>
        /// 请求订单数据
        /// </summary>
        /// <returns></returns>
        public static bool RequestOrderData()
        {
            string tmp = string.Format("FExeProcID={0}\r\nFRobotPort={1}\r\n", Program.pid, m_UDPPORT);
            udpdd.theUDPSend((int)TRANSTYPE.TRANS_REQUEST_ORDER, tmp, OrdNo);
            if (the_nRC2Port == 0)
            {//读本地
                m_strOrderData = FileRW.ReadFile("info.txt");
            }
            else
            { //服务器获取
                m_strOrderData = "";
                string tmp1 = string.Format("FExeProcID={0}\r\nFRobotPort={1}\r\n", Program.pid, m_UDPPORT);
                udpdd.theUDPSend((int)TRANSTYPE.TRANS_REQUEST_ORDER, tmp, OrdNo);
                for (int i = 0; i < 30; i++)
                {
                    if (m_strOrderData != "")
                    {
                        tmp = string.Format("端口号{0}订单号{1}进程号{2}", the_nRC2Port, OrdNo, Program.pid);
                        WriteToFile(tmp);
                        Thread.Sleep(100);
                        return true;
                    }
                    Thread.Sleep(100);
                }
                WriteToFile("请求数据失败\r\n");
                return false;


            }
            return true;
        }
        /// <summary>
        /// 读取订单数据
        /// </summary>
        /// <returns></returns>
        public bool ReadOrderDetail()
        {
            if (m_strOrderData == "")
            {
                WriteToFile(("==========> 订单数据为空 <==========\n"));
                return false;
            }

            m_strArea = MyStr.FindStr(m_strOrderData, "<GameZone>", "</GameZone>");
            m_strServer = MyStr.FindStr(m_strOrderData, "<GameServer>", "</GameServer>");
            m_strAccount = MyStr.FindStr(m_strOrderData, "<SellerAccount>", "</SellerAccount>");
            m_strPassword = MyStr.FindStr(m_strOrderData, "<SellerPassword>", "</SellerPassword>");
            m_strSellerRoleName = MyStr.FindStr(m_strOrderData, "<SellerRoleName>", "</SellerRoleName>");
            m_strSecondPwd = MyStr.FindStr(m_strOrderData, "<SellerSecondPwd>", "</SellerSecondPwd>");
            m_strSellerItemId = MyStr.FindStr(m_strOrderData, "<SellerItemId>", "</SellerItemId>");
            m_strSellerItemNum = MyStr.FindStr(m_strOrderData, "<SellerItemNum>", "</SellerItemNum>");
            m_strBuyerRoleName = MyStr.FindStr(m_strOrderData, "<BuyerRoleName>", "</BuyerRoleName>");
            m_strBuyerItemNum = MyStr.FindStr(m_strOrderData, "<BuyerItemNum>", "</BuyerItemNum>");
            m_strBuyerItemNum = m_strBuyerItemNum + "0000";
            m_strArea = m_strArea.Trim();



            string strlog = string.Format("游戏名[{0}]", m_strGameName);
            WriteToFile(strlog);
            if (m_strAccount == "")
            {
                WriteToFile("帐号为空");
                return false;
            }
            if (m_strPassword == "")
            {
                WriteToFile("密码为空");
                return false;
            }
            char[] acc = m_strAccount.ToCharArray();
            for (int i = 0; i < acc.Length; i++)
            {
                if (char.IsControl(acc[i]))
                {
                    WriteToFile("账号含有不可见的转义字符");
                    return false;
                }
            }
            char[] pwd = m_strPassword.ToCharArray();
            for (int i = 0; i < pwd.Length; i++)
            {
                if (char.IsControl(pwd[i]))
                {
                    WriteToFile("密码含有不可见的转义字符");
                    return false;
                }
            }



            return true;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void AppInit()
        {
            //Kernel32API.WinExec("rasphone -d 宽带连接",6);
            IntPtr RChwnd = IntPtr.Zero;

            RChwnd = User32API.FindWindow(null, "GTR_RC_Client ( Version: 1.2.6.1 )");
            if(RChwnd==IntPtr.Zero){
                RChwnd = User32API.FindWindow("#32770", null);
            }
            if(RChwnd!=IntPtr.Zero){
                
                User32API.ShowWindow(RChwnd, ShowWindowCmd.SW_SHOWMINIMIZED);
            }

            string tmp;
            Version ApplicationVersion = new Version(Application.ProductVersion);
            tmp = string.Format("IP:{0},版本号:{1},脚本端口{2}", Game.GetLocalIp(), ApplicationVersion.ToString(), m_UDPPORT);
            WriteToFile(tmp);
            return;
        }
        /// <summary>
        ///线程暂停
        /// </summary>
        /// <param name="time">时间单位毫秒</param>
        public static void Sleep(int time)
        {
            Thread.Sleep(time);
            return;
        }
        /// <summary>
        ///日志输出
        /// </summary>
        /// <param name="tmp">日志内容</param>
        public static void WriteToFile(string tmp)
        {

            Program.AppRunTime = 0;
            if (Program.bRelease)
            {
                udpdd.theUDPSend(18, tmp, OrdNo);
                Console.WriteLine(tmp);
                FileRW.WriteToFile(tmp);
            }
            else
            {
                FileRW.WriteToFile(tmp);
                Console.WriteLine(tmp);
            }
            return;
        }
        /// <summary>
        /// 重启电脑
        /// </summary>
        public void RestartPC()
        {
#if DEBUG
#else
            System.Diagnostics.Process.Start("shutdown", @"/r");
#endif
            return;
        }
        /// <summary>
        /// 截图
        /// </summary>
        public void CaptureJpg()
        {
            WriteToFile("重启计算机");
            string tmp = SetPicPathBmp(LocalPicPath, OrdNo);
            Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
            User32API.MakeSureDirectoryPathExists(m_strCapturePath);
            RECT rect = new RECT(0, 0, 1200, 890);
            Game.CaptureBmp(bm, rect, tmp);
            return;
        }
        /// <summary>
        /// 设置截图路径
        /// </summary>
        /// <param name="str">文件夹路径</param>
        /// <param name="strPicID">图片编号</param>
        /// <returns>图片路径</returns>
        public string SetPicPath(string str, string strPicID)
        {

            if (strLastPicID != strPicID)
                PicNum = 1;
            string strFileName;
            if (strPicID == "")
                strFileName = string.Format("{0}R_{1:00}.bmp", str, PicNum++);

            else
                strFileName = string.Format("{0}{1}_{2:00}.bmp", str, strPicID, PicNum++);
            strLastPicID = strPicID;
            return strFileName;
        }
        /// <summary>
        /// 设置截图路径-做单异常截图
        /// </summary>
        /// <param name="str">文件夹路径</param>
        /// <param name="strPicID">图片编号</param>
        /// <returns>图片路径</returns>
        public string SetPicPathBmp(string str, string strPicID)
        {
            if (strLastPicID != strPicID)
                PicNum = 1;
            string m_month;
            m_month = string.Format("{0}{1}_{2}\\{3}\\{4}", str, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, strPicID);
            if (!Directory.Exists(m_month))
            {
                Directory.CreateDirectory(m_month);
            }
            string strFileName;
            if (strPicID == "")
                strFileName = string.Format("{0}\\R_{1:00}.bmp", m_month, PicNum++);
            else
                strFileName = string.Format("{0}\\{1}_{2:00}.bmp", m_month, strPicID, PicNum++);
            strLastPicID = strPicID;
            return strFileName;
        }
        /// <summary>
        /// bmp截图
        /// </summary>
        /// <param name="strPicName">名称</param>
        public void CaptureBmpInRect(string strPicName)
        {
            IntPtr desk = User32API.GetDesktopWindow();
            m_hGameWnd =User32API.FindWindow(null, "奇迹世界SUN");
            RECT rt = new RECT(0, 0, 0, 0);
            if (m_hGameWnd == IntPtr.Zero || strPicName == "失败")
            {
                m_hGameWnd = desk;
            }
            User32API.GetWindowRect(m_hGameWnd, out rt);
            if((rt.right-rt.left)<100||(rt.bottom-rt.top)<100)
                m_hGameWnd = desk;
            string str = "E:\\piccQQSJ\\";
            string month = null;
            string day = null;
            if (DateTime.Now.Month > 9)
            {
                 month = DateTime.Now.Month.ToString();
            }
            else {
                month = "0" + DateTime.Now.Month;
            }
            if (DateTime.Now.Day > 9)
            {
                day = DateTime.Now.Day.ToString();
            }
            else
            {
                day = "0" + DateTime.Now.Day;
            }
            
            
            
            
            string path = string.Format("{0}{1}_{2}\\{3}\\{4}\\", str, DateTime.Now.Year, month, day, OrdNo);

            string Time = DateTime.Now.Hour+""+ DateTime.Now.Minute +""+ DateTime.Now.Second + "_";

            strPicName = path +Time+ strPicName + ".bmp";

            Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
            User32API.MakeSureDirectoryPathExists(strPicName);

            RECT rect = new RECT(rt.left,rt.top,rt.right,rt.bottom);
            Game.CaptureBmp(bm, rect,  strPicName);
            

        }
        /// <summary>
        /// 答题角色图
        /// </summary>
        /// <param name="strPicName">名称</param>
        /// <param name="left">边界：左</param>
        /// <param name="top">边界：上</param>
        /// <param name="right">边界：右</param>
        /// <param name="bottom">边界：下</param>
        public void CaptureBmpInRect1(string strPicName, int left, int top, int right, int bottom)
        {
            string PicName = m_strProgPath + "\\答题\\";
            Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
            User32API.MakeSureDirectoryPathExists(strPicName);
            strPicName = PicName + strPicName + ".bmp";
            RECT rect = new RECT(left, top, right, bottom);
            Game.CaptureBmp(bm, rect, strPicName);

            //根据卖家角色名生成图片
            Bitmap bmp = new Bitmap(right - left+21, 20);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, right - left+20, 20));
            g.DrawString("卖家角色名：" + m_strSellerRoleName, new Font("宋体", 11, FontStyle.Regular), Brushes.Black, new PointF(0, 0));
            bmp.Save(PicName + "角色.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            CutImage(PicName + "角色序号模板.bmp", PicName + "角色序号.bmp", right - left, bottom - top, 0, 0);
        }
        ///   <summary>   
        ///   从图片中截取部分生成新图   
        ///   </summary>   
        ///   <param   name= "OriginalFile "> 原始图片路径 </param>   
        ///   <param   name= "SaveFile "> 生成新图路径 </param>   
        ///   <param   name= "width "> 截取图片宽度 </param>   
        ///   <param   name= "height "> 截取图片高度 </param>   
        ///   <param   name= "CoordX "> 截图原图片X坐标 </param>   
        ///   <param   name= "CoordY "> 截取原图片Y坐标 </param>   
        public static void CutImage(string OriginalFile, string SaveFile, int width, int height, int CoordX, int CoordY)
        {
            //载入底图   
            Image FromImage = Image.FromFile(OriginalFile);
            int x = 0;   //截取X坐标   
            int y = 0;   //截取Y坐标   
            //原图宽与生成图片宽   之差       
            //当坐标小于0(即原图宽小于要生成的图)时，新图宽度为较小者   即原图宽度   X坐标则为0     
            //当坐标大于0(即原图宽大于要生成的图)时，新图宽度为设置值   即width X坐标则为   sX与CoordX之间较小者      
            int sX = FromImage.Width - width;
            int sY = FromImage.Height - height;
            if (sX > 0)
            {
                x = sX > CoordX ? CoordX : sX;
            }
            else
            {
                width = FromImage.Width;
            }
            if (sY > 0)
            {
                y = sY > CoordY ? CoordY : sY;
            }
            else
            {
                height = FromImage.Height;
            }

            //创建新图位图   
            Bitmap bitmap = new Bitmap(width, height);
            //创建作图区域   
            Graphics graphic = Graphics.FromImage(bitmap);
            //截取原图相应区域写入作图区   
            graphic.DrawImage(FromImage, 0, 0, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            //从作图区生成新图   
            Image SaveImage = Image.FromHbitmap(bitmap.GetHbitmap());
            //保存图象   
            SaveImage.Save(SaveFile, ImageFormat.Bmp);
            //释放资源   
            SaveImage.Dispose();
            bitmap.Dispose();
            graphic.Dispose();
        }  
        /// <summary>
        /// 水印添加
        /// </summary>
        /// <param name="filePic">原图</param>
        /// <param name="rect">水印边界</param>
        /// <returns></returns>
        public bool WaterMark(string filePic, RECT rect)
        {
            if (filePic.IndexOf(".bmp") < 0)
                filePic += ".bmp";
            if (!File.Exists(filePic))
            {

                FileRW.WriteToFile(filePic + "<< 文件不存在！");
                return false;
            }
            Bitmap srcBit = (Bitmap)Bitmap.FromFile(filePic, false);
            Rectangle srcRect = new Rectangle(0, 0, srcBit.Width, srcBit.Height);
            BitmapData pBData = srcBit.LockBits(srcRect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            int Swidth = pBData.Width;
            int Sheight = pBData.Height;
            byte[] tData = new byte[Swidth * Sheight * 4];
            System.Runtime.InteropServices.Marshal.Copy(pBData.Scan0, tData, 0, pBData.Stride * pBData.Height);
            srcBit.UnlockBits(pBData);
            srcBit.Dispose();

            for (int y = 0; y < Sheight; y++)
            {
                int num = y * pBData.Stride;
                for (int x = 0; x < Swidth; x++)
                {
                    if (x > rect.left && y > rect.top && x < rect.right && y < rect.bottom)
                    {
                        tData[num + 2] = 18;
                        tData[num + 1] = 18;
                        tData[num + 0] = 18;
                    }
                    num += 4;
                }
            }
            try
            {
                ImageTool.CreatBmpFromByte(tData, Swidth, Sheight).Save(filePic);
            }
            catch (Exception err)
            {
                WriteToFile(err.ToString());
            }
            return true;
        }
        /// <summary>
        /// 拼图
        /// </summary>
        /// <param name="strAllPic">图片名称集合</param>
        /// <param name="strPicID">拼图ID</param>
        /// <param name="bVerbical">true：生成bmp；false：生成jpg</param>
        /// <returns></returns>
        public int PinTu(string strAllPic, string strPicID, bool bVerbical)
        {
            string m_strProgPath1 =m_strProgPath + "\\答题\\";
            string[] arrPicName;
            int num = SplitString(strAllPic, ",", out arrPicName);

            for (int i = 0; i < num + 1; i++)
            {
                if (arrPicName[i] == "换行")
                {
                    CreatePlate("换行", "", nZHPicWidth);
                    continue;
                }

                CreatePlate(m_strProgPath1 + arrPicName[i], "", nZHPicWidth);
            }
            string strPic = m_strProgPath1 + "模板.bmp";
            if (bVerbical)
            {
                if (!CreatePlate("生成图片", strPic, 0))
                    return 1;
            }
            else
            {
                if (!CreatePlate("生成图片", strPic, nZHPicWidth))
                    return 1;
            }
            Bitmap bbmp = new Bitmap(strPic, true);
            int x = 0, y = 0, z = 0;
            for (int i = 0; i < num + 1; i++)
            {

                if (arrPicName[i] == "换行")
                {
                    //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "换行");
                    ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "换行");
                    continue;
                }
                if (!File.Exists(m_strProgPath1 + arrPicName[i] + ".bmp"))
                    continue;
                Bitmap sbmp = new Bitmap(m_strProgPath1 + arrPicName[i] + ".bmp", true);
                //ImageTool.CreatBmpFromByte(bbmp, sbmp, ref x, ref y, ref z, "");
                ImageTool.BmpInsert(bbmp, sbmp, ref x, ref y, ref z, "");
                sbmp.Dispose();

                //删除小图
                if (Program.bRelease)
                    File.Delete(m_strProgPath1 + arrPicName[i] + ".bmp");


            }
            string PicName = SetPicPath(m_strProgPath, strPicID);
            if (bVerbical)
                //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "").Save(m_strCapturePath + strPicID + ".bmp", ImageFormat.Bmp);
                ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "").Save(m_strProgPath1 + strPicID + ".bmp", ImageFormat.Bmp);
            else
            {

                string strJpg = PicName.Replace(".bmp", ".jpg");
                //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "").Save(strJpg, ImageFormat.Jpeg);
                ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "").Save(strJpg, ImageFormat.Jpeg);
                
            }
            bbmp.Dispose();
            File.Delete(m_strProgPath1 + "模板.bmp");
            return 1;
        }
        /// <summary>
        /// 提取字符串
        /// </summary>
        /// <param name="strScr">原字符串</param>
        /// <param name="strFG">分隔符</param>
        /// <param name="strArray">得到字符串数组</param>
        /// <returns></returns>
        public int SplitString(string strScr, string strFG, out string[] strArray)
        {
            strArray = new string[50];
            int n = 0;
            int num = 0;
            while (true)
            {
                n = strScr.IndexOf(strFG);
                if (n < 0)
                {
                    strArray[num] = strScr;
                    break;
                }
                strArray[num] = strScr.Substring(0, n);
                strScr = strScr.Substring(n + 1, strScr.Length - n - 1);
                if (strScr == "")
                    break;
                num++;
            }
            return num;
        }
        /// <summary>
        /// 图片生成
        /// </summary>
        /// <param name="filePic">操作标示</param>
        /// <param name="strPicID">图片ID</param>
        /// <param name="width">宽</param>
        /// <returns></returns>
        public bool CreatePlate(string filePic, string strPicID, int width)
        {
            if (filePic == "生成图片")
            {
                bPicFull = true;
                if (ptMAX.X == 0 && ptMAX.Y == 0)
                {
                    WriteToFile("大图为空\r\n");
                    bPicFull = false;
                    return false;
                }

                if (ptBigPic.Y > 0)//多排
                {
                    ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                    ptMAX.Y += ptBigPic.Y;
                }
                else
                    ptMAX.X += ptBigPic.X;

                if (width != nZHPicWidth)  //取消宽度限制
                    width = ptMAX.X;
                //byte[] pBMPData = new byte[(width + 3) / 4 * 4 * ptMAX.Y * 4];
                byte[] pBMPData = new byte[width * 4 * ptMAX.Y];


                try
                {
                    if (filePic == "生成图片")
                    {
                        ImageTool.CreatBmpFromByte1(pBMPData, width, ptMAX.Y).Save(strPicID);
                    }
                    else
                    {
                        ImageTool.CreatBmpFromByte(pBMPData, width, ptMAX.Y).Save(strPicID);
                    }
                }
                catch (Exception err)
                {
                    WriteToFile(err.ToString());
                    //throw err;
                }
                //ImageTool.CreatBmpFromByte(pBMPData, width, ptMAX.Y).Dispose();
                bPicFull = false;
                ptBigPic.X = 0;
                ptBigPic.Y = 0;
                ptMAX.Y = 0;
                ptMAX.X = 0;
                return true;
            }
            if (filePic == "换行")
            {
                ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                ptBigPic.X = 0;
                ptBigPic.Y += ptMAX.Y + 1;
                ptMAX.Y = 0;
                return true;
            }
            if (filePic.IndexOf(".bmp") < 0)
                filePic += ".bmp";
            if (!File.Exists(filePic))
            {

                FileRW.WriteToFile(filePic + "<< 文件不存在！");
                return false;
            }
            Bitmap srcBit = (Bitmap)Bitmap.FromFile(filePic, false);
            Rectangle srcRect = new Rectangle(0, 0, srcBit.Width, srcBit.Height);
            BitmapData pBData = srcBit.LockBits(srcRect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);


            //System.Runtime.InteropServices.Marshal.Copy(pBData.Scan0, tData, 0, pBData.Stride * pBData.Height);
            srcBit.UnlockBits(pBData);
            srcBit.Dispose();




            int Swidth = pBData.Width;
            int Sheight = pBData.Height;

            if ((Lwidth - ptBigPic.X) < (Swidth/*+8*/))
            {
                //WriteToFile("宽度不足,换行\r\n");
                ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                ptBigPic.X = 0;
                ptBigPic.Y += ptMAX.Y + 1;
                ptMAX.Y = 0;
                //WriteToFile("ptBigPic.y=%d\r\n",ptBigPic.y);
            }

            if ((Lheight - ptBigPic.Y) < Sheight)
            {
                //WriteToFile("Lheight-ptBigPic.y={0}-{1}",Lheight,ptBigPic.Y);
                WriteToFile("高度不足\r\n");
                return false;

            }


            Point ptLPic = new Point(0, 0);
            ptLPic.Y = ptBigPic.Y;
            ptLPic.X = ptBigPic.X;


            ptMAX.Y = Math.Max(ptMAX.Y, Sheight);/*ptMAX.y<Sheight?Sheight:ptMAX.y;*/


            ptBigPic.X += Swidth;

            if (!bPicFull)
                return true;

            return true;
        }
        /// <summary>
        /// 请求人工答题
        /// </summary>
        /// <param name="CodeType">数据格式 3</param>
        /// <param name="ImagePath">图片路径</param>
        /// <param name="Explain"></param>
        /// <param name="time">答题超时时间：单位秒</param>
        /// <returns>答题结果</returns>
        public string RequestSafeCardInfo(int CodeType, string ImagePath, string Explain, int time)
        {
            #region 说明
            //请求订单数据 & 接收订单数据 & 发送确认
            //	TRANS_REQ_IDCODE_RESULT  = 30,    //机器人请求GTR处理验证码               ( ROBOT -> RC2 ) 
            //TRANS_RES_IDCODE_RESULT  = 31,    //发送处理完的验证码的到机器人程序      ( RC2 -> ROBOT ) 
            //TRANS_IDCODE_INPUT_RESULT = 32,   //机器人输入验证码后的结果发送给客户端  ( ROBOT -> RC2 )
            // 
            //30 数据格式:
            //FCodeType=  答题类型(不能为空)
            //1. 文字验证码.
            //2. 密保验证码.
            //3. 坐标验证码.
            //FImageName= 验证码图片文件的全路径(不能为空)
            //FQuestion=  一些说明文本(可为空) 
            //FTimeout=   超时值(单位秒)
            //FSmsMobile=%s\r\n
            //FSmsValue=%s\r\n
            //FSmsAddress=%s\r\n
            #endregion
            if (OrdNo == "测试订单")
            {
                WriteToFile("测试订单,选择1");
                return "1";
            }
            IsAnswer = false;
            string strSendData;
            WriteToFile("发送验证码");
            m_strOrderData = "";
            strSendData = string.Format("FCodeType={0}\r\nFImageName={1}\r\nFQuestion={2}\r\nFTimeout={3}\r\n", CodeType, ImagePath, Explain, time);
            udpdd.theUDPSend(30, strSendData, OrdNo);
            Sleep(1000);
            for (int i = 0; i < time; i++)
            {
                if ("" != m_strOrderData)
                {
                    yzmTimes++;
                    IsAnswer = true;
                    string tmp;
                    tmp = string.Format("答题返回:{0}", m_strOrderData);
                    WriteToFile(tmp);
                    return m_strOrderData;
                }
                Sleep(1000);
                if (i % 20 == 15)
                    WriteToFile("等待角色返回");
            }
            WriteToFile("等待角色返回超时");
            return "error";
        }
        /// <summary>
        ///post方法调用web接口
        /// </summary>
        /// <param name="url">接口url</param>
        /// <param name="postData">数据</param>
        /// <returns>接口返回结果</returns>
        public static string PostUrlData(string url, string postData)
        {

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            System.Net.HttpWebRequest objWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            objWebRequest.Method = "POST";
            objWebRequest.ContentType = "application/x-www-form-urlencoded";
            objWebRequest.ContentLength = byteArray.Length;
            Stream newStream = objWebRequest.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length); //写入参数 
            newStream.Close();
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);//Encoding.Default
            string textResponse = sr.ReadToEnd(); // 返回的数据
            return textResponse;
        }
        public IntPtr ReturnGameWindowHwnd(IntPtr hwnd, string strTitleName, bool isWindow)
        {
            while (hwnd != IntPtr.Zero)
            {
                StringBuilder strTitle = new StringBuilder(1024);
                User32API.GetWindowText(hwnd, strTitle, 1024);
                string cstrTitle = strTitle.ToString();
                if (cstrTitle == strTitleName)
                {
                    if (isWindow)
                    {
                        if (User32API.IsWindowVisible(hwnd))
                            return hwnd;
                        else
                            return IntPtr.Zero;
                    }
                    else
                        return hwnd;
                }
                else
                {
                    IntPtr childHwnd = User32API.GetWindow(hwnd, GetWindowEnums.GW_CHILD);
                    if (childHwnd != IntPtr.Zero)
                    {
                        IntPtr hwndS = ReturnGameWindowHwnd(childHwnd, strTitleName, isWindow);
                        if (hwndS != IntPtr.Zero)
                            return hwndS;
                    }
                }
                hwnd = User32API.GetWindow(hwnd, GetWindowEnums.GW_HWNDNEXT);

            }
            return IntPtr.Zero;
        }

    }

}

