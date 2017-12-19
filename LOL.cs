using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging; //ImageFormat.Jpeg
using System.Web;
using System.Net;
using System.Text.RegularExpressions;//正则
using System.Windows.Forms;
using RC_ZH_LOL;

namespace GTR
{
    class LOL
    {
        #region 初始化通信变量
        //订单数据
        public static string m_strOrderData;
        //udpsockets
        public static udpSockets udpdd;
        //脚本端口
        public static int m_UDPPORT = 6801;
        //RC传入端口
        public static int the_nRC2Port = 1;
        //订单类型-交易单/发布单
        public static string m_strOrderType;
        //订单号
        public static string OrdNo = "测试订单"; //"MZH-160607000000001";
        //订单状态
        public int Status;
        public int IsStop = 1;
        public static IntPtr ChangerHWND;

        #endregion

        #region 初始化程序变量

        //验证码返回
        public static string mouseP = "";
        //验证码回答次数
        public int yzmTimes = 0;
        int IsCallPone = 1;
        string m_mobible = "";
        int WeGametime = 0;
        bool noDel = false;
        bool IsGetZie = false;
        bool IsSucess = false;
        public string IsNeedRecognition;
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
        string m_strLastName;
        int m_nPicNum;
        static int picNum = 0;
        public int time = 0;
        public int QWE = 0;
        //拼图找不到图片次数
        int C = 0;
        //拥有符文页数
        public int RenusNum;
        //拥有的英雄数量
        public Int64 intHero = 0;
        //拥有的皮肤数量
        public Int64 intSkin = 0;
        //拥有的金币
        public Int32 intCoin = 0;
        public Int32 intLevel = 0;
        //点券
        public Int32 djc = 0;
        //输出截图目录
        public bool IsSendCapPath = false;

        //段位
        public string Level = "当前赛季无";
        //等级
        public string grade = "";
        //QQ好友数量
        string FriendNumber = "";
        //身份证绑定
        string IDNumber = "";
        //QQ等级
        string QQLevel = "";
        string emailband = "";
        //绑定手机号码
        string BoundMobilePhoneNumber = "";
        string FriendN = "";
        bool zzb = true;

        //遍历的文件名数组
        string[] FileArr;
        bool isGoTGP = true;
        string wwPath = m_strPicPath + "ww\\";
        int writetime = 0;

        struct CoinStruct
        {
            public int x;
            public String no;
        }
        //答题标志
        public static bool IsAnswer;
        //邮寄标志
        public static bool IsAskMail;
        //移交标志
        public static bool bYiJiao = false;
        //进入游戏标志
        public static bool IfEnter = false;
        //M站点订单标志
        public static bool MZH = false;
        string QQstuctd;
        //窗口句柄
        public static IntPtr m_hGameWnd;
        public static IntPtr HwndWnd;
        public static IntPtr m_hGameWndTGP;
        //程序所在路径
        private string m_strProgPath = System.Windows.Forms.Application.StartupPath;
        //匹配图片路径
        public static string m_strPicPath = System.Windows.Forms.Application.StartupPath + @"\英雄联盟\";
        //异常截图保存路径
        public static string LocalPicPath = "E:\\英雄联盟账号截图\\";
        //[WebMethod]
        //public string Project(string paramaters)
        //{

        //    return paramaters;

        //}
        //订单详细数据
        public string m_strVirtualIP;
        public string m_strLocalIP;
        public string m_GameTitle;
        public string m_strGamePath;
        public string m_strAccount = "";
        public string m_strPassword;
        public string m_strGameName;
        public string m_strArea;
        public string m_strServer;
        public string m_strSellerRole;
        public string m_strSecondPwd;
        public string m_strGameStartFile;
        public string m_strMbkID;
        public string m_strMbkImage;
        public string m_strMbkString;
        public string m_strCapturePath = "";
        public string m_GameId;
        public string GameID;
        public static StringBuilder strb = new StringBuilder();
        #endregion
        //主函数入口
        public void StartToWork()
        {
            #region 虚拟机判断 IP获取 异常截图目录设置

            string ss = m_strProgPath.Substring(0, 1);

            if (ss == "C")
            {
                LocalPicPath = "Z:\\jiaoben\\";
                m_strLocalIP = FileRW.ReadFile("Z:\\jiaoben\\IP.txt");
                string temp = string.Format("该机为虚拟机,虚拟机IP:{0},实体机IP:{1}", Game.GetLocalIp(), m_strLocalIP);
                WriteToFile(temp);
                if (m_strLocalIP == "" && File.Exists(LocalPicPath))
                {
                    WriteToFile("找不到异常截图存放路径，需要在该做单机共享F盘");
                    return;//直接卡死RC
                }
            }
            else
            {
                WriteToFile("该机为做单机");

                User32API.WinExec("rasphone -h 宽带连接", 2); //连
                Sleep(1000);
                User32API.WinExec("rasphone -d 宽带连接", 2); //断
                for (int i = 0; i < 8; i++)
                {
                    if (User32API.FindWindow(null, "正在连接到 宽带连接...") != IntPtr.Zero)
                    {
                        Sleep(3000);
                        WriteToFile("宽带连接中...");
                    }
                    if (User32API.FindWindow(null, "连接到 宽带连接 时出错") != IntPtr.Zero)
                    {
                        WriteToFile("错误拨号失败不进行拨号");
                        Game.tskill("rasphone");
                        Sleep(500);
                        if (User32API.FindWindow(null, "连接到 宽带连接 时出错") == IntPtr.Zero)
                            break;
                    }
                    if (User32API.FindWindow(null, "网络连接") != IntPtr.Zero)
                    {
                        WriteToFile("拨号失败不进行拨号");
                        Game.tskill("rasphone");
                        if (User32API.FindWindow(null, "网络连接") == IntPtr.Zero)
                            break;
                    }
                }
            }

            #endregion
            try
            {
                DeleteFolder(LocalPicPath, 7);
                Status = GameProc();
            }
            catch (System.Exception ex)
            {
                WriteToFile(ex.ToString());
                Status = 2120;
            }
            CloseGames();
            if (MZH && Status != 1000)
            {
                if (Status > 2000 && Status < 3000)
                    Status += 2000;
            }
            string tmp = "";
            string IPoneList = "";
            IPoneList = FileRW.ReadFile("ipone.txt");
            if (Status == 1000)
            {
                tmp = string.Format("截图成功,共{0}张\r\n", picNum);
                WriteToFile(tmp);
            }
            if (Status > 1000)
            {
                CaptureJpg("订单失败");
                FileRW.DeleteTmpFiles(m_strCapturePath + OrdNo);
            }

            if ((Status == 3000 || Status == 3333) && m_mobible != "" && m_strOrderType == "发布单" && !OrdNo.Contains("M") && !IPoneList.Contains(m_mobible))
            {
                //-------电话短信---------
                if (IsCallPone == 0)
                {
                    string PostData = string.Format("gameId={0}&OrdNo={1}&Status={2}", "60", OrdNo, Status);
                    string strHTML = PostUrlData(" http://172.16.74.147:8010/WebService.asmx/SetPhoneData", PostData);
                    WriteToFile(strHTML);
                    Status = 3333;
                    tmp = string.Format("移交状态={0}\r\n", Status);
                    WriteToFile(tmp);
                    tmp = string.Format("FStatus={0}\r\n", Status);
                }
                if (IsCallPone == 1 && Status == 3333)
                {
                    string PostData = string.Format("Method=CallSellerMobFB&OrderNO={0}&SellerMob={1}&GameName=英雄联盟", OrdNo, m_mobible);
                    string strHTML = PostUrlData("http://192.168.36.8/OrderAid.aspx", PostData);


                    WriteToFile("已拨打卖家" + m_mobible.ToString() + "电话");

                    //接口2,发短信

                    string SendData = string.Format("【5173网】{0}在截图认证中出现异地情况,请将密码修改为原密码加0,修改完成后回复“xmm”为您重新截图。", m_strAccount);
                    PostData = string.Format("Method=SendM&SendContent={0}&SendMob={1}&OrderNO={2}", SendData, m_mobible, OrdNo);
                    string strHTML2 = PostUrlData("http://192.168.36.8/OrderAid.aspx", PostData);

                    WriteToFile("已发送短信");

                    PostData = string.Format("gameId={0}&OrdNo={1}&Status=0", "60", OrdNo);
                    string strHTML3 = PostUrlData(" http://172.16.74.147:8010/WebService.asmx/SetPhoneData", PostData);
                    WriteToFile(strHTML3);


                    DateTime CurrentTime = System.DateTime.Now.AddHours(1);
                    string strYMD = CurrentTime.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    tmp = string.Format("移交状态={0}\r\nFPlanTime={1}\r\n", 0, strYMD);
                    WriteToFile(tmp);
                    WriteToFile("下次执行时间" + strYMD);
                }

                if (IsCallPone == 1 && Status == 3000)
                {
                    tmp = string.Format("移交状态={0}\r\n", Status);
                    WriteToFile(tmp);
                    tmp = string.Format("FStatus={0}\r\n", Status);
                }
                //-----------------------
            }
            else
            {
                if (IPoneList.Contains(m_mobible) && Status == 3333)
                    WriteToFile("大卖家已忽略打电话");
                tmp = string.Format("移交状态={0}\r\n", Status);
                WriteToFile(tmp);
                tmp = string.Format("FStatus={0}\r\n", Status);
            }

            #region//①记录做单情况 ②连续失败5单重启电脑（频繁重启导致订单超时）
            StringBuilder retVal = new StringBuilder(256);
            User32API.GetPrivateProfileString("记录参数", "ADSL本次做单", "", retVal, 256, m_strProgPath + "\\adsl.ini");
            int num = 0;
            if (retVal.ToString() != "")
                num = int.Parse(retVal.ToString());
            if (num > 100)
            {
                User32API.WritePrivateProfileString("记录参数", "ADSL本次做单", "0", m_strProgPath + "\\adsl.ini");
                Sleep(2500);
            }
            else
            {
                if ((Status > 1000 && Status < 3000) || Status > 4000)
                {
                    string strNum = string.Format("{0}", num + 20);
                    User32API.WritePrivateProfileString("记录参数", "ADSL本次做单", strNum, m_strProgPath + "\\adsl.ini");
                }
            }


            StringBuilder retVal1 = new StringBuilder(256);
            User32API.GetPrivateProfileString("记录参数", "连续失败", "", retVal1, 255, m_strProgPath + "\\adsl.ini");
            int num1 = int.Parse(retVal1.ToString());
            if ((Status > 1000 && Status < 3000 && Status != 3333) || (Status > 4000 && Status != 4050))
            {
                if (num1 == 5)
                {
                    User32API.WritePrivateProfileString("记录参数", "连续失败", "0", m_strProgPath + "\\adsl.ini");
                    RestartPC();//重启电脑
                }
                else
                {
                    string strNum1 = string.Format("{0}", num1 + 1);
                    User32API.WritePrivateProfileString("记录参数", "连续失败", strNum1, m_strProgPath + "\\adsl.ini");

                }
            }
            #endregion

            if (the_nRC2Port != 0)
            {
                for (int j = 0; j < 2; j++)
                {
                    try
                    {
                        udpdd.theUDPSend((int)TRANSTYPE.TRANS_ORDER_END, tmp, OrdNo);//发送UDP
                    }
                    catch (Exception ex)
                    {
                        WriteToFile(ex.ToString());
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        if (bYiJiao)
                        {
                            WriteToFile("移交成功");

                            break;
                        }
                        Sleep(100);
                    }
                    if (bYiJiao)
                    {
                        break;
                    }
                    if (j == 1)
                        WriteToFile("移交失败");
                }
            }
            else
            {
                WriteToFile("端口为0");
            }
            return;
        }
        /// 主函数
        /// <returns>订单状态</returns>
        public int GameProc()
        {
            #region 加载鼠标驱动
            if (!KeyMouse.InitKeyMouse())
            {
                WriteToFile("驱动加载失败");
                return 2260;
            }
            #endregion

            #region 判断订单类型请求订单信息
            if (OrdNo.IndexOf("MZH") == 0)
                MZH = true;
            int n = OrdNo.IndexOf("-");
            if (n > 0 || OrdNo == "测试订单" || MZH)
                m_strOrderType = "发布单";
            else
                m_strOrderType = "交易单";
            if (!RequestOrderData())
                return 2260;
            if (!ReadOrderDetail())
                return 2260;
            #endregion

            #region 账号中文审核
            if (Regex.IsMatch(m_strAccount, @"[\u4e00-\u9fa5]"))
            {
                WriteToFile("账号含有中文");
                WriteToFile(m_strAccount);
                return 3000;
            }
            if (Regex.IsMatch(m_strPassword, @"[\u4e00-\u9fa5]"))
            {
                WriteToFile("密码含有中文");
                WriteToFile(m_strPassword);
                return 3000;
            }
            #endregion

            #region 获取账号是否打过电话

            //-----------测试区域-----------
            //m_mobible = "18672290630";
            //string IPoneList = FileRW.ReadFile("ipone.txt");
            //if (!IPoneList.Contains(m_mobible)) {
            //    Sleep(1000);
            //}

            //OrdNo = "DB088-20171025-34361693";
            //Status = 3000;
            //-----------------------------
            if (m_mobible != "" && OrdNo.Contains("-") && !OrdNo.Contains("MZH"))
            {
                string postData = string.Format("PublishNO={0}", OrdNo);
                string strHTML = PostUrlData("http://gtr.5173.com:7080/RobotCallback.asmx/MobileResult", postData);
                IsCallPone = Convert.ToInt32(MyStr.FindStr(strHTML, "/\">", "</string>"));
                if (IsCallPone == 0)
                {
                    WriteToFile("网页获取值为：" + IsCallPone.ToString() + ",已经拨打过卖家电话");
                    m_strPassword += "0";
                    WriteToFile("已在原密码上加0");
                }
                else
                {
                    WriteToFile("网页获取值为：" + IsCallPone.ToString() + ",未拨打过卖家电话");
                }
            }
            else
                WriteToFile("卖家没有填写手机号或不是主站发布单");

            AppInit();//IP地址 版本号
            #endregion

            //CheckHuaDong();

            for (int i = 0; i < 4; i++)
            {
                #region 关闭进程
                CloseGames();
                m_hGameWnd = User32API.GetDesktopWindow();
                for (int j = 0; j < 40; j++)
                {
                    int a = 5;
                    a = a * j;
                    Sleep(10);
                    KeyMouse.MouseMove(m_hGameWnd, 926 + a, 1007);
                }
                #endregion

                #region 网页资料验证
                if (MZH && i == 0)
                {
                    try
                    {
                        int a = ReadQQ();
                        if (a == 3000 || a == 3333)
                        {
                            return a;
                        }

                    }
                    catch (Exception e)
                    {
                        WriteToFile(e.ToString());
                    }

                    if (!zzb)
                        return 3301;
                }
                #endregion

                #region TGP 协议获取数据
                Status = GetHeroAndSkin();
                if (Status == 2120U && i == 0)
                    continue;
                if (Status <= 1000)
                {
                    try
                    {
                        LOLRead(m_strProgPath + @"\RoleInfo.txt");//读取账号信息
                        if (Status == 3010)
                            return Status;
                    }
                    catch (Exception ex)
                    {
                        WriteToFile(ex.ToString());
                    }
                    if (Status != 1000)
                        return 2120;
                    if (intHero > 0)
                        Status = ReplaceTGP(true, m_strProgPath + @"\Hero\");//英雄拼图
                    if (intSkin > 0 && Status <= 1000)
                        Status = ReplaceTGP(false, m_strProgPath + @"\Skin\");//皮肤拼图 

                }
                #endregion

                #region TGP登录验证账号 是否密码错 无限验证
                if (Status > 1000 && Status != 3333 && !IsSucess)
                {
                    CloseGames();
                    Status = RunGame();//开启TGP
                    if (Status > 1000)
                    {
                        IsSucess = true;
                        continue;
                    }
                    Status = EnterAccPwd();
                    if (Status == 2120)
                        continue;
                }
                #endregion

                #region TGP验证登录成功 协议获取失败 网页工具方案
                if (IsSucess && Status > 1000 && Status != 3333)
                {
                    if (true)
                    {
                        WriteToFile("TGP登录成功工具异常,使用LOL网页工具");
                        IsGetZie = true;
                        GetLOLInfo();
                        try
                        {
                            LOLRead2(m_strProgPath + @"\RoleInfo.txt");
                        }
                        catch (Exception ex)
                        {
                            WriteToFile("网页获取失败");
                        }
                        if (intHero > 0)
                            Status = ReplaceTGP(true, m_strProgPath + @"\Hero\");//英雄拼图
                        if (intSkin > 0 && Status <= 1000)
                            Status = ReplaceTGP(false, m_strProgPath + @"\Skin\");//皮肤拼图 
                    }

                }
                #endregion

                #region 向服务器发送获取的内容
                if (Status == 1000 && MZH)
                {
                    WriteToFile("向服务器发送帐号信息");
                    try
                    {
                        AutoVerifya();
                    }
                    catch (Exception ex)
                    {
                        WriteToFile(ex.ToString());
                        WriteToFile("向服务器发送帐号信息异常");
                    }

                }
                #endregion
                break;
            }
            return Status;
        }
        // 启动TGP
        public int RunGame()
        {

            bool NoPlayWeGame = false;
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            RECT ra = new RECT();

            for (int i = 0; i < 3; i++)
            {
                Game.RunCmd("taskkill /im  tgp_daemon.exe /F");
                Sleep(500);
                myapp.RunBat(m_strGameStartFile);
                WriteToFile(m_strGameStartFile);
                Sleep(5000);
                m_hGameWnd = User32API.FindWindow(null, "WeGame");
                if (m_hGameWnd == IntPtr.Zero)
                    continue;
                for (int y = 0; y < 60; y++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "WeGame登录.bmp", 800, 400, 1100, 750);
                    if (pa.X > 0)
                    {
                        WriteToFile("进入WeGame账号输入界面");
                        return 1;
                    }
                    if (y % 6 == 0)
                    {
                        WriteToFile("等待WeGame更新");
                    }
                    Sleep(2000);
                }
            }
            WriteToFile("WeGame重启三次均失败");
            return 2260;
        }
        //输入帐号密码      
        public int EnterAccPwd()
        {
            //--------------------------------------------------
            //WriteToFile("TGP登录成功");
            //IsSucess = true;
            //return 2120;
            // -------------------------------------------------
            #region 初始化变量
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            Point pc = new Point(-1, -2);
            Point pd = new Point(-1, -2);
            RECT rt = new RECT(0, 0, 1024, 768);
            //验证码
            string Yzm = "";
            //密码输入次数
            int k = 0;
            //账号输入次数
            int l = 0;
            m_hGameWnd = User32API.FindWindow(null, "WeGame");
            #endregion

            for (int q = 0; q < 6; q++)
            {
                Sleep(3000);

                #region WeGame
                pd = ImageTool.fPic(m_strPicPath + "WeGame登录.bmp", 838, 423, 1069, 713);
                if (pd.X > 0)
                {
                    #region 输入账号密码并登陆
                    //点击账号输入框
                    KeyMouse.MouseClick(m_hGameWnd, pd.X + 60, pd.Y - 133, 1, 2, 1000);
                    //模拟键盘输入Backspace
                    for (int a = 0; a < 2; a++)
                    {
                        KeyMouse.SendBackSpaceKey(2);
                        Sleep(500);
                    }
                    WriteToFile("输入账号");
                    //输入账号
                    KeyMouse.SendKeys(m_strAccount, 200);
                    Thread.Sleep(1000);
                    KeyMouse.MouseClick(m_hGameWnd, pd.X + 18, pd.Y - 92, 1, 2, 1000);
                    //输入密码
                    WriteToFile("输入密码");
                    KeyMouse.SendKeys(m_strPassword, 300);
                    CaptureJpg("账号密码");
                    //CaptureJpg();
                    string accountPassword = "输入账号：[" + m_strAccount + "]" + "密码[" + m_strPassword.Length.ToString() + "]位完成";
                    //日志显示输入账号信息，密码位数
                    WriteToFile(accountPassword);
                    Sleep(1000);
                    for (int z = 0; z < 3; z++)
                    {
                        Sleep(500);
                        pd = ImageTool.fPic(m_strPicPath + "WeGame登录.bmp", 838, 423, 1069, 713);
                        if (pd.X > 0)
                        {
                            KeyMouse.MouseClick(m_hGameWnd, pd.X + 10, pd.Y + 10, 1, 1, 500);//点击登录
                        }
                    }
                    #endregion

                    #region 判断是否出现验证码
                    for (int p = 0; p < 10; p++)
                    {
                        Sleep(1000);
                        //判断是否出现验证码窗口
                        pb = ImageTool.fPic(m_strPicPath + "We验证码.bmp", 853, 356, 1063, 657, 50);
                        pa = ImageTool.fPic(m_strPicPath + "We验证码1.bmp", 853, 356, 1063, 657, 50);
                        pc = ImageTool.fPic(m_strPicPath + "We验证码2.bmp", 853, 356, 1063, 657, 50);

                        if (pa.X > 0 || pb.X > 0 || pc.X > 0)
                        {
                            //验证码答题
                            Status = verificationCode1();
                            if (Status > 1000)
                                return Status;
                            //codeRight(0);
                            break;
                        }
                        if (p == 9)
                        {
                            WriteToFile("未检测到验证码窗口，进入下一步操作");
                        }
                        if (p % 4 == 0)
                        {
                            WriteToFile("检测是否有验证码窗口");
                        }

                    }
                    #endregion

                    #region 判断是否密码错误 异地登录
                    //判断是否密码错误
                    pa = ImageTool.fPic(m_strPicPath + "We密码不正确.bmp", 838, 377, 1069, 713);
                    if (pa.X > 0)
                    {
                        k++;
                        WriteToFile("第" + k.ToString() + "次账号密码错误");
                        if (k == 3)
                        {
                            CaptureJpg("密码错3次");
                            return 3000;
                        }
                        KeyMouse.MouseClick(m_hGameWnd, 908, 582, 1, 1, 1000);
                        continue;
                    }

                    //判断异地登录
                    pa = ImageTool.fPic(m_strPicPath + "We异地登录.bmp", 800, 400, 1100, 560);
                    if (pa.X > 0)
                    {
                        WriteToFile("异地登陆导致无法登陆");
                        CaptureJpg();
                        return 3333;
                    }

                    //判断修改密码
                    pa = ImageTool.fPic(m_strPicPath + "We修改密码.bmp", 800, 400, 1100, 560);
                    if (pa.X > 0)
                    {
                        WriteToFile("该帐号已修改密码");
                        CaptureJpg();
                        return 2120;
                    }

                    //判断登录异常
                    pa = ImageTool.fPic(m_strPicPath + "We登录失败.bmp", 800, 400, 1100, 560);
                    if (pa.X > 0)
                    {
                        KeyMouse.MouseClick(m_hGameWnd, pa.X + 5, pa.Y + 5, 1, 1, 1000);
                        WriteToFile("未识别的问题窗口");
                        CaptureJpg("未判断的问题窗口");
                    }
                    #endregion
                }
                #endregion

                #region 判断账号页面是否消失
                KeyMouse.MouseMove(0, 0);
                Sleep(500);
                pd = ImageTool.fPic(m_strPicPath + "WeGame登录.bmp", 838, 423, 1069, 713);
                if (pd.X < 0)
                {
                    WriteToFile("账号输入页面消失，输入帐号密码阶段完成");
                    break;
                }

                if (q == 5)
                {
                    WriteToFile("未知原因登录失败，以保存截图");
                    return 2260;
                }

                #endregion
            }

            #region //账号问题检测 找到TGP界面标志跳出
            for (int y = 0; y < 3; y++)
            {
                WriteToFile("检测是否有问题窗口出现");
                Sleep(3000);

                #region 判断跳出标志是否存在
                //WeGame
                pa = ImageTool.fPic(m_strPicPath + "We商店界面.bmp", 600, 70, 700, 140);
                if (pa.X > 0)
                    break;
                #endregion

                //判断账号是否存在异常（被冻结）
                pa = ImageTool.fPic(m_strPicPath + "We异地登录.bmp", 800, 400, 1100, 730);
                if (pa.X > 0)
                {
                    WriteToFile("异地登陆导致无法登陆");
                    CaptureJpg();
                    return 3333;
                }


                //判断账号是否已修改密码
                pa = ImageTool.fPic(m_strPicPath + "We修改密码.bmp", 800, 400, 1100, 730);
                if (pa.X > 0)
                {
                    WriteToFile("该帐号已修改密码");
                    CaptureJpg();
                    return 2120;
                }
            }
            #endregion

            WriteToFile("TGP登录成功");
            IsSucess = true;
            return 2120;
        }
        //QQ资料读取
        public int ReadQQ()
        {
            //调用网页验证
            CheckAccount();
            StringBuilder strBuilder = new StringBuilder(512);
            StringBuilder strBuilder1 = new StringBuilder(512);
            StringBuilder strBuilder2 = new StringBuilder(512);
            StringBuilder strBuilder3 = new StringBuilder(512);
            StringBuilder strBuilder4 = new StringBuilder(512);
            StringBuilder strBuilderMax = new StringBuilder(512);
            //获取配置文件信息
            User32API.GetPrivateProfileString("账号信息", "执行状态", "", strBuilderMax, 512, m_strProgPath + "\\roleInfo.ini");
            User32API.GetPrivateProfileString("账号信息", "证件号码", "", strBuilder, 512, m_strProgPath + "\\roleInfo.ini");
            User32API.GetPrivateProfileString("账号信息", "密保手机", "", strBuilder1, 512, m_strProgPath + "\\roleInfo.ini");
            User32API.GetPrivateProfileString("账号信息", "QQ等级", "", strBuilder2, 512, m_strProgPath + "\\roleInfo.ini");
            User32API.GetPrivateProfileString("账号信息", "QQ好友", "", strBuilder3, 512, m_strProgPath + "\\roleInfo.ini");
            User32API.GetPrivateProfileString("账号信息", "邮箱", "", strBuilder4, 512, m_strProgPath + "\\roleInfo.ini");

            QQstuctd = strBuilderMax.ToString();
            if (QQstuctd == "2000")
            {
                WriteToFile("账号密码错误");
                return 3000;
            }
            if (QQstuctd == "2300")
            {
                WriteToFile("账号被冻结");
                return 3333;
            }
            //将此实例中的子字符串的值转换为System.string
            FriendNumber = strBuilder3.ToString();
            IDNumber = strBuilder.ToString();
            if (IDNumber.Equals("已设置"))
            {
                IDNumber = "身份证已设置";
            }
            if (IDNumber.Equals("未设置"))
                IDNumber = "身份证未设置";
            QQLevel = strBuilder2.ToString();
            BoundMobilePhoneNumber = strBuilder1.ToString();
            emailband = strBuilder4.ToString();

            if (emailband == "已设置")
                emailband = "已绑定邮箱";
            if (emailband == "未设置")
                emailband = "未绑定邮箱";
            //if (BoundMobilePhoneNumber="")
            if (BoundMobilePhoneNumber != "")
                BoundMobilePhoneNumber = "手机号已绑定";
            else
                BoundMobilePhoneNumber = "手机号未绑定";
            WriteToFile("QQ好友数量：" + FriendNumber);
            WriteToFile("身份证绑定状态：" + IDNumber);
            WriteToFile("QQ等级：" + QQLevel);
            WriteToFile("密保手机：" + BoundMobilePhoneNumber);
            return 1;
        }
        //验证码答题新版
        public int verificationCode1()
        {
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            Point pc = new Point(-1, -2);
            Point pa1 = new Point(-1, -2);
            Point pb1 = new Point(-1, -2);
            Point pc1 = new Point(-1, -2);
            //验证码
            string Yzm = "";
            m_GameTitle = "腾讯游戏平台";
            m_hGameWnd = User32API.FindWindow(null, m_GameTitle);
            if (m_hGameWnd == IntPtr.Zero)
                m_hGameWnd = User32API.FindWindow(null, "WeGame");
            for (int i = 0; i < 8; i++)
            {
                WriteToFile("发现验证码");
                //将要识别的验证码图片截取下来
                CaptureBmpInRect("\\验证码", m_strProgPath, 861, 444, 989, 496);
                Sleep(100);
                WriteToFile("输入验证码");
                KeyMouse.SendBackSpaceKey(5);
                //自动答题
                if (i > 5)
                {
                    if (i == 6)
                        WriteToFile("转入人工答题");
                    jpgResize(m_strProgPath + "\\验证码.bmp", m_strProgPath + "\\验证码1.jpg", 125, 52);
                    //人工答题返回的验证码
                    Yzm = RequestSafeCardInfo(1, m_strProgPath + "\\验证码1.jpg", "", 180);
                }
                else
                    Yzm = AutoVerify(m_strProgPath + "\\验证码.bmp", 60);
                if (Yzm.Length >= 4)
                    Yzm = Yzm.Substring(0, 4);
                Sleep(500);
                KeyMouse.SendKeys(Yzm, 200);
                WriteToFile(Yzm);
                User32API.SetForegroundWindow(m_hGameWnd);
                KeyMouse.MouseClick(m_hGameWnd, 696 + 210, 383 + 248, 1, 1, 2000);
                pb1 = ImageTool.fPic(m_strPicPath + "We验证码.bmp", 853, 356, 1063, 657, 50);
                pa1 = ImageTool.fPic(m_strPicPath + "We验证码1.bmp", 853, 356, 1063, 657, 50);
                pc1 = ImageTool.fPic(m_strPicPath + "We验证码2.bmp", 853, 356, 1063, 657, 50);

                if (pa1.X <= 0 && pb1.X <= 0 && pc1.X <= 0)
                {

                    WriteToFile("验证成功");
                    //检测TGP进程是否消失
                    m_hGameWnd = User32API.FindWindow(null, m_GameTitle);
                    if (m_hGameWnd == IntPtr.Zero)
                    {
                        m_hGameWnd = User32API.FindWindow(null, "WeGame");
                        if (m_hGameWnd == IntPtr.Zero)
                        {
                            WriteToFile("TGP进程消失");
                            return 2260;
                        }
                    }
                    return 1;
                    if (i > 5)
                        codeRight(1);
                    else
                        RGcodeRight(0);
                }
                if (i <= 5)
                    codeRight(0);
                else
                    RGcodeRight(1);
            }
            WriteToFile("人工答题失败");
            return 2230;

        }
        /// <summary>
        /// 关闭游戏
        /// </summary>
        public void CloseGames()
        {
            Game.RunCmd("taskkill /im  tgp_daemon.exe /F");
            Game.RunCmd("taskkill /im  LOL.exe /F");
            Game.RunCmd("taskkill /im  TGP.exe /F");
        }
        /// <summary>
        /// 请求订单数据
        /// </summary>
        /// <returns></returns>
        public static bool RequestOrderData()
        {
            if (the_nRC2Port == 0)
            {//读本地
                m_strOrderData = FileRW.ReadFile("info.txt");
            }
            else
            { //服务器获取
                m_strOrderData = "";
                string tmp = string.Format("FExeProcID={0}\r\nFRobotPort={1}\r\n", Program.pid, m_UDPPORT);
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
            string m_RegInfos = MyStr.FindStr(m_strOrderData, "<RegInfos>", "</RegInfos>");
            string strItem = MyStr.FindStr(m_RegInfos, "<Name>游戏账号</Name>", "</RegInfoItem>");
            m_strAccount = MyStr.FindStr(strItem, "<Value>", "</Value>");
            if (m_strAccount == "")
            {
                strItem = MyStr.FindStr(m_RegInfos, "<Name>游戏帐号</Name>", "</RegInfoItem>");
                m_strAccount = MyStr.FindStr(strItem, "<Value>", "</Value>");
            }
            strItem = MyStr.FindStr(m_RegInfos, "<Name>游戏密码</Name>", "</RegInfoItem>");
            m_strPassword = MyStr.FindStr(strItem, "<Value>", "</Value>");
            strItem = MyStr.FindStr2(m_RegInfos, "<Name>游戏角色名</Name>", "</RegInfoItem>");
            m_strSellerRole = MyStr.FindStr(strItem, "<Value>", "</Value>");
            strItem = MyStr.FindStr(m_RegInfos, "<Name>仓库密码</Name>", "</RegInfoItem>");
            m_strSecondPwd = MyStr.FindStr(strItem, "<Value>", "</Value>");
            strItem = MyStr.FindStr(m_RegInfos, "<Name>用户类别</Name>", "</RegInfoItem>");
            strItem = MyStr.FindStr(m_RegInfos, "<Name>IsNeedRecognition</Name>", "</RegInfoItem>");
            IsNeedRecognition = MyStr.FindStr(strItem, "<Value>", "</Value>");
            m_mobible = MyStr.FindStr(m_strOrderData, "<SellerMobile>", "</SellerMobile>");
            WriteToFile("是否需要获取QQ信息：" + IsNeedRecognition);
            if (m_strAccount == "")
            {
                WriteToFile("帐号为空");
                try
                {
                    WriteToFile(m_RegInfos);
                }
                catch { WriteToFile("打出订单详情失败"); }
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
            m_strGameName = MyStr.FindStr(m_strOrderData, "<GameName>", "</GameName>");
            m_strArea = MyStr.FindStr(m_strOrderData, "<GameArea>", "</GameArea>");
            m_strServer = MyStr.FindStr(m_strOrderData, "<GameServer>", "</GameServer>");
            m_strGameStartFile = MyStr.FindStr(m_strOrderData, "<GamePath>", "</GamePath>");
            m_GameId = MyStr.FindStr(m_strOrderData, "<GameId>", "</GameId>");
            m_strGameStartFile.LastIndexOf('\\');
            m_strGamePath = m_strGameStartFile.Substring(0, m_strGameStartFile.Substring(0, m_strGameStartFile.LastIndexOf('\\')).LastIndexOf('\\') + 1);
            m_strMbkID = MyStr.FindStr(m_strOrderData, "<Passpod_Id>", "</Passpod_Id>");
            m_strMbkImage = MyStr.FindStr(m_strOrderData, "<SafeCardPath>", "</SafeCardPath>");
            m_strMbkString = MyStr.FindStr(m_strOrderData, "<Passpod_Content>", "</Passpod_Content>");
            m_strCapturePath = MyStr.FindStr(m_strOrderData, "<CapturePath>", "</CapturePath>");
            string strlog = string.Format("游戏名[{0}]", m_strGameName);
            WriteToFile(strlog);
            int tt = m_strCapturePath.LastIndexOf("\\");
            if (m_strCapturePath == "")
                m_strCapturePath = "C:\\拼图\\";
            else if (tt > 0)
                m_strCapturePath += "\\";
            return true;
        }
        public void AppInit()
        {
            string tmp;
            Version ApplicationVersion = new Version(Application.ProductVersion);
            tmp = string.Format("IP:{0},版本号:{1},脚本端口{2}", Game.GetLocalIp(), ApplicationVersion.ToString(), m_UDPPORT);
            WriteToFile(tmp);
            return;
        }
        /// <summary>
        ///线程暂停
        /// </summary>
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
            System.Diagnostics.Process.Start("shutdown", @"/r /t 0");
            return;
        }
        /// <summary>
        /// 请求人工答题
        /// </summary>
        /// <param name="CodeType">数据格式</param>
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
                Console.Write("请输入密保：");
                return Console.ReadLine();
            }
            IsAnswer = false;
            string strSendData;
            WriteToFile("发送验证码...");
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
                    WriteToFile("等待验证码...");
            }
            WriteToFile("等待验证码超时...");
            return "";
        }
        public string AutoVerify(string ImagePath, int GameId)
        {
            if (MZH)
                GameId = 100;
            System.IO.MemoryStream m = new System.IO.MemoryStream();
            System.Drawing.Bitmap bp = new System.Drawing.Bitmap(ImagePath);
            bp.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] b = m.ToArray();
            string base64string = Convert.ToBase64String(b);
            bp.Dispose();
            string strHTML = "";
            String postData = string.Format("orderNo={0}&GameId={1}&JpgBase64={2}", OrdNo, GameId, HttpUtility.UrlEncode(base64string));
            try
            {
                strHTML = PostUrlData("http://192.168.36.245/autotalk/service.asmx/UploadJpgBase64", postData);
                strHTML = MyStr.FindStr(strHTML, "\">", "<");
                return strHTML;
            }
            catch (Exception e)
            {
                WriteToFile(e.Message);
                return "";
            }
            //string code = MyStr.FindStr(strHTML, "\">", "<");
            //WriteToFile("自动答题返回:" + code);
        }
        //获取数据，发送服务器(M)
        public string AutoVerifya()
        {
            //将发送的内容分割开（JSON格式）
            string qq = ",\"gtr_eleven\":";
            string ee = ",\"gtr_ten\":";
            string ww = "{\"gtr_eighteen\":";
            string rr = ",\"gtr_six\":";
            string yy = ",\"dan_name\":";
            string uu = ",\"hero_num\":";
            string ii = ",\"skin_num\":";
            string oo = ",\"gtr_seven\":";
            string pp = ",\"gtr_eight\":";
            string aa = ",\"gtr_bind_mobile\":";
            string bb = ",\"id\":";
            string cc = "}";
            string ida = "\"";
            string zz = "\"property_values\":";
            //获取在之前读取出来的内容并将它们进行格式化
            string IDn = string.Format("{0}", IDNumber);
            string BMPN = string.Format("{0}", BoundMobilePhoneNumber);

            string Grade = string.Empty;
            int qa = 0;
            try
            {
                if (string.IsNullOrEmpty(QQLevel))
                    QQLevel = "0";
                qa = int.Parse(QQLevel);
            }
            catch (Exception err)
            {
                WriteToFile(err.ToString());
                WriteToFile("QQ等级获取异常！");
            }
            if (qa == 0)
                Grade = "QQ等级0级";
            if (0 < qa && qa <= 10)
                Grade = "QQ等级1-10级";
            if (10 < qa && qa <= 20)
                Grade = "QQ等级11-20级";
            if (20 < qa && qa <= 40)
                Grade = "QQ等级21-40级";
            if (qa > 40)
                Grade = "QQ等级40级以上";

            if (FriendNumber.Equals("0"))
                FriendN = "无QQ好友";
            else if (FriendNumber == "")
                FriendN = "";
            else
                FriendN = "有QQ好友";
            // string FriendN = string.Format("{0}", FriendNumber);
            string LEVEL = string.Format("{0}", Level);
            string intskin = string.Format("{0}个皮肤", intSkin);
            string intrenus = string.Format("{0}", RenusNum);
            if (RenusNum == 0)
                intrenus = "";
            string inthero = string.Format("{0}个英雄", intHero);
            string Glevel = string.Format("{0}级", grade);
            string intcoin = string.Empty;
            //if (intCoin <= 1000)
            //    intcoin = "1千以下金币";
            //if (intCoin > 1000 && intCoin <= 2000)
            //    intcoin = "1000-2000金币";
            //if (intCoin > 2000 && intCoin <= 5000)
            //    intcoin = "2000-5000金币";
            //if (intCoin > 5000 && intCoin <= 10000)
            //    intcoin = "5000-10000金币";
            //if (intCoin > 10000)
            //    intcoin = "1万以上金币";
            intcoin = intCoin.ToString();
            string strdjc = string.Format("{0}点券", djc);
            string ordNo = OrdNo;
            ordNo = ordNo.Replace("-", "");



            String property_values = "[{\"身份证\":\"" + IDn + "\",\"QQ等级\":\"" + Grade + "\",\"QQ好友\":\"" + FriendN + "\",\"等级\":\"" + Glevel + "\",\"英雄\":\"" + inthero + "\",\"皮肤\":\"" + intskin + "\",\"单双排段位\":\"" + Level + "\"}]";

            //拼成完整的JSON格式文本
            // String strData = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}{16}{17}{18}{19}{20}{21}{22}{23}{24}{25}{26}{27}{28}{29}{30}{31}{32}{33}{34}{35}{36}{37}{38}{39}{40}{41}{42}{43}{44}{45}", ww, ida, IDn, ida, ee, ida, FriendN, ida, qq, ida, Grade, ida, rr, ida, Glevel, ida, yy, ida, LEVEL, ida, uu, ida, inthero, ida, ii, ida, intskin, ida, oo, ida, intrenus, ida, pp, ida, intcoin, ida, aa, ida, BMPN, ida, bb, ida, ordNo, zz, property_values,cc);
            //在上面拼好的内容前加上4$_Y&wqz$#9!ck_data={0}


            // string strData = "{\"property\":[{\"QQ好友\":\"" + FriendN + "\",\"id\":136},{\"身份证\":\"" + IDn + "\",\"id\":138},{\"等级\":\"" + Glevel + "\",\"id\":139},{\"QQ等级\":\"" + Grade + "\",\"id\":137},{\"手机号码\":\"" + BMPN + "\",\"id\":140},{\"邮箱\":\"" + emailband + "\",\"id\":141},{\"英雄\":\"" + inthero + "\",\"id\":132},{\"皮肤\":\"" + intskin + "\",\"id\":133},{\"单双排段位\":\"" + LEVEL + "\",\"id\":135},{\"金币数量\":\"" + intcoin + "\",\"id\":143}],\"id\":\"" + ordNo + "\",\"property_values\":" + property_values + "}";


            //FriendN = "有QQ好友";
            //IDn = "身份证已设置";
            //Glevel = "30";
            //Grade = "QQ等级40级以上";
            //BMPN = "";
            //emailband = "";
            //inthero = "100";
            //intskin = "99";
            //LEVEL = "最强王者";
            //intcoin = "";
            //ordNo = "MZH170802000000018";

            string strData = "{\"property\":[{\"pname\":\"QQ好友\",\"pvalue\":\"" + FriendN + "\",\"id\":\"6786\",\"mainid\":\"BCP3\"},{\"pname\":\"身份证\",\"pvalue\":\"" + IDn + "\",\"id\":\"6784\",\"mainid\":\"BCP1\"},{\"pname\":\"等级\",\"pvalue\":\"" + Glevel + "\",\"id\":\"6787\",\"mainid\":\"BCP4\"},{\"pname\":\"QQ等级\",\"pvalue\":\"" + Grade + "\",\"id\":\"6785\",\"mainid\":\"BCP2\"},{\"pname\":\"手机绑定选择\",\"pvalue\":\"" + BMPN + "\",\"id\":\"11032\",\"mainid\":\"0\"},{\"pname\":\"邮箱绑定选择\",\"pvalue\":\"" + emailband + "\",\"id\":\"11033\",\"mainid\":\"0\"},{\"pname\":\"英雄\",\"pvalue\":\"" + inthero + "\",\"id\":\"6788\",\"mainid\":\"BCP5\"},{\"pname\":\"皮肤\",\"pvalue\":\"" + intskin + "\",\"id\":\"6789\",\"mainid\":\"BCP6\"},{\"pname\":\"单双排段位\",\"pvalue\":\"" + LEVEL + "\",\"id\":\"6790\",\"mainid\":\"BCP7\"},{\"pname\":\"金币数量\",\"pvalue\":\"" + intcoin + "\",\"id\":\"11039\",\"mainid\":\"0\"},{\"pname\":\"游戏点券\",\"pvalue\":\"" + strdjc + "\",\"id\":\"11040\",\"mainid\":\"0\"}],\"id\":\"" + ordNo + "\",\"property_values\":" + property_values + "}";

            string temp = string.Format("4$_Y&wqz$#9!ck_data={0}", strData);

            //获取temp的MD5
            string strMd5 = GetStringMD5(temp);
            string postData = "{\"data\":" + strData + "," + "\"mainSign\":\"" + strMd5 + "\"}";
            WriteToFile(postData);
            string strHTML = "";
            try
            {
                //将内容发送至接口
                //strHTML = Post("https://m.5173.com/api/mobile-goodsSearch-service/rs/gtrAliyunService/gtrToAliYun", postData);
                strHTML = Post("https://m.5173.com/api/mobile-goods-service/rs/gtrAliyunService/gtrToAliYun", postData);
                //strHTML = Post("http://192.168.42.38:8086/mobile-goods-service/rs/gtrAliyunService/gtrToAliYun", postData);

                //验证发送的数据是否正确
                //string ss = string.Format("身份证：{0}\r\n                  QQ好友：{1}\r\n                  QQ等级：{2}\r\n                  角色等级：{3}\r\n                  段位等级：{4}\r\n                  英雄个数：{5}\r\n                  皮肤个数：{6}\r\n                  符文页：{7}\r\n                  金币数量：{8}\r\n                  原绑定手机号：{9}", IDn, FriendN, Grade, Glevel, LEVEL, inthero, intskin, intrenus, intcoin, BMPN);
                //通过返回的日志检测数据是否发送成功，并且内容符合要求
                WriteToFile(strHTML);

                return strHTML;
            }
            catch (Exception e)
            {
                WriteToFile(e.Message);
                WriteToFile("动态属性上传异常");
                return "";
            }
            WriteToFile("向服务器发送帐号信息成功");
        }
        //获取数据，发送服务器
        public string AutoVerifyb()
        {
            //判断拉取网页信息是否成功
            string reviewedresult = "1";
            //有无QQ好友
            string Friend = "";
            //QQ等级范围
            string QQlevelRange = "";
            string postData = "";
            if (FriendNumber == "0")
                Friend = "无QQ好友";
            else if (FriendNumber == "")
                Friend = "";
            else
                Friend = "有QQ好友";
            int a = int.Parse(QQLevel);
            if (a == 0)
            {
                QQlevelRange = "QQ等级0级";
            }
            if (0 < a && a <= 10)
            {
                QQlevelRange = "QQ等级1-10级";
            }
            if (10 < a && a <= 20)
            {
                QQlevelRange = "QQ等级11-20级";
            }
            if (20 < a && a <= 30)
            {
                QQlevelRange = "QQ等级21-30级";
            }
            if (30 < a && a <= 40)
            {
                QQlevelRange = "QQ等级31-40级";
            }
            if (a > 40)
            {
                QQlevelRange = "QQ等级40级以上";
            }
            //判断网页认证是否成功
            if (IDNumber == "" || Friend == "")
            {
                reviewedresult = "2";
                IDNumber = "0";
                QQlevelRange = "0";
                a = 0;
                Friend = "0";
                Level = "0";
                grade = "0";
            }
            //获取在之前读取出来的内容并将它们进行格式化
            string IDn = string.Format("身份证{0}#", IDNumber);
            string Grade = string.Format("QQ等级：QQ{0}级：{1}#", a, QQlevelRange);
            string FriendN = string.Format("{0}#", Friend);
            string LEVEL = string.Format("排位：{0}", Level);
            string inthero = string.Format("{0}个英雄", intHero);
            string intskin = string.Format("{0}个皮肤", intSkin);
            string Glevel = string.Format("{0}级", grade);
            string ordNO = OrdNo;
            string strHTML = "";
            string reviewed = "";
            if (FriendN == "" || Grade == "")
            {
                FriendN = "0";
                Grade = "0";
            }

            reviewed = string.Format("身份证：{0}{1}QQ好友：QQ好友{2}个：{3}等级：{4}：{5}#英雄：{6}：{7}#皮肤：{8}：{9}#{10}：排位:{11}", IDn, Grade, QQLevel, FriendN, Glevel, grade, inthero, intHero, intskin, intSkin, LEVEL, Level);
            postData = string.Format("?PublishNO=" + ordNO + "&ReviewedResult=" + reviewedresult + "&Reviewed=" + reviewed);
            WriteToFile(postData);
            try
            {
                //将内容发送至接口
                strHTML = Get("http://gtr.5173.com:7080/RobotCallback.asmx/GetByRobotReviewed", postData);
                //通过返回的日志检测数据是否发送成功，并且内容符合要求
                WriteToFile(strHTML);
                return strHTML;
            }
            catch (Exception e)
            {
                WriteToFile(e.Message);
                return "";
            }
        }
        public static string GetStringMD5(string strPwd)
        {
            System.Security.Cryptography.MD5
            md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strPwd);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }
            return ret.PadLeft(32, '0');
        }
        public static string PostUrlData(string url, string postData)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            System.Net.HttpWebRequest objWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            objWebRequest.Method = "POST";
            objWebRequest.ContentType = "application/x-www-form-urlencoded";
            objWebRequest.ContentLength = byteArray.Length;
            Stream newStream = objWebRequest.GetRequestStream();
            // Send the data. 
            newStream.Write(byteArray, 0, byteArray.Length); //写入参数 
            newStream.Close();
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);//Encoding.Default
            string textResponse = sr.ReadToEnd(); // 返回的数据
            return textResponse;
        }
        //发送数据（JSON格式）
        public static string Post(string url, string postData)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            System.Net.HttpWebRequest objWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            objWebRequest.Method = "POST";
            objWebRequest.ContentType = "application/json";
            objWebRequest.ContentLength = byteArray.Length;
            Stream newStream = objWebRequest.GetRequestStream();
            // Send the data. 
            newStream.Write(byteArray, 0, byteArray.Length); //写入参数 
            newStream.Close();
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);//Encoding.Default
            string textResponse = sr.ReadToEnd(); // 返回的数据
            return textResponse;
        }

        public static string Get(string url, string postData)
        {
            //url = string.Format(url + "{0}", postData);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        /// <summary>
        /// 人工答题结果返回
        /// </summary>  
        /// <param name="isTrue">正确与否：0正确；1错误</param>  
        /// <seealso cref="isTrue">  
        ///String类型的信息</seealso>  
        public static void RGcodeRight(int isTrue)
        {
            if (isTrue == 0)
                udpdd.theUDPSend(32, "FCodeType=1\r\nFResult=0\r\n", OrdNo);
            else if (isTrue == 3)
                udpdd.theUDPSend(32, "FCodeType=3\r\nFResult=0\r\n", OrdNo);
            else
                udpdd.theUDPSend(32, "FCodeType=1\r\nFResult=1\r\n", OrdNo);
        }

        /// <summary>
        /// 外包答题结果返回
        /// </summary>
        /// <param name="isTrue">正确与否：0错误；1正确</param>
        public static void codeRight(int isTrue)
        {
            string strHTML = "";
            String postData = string.Format("orderNo={0}&IsTrue={1}", OrdNo, isTrue);
            try
            {
                strHTML = PostUrlData("http://192.168.36.245/autotalk/service.asmx/ResultAnswer2", postData);
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            return;
        }
        /// <summary>
        /// 删除指定日期之前的文件夹
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="time">时间：单位天</param>
        public void DeleteFolder(string path, int time)
        {
            WriteToFile("删除文件");
            try
            {
                DirectoryInfo dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    DirectoryInfo[] childs = dir.GetDirectories();
                    foreach (DirectoryInfo child in childs)
                    {
                        DirectoryInfo[] m_childs = child.GetDirectories();
                        foreach (DirectoryInfo m_child in m_childs)
                        {
                            DateTime dt = m_child.CreationTime;
                            TimeSpan ts = DateTime.Now - dt;
                            if (ts.Days >= time)
                                child.Delete(true);
                        }
                    }
                }
            }
            catch (Exception e) { WriteToFile(e.ToString()); }
        }
        /// <summary>
        /// 截图
        /// </summary>
        public void CaptureJpg()
        {
            //检测截图路径是否有异常
            try
            {
                string tmp = SetPicPathBmp(LocalPicPath, OrdNo, "");
                if (!IsSendCapPath)
                {
                    WriteToFile("截图目录：" + tmp);
                    IsSendCapPath = true;
                }
                Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
                User32API.MakeSureDirectoryPathExists(m_strCapturePath);
                try
                {
                    RECT rect = new RECT(0, 0, 1280, 900);
                    Game.CaptureBmp(bm, rect, tmp);
                    return;
                }
                catch
                {
                    RECT rect = new RECT(0, 0, 1024, 768);
                    Game.CaptureBmp(bm, rect, tmp);
                    return;
                }

            }
            //如果路径有异常则将路径改为 "Z:\\jiaoben\\"
            catch
            {
                LocalPicPath = "Z:\\jiaoben\\";
                string tmp = SetPicPathBmp(LocalPicPath, OrdNo, "");
                if (!IsSendCapPath)
                {
                    WriteToFile("异常截图目录：" + tmp);
                    IsSendCapPath = true;
                }
                Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
                User32API.MakeSureDirectoryPathExists(m_strCapturePath);
                try
                {
                    RECT rect = new RECT(0, 0, 1280, 1024);
                    Game.CaptureBmp(bm, rect, tmp);
                    return;
                }
                catch
                {
                    RECT rect = new RECT(0, 0, 1024, 768);
                    Game.CaptureBmp(bm, rect, tmp);
                    return;
                }


            }
        }
        public void CaptureJpg(string picName)
        {
            //检测截图路径是否有异常
            try
            {
                string tmp = SetPicPathBmp(LocalPicPath, OrdNo, picName);
                if (!IsSendCapPath)
                {
                    WriteToFile("截图目录：" + tmp);
                    IsSendCapPath = true;
                }

                Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
                User32API.MakeSureDirectoryPathExists(m_strCapturePath);
                try
                {
                    RECT rect = new RECT(0, 0, 1280, 1024);
                    Game.CaptureBmp(bm, rect, tmp);
                    return;
                }
                catch
                {
                    RECT rect = new RECT(0, 0, 1024, 768);
                    Game.CaptureBmp(bm, rect, tmp);
                    return;
                }

            }
            //如果路径有异常则将路径改为 "Z:\\jiaoben\\"
            catch
            {
                LocalPicPath = "Z:\\jiaoben\\";
                string tmp = SetPicPathBmp(LocalPicPath, OrdNo, picName);
                if (!IsSendCapPath)
                {
                    WriteToFile("截图目录：" + tmp);
                    IsSendCapPath = true;
                }
                Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
                User32API.MakeSureDirectoryPathExists(m_strCapturePath);
                try
                {
                    RECT rect = new RECT(0, 0, 1280, 1024);
                    Game.CaptureBmp(bm, rect, tmp);
                    return;
                }
                catch
                {
                    RECT rect = new RECT(0, 0, 1024, 768);
                    Game.CaptureBmp(bm, rect, tmp);
                    return;
                }


            }
        }
        //图片缩放bmp
        public bool picResize(string strFile, string strNewFile, int intWidth, int intHeight)
        {
            System.Drawing.Bitmap objPic, objNewPic;
            try
            {

                if (!File.Exists(strFile))
                    return false;
                objPic = new System.Drawing.Bitmap(strFile);
                if (intHeight <= 0)
                    intHeight = (intWidth * objPic.Height / objPic.Width);
                objNewPic = new System.Drawing.Bitmap(objPic, intWidth, intHeight);
                objPic.Dispose();
                objPic = null;
                string newFile = strNewFile.Substring(0, strNewFile.LastIndexOf('.'));
                objNewPic.Save(newFile + ".bmp", ImageFormat.Jpeg);
                objNewPic.Dispose();
                // File.Delete(strFile);

            }
            catch (Exception exp)
            {
                return false;
            }
            finally
            {

                objNewPic = null;
            }
            return true;
        }
        //图片缩放
        public bool jpgResize(string strFile, string strNewFile, int intWidth, int intHeight)
        {
            System.Drawing.Bitmap objPic, objNewPic;
            try
            {

                if (!File.Exists(strFile))
                    return false;
                objPic = new System.Drawing.Bitmap(strFile);
                if (intHeight <= 0)
                    intHeight = (intWidth * objPic.Height / objPic.Width);
                objNewPic = new System.Drawing.Bitmap(objPic, intWidth, intHeight);
                objPic.Dispose();
                objPic = null;
                string newFile = strNewFile.Substring(0, strNewFile.LastIndexOf('.'));
                objNewPic.Save(newFile + ".jpg", ImageFormat.Jpeg);
                objNewPic.Dispose();
                //File.Delete(strFile);

            }
            catch (Exception exp)
            {
                return false;
            }
            finally
            {

                objNewPic = null;
            }
            return true;
        }
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
        public string SetPicPathBmp(string str, string strPicID, string picName)
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
            if (picName != "")
                strFileName = string.Format("{0}\\{1}.bmp", m_month, picName);
            else if (strPicID == "")
                strFileName = string.Format("{0}\\R_{1:00}.bmp", m_month, PicNum++);
            else
                strFileName = string.Format("{0}\\{1}_{2:00}.bmp", m_month, strPicID, PicNum++);
            strLastPicID = strPicID;
            return strFileName;
        }
        /// <summary>
        /// bmp截图
        /// </summary>
        /// <param name="bm">bmp图片流</param>
        /// <param name="strPicName">截图名称</param>
        /// <param name="left">边界：左</param>
        /// <param name="top">边界：上</param>
        /// <param name="right">边界：右</param>
        /// <param name="bottom">边界：下</param>
        public void CaptureBmpInRect(string strPicName, int left, int top, int right, int bottom)
        {
            //if (m_strLastName == strPicName)
            //{
            //    strPicName += "1";
            //}
            strPicName += ".bmp";
            Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
            User32API.MakeSureDirectoryPathExists(m_strCapturePath);
            RECT rect = new RECT(left, top, right, bottom);
            Game.CaptureBmp(bm, rect, m_strCapturePath + strPicName);
            //m_strLastName = strPicName;
            return;
        }
        public void CaptureBmpInRect(string strPicName, string strPicPath, int left, int top, int right, int bottom)
        {
            //if (m_strLastName == strPicName)
            //{
            //    strPicName += "1";
            //}
            strPicName += ".bmp";
            Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
            User32API.MakeSureDirectoryPathExists(strPicPath);
            RECT rect = new RECT(left, top, right, bottom);
            Game.CaptureBmp(bm, rect, strPicPath + strPicName);
            //m_strLastName = strPicName;
            return;
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
            strArray = new string[2000];
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
                goto NEXT_STEP;
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

        NEXT_STEP:
            if (ptMAX.X == 0 && ptMAX.Y == 0)
            {
                WriteToFile("大图为空\r\n");
                C = C + 1;
                bPicFull = false;
                return false;
            }

            if (ptBigPic.Y > 0)//多排
            {
                ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                ptMAX.Y += ptBigPic.Y;
            }
            else
            {
                if (ptBigPic.X < 130)
                    ptMAX.X = ptBigPic.X + 130;//当图片小于水印尺寸时，则还需加上水印的尺寸（水印宽度为130）
                else
                    ptMAX.X += ptBigPic.X;
            }

            if (width != nZHPicWidth || MZH)  //取消宽度限制
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
        //网页认证
        private string GetTheVison(string ExeName)
        {

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://172.16.74.11:38800/ZH_QQ_CHECK/syupversion.txt");
                WebResponse response = request.GetResponse();
                Stream myStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, System.Text.Encoding.GetEncoding("gb2312"));
                string jsContent = sr.ReadToEnd();// FileRW.ReadFile("d:\\dq_server_237.js");
                if (jsContent != "")
                {
                    int ipos = jsContent.IndexOf(ExeName);
                    if (ipos >= 0)
                    {
                        jsContent = jsContent.Substring(ipos + ExeName.Length + 1);
                        ipos = jsContent.IndexOf("\r\n");
                        if (ipos >= 0)
                        {
                            jsContent = jsContent.Substring(0, ipos);
                        }
                        return jsContent;
                    }
                }
            }
            catch
            {
                WriteToFile("版本获取失败");
            }
            return "";
        }
        public void CheckVersion()
        {
            long a;
            WebClient client = new WebClient();
            string URLAddress = @"http://172.16.74.11:38800/ZH_QQ_CHECK/腾讯网游系列.exe";

            string receivePath = m_strProgPath + "\\";
            string localpath = m_strProgPath + "\\QQlogin.exe";
            if (!File.Exists(localpath))
            {
                client.DownloadFile(URLAddress, receivePath + System.IO.Path.GetFileName(URLAddress));
                string localpath1 = m_strProgPath + "\\腾讯网游系列.exe";
                Game.StartProcess(localpath1, "start");
                Sleep(2000);
            }
            System.Diagnostics.FileVersionInfo fv = System.Diagnostics.FileVersionInfo.GetVersionInfo(localpath);
            string WebVersion = GetTheVison("TengXun");
            if (fv.FileVersion == WebVersion)
            {
                return;
            }
            else
            {
                client.DownloadFile(URLAddress, receivePath + System.IO.Path.GetFileName(URLAddress));
                string localpath1 = m_strProgPath + "\\腾讯网游系列.exe";
                Game.StartProcess(localpath1, "start");
                Sleep(2000);
                return;
            }
            //WebClient client = new WebClient();
            //client.DownloadFile(URLAddress, receivePath + System.IO.Path.GetFileName(URLAddress));//通过url下载
            //Game.StartProcess(localpath, "start");
            //return;
        }
        private int CheckAccount()
        {
            int status = 1;
            string strlog;
            strlog = "等待审核结果...";
            bool flag = false;
            bool WaitFlag = false;
            int SecondN = 0;
            CheckVersion();
            System.Threading.Mutex mutex = new System.Threading.Mutex(false, "QQlogin", out flag);
            try
            {
                WaitFlag = mutex.WaitOne();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            //GTR.Cookies ck = new Cookies();
            //ck.CleanAll();//清cookies
            for (int j = 0; j < 2; j++)
            {
                Sleep(5000);
                string orderdata;
                WriteToFile("获取帐号密码中");
                orderdata = string.Format("{0} {1} {2} {3} {4}", m_strAccount, m_strPassword, OrdNo, "1", the_nRC2Port.ToString());
                Game.StartProcess(m_strProgPath + "\\QQlogin.exe", orderdata);
                m_hGameWnd = IntPtr.Zero;
                m_hGameWnd = User32API.FindWindow(null, "QQlogin");
                for (int i = 0; i < 10; i++)
                {
                    if (i == 9)
                    {
                        WriteToFile("等待审核程序开启超时");
                        Game.tskill("WangYiFGF");
                        Game.tskill("WangYi");
                        Game.tskill("MailCheck");
                        mutex.ReleaseMutex();
                        return 2120;
                    }
                    if (Game.processStay("QQlogin"))
                    {
                        if (m_hGameWnd != IntPtr.Zero)
                        {
                            WriteToFile("打开成功");
                            break;
                        }
                        else
                            m_hGameWnd = User32API.FindWindow(null, "QQlogin");
                    }
                    Thread.Sleep(2000);
                }
                status = ProcessStpe();
                WriteToFile("清除残留进程...");
                Game.tskill("QQlogin");
                Game.tskill("QQlogin");
                Game.tskill("MailCheck");
                if (status == 3120)
                    continue;
                else
                    break;
            }
            mutex.ReleaseMutex();
            return status;
        }
        public int ProcessStpe()
        {
            for (int i = 0; i < 50; i++)
            {
                m_hGameWnd = User32API.FindWindow(null, "QQlogin");
                if (m_hGameWnd != IntPtr.Zero)
                    Sleep(6000);
                else
                {
                    StringBuilder retVal = new StringBuilder(512);
                    StringBuilder strBuilder = new StringBuilder(512);
                    StringBuilder strBuilder1 = new StringBuilder(512);
                    StringBuilder strBuilder2 = new StringBuilder(512);
                    StringBuilder strBuilder3 = new StringBuilder(512);
                    StringBuilder strBuilder4 = new StringBuilder(512);
                    try
                    {
                        User32API.GetPrivateProfileString("账号信息", "执行状态", "", retVal, 512, m_strProgPath + "\\roleInfo.ini");
                        int a = int.Parse(retVal.ToString());
                        if (a != 1000)
                            return a;
                        User32API.GetPrivateProfileString("账号信息", "证件号码", "", strBuilder, 512, m_strProgPath + "\\roleInfo.ini");
                        User32API.GetPrivateProfileString("账号信息", "密保手机", "", strBuilder1, 512, m_strProgPath + "\\roleInfo.ini");
                        User32API.GetPrivateProfileString("账号信息", "至尊宝", "", strBuilder2, 512, m_strProgPath + "\\roleInfo.ini");
                        User32API.GetPrivateProfileString("账号信息", "QQ等级", "", strBuilder3, 512, m_strProgPath + "\\roleInfo.ini");
                        User32API.GetPrivateProfileString("账号信息", "QQ好友", "", strBuilder4, 512, m_strProgPath + "\\roleInfo.ini");
                        return a;

                    }
                    catch
                    {
                        WriteToFile("QQ网页审核崩溃");
                    }

                }
                if (i % 9 == 0)
                    WriteToFile("获取QQ账号信息中...");
            }
            return 3120;
        }

        //取代TGP获取英雄和皮肤并且拼图，更改路径
        /// <summary>
        /// 获取英雄与皮肤
        /// </summary>
        /// <returns></returns>
        public int GetHeroAndSkin()
        {
            Point pt = new Point(-1, -2);
            string strWinTitle = string.Empty;
            int index = 0;

            for (int i = 0; i < 10; i++)
            {
                #region 启动TGP.exe
                Game.RunCmd("taskkill /im  LOL.exe /F");
                Game.RunCmd("taskkill /im  TGP.exe /F");
                Game.RunCmd("taskkill /im  QQlogin.exe /F");
                Game.RunCmd("taskkill /im  WerFault.exe /F");

                Game.StartProcess(m_strProgPath + @"\TGP.exe", "start");
                Sleep(1000 * 5);
                strWinTitle = WinTitle();
                if (m_hGameWnd != IntPtr.Zero && strWinTitle == "TGP")
                {
                    CaptureJpg();
                    WriteToFile("应用程序启动成功");
                }
                else
                {
                    WriteToFile("应用程序打开失败");
                    continue;
                }

                TuningWin();//调用调整窗口函数

                pt = ImageTool.fPic(wwPath + "易.bmp", 0, 0, 50, 50);
                if (pt.X <= 0)
                    pt = ImageTool.fPic(wwPath + "xnj易.bmp", 0, 0, 50, 50);
                if (pt.X <= 0 && i > 2)
                {
                    if (i >= 2)
                    {
                        return 2120;
                    }
                    WriteToFile("网页加载失败，启用TGP");
                    continue;
                }
                #endregion

                #region  登录
                if (pt.X > 0)
                {
                    //-----------------------------------------------------------------------------------------
                    TuningWin();//调用调整窗口函数
                    WriteToFile("开始输入账号[" + m_strAccount + "] 密码[" + m_strPassword.Length + "]位");
                    KeyMouse.MouseClick(220, 80, 1, 1, 500);//点击账号输入框
                    KeyMouse.SendKeys(m_strAccount, 200); //输入账号
                    //-----------------------------------------------------------------------------------------
                    KeyMouse.MouseClick(220, 115, 1, 1, 500);//点击密码输入框
                    KeyMouse.SendKeys(m_strPassword, 300); //输入密码
                    KeyMouse.MouseClick(127, 274, 1, 1, 3000);//点击登录按钮
                    //-----------------------------------------------------------------------------------------                         
                    switch (CheckPW())//调用登录验证函数
                    {
                        case 2:
                            {
                                index++;
                                WriteToFile("第" + index.ToString() + "次账号密码错误");
                                if (index < 3)
                                    continue;
                                CaptureJpg();
                                return 3000;
                            }
                        case 3:
                            return 3333;
                        case 4:
                            return 2230;
                        case 6:
                            return 3700;
                        case 7:
                            return 3360;
                        case 5:
                            continue;

                    }
                }
                #endregion

                #region 绑定大区
                if (WinTitle().Contains("核对密码成功") || WinTitle().Contains("裁决之镰"))
                {
                    Sleep(1000);
                    if (IsCallPone == 0)
                    {
                        string PostData = string.Format("OrdNo={0}", OrdNo);
                        string strHTML = PostUrlData(" http://172.16.74.147:8010/WebService.asmx/IsSendData", PostData);
                        string IsSend = MyStr.FindStr(strHTML, "/\">", "</string>");
                        if (IsSend == "1")
                            WriteToFile("已经通知过主站");
                        else
                        {
                            WriteToFile("通知主站密码正确并导入数据");
                            PostData = string.Format("PublishNO={0}", OrdNo);
                            strHTML = PostUrlData("http://gtr.5173.com:7080/RobotCallback.asmx/ChangePassword", PostData);
                            WriteToFile(strHTML);

                            PostData = string.Format("gameId={0}&OrdNo={1}&Status={2}", "60", OrdNo, "1000");
                            strHTML = PostUrlData(" http://172.16.74.147:8010/WebService.asmx/SetPhoneData", PostData);
                            WriteToFile(strHTML);
                        }
                    }
                    Sleep(3000);//等待窗口加载完毕方便拖动
                    TuningWin();//调用调整窗口函数
                    WriteToFile("订单大区:" + m_strServer);
                    KeyMouse.MouseClick(370, 50, 1, 1, 500);//点击区服输入框
                    tagPoint at = new tagPoint();
                    at.x = 370;
                    at.y = 50;
                    IntPtr s_hGameWnd = User32API.WindowFromPoint(at);//该函数获得包含指定点的窗口的句柄
                    Game.SendString(s_hGameWnd, m_strServer, true);
                    Sleep(1000);
                    KeyMouse.MouseClick(520, 50, 1, 1, 500);//点击获取指定区服数据按钮
                }
                #endregion

                #region 加载资源
                for (int k = 0; k < 300; k++)
                {
                    Sleep(1000);
                    if (k % 20 == 0 || WinTitle().Contains("开始获取数据"))
                        WriteToFile("正在获取数据");
                    if (WinTitle().Contains("获取完成"))
                    {
                        CaptureJpg();
                        WriteToFile("图片加载完毕");
                        Sleep(1000 * 2);
                        if (File.Exists(m_strProgPath + @"\RoleInfo.txt"))
                        {
                            FileInfo fi = new FileInfo(m_strProgPath + @"\RoleInfo.txt");
                            if (fi.Length == 0)
                            {
                                WriteToFile("文本加载失败");
                                return 2120;
                            }
                            else
                            {
                                WriteToFile("加载完毕");
                                return 1000;
                            }
                        }
                    }
                    if (WinTitle().Contains("获取数据失败"))
                    {
                        WriteToFile("获取数据失败");
                        return 2120;
                    }
                }
                WriteToFile("数据加载超时");
                return 2120;
                #endregion
            }
            WriteToFile("未知原因工具获取失败");
            return 2120;

        }
        public int GetLOLInfo()
        {
            Point pt = new Point(-1, -2);
            string strWinTitle = string.Empty;
            File.Delete(m_strProgPath + @"\RoleInfo.txt");
            for (int i = 0; i < 3; i++)
            {
                //--------------------------------------------测试
                //m_strServer = "黑色玫瑰";
                //m_strAccount = "2747902166";
                //m_strPassword = "1234..cc";
                //--------------------------------------------
                Game.RunCmd("taskkill /im  TGP.exe /F");
                Game.RunCmd("taskkill /im  LOL.exe /F");
                Game.StartProcess(m_strProgPath + @"\LOL.exe", m_strServer);
                Sleep(1000 * 3);
                strWinTitle = WinTitle();
                if (m_hGameWnd != IntPtr.Zero && strWinTitle.Contains("LOL登录"))
                {
                    CaptureJpg();
                    WriteToFile("应用程序启动成功,等待网页加载");
                }
                else
                {
                    WriteToFile("应用程序打开失败");
                    continue;
                }

                TuningWin();//调用调整窗口函数 
                //-------------------------------------------------------------------
                for (int z = 0; z < 3; z++)
                {
                    if (z > 0)
                        KeyMouse.SendF5Key();
                    Sleep(10000);
                    pt = ImageTool.fPic(wwPath + "w账号密码登录.bmp", 100, 200, 300, 500, 60);
                    if (pt.X < 0)
                        pt = ImageTool.fPic(wwPath + "xw账号密码登录.bmp", 100, 200, 300, 500, 60);
                    if (pt.X < 0)
                        continue;
                    KeyMouse.MouseClick(194, 380, 1, 1, 1000);//点击账号密码登录按钮
                    pt = ImageTool.fPic(wwPath + "x登录.bmp", 140, 250, 300, 350, 60);
                    if (pt.X < 0)
                        pt = ImageTool.fPic(wwPath + "x登录1.bmp", 140, 250, 300, 350, 60);
                    if (pt.X < 0)
                        pt = ImageTool.fPic(wwPath + "x登录2.bmp", 140, 250, 300, 350, 60);
                    if (pt.X < 0)
                        pt = ImageTool.fPic(wwPath + "x登录3.bmp", 140, 250, 300, 350, 60);
                    if (pt.X < 0)
                        pt = ImageTool.fPic(wwPath + "x登录4.bmp", 140, 250, 300, 350, 60);
                    if (pt.X < 0)
                        pt = ImageTool.fPic(wwPath + "LOL网页获取登录.bmp", 140, 250, 300, 350, 60);
                    if (pt.X > 0)
                        break;
                }
                if (pt.X < 0)
                    continue;
                //-------------------------------------------------------------------
                KeyMouse.MouseClick(311, 179, 1, 1, 500);//点击账号输入框
                KeyMouse.SendKeys(m_strAccount, 200); //输入账号               
                KeyMouse.MouseClick(199, 232, 1, 1, 500);//点击密码输入框
                KeyMouse.SendKeys(m_strPassword, 300); //输入密码
                KeyMouse.MouseClick(199, 288, 1, 1, 3000);
                WriteToFile("账号密码输入完成");

                for (int k = 0; k < 5; k++)
                {
                    Sleep(1000);
                    pt = ImageTool.fPic(wwPath + "LOL网页验证码.bmp", 0, 0, 0, 0, 60);
                    if (pt.X < 0)
                        pt = ImageTool.fPic(wwPath + "w验证.bmp", 0, 0, 0, 0, 60);
                    if (pt.X > 0)
                    {
                        WriteToFile("需要输入验证码");
                        CaptureBmpInRect("验证码", m_strProgPath, pt.X - 213, pt.Y - 76, pt.X - 74, pt.Y - 24);
                        string yzm = AutoVerify(m_strProgPath + "验证码.BMP", 100);
                        WriteToFile("验证码返回：" + yzm);
                        KeyMouse.MouseClick(pt.X - 13, pt.Y - 44, 1, 1, 300);
                        KeyMouse.SendKeys(yzm, 100); //输入验证码
                        KeyMouse.MouseClick(pt.X + 5, pt.Y + 5, 1, 1, 300);
                        continue;
                    }
                    try
                    {
                        int a = CheckHuaDong();
                        if (a > 1000)
                            return a;
                        if (pt.X < 0 && k > 8)
                            break;
                    }
                    catch (Exception ex)
                    {
                        WriteToFile(ex.ToString());
                    }
                }
                Sleep(3000);
                //-------------------------------句柄窗口测试----------------------------
                if (true)
                {
                    List<int> listhwnd = myapp.EnumWindow("", "#32770");
                    foreach (int hwdl in listhwnd)
                    {
                        IntPtr hwdl2 = (IntPtr)hwdl;
                        string winTitle = User32API.GetWindowText(hwdl2).Trim();//获取窗口标题
                        //WriteToFile(winTitle);
                        if (winTitle.Contains("封号"))
                        {
                            WriteToFile("封号");
                            return 3360;
                        }
                    }
                }
                //---------------------------------------------------------------------
                if (WinTitle().Contains("召唤师数据"))
                {
                    WriteToFile("登录成功,数据加载中...");
                    if (IsCallPone == 0)
                    {
                        WriteToFile("通知主站密码正确并导入数据");
                        string PostData = string.Format("PublishNO={0}", OrdNo);
                        string strHTML = PostUrlData("http://gtr.5173.com:7080/RobotCallback.asmx/ChangePassword", PostData);
                        WriteToFile(strHTML);

                        PostData = string.Format("gameId={0}&OrdNo={1}&Status={2}", "60", OrdNo, "1000");
                        strHTML = PostUrlData(" http://172.16.74.147:8010/WebService.asmx/SetPhoneData", PostData);
                        WriteToFile(strHTML);
                    }
                    for (int z = 0; z < 30; z++)
                    {
                        Sleep(2000);
                        if (File.Exists(m_strProgPath + @"\RoleInfo.txt"))
                        {
                            WriteToFile("文本加载完毕...");
                            return 1;
                        }
                    }
                }
            }
            return 1;
        }

        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <returns></returns>
        public string WinTitle()
        {
            m_hGameWnd = User32API.FindWindow("WTWindow", null);//根据窗口类名获取句柄
            string winTitle = User32API.GetWindowText(m_hGameWnd).Trim();//获取窗口标题
            return winTitle;
        }
        /// <summary>
        /// 调整窗口
        /// </summary>
        public void TuningWin()
        {
            RECT rt = new RECT();
            Point at = new Point(-1, -2);

            for (int k = 0; k < 3; k++)
            {
                m_hGameWnd = User32API.FindWindow("WTWindow", null);//根据窗口类名获取句柄
                if (m_hGameWnd == IntPtr.Zero)
                {
                    string HwndList = User32API.FindWindowEx_ByProcessName("TGP");
                    int i;
                    int.TryParse(HwndList, out i);
                    m_hGameWnd = (IntPtr)i;
                }
                if (User32API.IsWindowVisible(m_hGameWnd))
                {
                    break;
                }
            }

            for (int k = 0; k < 20; k++)
            {
                User32API.GetWindowRect(m_hGameWnd, out rt);
                User32API.MoveWindow(m_hGameWnd, 0, 0, rt.Width, rt.Height, true);//拖动到左上角
                User32API.SwitchToThisWindow(m_hGameWnd, true);//创建指定窗口的线程设置到前台,并且激活该窗口
                User32API.SetForegroundWindow(m_hGameWnd);
                Sleep(200);
                at = ImageTool.fPic(wwPath + "易.bmp", 1, 1, 30, 30);
                if (at.X <= 0)
                    at = ImageTool.fPic(wwPath + "xnj易.bmp", 1, 1, 30, 30);
                if (at.X > 0 && k > 1)
                {
                    //WriteToFile("TGP窗口拖至左上角");
                    return;
                }
            }
            WriteToFile("TGP窗口拖动失败");
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <returns></returns>
        public int CheckPW()
        {
            Point at = new Point(-1, -2);
            WriteToFile("登录验证");
            for (int l = 0; l < 20; l++)
            {
                if (WinTitle().Contains("您的网络环境可能发生了变化"))
                {
                    WriteToFile("网络环境发生变化,需要卖家改密重新发布");
                    CaptureJpg("网络环境异常");
                    return 6;
                }
                if (WinTitle().Contains("需要验证码"))
                {
                    WriteToFile("需要输入验证码");
                    if (!PIN())//调用验证码函数
                        return 4;
                }
                if (WinTitle().Contains("冻结") || WinTitle().Contains("回收") || WinTitle().Contains("无法登陆"))
                {
                    WriteToFile("异地登录导致账号无法登录");
                    CaptureJpg("无法登陆");
                    return 3;
                }
                if (WinTitle().Contains("密码不正确") || WinTitle().Contains("UDP") || WinTitle().Contains("已自动选用"))
                {
                    return 2;
                }
                if (WinTitle().Contains("封号"))
                {
                    WriteToFile(WinTitle());
                    return 7;
                }

                if (WinTitle().Contains("核对密码成功"))
                {
                    WriteToFile("核对密码成功");
                    CaptureJpg("核对密码成功");
                    return 1;
                }

                if (WinTitle().Contains("裁决之镰"))
                {
                    WriteToFile("核对密码成功,账号裁决");
                    CaptureJpg("核对密码成功");
                    return 1;
                }
                Sleep(300);
            }
            return 5;
        }
        /// <summary>
        /// ww验证码
        /// </summary>
        public bool PIN()
        {
            string wwPath = m_strPicPath + "ww\\";
            string yzm = string.Empty;
            int ytimes = 1;

            for (int i = 0; i < 30; i++)
            {
                TuningWin();//调用调整窗口函数
                CaptureJpg("验证码截屏");
                CaptureBmpInRect("验证码", m_strProgPath, 26, 132, 224, 199);
                if (ytimes <= 5)
                {
                    yzm = AutoVerify(m_strProgPath + "验证码.BMP", 100);
                }
                else
                {
                    if (ytimes == 6)
                        WriteToFile("外包答题错误多次，转到人工答题");
                    jpgResize(m_strProgPath + "验证码.bmp", m_strProgPath + "验证码1.jpg", 125, 52);
                    //人工答题返回的验证码
                    yzm = RequestSafeCardInfo(1, m_strProgPath + "验证码1.jpg", "", 180);
                }
                if (yzm.Length != 4)
                {
                    KeyMouse.MouseClick(127, 274, 1, 1, 500);//点击刷新验证码
                    continue;
                }
                WriteToFile("第" + ytimes + "次输入验证码:" + yzm);
                KeyMouse.MouseClick(62, 228, 1, 1, 500);//点击验证码输入框
                KeyMouse.SendBackSpaceKey(4);//删除残留验证码字母
                KeyMouse.SendKeys(yzm, 200);
                Sleep(1000);
                if (WinTitle().Contains("验证码错误") || WinTitle().Contains("需要验证码") || WinTitle().Contains("可能有误"))
                {
                    WriteToFile("验证码错误");
                    if (ytimes <= 5)
                        codeRight(0);
                    else
                        RGcodeRight(1);
                    if (ytimes >= 8)
                    {
                        WriteToFile("验证失败");
                        CaptureJpg("验证失败");
                        return false;
                    }
                    ytimes++;
                    KeyMouse.MouseClick(127, 274, 1, 1, 500);//点击刷新验证码
                    continue;
                }
                else
                    break;
            }
            if (ytimes <= 5)
                codeRight(1);
            else
                RGcodeRight(0);
            return true;
        }
        /// <summary>
        /// 遍历拼图英雄与皮肤
        /// </summary>
        /// <param name="isHero"></param>
        /// <param name="picPath"></param>
        /// <returns></returns>
        public int ReplaceTGP(bool isHero, string picPath)
        {
            if (TraversalFile(picPath) <= 0)
                return 2120;
            int re = 0;
            int ft = 1;
            int max = 0;
            int hp = 11;
            int sl = 88;
            string type = "h";
            string[] arrTemp;
            arrTemp = FileArr;

            List<string> listTemp = new List<string>();//先定义list集合

            if (!isHero)
            {
                hp = 7;
                sl = 35;
                type = "s";
                WriteToFile("开始皮肤拼图");
            }
            else { WriteToFile("开始英雄拼图"); }
            if (FileArr.Length > sl)
            {
                ft = FileArr.Length / sl;
                re = FileArr.Length % sl;
                if (re != 0)
                    ft += 1;
            }
            for (int t = 0; t < ft; t++)
            {
                string pStr = string.Empty;
                if (arrTemp.Length >= sl)
                    max = sl;
                else
                    max = arrTemp.Length;
                for (int j = 0; j < max; j++)
                {
                    if ((j + 1) % sl == 0 || j + 1 == arrTemp.Length)
                    {
                        pStr += arrTemp[j];
                        arrTemp = null;
                        for (int k = sl * (t + 1); k < FileArr.Length; k++)
                        {
                            listTemp.Add(FileArr[k]);
                        }
                        arrTemp = listTemp.ToArray();
                        listTemp.Clear();
                        break;
                    }
                    else
                        pStr += arrTemp[j] + ",";
                    if ((j + 1) % hp == 0)
                        pStr += "换行,";
                }
                string pn = string.Empty;
                if (type == "h")//英雄
                {
                    try
                    {
                        PinTuPng(pStr, "LOL5", false, picPath);
                    }
                    catch (Exception ex) { WriteToFile(ex.ToString()); }
                    pn = "LOL5_0" + (t + 1).ToString() + ".jpg";
                }
                else//皮肤
                {
                    try
                    {
                        PinTuPng(pStr, "LOL3", false, picPath);
                    }
                    catch (Exception ex) { WriteToFile(ex.ToString()); }
                    if (t > 8)
                        pn = "LOL3_" + (t + 1).ToString() + ".jpg";
                    else
                        pn = "LOL3_0" + (t + 1).ToString() + ".jpg";
                }
                if (File.Exists(m_strCapturePath + pn))
                    File.Delete(m_strCapturePath + pn);
                File.Move(picPath + pn, m_strCapturePath + pn);
            }
            return 1000;
        }
        /// <summary>
        /// 拼图png
        /// </summary>
        /// <param name="strAllPic"></param>
        /// <param name="strPicID"></param>
        /// <param name="bVerbical"></param>
        /// <returns></returns>
        public int PinTuPng(string strAllPic, string strPicID, bool bVerbical, string picPath)
        {
            string[] arrPicName;
            int num = SplitString(strAllPic, ",", out arrPicName);

            for (int i = 0; i < num + 1; i++)
            {
                if (arrPicName[i] == "换行")
                {
                    CreatePlatePng("换行", "", nZHPicWidth);
                    continue;
                }

                CreatePlatePng(picPath + arrPicName[i], "", nZHPicWidth);
            }
            string strPic = picPath + "模板.bmp";
            if (bVerbical)
            {
                if (!CreatePlatePng("生成图片", strPic, 0))
                    return 1;
            }
            else
            {
                if (!CreatePlatePng("生成图片", strPic, nZHPicWidth))
                    return 1;
            }
            Bitmap bbmp = new Bitmap(strPic, true);
            int x = 0, y = 0, z = 0;
            for (int i = 0; i < num + 1; i++)
            {

                if (arrPicName[i] == "换行")
                {
                    //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "换行");
                    try
                    {
                        ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "换行");//第一个BmpInsert
                    }
                    catch (Exception ex)
                    {
                        WriteToFile("BmpInsert1");
                        WriteToFile(ex.ToString());
                    }
                    continue;
                }
                if (!File.Exists(picPath + arrPicName[i] + ".png"))
                    continue;
                Bitmap sbmp = new Bitmap(picPath + arrPicName[i] + ".png", true);
                try
                {
                    ImageTool.BmpInsert(bbmp, sbmp, ref x, ref y, ref z, "");//第二个BmpInsert
                }
                catch (Exception ex)
                {
                    WriteToFile("BmpInsert2");
                    WriteToFile(ex.ToString());
                }
                Sleep(50);
                try
                {
                    sbmp.Dispose();
                }
                catch (Exception e)
                {
                    WriteToFile(e.ToString());
                    WriteToFile(arrPicName[i]);
                    Sleep(500);
                    sbmp.Dispose();
                }

                //删除小图
                if (Program.bRelease)
                {
                    string[] arrPicRank = new string[] { "当前赛季无", "青铜", "白银", "黄金", "铂金", "钻石", "大师", "最强" };

                    for (int o = 0; o < 8; o++)
                    {
                        if (arrPicName[i].Contains(arrPicRank[o]))
                        {
                            noDel = true;
                            break;
                        }
                    }
                    if (!noDel)
                        File.Delete(picPath + arrPicName[i] + ".png");
                }
            }
            string PicName = SetPicPath(picPath, strPicID);
            if (bVerbical)
            {
                try
                {
                    //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "").Save(m_strCapturePath + strPicID + ".bmp", ImageFormat.Bmp);
                    ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "").Save(picPath + strPicID + ".bmp", ImageFormat.Bmp);//第三个BmpInsert
                }
                catch (Exception ex)
                {
                    WriteToFile("BmpInsert3");
                    WriteToFile(ex.ToString());
                }
            }
            else
            {
                //if (OrdNo.IndexOf("MZH") == 0 || OrdNo == "测试订单")
                //{
                //    Bitmap sbmp = new Bitmap(m_strPicPath + "水印.bmp", true);
                //    try
                //    {
                //        ImageTool.BmpInsert(bbmp, sbmp, ref x, ref y, ref z, "水印");//第四个BmpInsert
                //    }
                //    catch (Exception ex)
                //    {
                //        WriteToFile("BmpInsert4");
                //        WriteToFile(ex.ToString());
                //    }
                //    sbmp.Dispose();
                //}
                string strJpg = PicName.Replace(".bmp", ".jpg");
                //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "").Save(strJpg, ImageFormat.Jpeg);
                try
                {
                    if (m_strOrderType == "发布单")
                    {
                        CreatWaterMark(strJpg, ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, ""));
                    }
                    else
                        ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "").Save(strJpg, ImageFormat.Jpeg);//第五个BmpInsert 
                }
                catch (Exception ex)
                {
                    WriteToFile("BmpInsert5");
                    WriteToFile(ex.ToString());
                }
                picNum++;
            }
            bbmp.Dispose();

            for (int j = 0; j < 3; j++)
            {
                File.Delete(strPic);
                if (File.Exists(strPic))
                {
                    Sleep(1000);
                    bbmp.Dispose();
                    continue;
                }
                break;
            }
            File.Delete(picPath + "RoleList.png");
            File.Delete(picPath + "TierInfo.png");
            return 1;
        }
        /// <summary>
        /// 遍历文件
        /// </summary>
        /// <returns></returns>
        public int TraversalFile(string dirPath)
        {
            List<string> list = new List<string>();//先定义list集合
            int count = 0;
            //在指定目录查找文件
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo Dir = new DirectoryInfo(dirPath);
                try
                {
                    foreach (FileInfo file in Dir.GetFiles())//查找子目录 
                    {
                        string arrName = string.Empty;
                        count++;
                        if (file.Name.Contains("jpg"))
                        {

                            using (FileStream fs = new FileStream(dirPath + file.Name, FileMode.Open, FileAccess.Read))
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                                Bitmap map = new Bitmap(image);
                                arrName = file.Name.Replace(".jpg", "");
                                if (IsGetZie)
                                    picResizePng(dirPath + file.Name, dirPath + arrName + ".png", 122, 222);
                                else
                                    picResizePng(dirPath + file.Name, dirPath + arrName + ".png", image.Width, image.Height);
                            }
                            file.Delete();
                        }
                        else if (file.Name.Contains("png"))
                            arrName = file.Name.Replace(".png", "");

                        list.Add(arrName); //给list赋值                    
                    }
                    list.Sort();
                    FileArr = list.ToArray();//再转成数组   
                    if (dirPath.Contains("Hero"))
                    {
                        WriteToFile("共有" + intHero + "个英雄图片，读取的数组长度为" + count);
                        if (count < intHero)
                        {
                            WriteToFile("获取英雄数量与读取不匹配,等待程序下载");
                            if (IsGetZie)
                                KeyMouse.MouseClick(520, 50, 1, 2, 500);//点击获取指定区服数据按钮
                            Sleep(3000);
                        }
                    }
                    if (dirPath.Contains("Skin"))
                    {
                        WriteToFile("共有" + intSkin + "个皮肤图片，读取的数组长度为" + count);
                        if ((count + 2) < intSkin)
                        {
                            writetime++;
                            WriteToFile("获取皮肤下载数量不匹配,等待程序下载");
                            if (IsGetZie)
                                KeyMouse.MouseClick(520, 50, 1, 2, 500);//点击获取指定区服数据按钮
                            Sleep(20 * 1000);
                            TraversalFile(m_strProgPath + @"\Skin\");
                            if (writetime == 5)
                            {
                                WriteToFile("等待超过100秒");
                                return 0;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return FileArr.Length;
        }
        /// <summary>
        /// 读取账号游戏信息
        /// </summary>
        public void LOLRead(string path)
        {
            // 创建文件流对象
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);
                string str = Encoding.GetEncoding("GB18030").GetString(array);
                try
                {
                    if (str.Contains("点券") && str.Contains("金币"))
                    {
                        WriteToFile("角色获取成功");
                    }
                    else
                    {
                        WriteToFile("该服务器无角色");
                        Status = 3010;
                        return;
                    }
                    string[] strArr = Regex.Split(str, "\r\n");
                    string newStr = string.Empty;
                    for (int i = 0; i < 7; i++)
                    {
                        newStr += strArr[i] + "\r\n";
                    }
                    WriteToFile(newStr);
                }
                catch (Exception e) { WriteToFile(e.ToString()); }
                //string name = MyStr.FindStr(str, "召唤师名字：", "\r\n").Trim();

                grade = MyStr.FindStr(str, "等级：", "\r\n").Trim();
                Level = MyStr.FindStr(str, "S7单双排：", "\r\n").Trim();
                string strRANK = Level;
                if (Level == "无段位")
                {
                    Random ro = new Random();
                    int iResult = ro.Next(1, 5);
                    if (iResult >= 4)
                        Level = "段位无-菲欧娜";
                    else if (iResult >= 3)
                        Level = "段位无-卢锡安";
                    else if (iResult >= 2)
                        Level = "段位无-亚索";
                    else if (iResult >= 1)
                        Level = "段位无-易";
                    strRANK = "当前赛季无";
                }
                if (Level.Contains("最强王者"))
                {
                    Level = "最强王者";
                    strRANK = "最强王者";
                }
                if (Level.Contains("超凡大师"))
                {
                    Level = "超凡大师";
                    strRANK = "超凡大师";
                }
                //Level = "荣耀黄金II";
                string Level2 = MyStr.FindStr(str, "S7灵活排位：", "\r\n").Trim();
                djc = int.Parse(MyStr.FindStr(str, "点券：", "\r\n").Trim());
                intCoin = int.Parse(MyStr.FindStr(str, "金币：", "\r\n").Trim());
                string hero = MyStr.FindStr(str, "英雄数量：", "\r\n").Trim();
                string skin = MyStr.FindStr(str, "皮肤数量：", "\r\n").Trim();
                if (hero == "" || hero == null)
                    hero = "0";
                if (skin == "" || skin == null)
                    skin = "0";
                intHero = int.Parse(hero);
                intSkin = int.Parse(skin);

                int width = 250;

                string LOLTempPath = m_strProgPath + @"\LOLTemp\";
                try
                {
                    if (File.Exists(@m_strCapturePath + "temp.png"))
                        File.Delete(@m_strCapturePath + "temp.png");
                    Bitmap bmp = new Bitmap(LOLTempPath + "排版.bmp"); //读取需要添加文字的图片 特别排版    排版
                    Graphics g1 = Graphics.FromImage(bmp);
                    Font font1 = new Font("微软雅黑", 16, FontStyle.Bold);                       //设置字体和大小
                    SolidBrush sbrush1 = new SolidBrush(Color.FromArgb(218, 218, 218));                //设置字体颜色             
                    g1.DrawString(grade, font1, sbrush1, new PointF(353, 32));
                    g1.DrawString(strRANK, font1, sbrush1, new PointF(353, 58));
                    g1.DrawString(Level2, font1, sbrush1, new PointF(353, 84));
                    g1.DrawString(djc.ToString(), font1, sbrush1, new PointF(353, 109));
                    g1.DrawString(intCoin.ToString(), font1, sbrush1, new PointF(353, 136)); //文字在图片上的坐标x,y
                    g1.DrawString(hero, font1, sbrush1, new PointF(353, 162));
                    g1.DrawString(skin, font1, sbrush1, new PointF(353, 189));
                    if (!Directory.Exists(m_strCapturePath))
                        Directory.CreateDirectory(m_strCapturePath);
                    bmp.Save(@m_strCapturePath + "temp.png");
                    PicAddWaterMark1(m_strCapturePath + "\\temp.png", LOLTempPath + (Level + ".jpg"), 29, 29, false);//29 29
                    bmp.Dispose();
                    PinTuPng("temp", "LOL1", false, m_strCapturePath);
                }
                catch (Exception ex)
                {
                    WriteToFile(ex.ToString());
                }
                if ((m_strOrderType == "发布单" && m_GameId == "60") || (OrdNo == "测试订单"))
                {
                    File.Copy(m_strProgPath + "\\TierInfo.jpg", m_strCapturePath + "LOL7_01.jpg", true);
                    if (strRANK == "当前赛季无")
                        jpgResize(LOLTempPath + (strRANK + ".png"), m_strCapturePath + "LOL6_01.jpg", 150, 150);
                    else
                        jpgResize(LOLTempPath + (strRANK + ".jpg"), m_strCapturePath + "LOL6_01.jpg", 150, 150);
                    WriteToFile("热门原图截图成功");
                }
                if (File.Exists(m_strProgPath + "\\RoleList.jpg"))
                {
                    if (m_GameId == "100" || m_strOrderType == "发布单")
                    {
                        using (FileStream fsm = new FileStream(m_strProgPath + "\\RoleList.jpg", FileMode.Open, FileAccess.Read))
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(fsm);
                            picResize(LOLTempPath + "\\LOL水印.bmp", m_strProgPath + "\\LOL水印.bmp", 100, image.Height - 28);
                        }
                        PicAddWaterMark1(m_strProgPath + "\\RoleList.jpg", m_strProgPath + "\\LOL水印.bmp", 160, 26);
                    }
                    using (FileStream fsm = new FileStream(m_strProgPath + "\\RoleList.jpg", FileMode.Open, FileAccess.Read))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(fsm);
                        image.Save(@m_strProgPath + "\\RoleList.png", ImageFormat.Png);
                    }
                    File.Move(m_strProgPath + "\\RoleList.png", m_strCapturePath + "RoleList.png");
                    PinTuPng("RoleList", "LOL1", false, m_strCapturePath);
                }
                if (File.Exists(m_strProgPath + "\\TierInfo.jpg"))
                {
                    using (FileStream fsm = new FileStream(m_strProgPath + "\\TierInfo.jpg", FileMode.Open, FileAccess.Read))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(fsm);
                        image.Save(@m_strProgPath + "\\TierInfo.png", ImageFormat.Png);
                    }
                    File.Move(m_strProgPath + "\\TierInfo.png", m_strCapturePath + "TierInfo.png");
                    PinTuPng("TierInfo", "LOL1", false, m_strCapturePath);
                }
                if (File.Exists(m_strProgPath + "\\Punish.jpg"))
                {
                    using (FileStream fsm = new FileStream(m_strProgPath + "\\Punish.jpg", FileMode.Open, FileAccess.Read))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(fsm);
                        image.Save(@m_strProgPath + "\\Punish.png", ImageFormat.Png);
                    }
                    File.Move(m_strProgPath + "\\Punish.png", m_strCapturePath + "Punish.png");
                    PinTuPng("Punish", "LOL1", false, m_strCapturePath);
                    WriteToFile("检测到账号处于裁决之镰");
                }

            }
            Sleep(1000);
        }
        /// <summary>
        /// 读取账号游戏信息
        /// </summary>
        public void LOLRead2(string path)
        {
            // 创建文件流对象
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);
                string str = Encoding.GetEncoding("GB18030").GetString(array);
                try
                {
                    string[] strArr = Regex.Split(str, "\r\n");
                    string newStr = string.Empty;
                    for (int i = 0; i < 7; i++)
                    {
                        newStr += strArr[i] + "\r\n";
                    }
                    WriteToFile(newStr);
                }
                catch (Exception e) { WriteToFile(e.ToString()); }
                string name = MyStr.FindStr(str, "召唤师名字：", "\r\n").Trim();
                grade = MyStr.FindStr(str, "召唤师等级：", "\r\n").Trim();
                Level = MyStr.FindStr(str, "召唤师段位：", "\r\n").Trim();
                string strRANK = Level;
                if (Level == "无段位")
                {
                    Random ro = new Random();
                    int iResult = ro.Next(1, 5);
                    if (iResult >= 4)
                        Level = "段位无-菲欧娜";
                    else if (iResult >= 3)
                        Level = "段位无-卢锡安";
                    else if (iResult >= 2)
                        Level = "段位无-亚索";
                    else if (iResult >= 1)
                        Level = "段位无-易";
                    strRANK = "当前赛季无";
                }
                if (Level.Contains("最强王者"))
                {
                    Level = "最强王者";
                    strRANK = "最强王者";
                }
                if (Level.Contains("超凡大师"))
                {
                    Level = "超凡大师";
                    strRANK = "超凡大师";
                }
                djc = int.Parse(MyStr.FindStr(str, "点券：", "\r\n").Trim());
                intCoin = int.Parse(MyStr.FindStr(str, "金币：", "\r\n").Trim());
                string hero = MyStr.FindStr(str, "英雄数量：", "\r\n").Trim();
                string skin = MyStr.FindStr(str, "皮肤数量：", "\r\n").Trim();
                if (hero == "" || hero == null)
                    hero = "0";
                if (skin == "" || skin == null)
                    skin = "0";
                intHero = int.Parse(hero);
                intSkin = int.Parse(skin);

                int width = 250;

                string LOLTempPath = m_strProgPath + @"\LOLTemp\";
                try
                {
                    if (File.Exists(@m_strCapturePath + "temp.png"))
                        File.Delete(@m_strCapturePath + "temp.png");
                    Bitmap bmp = new Bitmap(LOLTempPath + "特别排版.bmp");        //读取需要添加文字的图片    
                    Graphics g1 = Graphics.FromImage(bmp);
                    Font font1 = new Font("微软雅黑", 16, FontStyle.Bold);                       //设置字体和大小
                    SolidBrush sbrush1 = new SolidBrush(Color.FromArgb(218, 218, 218));                //设置字体颜色 
                    if (m_strOrderType == "发布单")
                        g1.DrawString("█████████", font1, sbrush1, new PointF(353, 32));
                    else
                        g1.DrawString(name, font1, sbrush1, new PointF(353, 32));
                    g1.DrawString(grade, font1, sbrush1, new PointF(353, 58));
                    g1.DrawString(strRANK, font1, sbrush1, new PointF(353, 84));
                    g1.DrawString(djc.ToString(), font1, sbrush1, new PointF(353, 109));
                    g1.DrawString(intCoin.ToString(), font1, sbrush1, new PointF(353, 136)); //文字在图片上的坐标x,y
                    g1.DrawString(hero, font1, sbrush1, new PointF(353, 162));
                    g1.DrawString(skin, font1, sbrush1, new PointF(353, 189));
                    if (!Directory.Exists(m_strCapturePath))
                        Directory.CreateDirectory(m_strCapturePath);
                    bmp.Save(@m_strCapturePath + "temp.png");
                    PicAddWaterMark1(m_strCapturePath + "\\temp.png", LOLTempPath + (Level + ".jpg"), 29, 29, false);
                    PinTuPng("temp", "LOL1", false, m_strCapturePath);
                }
                catch (Exception ex)
                {
                    WriteToFile(ex.ToString());
                }
                if ((m_strOrderType == "发布单" && m_GameId == "60") || (OrdNo == "测试订单"))
                {
                    if (strRANK == "当前赛季无")
                    {
                        jpgResize(LOLTempPath + (Level + "150.jpg"), m_strCapturePath + "LOL6_01.jpg", 150, 150);
                    }
                    else
                        jpgResize(LOLTempPath + (Level + ".jpg"), m_strCapturePath + "LOL6_01.jpg", 150, 150);
                    WriteToFile("热门原图截图成功");
                }
                if (File.Exists(m_strProgPath + "\\Punish.jpg"))
                {
                    using (FileStream fsm = new FileStream(m_strProgPath + "\\Punish.jpg", FileMode.Open, FileAccess.Read))
                    {
                        System.Drawing.Image image = System.Drawing.Image.FromStream(fsm);
                        image.Save(@m_strProgPath + "\\Punish.png", ImageFormat.Png);
                    }
                    File.Move(m_strProgPath + "\\Punish.png", m_strCapturePath + "Punish.png");
                    PinTuPng("Punish", "LOL1", false, m_strCapturePath);
                    WriteToFile("检测到账号处于裁决之镰");
                }
            }
            Sleep(1000);
        }
        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="strNewFile"></param>
        /// <param name="intWidth"></param>
        /// <param name="intHeight"></param>
        /// <returns>参数:原文件完整名称,新文件完整名称,新的宽度,新的高度(若高度为0,按比例缩放)</returns>
        public bool picResizePng(string strFile, string strNewFile, int intWidth, int intHeight)
        {
            System.Drawing.Bitmap objPic, objNewPic;
            try
            {

                if (!File.Exists(strFile))
                    return false;
                objPic = new System.Drawing.Bitmap(strFile);
                if (intHeight <= 0)
                    intHeight = (intWidth * objPic.Height / objPic.Width);
                objNewPic = new System.Drawing.Bitmap(objPic, intWidth, intHeight);
                objPic.Dispose();
                objPic = null;
                string newFile = strNewFile.Substring(0, strNewFile.LastIndexOf('.'));
                objNewPic.Save(newFile + ".png", ImageFormat.Png);
                objNewPic.Dispose();
                File.Delete(strFile);

            }
            catch (Exception exp)
            {
                return false;
            }
            finally
            {

                objNewPic = null;
            }
            return true;
        }
        /// <summary>
        /// 图片生成png
        /// </summary>
        /// <param name="filePic">操作标示</param>
        /// <param name="strPicID">图片ID</param>
        /// <param name="width">宽</param>
        /// <returns></returns>
        public bool CreatePlatePng(string filePic, string strPicID, int width)
        {
            if (filePic == "生成图片")
            {
                bPicFull = true;
                goto NEXT_STEP;
            }
            if (filePic == "换行")
            {
                ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                ptBigPic.X = 0;
                //ptBigPic.Y += ptMAX.Y + 1;
                ptBigPic.Y += ptMAX.Y;
                ptMAX.Y = 0;
                return true;
            }
            if (filePic.IndexOf(".png") < 0)
                filePic += ".png";
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

        NEXT_STEP:
            if (ptMAX.X == 0 && ptMAX.Y == 0)
            {
                WriteToFile("大图为空\r\n");
                C = C + 1;
                bPicFull = false;
                return false;
            }

            if (ptBigPic.Y > 0)//多排
            {
                ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                ptMAX.Y += ptBigPic.Y;
            }
            else
            {
                if (ptBigPic.X < 130)
                    ptMAX.X = ptBigPic.X + 130;//当图片小于水印尺寸时，则还需加上水印的尺寸（水印宽度为130）
                else
                    ptMAX.X += ptBigPic.X;
            }

            if (width != nZHPicWidth || MZH)  //取消宽度限制
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
        public bool PicAddWaterMark1(string filePic, string filewater, int left, int top, bool del)
        {
            if (!File.Exists(filePic))
            {
                FileRW.WriteToFile(filePic + "<< 文件不存在！");
                return false;
            }


            Bitmap srcBit = (Bitmap)Bitmap.FromFile(filePic, true);
            Bitmap waterBit = (Bitmap)Bitmap.FromFile(filewater, false);

            int x = left, y = top;
            Rectangle srcRect = new Rectangle(x, y, waterBit.Width, waterBit.Height);
            Rectangle waterRect = new Rectangle(0, 0, waterBit.Width, waterBit.Height);
            BitmapData srcBData = srcBit.LockBits(srcRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);//Format24bppRgb
            BitmapData waterBData = waterBit.LockBits(waterRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            srcBData.Scan0 = waterBData.Scan0;
            srcBit.UnlockBits(srcBData);
            waterBit.UnlockBits(waterBData);
            srcRect = new Rectangle(0, 0, srcBit.Width, srcBit.Height);
            srcBData = srcBit.LockBits(srcRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            int Swidth = srcBData.Width;
            int Sheight = srcBData.Height;
            byte[] tData = new byte[Swidth * Sheight * 4];
            System.Runtime.InteropServices.Marshal.Copy(srcBData.Scan0, tData, 0, srcBData.Stride * srcBData.Height);
            srcBit.UnlockBits(srcBData);
            srcBit.Dispose();
            waterBit.Dispose();
            try
            {
                ImageTool.CreatBmpFromByte(tData, Swidth, Sheight).Save(filePic, ImageFormat.Jpeg);
            }
            catch (Exception err)
            {
            }
            if (del)
            {
                File.Delete(filewater);
            }
            return true;
        }
        public bool PicAddWaterMark1(string filePic, string filewater, int left, int top)
        {
            if (!File.Exists(filePic))
            {
                FileRW.WriteToFile(filePic + "<< 文件不存在！");
                return false;
            }


            Bitmap srcBit = (Bitmap)Bitmap.FromFile(filePic, true);
            Bitmap waterBit = (Bitmap)Bitmap.FromFile(filewater, false);

            int x = left, y = top;
            Rectangle srcRect = new Rectangle(x, y, waterBit.Width, waterBit.Height);
            Rectangle waterRect = new Rectangle(0, 0, waterBit.Width, waterBit.Height);
            BitmapData srcBData = srcBit.LockBits(srcRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);//Format24bppRgb
            BitmapData waterBData = waterBit.LockBits(waterRect, ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            srcBData.Scan0 = waterBData.Scan0;
            srcBit.UnlockBits(srcBData);
            waterBit.UnlockBits(waterBData);
            srcRect = new Rectangle(0, 0, srcBit.Width, srcBit.Height);
            srcBData = srcBit.LockBits(srcRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            int Swidth = srcBData.Width;
            int Sheight = srcBData.Height;
            byte[] tData = new byte[Swidth * Sheight * 4];
            System.Runtime.InteropServices.Marshal.Copy(srcBData.Scan0, tData, 0, srcBData.Stride * srcBData.Height);
            srcBit.UnlockBits(srcBData);
            srcBit.Dispose();
            waterBit.Dispose();
            try
            {
                ImageTool.CreatBmpFromByte(tData, Swidth, Sheight).Save(filePic, ImageFormat.Jpeg);
            }
            catch (Exception err)
            {
            }

            File.Delete(filewater);

            return true;
        }
        public void CreatWaterMark(string strJpg, Bitmap bit)
        {
            #region 滤色处理
            Bitmap picbmp = bit;
            Rectangle srcRect = new Rectangle(0, 0, picbmp.Width > 1000 ? 1000 : picbmp.Width, picbmp.Height > 1000 ? 1000 : picbmp.Height);
            BitmapData bigBData1 = picbmp.LockBits(srcRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Bitmap picbtm = new Bitmap(picbmp.Width, picbmp.Height, bigBData1.Stride, PixelFormat.Format32bppArgb, bigBData1.Scan0);//原本
            Bitmap mybm = new Bitmap(m_strPicPath + "背景.bmp");
            int Width = mybm.Width;
            int height = mybm.Width;
            Bitmap bm = new Bitmap(Width, height);//初始化一个记录滤色效果的图片对象
            int x, y;
            Color pixel;

            for (x = 0; x < Width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前坐标的像素值
                    if (pixel.R == 8 && pixel.G == 8 && pixel.B == 8)
                        bm.SetPixel(x, y, Color.FromArgb(0, pixel.R, pixel.G, pixel.B));//绘图
                    else if (pixel.R == 42 && pixel.G == 42 && pixel.B == 42)
                        bm.SetPixel(x, y, Color.FromArgb(90, 242, 242, 242));//绘图
                    else if (pixel.R > 42 && pixel.G > 42 && pixel.B > 42)
                    {
                        int a = 90 - (pixel.R + pixel.G + pixel.B - 42 * 3) / 3;
                        bm.SetPixel(x, y, Color.FromArgb(a, 242, 242, 242));//绘图
                    }
                    else
                    {
                        int a = 90 + (pixel.R + pixel.G + pixel.B - 42 * 3) / 3;
                        bm.SetPixel(x, y, Color.FromArgb(a, 242, 242, 242));//绘图
                    }
                }
            }
            BitmapData bigBData = bm.LockBits(srcRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Bitmap bmp2 = new Bitmap(picbmp.Width > 1000 ? 1000 : picbmp.Width, picbmp.Height > 1000 ? 1000 : picbmp.Height, bigBData.Stride, PixelFormat.Format32bppArgb, bigBData.Scan0);
            Sleep(50);

            Graphics graph1 = Graphics.FromImage(picbtm);//picbtm
            graph1.DrawImage(bmp2, 0, 0);
            picbtm.Save(strJpg, ImageFormat.Jpeg);

            graph1.Dispose();
            bmp2.Dispose();
            bm.UnlockBits(bigBData);
            bm.Dispose();
            mybm.Dispose();
            picbmp.Dispose();
            Sleep(100);
            #endregion
        }
        public int CheckHuaDong()
        {
            int result = 0;
            Point pt = new Point(-1, -2);
            Point pa = new Point(-1, -2);
            pt = ImageTool.fPic(m_strPicPath + "\\ww\\滑动验证码.bmp", 0, 0, 900, 200);//360
            pa = ImageTool.fPic(m_strPicPath + "\\ww\\滑动按钮.bmp", 0, 0, 900, 360);
            if (pt.X > 0 && pa.X > 0)
            {
                //WriteToFile(pa.X.ToString() + pa.Y.ToString());
                KeyMouse.MouseClick(pt.X - 66, pa.Y + 11, 1, 3, 1000);
                CaptureBmpInRect("\\滑动验证图", m_strProgPath, pt.X - 66, pt.Y + 56, pt.X + 175, pt.Y + 213);
                string strResult = RequestSafeCardInfo(3, m_strProgPath + "\\滑动验证图.bmp", "", 90);

                if (strResult == "" || strResult == "*")
                    return 2230;
                if (strResult.Contains(","))
                {
                    string[] sArray = strResult.Split(',');
                    result = int.Parse(sArray[0]);
                }
                else
                {
                    string[] sArray = strResult.Split('，');
                    result = int.Parse(sArray[0]);
                }

                if (result == 0)
                {
                    WriteToFile("放弃答题");
                    return 2330;
                }
                KeyMouse.MouseClick(pt.X - 66 + result, pa.Y + 11, 1, 4, 1000);
                RGcodeRight(3);
            }
            return 1;
        }
        /// <summary>
        /// 获取指定文件的字节长度
        /// </summary>
        /// <returns></returns>
        public bool CheckSkinTure()
        {
            string SkinPath = @"\\192.168.60.57\虚拟机共享\24005.jpg";
            long lSize = 0;
            lSize = new FileInfo(SkinPath).Length;
            if (lSize == 78.9 * 1024)
                return true;
            return false;
        }
    }
}
