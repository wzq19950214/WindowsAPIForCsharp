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
using System.Diagnostics;
using System.Security.Principal;
using System.Runtime.InteropServices;
using RC_ZH_LOL;
//using System.Threading.Tasks;
namespace GTR
{
    class MY
    {
        #region 初始化通信变量
        //订单数据
        public static string m_strOrderData;
        //udpsockets
        public static udpSockets udpdd;
        //验证码回答次数
        public int yzmTimes = 0;
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
        #endregion

        #region 初始化程序变量

        //截图默认尺寸
        bool zhongduan = false;
        private static Thread jiankong;
        Point ptMove = new Point(-25, -10);
        public const int Lwidth = 1280;
        public const int Lheight = 1200;
        public const int nZHPicWidth = 880;
        //拼图是否已满
        public bool bPicFull = false;
        public Point ptBigPic;
        //是否测试新功能
        bool isTest = false;
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
        //struct CoinStruct
        //{
        //    public int x;
        //    public String no;
        //}
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
        //验证码（jpg）
        string mousea;
        //窗口句柄
        public static IntPtr m_hGameWnd;
        //切换句柄
        public static IntPtr m_hChangeWnd;
        //程序所在路径
        private string m_strProgPath = System.Windows.Forms.Application.StartupPath;
        //匹配图片路径
        public static string m_strPicPath = System.Windows.Forms.Application.StartupPath + @"\梦幻西游\";
        //异常截图保存路径
        public static string LocalPicPath = "E:\\梦幻西游2账号截图\\";
        public int AccType;
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
        public string IsNeedRecognition;
        public static StringBuilder strb = new StringBuilder();
        #endregion
        //主函数入口
        public void StartToWork()
        {
            #region 做单机拨号

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

            #endregion

            m_hGameWnd = User32API.GetDesktopWindow();
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
            if (Status > 1000)
            {
                if (zhongduan)
                    Status = 2050;
                CaptureJpg("订单失败");
                FileRW.DeleteTmpFiles(m_strCapturePath + OrdNo);
            }
            if (MZH && Status != 1000)
            {
                if (Status > 2000 && Status < 3000)
                    Status += 2000;
            }
            string tmp;
            if (Status == 1000)
            {
                tmp = string.Format("截图成功,共{0}张\r\n", picNum);
                WriteToFile(tmp);
            }

            CloseGames();
            tmp = string.Format("移交状态={0}\r\n", Status);
            WriteToFile(tmp);
            tmp = string.Format("FStatus={0}\r\n", Status);

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
            try
            {
                jiankong.Abort();
            }
            catch { };
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

            #region 邮箱类型判断
            if (m_strAccount.Contains("@163.com"))
                AccType = 0;
            else if (m_strAccount.Contains("@qq.com"))
                AccType = 1;
            else if (m_strAccount.Contains("@sina.com"))
                AccType = 2;
            else if (m_strAccount.Contains("@126.com"))
                AccType = 3;
            else if (m_strAccount.Contains("@vip.qq.com"))
                AccType = 4;
            else
            {
                WriteToFile("未填写正确的邮箱格式，默认后缀@163.com");
                AccType = 0;
            }
            try
            {
                m_strAccount = m_strAccount.Substring(0, m_strAccount.IndexOf("@"));
            }
            catch
            { }
            AppInit();//IP地址 版本号
            jiankong = new Thread(new ThreadStart(fun1));
            jiankong.Start();
            #endregion

            for (int i = 0; i < 3; i++)
            {
                //-------------------------------------
                //测试区域
                //Sleep(3000);
                //GameCarPic();

                #region 拼图测试
                //PinTu("人物属性,师门技能,辅助技能,换行,修炼技能,召唤兽属性", "MHXY1", false);
                //PinTu("人物总览,召唤兽总览,坐骑总览", "MHXY2", false);
                //PinTu("道具总览,行囊总览", "MHXY2", false);
                //PinTu("装备1,装备2,换行,装备3,装备4,换行,装备5,装备6", "MHXY3", false);
                //PinTu("装备套装", "MHXY3", false);
                //PinTu("锦衣", "MHXY4", false);
                //PinTu("法宝", "MHXY4", false);
                //PinTu("我的成就", "MHXY5", false);
                //PinTu("服务器成就,服务器成就1", "MHXY5", false);
                //PinTu("宠物", "MHXY6", false);
                //PinTu("坐骑属性,坐骑技能", "MHXY6", false);
                #endregion

                #region 找图测试
                //Point pa = new Point(-1, -2);
                //pa = ImageTool.fPic(m_strPicPath + "装备上标.bmp", 0, 0, 0, 0, 30);//线上
                //pa = ImageTool.fPic(m_strPicPath + "装备下标.bmp", 0, 0, 0, 0, 30);//线上
                //pa = ImageTool.fPic(m_strPicPath + "鼠标3.bmp", 0, 0, 0, 0, 40);//线上
                //if (pa.X > 0)
                //{
                //    WriteToFile("更新成功");
                //    KeyMouse.MouseClick(m_hGameWnd, pa.X + 15, pa.Y + 15, 1, 1, 1000);
                //    return 1;
                //}
                #endregion

                #region 找字测试
                //m_strArea = "升级前请关闭其他正在运行的客户端";
                //Point pa = new Point(-1, -2);
                //RECT rtA = new RECT(583, 763, 880, 900);
                //for (int a = 0; a < 10; a++)
                //{
                //    pa = ImageTool.FindText(m_hGameWnd, m_strArea, Color.FromArgb(255, 255, 0), "宋体", 16, FontStyle.Regular, 0, 0, rtA, 30);
                //    if (pa.X > 0)
                //    {
                //        KeyMouse.MouseClick(pa.X + 5, pa.Y + 5, 1, 1, 500);
                //        break;
                //    }
                //    Sleep(200);
                //}
                #endregion

                //--------------------------------------

                #region 功能一：自动登录游戏
                CloseGames();
                Status = RunGame();
                if (Status > 1000)
                {
                    if (Status == 2120)
                        continue;
                    return Status;
                }
                Status = EnterAccPwd();
                if (Status > 1000)
                {
                    if (Status == 2120)
                        continue;
                    return Status;
                }
                Status = SelectServer();
                if (Status > 1000)
                {
                    if (Status == 2120)
                        continue;
                    return Status;
                }
                Status = SelectRole();
                if (Status > 1000)
                    return Status;
                #endregion

                Status = GameCarPic();
                if (Status >= 1000)
                {
                    if (Status == 2120)
                        continue;
                    return Status;
                }
                

            }
            return Status;
        }
        // 启动TGP
        public int RunGame()
        {
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            for (int i = 0; i < 5; i++)
            {
                m_GameTitle = "梦幻西游 ONLINE";
                m_hGameWnd = User32API.FindWindow(null, m_GameTitle);
                //如果为空，按照指定路径打开游戏
                if (m_hGameWnd == IntPtr.Zero)
                {
                    myapp.RunBat(m_strGameStartFile);
                    WriteToFile(m_strGameStartFile);
                    m_GameTitle = "梦幻西游 ONLINE";
                    m_hGameWnd = User32API.FindWindow(null, m_GameTitle);
                    Sleep(1000);
                    if (m_hGameWnd == IntPtr.Zero)
                        continue;
                }

                //寻找登录界面
                for (int y = 0; y < 60; y++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "主界面登录.bmp", 0, 0, 0, 0, 80);
                    pb = ImageTool.fPic(m_strPicPath + "主界面登录1.bmp", 0, 0, 0, 0, 80);
                    if (pa.X > 0 || pb.X > 0)
                    {
                        pa = pa.X > 0 ? pa : pb;
                        WriteToFile("更新成功");
                        KeyMouse.MouseClick(m_hGameWnd, pa.X + 15, pa.Y + 15, 1, 1, 1000);
                        return 1;
                    }
                    if (y % 8 == 0)
                    {
                        WriteToFile("等待游戏更新");
                    }
                    Sleep(2000);
                }

            }
            WriteToFile("游戏开启五次失败");
            return 2260;
        }
        //输入帐号密码      
        public int EnterAccPwd()
        {
            //--------------------------------------------------

            // -------------------------------------------------
            #region 初始化变量
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            Point pc = new Point(-1, -2);
            RECT rtC = new RECT(560, 525, 704, 550);
            RECT rtCC = new RECT(580, 570, 886, 679);
            m_hGameWnd = User32API.GetDesktopWindow();
            #endregion
            for (int i = 0; i < 10; i++)
            {
                pa = ImageTool.fPic(m_strPicPath + "公告榜.bmp", 347, 418, 508, 516);
                if (pa.X > 0)
                {
                    KeyMouse.MouseClick(pa.X + 618, pa.Y + 375, 1, 1, 1000);
                    break;
                }
                if(i%4==0)
                    WriteToFile("等待公告界面");
                Sleep(1000);
            }
            if (pa.X < 0)
            {
                CaptureJpg("找不到公告榜");
                WriteToFile("公告界面加载失败");
                return 2260;
            }

            for (int i = 0; i < 5; i++)
            {
                Sleep(3000);
                pa = ImageTool.fPic(m_strPicPath + "账号界面标志.bmp", 798, 509, 852, 557);
                if (pa.X > 0)
                {
                   
                    pc = ImageTool.FindText(m_hGameWnd, "升级前请关闭其他正在运行的客户端", Color.FromArgb(255, 255, 0), "宋体", 16, FontStyle.Regular, 0, 0, rtCC, 30);
                    if (pc.X > 0)
                    {
                        WriteToFile("关闭测试服提醒，尝试登陆密码");
                        KeyMouse.MouseClick(793, 660, 1, 1, 500);
                    }
                    for (int j = 1; j < 4; j++)
                    {

                        #region 输入账号密码并登陆
                        KeyMouse.MouseClick(pa.X - 23, pa.Y + 11, 1, 1, 500); //点击账号输入框
                        KeyMouse.SendBackSpaceKey(30);
                        WriteToFile("输入账号密码");
                        KeyMouse.SendKeys(m_strAccount, 200);
                        Sleep(500);
                        KeyMouse.MouseClick(pa.X - 67, pa.Y + 35 + AccType * 18, 1, 1, 500);  //--账号后缀功能选已添加,默认163.com
                        KeyMouse.MouseClick(pa.X - 23, pa.Y + 51, 1, 1, 500);
                        KeyMouse.SendBackSpaceKey();
                        //WriteToFile("输入密码");
                        KeyMouse.SendKeys(m_strPassword, 300);
                        CaptureJpg("账号密码");
                        WriteToFile("输入账号：[" + m_strAccount + "]" + "密码[" + m_strPassword.Length.ToString() + "]位完成");
                        Sleep(1000);
                        KeyMouse.MouseClick(pa.X - 30, pa.Y + 122, 1, 1, 500);//点击登录

                        #endregion

                        #region 账号登录异常判断
                        for (int k = 0; k < 7; k++)
                        {
                            pb = ImageTool.fPic(m_strPicPath + "密码不正确.bmp", 718, 584, 781, 633);
                            if (pb.X > 0)
                                break;
                            pc = ImageTool.FindText(m_hGameWnd, "此帐号已经被锁定", Color.FromArgb(255, 255, 255), "宋体", 16, FontStyle.Regular, 0, 0, rtC, 30);
                            if (pc.X > 0)
                            {
                                WriteToFile("此帐号已经被锁定");
                                return 3700;
                            }

                            Sleep(300);
                        }
                        if (pb.X > 0)
                        {
                            WriteToFile("第" + j.ToString() + "次帐号密码错误");
                            if (j == 3)
                            {
                                CaptureJpg("密码错3次");
                                return 3000;
                            }
                            Sleep(500);
                            KeyMouse.MouseClick(pb.X - 17, pb.Y + 66, 1, 1, 500);
                            continue;
                        }

                        #endregion

                        #region 区服界面标志判断，return 1
                        for (int k = 0; k < 3; k++)
                        {
                            pb = ImageTool.fPic(m_strPicPath + "区服界面标志.bmp", 411, 440, 919, 713);
                            if (pb.X > 0)
                                return 1;
                            Sleep(500);
                        }
                        #endregion

                    }

                }

            }
           
            if (pa.X < 0)
            {
                CaptureJpg("找不到帐号界面标志");
                WriteToFile("账号界面加载失败");
                return 2260;
            }
            WriteToFile("未知情况登录失败");
            CaptureJpg("未知情况登录失败，找不到跳出值");
            return 2120;
        }
        public int SelectServer()
        {
            #region 初始化变量
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            Point pc = new Point(-1, -2);
            m_hGameWnd = User32API.GetDesktopWindow();
            RECT rtH = new RECT(446, 809, 913, 840);
            RECT rtA = new RECT(406, 434, 916, 707);
            RECT rtS = new RECT(406, 434, 920, 774);
            RECT rtCC = new RECT(580, 570, 886, 679);
            int WaitTime = 0;
            #endregion
            //--------------------------------------------------
            //找字测试
            // -------------------------------------------------

            for (int i = 0; i < 5; i++)
            {
                pb = ImageTool.fPic(m_strPicPath + "区服界面标志.bmp", 411, 440, 919, 713);
                if (pb.X > 0)
                {
                    KeyMouse.MouseMove(m_hGameWnd, 0, 0);
                    WriteToFile("选择区服");
                    #region 寻找已有角色的区服
                    for (int a = 0; a < 10; a++)
                    {
                        pa = ImageTool.FindText(m_hGameWnd, m_strServer, Color.FromArgb(0, 0, 0), "宋体", 14, FontStyle.Regular, 0, 0, rtH, 30);
                        if (pa.X > 0)
                        {
                            KeyMouse.MouseClick(pa.X + 5, pa.Y + 5, 1, 1, 500);
                            break;
                        }
                        Sleep(200);
                    }
                    #endregion
                    if (pa.X < 0)
                    {
                        //--新区功能未添加
                        #region 寻找大区
                        for (int a = 0; a < 10; a++)
                        {
                            pa = ImageTool.FindText(m_hGameWnd, m_strArea, Color.FromArgb(255, 255, 255), "宋体", 14, FontStyle.Regular, 0, 0, rtA, 30);
                            if (pa.X > 0)
                            {
                                KeyMouse.MouseClick(pa.X + 5, pa.Y + 5, 1, 1, 500);
                                break;
                            }
                            Sleep(200);
                        }
                        if (pa.X < 0)
                        {
                            WriteToFile("找不到服务器" + m_strArea);
                            CaptureJpg("找不到服务器" + m_strArea);
                            return 2260;
                        }
                        #endregion

                        #region 寻找具体区服
                        for (int a = 0; a < 10; a++)
                        {
                            pa = ImageTool.FindText(m_hGameWnd, m_strServer, Color.FromArgb(0, 0, 0), "宋体", 14, FontStyle.Regular, 0, 0, rtS, 30);
                            if (pa.X > 0)
                            {
                                KeyMouse.MouseClick(pa.X + 5, pa.Y + 5, 1, 1, 500);
                                break;
                            }
                            Sleep(200);
                        }

                        if (pa.X < 0)
                        {
                            if (File.Exists(m_strProgPath + "\\NewSeverPT.txt"))
                            {
                                Point pArea = Point.Empty, pServer = Point.Empty;
                                string PTData = FileRW.ReadFile("NewSeverPT.txt");
                                if (PTData.Contains(m_strArea + m_strServer))
                                {
                                    string m_RegInfos = MyStr.FindStr(PTData, m_strArea + m_strServer, "<pt>");
                                    pArea.X = Convert.ToInt32(MyStr.FindStr(m_RegInfos, "Ar.X=", ","));
                                    pArea.Y = Convert.ToInt32(MyStr.FindStr(m_RegInfos, "Ar.Y=", ","));
                                    pServer.X = Convert.ToInt32(MyStr.FindStr(m_RegInfos, "Sr.X=", ","));
                                    pServer.Y = Convert.ToInt32(MyStr.FindStr(m_RegInfos, "Sr.Y=", ","));
                                    KeyMouse.MouseClick(pArea.X, pArea.Y, 1, 1, 500);
                                    KeyMouse.MouseClick(pServer.X, pServer.Y, 1, 2, 500);
                                    return 1;
                                }
                            }
                            WriteToFile("找不到具体区" + m_strServer);
                            CaptureJpg("找不到具体区" + m_strServer);
                            return 2260;
                        }
                        #endregion
                    }

                    #region 服务器连接异常判断
                    WriteToFile("检测服务器异常窗口");
                    for (int a = 0; a < 20; a++)
                    {
                        //服务器繁忙
                        pa = ImageTool.fPic(m_strPicPath + "服务器繁忙.bmp", 555, 508, 666, 561);
                        if (pa.X > 0)
                            break;

                        //服务器繁忙
                        pa = ImageTool.fPic(m_strPicPath + "排队列队已满.bmp", 556, 501, 861, 588);
                        if (pa.X > 0)
                        {
                            WriteToFile("排队列队已满");
                            return 2370;
                        }

                        //服务器等待连接
                        pa = ImageTool.fPic(m_strPicPath + "等待连接服务器.bmp", 558, 583, 805, 635);
                        if (pa.X > 0)
                        {
                            WriteToFile("等待连接服务器");
                            WaitTime++;
                            Sleep(5000);
                            if (WaitTime > 4)
                            {
                                WriteToFile("连接服务器超时");
                                return 2370;
                            }
                            continue;
                        }
                        //测试服超版本
                        pa = ImageTool.FindText(m_hGameWnd, "升级前请关闭其他正在运行的客户端", Color.FromArgb(255, 255, 0), "宋体", 16, FontStyle.Regular, 0, 0, rtCC, 30);
                        if (pa.X > 0)
                        {
                            WriteToFile("账号所在服务器为测试服,无法登录高版本客户端,转人工");
                            KeyMouse.MouseClick(793, 660, 1, 1, 500);
                            return 3333;
                        }
                        //服务器无角色
                        pa = ImageTool.fPic(m_strPicPath + "无角色.bmp", 356, 443, 433, 471);
                        if (pa.X > 0)
                        {
                            WriteToFile("该区服无角色");
                            return 3010;
                        }
                        #region 判断条款服务界面标志，return 1
                        pa = ImageTool.fPic(m_strPicPath + "角色界面标志1.bmp", 612, 231, 832, 394);
                        pc = ImageTool.fPic(m_strPicPath + "角色界面标志2.bmp", 469, 748, 546, 789);
                        if (pa.X > 0 || pc.X > 0)
                            return 1;
                        #endregion
                        Sleep(300);
                    }
                    if (pa.X > 0)
                    {
                        KeyMouse.MouseClick(pa.X + 162, pa.Y + 135, 1, 1, 1000);
                        WriteToFile("服务器繁忙,重新选区进入");
                        Sleep(2000);
                        continue;
                    }
                    #endregion
                }
                Sleep(1000);
            }
            if (pb.X < 0)
            {
                WriteToFile("区服界面消失");
                CaptureJpg("区服界面消失");
                return 2260;
            }
            WriteToFile("多次进入,服务器繁忙,连接服务器超时");
            CaptureJpg("服务器繁忙");
            return 2370;
        }
        public int SelectRole()
        {
            #region 初始化变量
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            Point pc = new Point(-1, -2);
            RECT rtR = new RECT(547, 757, 651, 781);
            RECT rtA = new RECT(566, 543, 869, 587);
            m_hGameWnd = User32API.GetDesktopWindow();
            #endregion

            #region 点卡欠费查询
            WriteToFile("检查点卡是否欠费");
            for (int a = 0; a < 10; a++)
            {
                pa = ImageTool.FindText(m_hGameWnd, "你的帐号欠费300点", Color.FromArgb(255, 255, 255), "宋体", 16, FontStyle.Regular, 0, 0, rtA, 30);
                if (pa.X > 0)
                {
                    WriteToFile("点卡欠费已达上限,无法进入游戏");
                    return 3500;
                }
                pa = ImageTool.FindText(m_hGameWnd, "你的帐号欠费", Color.FromArgb(255, 255, 255), "宋体", 16, FontStyle.Regular, 0, 0, rtA, 30);
                if (pa.X > 0)
                {
                    string qianfei = CheckPic1(rtA.left, rtA.top, rtA.right, rtA.bottom);
                    WriteToFile("帐号点卡已欠费" + qianfei);
                    if (Convert.ToInt32(qianfei) > 300)
                    {
                        WriteToFile("点卡欠费已达上限,无法进入游戏");
                        return 3500;
                    }
                    KeyMouse.MouseClick(pa.X + 153, pa.Y + 84, 1, 1, 500);
                    break;
                }
                Sleep(200);
            }
            #endregion

            #region 寻找指定角色名
            for (int i = 0; i < 6; i++)
            {
                if (i != 0)
                    KeyMouse.MouseClick(680 + 100 * (i - (i / 3) * 3), 500 + 120 * (i / 3), 1, 1, 500);
                for (int a = 0; a < 15; a++)
                {
                    pa = ImageTool.FindText(m_hGameWnd, m_strSellerRole, Color.FromArgb(255, 255, 255), "宋体", 14, FontStyle.Regular, 0, 0, rtR, 30);
                    if (pa.X > 0)
                    {
                        WriteToFile("核实角色名" + pa.X.ToString() + "," + pa.Y.ToString());
                        if (!ImageTool.CheckRGB(pa.X - 4, pa.Y, pa.X - 1, pa.Y + 17, 255, 255, 255, 30))//匹配是否存在前缀
                        {
                            Sleep(300);
                            if (!ImageTool.CheckRGB(pa.X + ImageTool.checkrole + 1, pa.Y, pa.X + ImageTool.checkrole + 4, pa.Y + 17, 255, 255, 255, 30))//匹配是否存在前缀
                            {
                                KeyMouse.MouseClick(680 + 100 * (i - (i / 3) * 3), 500 + 120 * (i / 3), 1, 2, 500);
                                WriteToFile("找到指定角色名" + m_strSellerRole);
                                return 1;
                            }
                        }
                        WriteToFile("存在前缀或者后缀");
                    }
                    Sleep(200);
                }
            }

            WriteToFile("未找到指定卖家名,进默认角色");
            KeyMouse.MouseClick(680, 500, 1, 2, 500);
            return 1;
            #endregion
        }
        public int GameCarPic()
        {

            #region 判断是否进入游戏、核对区服、角色
            for (int i = 0; i < 60; i++)
            {
                Sleep(500);
                if (WinTitle().Contains(m_strArea) && WinTitle().Contains(m_strServer))
                {
                    WriteToFile("进入游戏成功，核对区服正确");
                    if (WinTitle().Contains(m_strSellerRole))
                        WriteToFile("核对用户名正确");
                    break;
                }
                if (i % 19 == 0)
                    WriteToFile("等待进入游戏核对区服");
                if (i == 59)
                {
                    WriteToFile("进入游戏超时或核对区服失败");
                    WriteToFile(WinTitle());
                    return 2260;
                }
                
            }
            User32API.SetForegroundWindow(m_hGameWnd);
            #endregion

            #region 初始化变量
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            Point pc = new Point(-1, -2);
            Point pzb = new Point(-1, -2);
            Point pzb1 = new Point(-1, -2);
            RECT rt = new RECT(775, 301, 856, 321);
            #endregion

            #region 人物属性、技能、奇经八脉 截图 召唤兽属性

            #region 等待人物窗口加载,人物属性截图
            for (int i = 0; i < 3; i++)
            {
                Sleep(2000);
                KeyMouse.SendAltAny('w');
                for (int j = 0; j < 6; j++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "人物属性.bmp", rt);
                    if (pa.X > 0)
                        break;
                    Sleep(500);
                }
                if (pa.X > 0)
                    break;
            }
            if (pa.X < 0)
            {
                WriteToFile("找不到[人物属性]");
                CaptureJpg("找不到[人物属性]");
                return 2260;
            }
            KeyMouse.MouseMove(600, 366);
            Sleep(1000);
            CaptureBmpInRect("人物属性", pa.X - 106, pa.Y - 4, pa.X + 174, pa.Y + 402, pa.X - 59, pa.Y + 29, pa.X + 61, pa.Y + 45);
            #endregion

            #region 辅助技能截图
            for (int i = 0; i < 3; i++)
            {
                MyClicklast(pa.X - 123, pa.Y + 211);
                for (int k = 0; k < 6; k++)
                {
                    pb = ImageTool.fPic(m_strPicPath + "辅助技能.bmp", rt);
                    if (pb.X > 0)
                        break;
                    Sleep(300);
                }
                if (pb.X > 0)
                    break;
            }
            if (pb.X < 0)
            {
                WriteToFile("找不到[辅助技能]");
                return 2260;
            }

            CaptureBmpInRect("辅助技能", pa.X - 106, pa.Y - 4, pa.X + 174, pa.Y + 402);
            #endregion

            #region 修炼技能截图
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 6; k++)
                {
                    MyClicklast(pa.X - 123, pa.Y + 280);
                    pb = ImageTool.fPic(m_strPicPath + "修炼技能.bmp", rt);
                    if (pb.X > 0)
                        break;
                    Sleep(300);
                }
                if (pb.X > 0)
                    break;
            }
            if (pb.X < 0)
            {
                WriteToFile("找不到[修炼技能]");
                return 2260;
            }
            CaptureBmpInRect("修炼技能", pa.X - 106, pa.Y - 4, pa.X + 174, pa.Y + 402);
            #endregion

            #region 师门技能截图
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 6; k++)
                {
                    MyClicklast(pa.X - 123, pa.Y + 140);
                    pb = ImageTool.fPic(m_strPicPath + "师门技能.bmp", rt);
                    if (pb.X > 0)
                        break;
                    Sleep(300);
                }
                if (pb.X > 0)
                    break;
            }
            if (pb.X < 0)
            {
                WriteToFile("找不到[师门技能]");
                return 2260;
            }
            CaptureBmpInRect("师门技能", pa.X - 106, pa.Y - 4, pa.X + 174, pa.Y + 402);
            #endregion

            #region 奇经八脉截图
            for (int i = 0; i < 3; i++)
            {
                MyClicklast(pb.X + 62, pb.Y + 348);
                for (int k = 0; k < 6; k++)
                {
                    pc = ImageTool.fPic(m_strPicPath + "奇经八脉.bmp", 600, 300, 690, 330);
                    if (pc.X > 0)
                        break;
                    Sleep(300);
                }
                if (pc.X > 0)
                    break;
            }
            if (pc.X < 0)
            {
                WriteToFile("找不到[奇经八脉]");
                return 2260;
            }
            CaptureBmpInRect("奇经八脉", pc.X - 277, pc.Y - 1, pc.X + 349, pc.Y + 461);
            Sleep(500);

            #endregion

            #region 等待召唤兽窗口加载,属性截图
            for (int i = 0; i < 3; i++)
            {
                KeyMouse.SendAltAny('o');
                for (int j = 0; j < 6; j++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "召唤兽属性.bmp", 453,298,553,322);
                    if (pa.X > 0)
                        break;
                    Sleep(500);
                }
                if (pa.X > 0)
                    break;
                Sleep(2000);
            }
            if (pa.X < 0)
            {
                WriteToFile("找不到[召唤兽属性]");
                CaptureJpg("找不到[召唤兽属性]");
                return 2260;
            }
            CaptureBmpInRect("召唤兽属性", pa.X - 142, pa.Y - 3, pa.X + 226, pa.Y + 448);
            #endregion
            #endregion

            #region 道具 行囊 人物  坐骑 召唤兽 装备 套装 锦衣 法宝 法宝套装

            #region 道具 行囊 坐骑 召唤兽 人物
            for (int i = 0; i < 3; i++)
            {
                KeyMouse.SendAltAny('e');
                for (int j = 0; j < 6; j++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "道具行囊.bmp", 595, 370, 686, 394);
                    if (pa.X > 0)
                        break;
                    Sleep(500);
                }
                if (pa.X > 0)
                    break;
                Sleep(2000);
            }
            if (pa.X < 0)
            {
                WriteToFile("找不到[道具行囊]");
                CaptureJpg("找不到[道具行囊]");
                return 2260;
            }
            CaptureBmpInRect("人物总览", pa.X - 230, pa.Y + 20, pa.X + 18, pa.Y + 325);
            MyClicklast(pa.X - 140, pa.Y + 35);
            Sleep(1000);
            CaptureBmpInRect("召唤兽总览", pa.X - 230, pa.Y + 20, pa.X + 18, pa.Y + 325);
            MyClicklast(pa.X - 78, pa.Y + 35);
            Sleep(1000);
            CaptureBmpInRect("坐骑总览", pa.X - 230, pa.Y + 20, pa.X + 18, pa.Y + 325);
            MyClicklast(pa.X + 69, pa.Y + 34);
            Sleep(1000);
            CaptureBmpInRect("道具总览", pa.X + 16, pa.Y + 20, pa.X + 300, pa.Y + 325);
            MyClicklast(pa.X + 150, pa.Y + 34);
            Sleep(1000);
            CaptureBmpInRect("行囊总览", pa.X + 16, pa.Y + 20, pa.X + 300, pa.Y + 325);
            CaptureJpg("行囊总览");

            #endregion

            #region 装备截图 装备套装

            //------------装备-----------
            MyClicklast(pa.X -206 , pa.Y + 34);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    MyClicklast1(405 + i * 190, 514 + 61 * j);
                    for(int z=0;z<10;z++)
                    {
                        pzb = ImageTool.fPic(m_strPicPath + "装备上标.bmp", 280, 250, 1000, 800, 30);
                        pzb1 = ImageTool.fPic(m_strPicPath + "装备下标.bmp", pzb.X > 0 ? pzb.X : 280, pzb.Y > 0 ? pzb.Y : 250, 1000, 800, 30);
                        if(pzb.X>0&&pzb1.X>0)
                        {
                            CaptureBmpInRect("装备" + (i * 3 + j + 1).ToString(), pzb.X+1, pzb.Y+1, pzb1.X + 19, pzb1.Y + 64);
                            break;
                        } 
                        Sleep(500);
                    }
                    if(pzb.X<0&&pzb1.X<0)
                    {
                        WriteToFile((i*3+j+1).ToString()+"号无装备");
                        CaptureJpg((i * 3 + j + 1).ToString() + "号无装备");
                    }
                    else if (pzb.X < 0 && pzb1.X > 0)
                    {
                        WriteToFile("找不到"+(i*3+j+1).ToString()+"号下标");
                        CaptureJpg("找不到" + (i * 3 + j + 1).ToString() + "号下标");
                    }
                    else if (pzb.X > 0 && pzb1.X < 0)
                    {
                        WriteToFile("找不到" + (i * 3 + j + 1).ToString() + "号上标");
                        CaptureJpg("找不到" + (i * 3 + j + 1).ToString() + "号上标");
                    }
                }
            }
            //---------------------------

            MyClicklast(pa.X - 63, pa.Y + 279);
            Sleep(1000);
            CaptureBmpInRect("装备套装", pa.X - 233, pa.Y - 72, pa.X + 46, pa.Y + 261);
            MyClicklast(pa.X - 63, pa.Y + 279);

            #endregion

            #region 锦衣截图
            for (int i = 0; i < 3; i++)
            {
                MyClicklast(pa.X - 142, pa.Y + 307);
                for (int k = 0; k < 6; k++)
                {
                    pb = ImageTool.fPic(m_strPicPath + "锦衣.bmp", 300, 360, 681, 408);
                    if (pb.X > 0)
                        break;
                    Sleep(500);
                }
                if (pb.X > 0)
                    break;
            }
            if (pb.X < 0)
            {
                WriteToFile("找不到[锦衣]");
                return 2260;
            }
            CaptureBmpInRect("锦衣", pb.X - 259, pb.Y - 5, pb.X + 295, pb.Y + 331);
            Sleep(500);
            #endregion 

            #region 法宝截图
            pa = new Point(-1, -2);
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    Sleep(1000);
                    KeyMouse.SendAltAny('e');
                    Sleep(1000);
                    KeyMouse.SendAltAny('e');
                }
                if (pa.X < 0)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        pa = ImageTool.fPic(m_strPicPath + "道具行囊.bmp", 595, 373, 686, 394);
                        if (pa.X > 0)
                            break;
                        Sleep(500);
                    }
                    if (pa.X < 0)
                        continue;
                }
                MyClicklast(pa.X - 83, pa.Y + 307);
                for (int k = 0; k < 6; k++)
                {
                    pb = ImageTool.fPic(m_strPicPath + "法宝.bmp", 618, 379, 660, 405);
                    if (pb.X > 0)
                        break;
                    Sleep(300);
                }
                if (pb.X > 0)
                    break;
            }
            if (pa.X < 0)
            {
                WriteToFile("找不到[道具行囊]");
                return 2260;
            }
            if (pb.X < 0)
            {
                WriteToFile("找不到[法宝]");
                return 2260;
            }
            CaptureBmpInRect("法宝", pb.X - 253, pb.Y - 86, pb.X + 289, pb.Y + 314);
            MyClicklast(pb.X - 34, pb.Y + 36);
            Sleep(500);
            #endregion

            #endregion

            #region 成长历程 成就集成

            #region 成长历程
            for (int i = 0; i < 3; i++)
            {
                Sleep(2000);
                KeyMouse.SendAltAny('n');
                for (int j = 0; j < 6; j++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "功成名就.bmp", 550,300,700,360);
                    if (pa.X > 0)
                        break;
                    Sleep(500);
                }
                if (pa.X > 0)
                    break;
            }
            if (pa.X < 0)
            {
                WriteToFile("找不到[功成名就]");
                CaptureJpg("找不到[功成名就]");
                return 2260;
            }
            CaptureBmpInRect("我的成就", pa.X - 249, pa.Y - 5, pa.X + 317, pa.Y + 437);
            Sleep(500);
            #endregion

            #region 成就集成
            MyClicklast(pa.X + 312, pa.Y + 166);
            Sleep(1000);
            MyClicklast(pa.X - 167, pa.Y + 53);//完成
            Sleep(1000);
            MyClicklast(pa.X - 161, pa.Y + 82);//服务器成就
            Sleep(2000);
            CaptureBmpInRect("服务器成就", pa.X - 249, pa.Y - 5, pa.X + 317, pa.Y + 437);
            MyClicklast(pa.X - 74, pa.Y + 319);//服务器成就下拉
            Sleep(2000);
            CaptureBmpInRect("服务器成就1", pa.X - 249, pa.Y - 5, pa.X + 317, pa.Y + 437);
            #endregion

            #endregion

            #region 宠物 坐骑属性 坐骑技能

            #region 宠物
            for (int i = 0; i < 3; i++)
            {
                Sleep(2000);
                KeyMouse.SendAltAny('p');
                for (int j = 0; j < 6; j++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "宠物.bmp", 548, 324, 631, 348);
                    if (pa.X > 0)
                        break;
                    Sleep(500);
                }
                if (pa.X > 0)
                    break;
            }
            if (pa.X < 0)
            {
                WriteToFile("找不到[宠物]");
                CaptureJpg("找不到[宠物]");
                return 2260;
            }
            CaptureBmpInRect("宠物", pa.X - 142, pa.Y - 5, pa.X + 176, pa.Y + 186);
            Sleep(500);
            #endregion

            #region 坐骑属性 坐骑技能
            pa = new Point(-1, -2);
            for (int i = 0; i < 3; i++)
            {
                if (i == 0)
                {
                    Sleep(1000);
                    KeyMouse.SendAltAny('e');
                    Sleep(1000);
                    KeyMouse.SendAltAny('e');
                }
                if (pa.X < 0)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        pa = ImageTool.fPic(m_strPicPath + "道具行囊.bmp", 595, 373, 686, 394);
                        if (pa.X > 0)
                            break;
                        Sleep(500);
                    }
                    if (pa.X < 0)
                        continue;
                }
                MyClicklast(pa.X - 78, pa.Y + 35);
                Sleep(1000);
                MyClicklast(pa.X - 146, pa.Y + 226);//坐骑属性
                CaptureJpg("坐骑属性");              
            }

            Sleep(500);
            #endregion

            #endregion

            #region 拼图
            WriteToFile("开始拼图");
            PinTu("人物属性,师门技能,辅助技能,换行,修炼技能,召唤兽属性","MHXY1",false);
            PinTu("人物总览,召唤兽总览,坐骑总览", "MHXY2", false);
            PinTu("道具总览,行囊总览", "MHXY2", false);
            PinTu("装备1,装备2,换行,装备3,装备4,换行,装备5,装备6", "MHXY3", false);
            PinTu("装备套装", "MHXY3", false);
            PinTu("锦衣", "MHXY4", false);
            PinTu("法宝", "MHXY4", false);
            PinTu("我的成就", "MHXY5", false);
            PinTu("服务器成就", "MHXY5", false);
            PinTu("服务器成就1", "MHXY5", false);
            PinTu("宠物", "MHXY6", false);
            PinTu("坐骑属性,坐骑技能", "MHXY6", false);
            PinTu("奇经八脉", "MHXY7", false);
            #endregion

            return 1000;

        }
        /// <summary>
        /// 关闭游戏
        /// </summary>
        public void CloseGames()
        {
            Game.RunCmd("taskkill /im  my.exe /F");
            Game.RunCmd("taskkill /im  mhtab.exe /F");
            Game.RunCmd("taskkill /im  mhmain.exe /F");
            Game.RunCmd("taskkill /im  xyqsvc.exe /F");
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
#if DEBUG
#else
            System.Diagnostics.Process.Start("shutdown", @"/r");
#endif
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
            return "";
        }
        //获取数据，发送服务器
        public string AutoVerifyb()
        {
            return "";
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
            url = string.Format("http://gtr.5173.com:7080/RobotCallback.asmx/GetByRobotReviewed{0}", postData);
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
        //返回验证结果,0为错误,1为正确
        public void VerifyResult(int isTrue)
        {
            string strHTML = "";
            String postData = string.Format("orderNo={0}&IsTrue={1}", OrdNo, isTrue);
            try
            {
                strHTML = PostUrlData("http://192.168.36.245/autotalk/service.asmx/ResultAnswer2", postData);
            }
            catch (Exception e)
            {
                WriteToFile(e.Message);
            }
        }
        /// <summary>
        ///答题是否正确
        /// </summary>
        /// <param name="isTrue">正确与否：0正确；1错误</param>
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
            catch
            {
                LocalPicPath = "C:\\梦幻西游2账号截图\\";
                string tmp = SetPicPathBmp(LocalPicPath, OrdNo, picName);
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
        public void CaptureJpg(bool vnc)
        {
            if (true)
            {
                //如果路径有异常则将路径改为 "Z:\\jiaoben\\"
                try
                {
                    LocalPicPath = "\\\\192.168.92.156\\vnc\\lol\\";
                    string tmp = SetPicPathBmp(LocalPicPath, OrdNo, "");
                    Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
                    User32API.MakeSureDirectoryPathExists(LocalPicPath);
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
                catch (Exception e)
                {
                    WriteToFile(e.ToString());
                    WriteToFile("连接达到最大");
                }
            }
            else
                return;
        }
        public void CaptureJpg(bool vnc, string PathName)
        {
            if (true)
            {

                try
                {
                    LocalPicPath = "\\\\192.168.92.156\\vnc\\lol\\" + PathName + "\\";
                    string tmp = SetPicPathBmp(LocalPicPath, OrdNo, "");
                    Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
                    User32API.MakeSureDirectoryPathExists(LocalPicPath);
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
                catch (Exception e)
                {
                    WriteToFile(e.ToString());
                    WriteToFile("连接达到最大");
                }
            }
            else
                return;
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
        //图片缩放
        public bool jpgResize(string strFile, string strNewFile, int intWidth, int intHeight, bool IsDeleteStrFile)
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
                if (IsDeleteStrFile)
                {
                    File.Delete(strFile);
                }

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
        public void CaptureBmpInRect(string strPicName, int left, int top, int right, int bottom, int waterleft, int watertop, int waterright, int waterbottom)
        {
            strPicName += ".bmp";
            Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
            User32API.MakeSureDirectoryPathExists(m_strCapturePath);
            RECT rect = new RECT(left, top, right, bottom);
            Game.CaptureBmp(bm, rect, m_strCapturePath + strPicName);
            if (waterleft > 0)
            {
                RECT rt = new RECT(waterleft - left, watertop - top, waterright - left, waterbottom - top);
                WaterMark(m_strCapturePath + strPicName, rt);
            }
            return;
        }
        /// <summary>
        /// bmp截图带水印遮掩
        /// </summary>
        /// <param name="bm">bmp图片流</param>
        /// <param name="strPicName">截图名称</param>
        /// <param name="left">边界：左</param>
        /// <param name="top">边界：上</param>
        /// <param name="right">边界：右</param>
        /// <param name="bottom">边界：下</param>
        /// <param name="waterleft">水印边界</param>
        /// <param name="watertop">水印边界</param>
        /// <param name="waterright">水印边界</param>
        /// <param name="waterbottom">水印边界</param>
        /// <param name="waterleft1">水印边界1</param>
        /// <param name="watertop1">水印边界1</param>
        /// <param name="waterright1">水印边界1</param>
        /// <param name="waterbottom1">水印边界1</param>
        public void CaptureBmpInRect(string strPicName, int left, int top, int right, int bottom, int waterleft, int watertop, int waterright, int waterbottom, int waterleft1, int watertop1, int waterright1, int waterbottom1)
        {
            strPicName += ".bmp";
            Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
            User32API.MakeSureDirectoryPathExists(m_strCapturePath);
            RECT rect = new RECT(left, top, right, bottom);
            Game.CaptureBmp(bm, rect, m_strCapturePath + strPicName);
            if (waterleft > 0)
            {
                RECT rt = new RECT(waterleft - left, watertop - top, waterright - left, waterbottom - top);
                WaterMark(m_strCapturePath + strPicName, rt);
            }
            if (waterleft1 > 0)
            {
                RECT rt1 = new RECT(waterleft1 - left, watertop1 - top, waterright1 - left, waterbottom1 - top);
                WaterMark(m_strCapturePath + strPicName, rt1);
            }
            return;
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
                        tData[num + 2] = 20;
                        tData[num + 1] = 20;
                        tData[num + 0] = 20;
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
        /// 
        public int PinTu(string strAllPic, string strPicID, bool bVerbical)
        {
            string[] arrPicName;
            int num = SplitString(strAllPic, ",", out arrPicName);

            for (int i = 0; i < num + 1; i++)
            {
                if (arrPicName[i] == "换行")
                {
                    CreatePlate("换行", "", nZHPicWidth);
                    continue;
                }

                CreatePlate(m_strCapturePath + arrPicName[i], "", nZHPicWidth);
            }
            string strPic = m_strCapturePath + "模板.bmp";
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
                if (!File.Exists(m_strCapturePath + arrPicName[i] + ".bmp"))
                    continue;
                Bitmap sbmp = new Bitmap(m_strCapturePath + arrPicName[i] + ".bmp", true);
                //ImageTool.CreatBmpFromByte(bbmp, sbmp, ref x, ref y, ref z, "");
                ImageTool.BmpInsert(bbmp, sbmp, ref x, ref y, ref z, "");
                Sleep(500);
                try
                {
                    sbmp.Dispose();
                }
                catch (Exception e)
                {
                    WriteToFile(e.ToString());
                    WriteToFile(arrPicName[i]);
                }

                //删除小图
                if (Program.bRelease)
                    File.Delete(m_strCapturePath + arrPicName[i] + ".bmp");


            }
            string PicName = SetPicPath(m_strCapturePath, strPicID);
            if (bVerbical)
                //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "").Save(m_strCapturePath + strPicID + ".bmp", ImageFormat.Bmp);
                ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "").Save(m_strCapturePath + strPicID + ".bmp", ImageFormat.Bmp);
            else
            {
                //if (OrdNo.IndexOf("MZH") == 0 || OrdNo == "测试订单")
                //{
                //    Bitmap sbmp = new Bitmap(m_strPicPath + "水印.bmp", true);
                //    ImageTool.BmpInsert(bbmp, sbmp, ref x, ref y, ref z, "水印");
                //    sbmp.Dispose();
                //}

                string strJpg = PicName.Replace(".bmp", ".jpg");
                //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "").Save(strJpg, ImageFormat.Jpeg);
                ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "").Save(strJpg, ImageFormat.Jpeg);
                picNum++;
            }
            bbmp.Dispose();
            File.Delete(m_strCapturePath + "模板.bmp");
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
        /// <summary>
        /// 获取窗口标题
        /// </summary>
        /// <returns></returns>
        public string WinTitle()
        {
            m_hGameWnd = User32API.FindWindow("MHXYMainFrame", null);//根据窗口类名获取句柄
            string winTitle = User32API.GetWindowText(m_hGameWnd).Trim();//获取窗口标题
            return winTitle;
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
                string strJpg = PicName.Replace(".bmp", ".jpg");
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
            File.Delete(picPath + "模板.bmp");
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
                                picResizePng(dirPath + file.Name, dirPath + arrName + ".png", image.Width, image.Height);
                            }
                            file.Delete();
                        }
                        else if (file.Name.Contains("png"))
                            arrName = file.Name.Replace(".png", "");

                        list.Add(arrName); //给list赋值                    
                    }
                    list.Sort();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return list.Count;
        }
        /// <summary>
        /// 遍历需要上传的jpg格式的图片带下划线。
        /// </summary>
        public void TraversalPicJpg()
        {
            if (Directory.Exists(m_strCapturePath))
            {
                DirectoryInfo Dir = new DirectoryInfo(m_strCapturePath);
                try
                {
                    foreach (FileInfo file in Dir.GetFiles("*.jpg"))//查找子目录 
                    {
                        string arrName = string.Empty;
                        if (file.Name.Contains("jpg") && file.Name.Contains("_"))
                        {
                            using (FileStream fs = new FileStream(m_strCapturePath + file.Name, FileMode.Open, FileAccess.Read))
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                                WriteToFile("图片名称：" + file.Name + ",图片宽度：" + image.Width + ",图片高度：" + image.Height);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
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
        public void fun1()
        {
            while (true)
            {
                List<int> listhwnd = myapp.EnumWindow("", "#32770");
                foreach (int hwdl in listhwnd)
                {
                    IntPtr hwdl2 = (IntPtr)hwdl;
                    string winTitle = User32API.GetWindowText(hwdl2).Trim();//获取窗口标题
                    //WriteToFile(winTitle);
                    if (winTitle.Contains("网络错误"))
                    {

                        if (myapp.FindControl(hwdl2, null, "网络错误，请重新登录").Count > 0)
                        {
                            WriteToFile("网络连接中断");
                            zhongduan = true;
                            jiankong.Abort();
                        }
                    }
                }
                Sleep(1000);
            }
        }
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
                //ImageTool.FindText(m_hGameWnd, "你的帐号欠费300点", Color.FromArgb(255, 255, 255), "宋体", 16, FontStyle.Regular, 0, 0, rtA, 30);
                at = ImageTool.FindText(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "宋体", 16, FontStyle.Regular, 0, 0, rt, 30);
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
                    //at = ImageTool.FindText1(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "宋体", 12, FontType, 0, 0, rt, 38);
                    at = ImageTool.FindText(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "宋体", 16, FontStyle.Regular, 0, 0, rt, 30);
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
        /// 切换窗口截
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="time"></param>
        public void MyMouserClick(int x, int y, int time)
        {
            for (int i = 0; i < 3; i++)
            {
                m_hChangeWnd = User32API.FindWindow(null, "账号GTR");
                if (m_hChangeWnd != IntPtr.Zero)
                {
                    //User32API.ShowWindow(m_hChangeWnd, ShowWindowCmd.SW_RESTORE);
                    //User32API.SetForegroundWindow(m_hChangeWnd);
                    //显示窗口
                    User32API.ShowWindow(m_hChangeWnd, ShowWindowCmd.SW_NORMAL);

                    //前端显示
                    User32API.SetForegroundWindow(m_hChangeWnd);
                }
                Sleep(200);
            }
            if (m_hChangeWnd == IntPtr.Zero)
            {
                WriteToFile("RC句柄不存在");
            }
            Sleep(1000);
            KeyMouse.MouseMove(x, y);
            Sleep(200);
            User32API.ShowWindow(m_hChangeWnd, ShowWindowCmd.SW_HIDE);
            Sleep(200);
            //KeyMouse.PressMouseKey();
            KeyMouse.PressMousekeyDouble(2);
            Sleep(100);
            //KeyMouse.MouseMove(600, 366);
            Sleep(time);
        }
        /// <summary>
        /// 模仿人工移动点击
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tb"></param>
        /// <param name="tc"></param>
        /// <param name="time"></param>
        public void MyClick(int x, int y, uint tb, uint tc, int time)
        {
            for (int i = 0; i < 3; i++)
            {
                m_hChangeWnd = User32API.FindWindow(null, "账号GTR");
                if (m_hChangeWnd != IntPtr.Zero)
                {
                    //User32API.ShowWindow(m_hChangeWnd, ShowWindowCmd.SW_RESTORE);
                    //User32API.SetForegroundWindow(m_hChangeWnd);
                    //显示窗口
                    User32API.ShowWindow(m_hChangeWnd, ShowWindowCmd.SW_NORMAL);

                    //前端显示
                    User32API.SetForegroundWindow(m_hChangeWnd);
                }
                Sleep(200);
            }
            if (m_hChangeWnd == IntPtr.Zero)
            {
                WriteToFile("RC句柄不存在");
            }
            Sleep(100);
            Point screenPoint = Point.Empty;
            int ptX = 0;
            int ptY = 0;
            int IntMoveX = 0;
            int IntMoveY = 0;
            for (int i = 0; i < 1000; i++)
            {
                screenPoint = Control.MousePosition;
                ptX = x - screenPoint.X;
                ptY = y - screenPoint.Y;
                //IntMoveX = ptX > 0 ? (IntMoveX = Math.Abs(ptX) / 2 > 5 ? 5 : ptX / 2) : (IntMoveX =-(Math.Abs(ptX) / 2 > 5 ? 5 : ptX / 2));
                //IntMoveY = ptY > 0 ? (IntMoveY = Math.Abs(ptY) / 2 > 5 ? 5 : ptY / 2 ): (IntMoveY =-(Math.Abs(ptY) / 2 > 5 ? 5 : ptY / 2));
                IntMoveX = ptX / 2;
                IntMoveY = ptY / 2;
                WriteToFile("鼠标坐标X:" + screenPoint.X.ToString() + "Y:" + screenPoint.Y.ToString() + "目标坐标X" + x.ToString() + "Y:" + y.ToString());
                if (Math.Sqrt(Math.Pow(Math.Abs(ptX), 2) + Math.Pow(Math.Abs(ptY), 2)) < 3)
                {
                    //KeyMouse.MouseMove(screenPoint.X, y);            
                    Sleep(300);
                    //User32API.ShowWindow(m_hChangeWnd, ShowWindowCmd.SW_HIDE);
                    screenPoint = Control.MousePosition;
                    ptX = x - screenPoint.X;
                    ptY = y - screenPoint.Y;
                    if (Math.Sqrt(Math.Pow(Math.Abs(ptX), 2) + Math.Pow(Math.Abs(ptY), 2)) > 4)
                    {
                        WriteToFile("鼠标被弹开");
                        continue;
                    }
                    WriteToFile("移到了");
                    KeyMouse.PressMouseKey();
                    KeyMouse.PressMousekeyDouble(2);
                    Sleep(3000);
                    WriteToFile("点击");

                    break;
                }
                myapp.mouse_event(1, IntMoveX, IntMoveY, 0, 0);
                //myapp.mouse_event(1, 0, IntMove, 0, 0);
                Sleep(200 + 50 * i);
            }

        }
        /// <summary>
        /// 修正点击
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="tb"></param>
        /// <param name="tc"></param>
        /// <param name="time"></param>
        public void MyClick2(int x, int y, uint tb, uint tc, int time)
        {
            int ptX = 0;
            int ptY = 0;
            Point screenPoint = Point.Empty;
            for (int i = 0; i < 20; i++)
            {
                KeyMouse.MouseMove(x, y);
                screenPoint = Control.MousePosition;//鼠标相对于屏幕左上角的坐标
                ptX = x - screenPoint.X;
                ptY = y - screenPoint.Y;
                if (Math.Abs(screenPoint.X - x) < 5 && Math.Abs(screenPoint.Y - y) < 5)
                    break;
            }
            if (Math.Abs(screenPoint.X - x) < 5 && Math.Abs(screenPoint.Y - y) < 5)
                KeyMouse.MouseClick(x, y, tb, tc, time);
            else
                WriteToFile("移动鼠标失败");
        }
        public static void WenDaoMouseMove(IntPtr hwnd, int x, int y)
        {
            IntPtr desk = User32API.GetDesktopWindow();
            User32API.SetForegroundWindow(desk);
            //User32API.ShowWindow(hwnd,ShowWindowCmd.SW_MINIMIZE);
            Sleep(500);
            KeyMouse.MouseMove(x, y);
            Sleep(500);
            User32API.SetForegroundWindow(hwnd);
            Sleep(500);
        }
        public void MyClick3(int x, int y, uint tb, uint tc, int time)
        {
            Sleep(100);
            Point screenPoint = Point.Empty;
            int ptX = 0;
            int ptY = 0;
            int IntMove = 5;
            for (int i = 0; i < 1000; i++)
            {
                screenPoint = Control.MousePosition;
                ptX = x - screenPoint.X;
                IntMove = ptX > 0 ? 5 : -5;
                WriteToFile("鼠标X:" + screenPoint.X.ToString() + "目标X" + x.ToString());
                if (Math.Abs(ptX) < 5)
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        screenPoint = Control.MousePosition;
                        ptY = y - screenPoint.Y;
                        IntMove = ptY > 0 ? 5 : -5;
                        WriteToFile("鼠标Y:" + screenPoint.Y.ToString() + "目标Y" + y.ToString());
                        if (Math.Abs(ptY) < 5)
                        {
                            return;
                        }
                        myapp.mouse_event(1, 0, IntMove, 0, 0);
                        Sleep(200);
                    }

                }
                myapp.mouse_event(1, IntMove, 0, 0, 0);

                Sleep(200);
            }

        }
        public bool MyClicklast(int x, int y)
        {
            KeyMouse.MouseMove(x, y);
            DateTime datest = DateTime.Now;
            TimeSpan ts = DateTime.Now - datest;
            int mx = 5;
            int my = 5;
            RECT REC = new RECT(320, 298, 961, 778);
            Point pt = new Point(0, 0);
            int qq = 0, pp = 0;
            //-----------测试--------
            //REC = new RECT(0, 0, 650, 537);
            //-----------------------
            while (true)
            {
                pt = ImageTool.fPic(m_strPicPath + "鼠标.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "鼠标1.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                else if (pt.X < 0)
                    myapp.mouse_event(1, -5, -5, 0, 0);
                if (pt.Y > 0)
                {
                    qq++;
                    int yy = y - pt.Y;
                    if (Math.Abs(yy) > 20)
                        my = yy / 3;
                    else if (Math.Abs(yy) > 3)
                        my = yy / 2;
                    else if (Math.Abs(yy) <= 3)
                        break;
                    myapp.mouse_event(1, 0, my, 0, 0);
                    if (qq % 50 == 0)
                        WriteToFile("正在移动鼠标");
                    Console.WriteLine("{0},{1}", 0, my);
                }
                Sleep(100);
                ts = DateTime.Now - datest;
                if ((int)ts.TotalSeconds % 20 == 0)
                    WriteToFile("查找鼠标位置");
                if (ts.TotalSeconds > 120)
                {
                    WriteToFile("超时");
                    return false;
                }
            }

            while (true)
            {
                pt = ImageTool.fPic(m_strPicPath + "鼠标.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "鼠标1.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                else if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "鼠标3.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                else if (pt.X < 0)
                    myapp.mouse_event(1, -5, -5, 0, 0);
                if (pt.X > 0)
                {
                    pp++;
                    int xx = x - pt.X;
                    if (Math.Abs(xx) > 20)
                        mx = xx / 3;
                    else if (Math.Abs(xx) > 3)
                        mx = xx / 2;
                    else if (Math.Abs(xx) <= 3)
                    {
                        KeyMouse.PressMouseKey();
                        return true;
                    }

                    myapp.mouse_event(1, mx, 0, 0, 0);
                    if (pp % 50 == 0)
                        WriteToFile("正在移动鼠标");
                    Console.WriteLine("{0},{1}", mx, 0);
                }
                Sleep(100);
                ts = DateTime.Now - datest;
                if ((int)ts.TotalSeconds % 20 == 0)
                    WriteToFile("查找鼠标位置");
                if (ts.TotalSeconds > 120)
                {
                    WriteToFile("超时");
                    return false;
                }
            }
            return true;

        }
        public bool MyClicklast1(int x, int y)
        {
            KeyMouse.MouseMove(x, y);
            DateTime datest = DateTime.Now;
            TimeSpan ts = DateTime.Now - datest;
            int mx = 5;
            int my = 5;
            RECT REC = new RECT(320, 298, 961, 778);
            Point pt = new Point(0, 0);
            int qq = 0, pp = 0;
            //-----------测试--------
            //REC = new RECT(0, 0, 650, 537);
            //-----------------------
            while (true)
            {
                pt = ImageTool.fPic(m_strPicPath + "鼠标.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "鼠标1.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                if (pt.X < 0)
                    myapp.mouse_event(1, -5, 5, 0, 0);
                if (pt.Y > 0)
                {
                    qq++;
                    int yy = y - pt.Y;
                    if (Math.Abs(yy) > 20)
                        my = yy / 3;
                    else if (Math.Abs(yy) > 3)
                        my = yy / 2;
                    else if (Math.Abs(yy) <= 3)
                        break;
                    myapp.mouse_event(1, 0, my, 0, 0);
                    if (qq % 50 == 0)
                        WriteToFile("正在移动鼠标");
                    Console.WriteLine("{0},{1}", 0, my);
                }
                Sleep(100);
                ts = DateTime.Now - datest;
                if ((int)ts.TotalSeconds % 20 == 0)
                    WriteToFile("查找鼠标位置");
                if (ts.TotalSeconds > 120)
                {
                    WriteToFile("超时");
                    return false;
                }
            }

            while (true)
            {
                pt = ImageTool.fPic(m_strPicPath + "鼠标.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "鼠标1.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                else if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "鼠标3.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                else if (pt.X < 0)
                    myapp.mouse_event(1, -5, -5, 0, 0);
                if (pt.X > 0)
                {
                    pp++;
                    int xx = x - pt.X;
                    if (Math.Abs(xx) > 20)
                        mx = xx / 3;
                    else if (Math.Abs(xx) > 3)
                        mx = xx / 2;
                    else if (Math.Abs(xx) <= 3)
                    {
                        return true;
                    }

                    myapp.mouse_event(1, mx, 0, 0, 0);
                    if (pp % 50 == 0)
                        WriteToFile("正在移动鼠标");
                    Console.WriteLine("{0},{1}", mx, 0);
                }
                Sleep(100);
                ts = DateTime.Now - datest;
                if ((int)ts.TotalSeconds % 20 == 0)
                    WriteToFile("查找鼠标位置");
                if (ts.TotalSeconds > 120)
                {
                    WriteToFile("超时");
                    return false;
                }
            }
            return true;

        }
        public void CreatWaterMark(string picnameandpath, Bitmap bit)
        {
            #region 滤色处理
            Bitmap picbmp = bit;
            Rectangle srcRect = new Rectangle(0, 0, picbmp.Width >= 1000 ? 1000 : picbmp.Width, picbmp.Height >= 1000 ? 1000 : picbmp.Height);

            Bitmap mybm = new Bitmap(m_strPicPath + "背景.bmp");
            int Width = mybm.Width;
            int height = mybm.Width;
            Bitmap bm = new Bitmap(Width, height);//初始化一个记录滤色效果的图片对象
            Color pixel;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < height; y++)
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
            Bitmap bmp2 = new Bitmap(picbmp.Width >= 1000 ? 1000 : picbmp.Width, picbmp.Height >= 1000 ? 1000 : picbmp.Height, bigBData.Stride, PixelFormat.Format32bppArgb, bigBData.Scan0);

            Sleep(50);

            Graphics graph1 = Graphics.FromImage(picbmp);//picbtm
            graph1.DrawImage(bmp2, 0, 0);
            picbmp.Save(picnameandpath, ImageFormat.Jpeg);

            graph1.Dispose();
            bmp2.Dispose();
            bm.UnlockBits(bigBData);
            bm.Dispose();
            mybm.Dispose();
            picbmp.Dispose();


            Sleep(100);
            //return picbtm;
            #endregion
        }
    }
}
