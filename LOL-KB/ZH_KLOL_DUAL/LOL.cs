using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging; //ImageFormat.Jpeg
using System.Web;
//using System.Web.Security;
using System.Net;
using System.Net.Sockets;
using System.Management;
using System.Text.RegularExpressions;//正则
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Principal;
using System.Web.Script.Serialization;
//using System.Threading.Tasks;
namespace GTR
{
    class LOL
    {
        #region 初始化通信变量
        //订单数据
        public static string m_strOrderData;
        //udpsockets
        public static udpSockets udpdd;
        //验证码返回
        public static string mouseP = "";
        //验证码回答次数
        public int yzmTimes = 0;
        //udp端口
        public static int m_UDPPORT = 6801;
        //脚本端口
        public static int the_nRC2Port = 0;
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
        //主站是否动态属性标志
        public string IsNeedRecognition;
        static int PicNum = 1;
        static string strLastPicID = "";
        string m_strLastName;
        int m_nPicNum;
        static int picNum = 0;
        public int time = 0;
        public int QWE = 0;
        //拥有符文页数
        public int RenusNum;
        //拥有的英雄数量
        public Int64 intHero = 0;
        //拥有的皮肤数量
        public Int64 intSkin = 0;
        //拥有的金币
        public Int32 intCoin = 0;
        public Int32 intLevel = 0;
        public bool IsNewGame = false;
        //输出截图目录
        public bool IsSendCapPath = false;
        string[] FileArr;
        //验证码（jpg）
        string mousea;
        //窗口句柄
        public static IntPtr m_hGameWnd;
        //程序所在路径
        private string m_strProgPath = System.Windows.Forms.Application.StartupPath;
        //匹配图片路径
        public static string m_strPicPath = System.Windows.Forms.Application.StartupPath + @"\英雄联盟\";
        //异常截图保存路径
        public static string LocalPicPath = "D:\\LOL韩服\\";
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
        static List<string> locallist = new List<string>();//先定义list集合
        static List<string> pngskin = new List<string>();//先定义list集合
        #endregion
        //主函数入口
        public void StartToWork()
        {

            #region  IP 截图路径 拨号
            m_strCapturePath = "E:\\拼图";
            if (m_strCapturePath == "")
            {
                WriteToFile("找不到截图存放、异常路径。");
            }
            WriteToFile(m_strCapturePath);
            Sleep(1000);
            User32API.WinExec("rasphone -h 宽带连接", 2); //断
            Sleep(3000);
            if (LocalConnectionStatus())
            {
                User32API.WinExec("rasphone -h 宽带连接", 2); //断
                Sleep(3000);
            }
            User32API.WinExec("rasphone -d 宽带连接", 2); //连
            for (int i = 0; i < 8; i++)
            {
                if (User32API.FindWindow(null, "正在连接到 宽带连接...") != IntPtr.Zero)
                {
                    Sleep(3000);
                    WriteToFile("宽带连接中...");
                }
                if (User32API.FindWindow(null, "连接到 宽带连接 时出错") != IntPtr.Zero)
                {
                    WriteToFile("错误拨号失败,停止做单");
                    Game.tskill("rasphone");
                    Sleep(500);
                    if (User32API.FindWindow(null, "连接到 宽带连接 时出错") == IntPtr.Zero)
                        RestartPC();
                }
                if (User32API.FindWindow(null, "网络连接") != IntPtr.Zero)
                {
                    WriteToFile("拨号失败，停止做单");
                    Game.tskill("rasphone");
                    RestartPC();
                    return;
                }
            }
            WriteToFile("拨号宽带连接成功");

            #endregion

            #region 主要功能块
            try
            {
                DeleteFolder(LocalPicPath, 7);
                DeleteFiles(m_strProgPath + @"\champion\");
                DeleteFiles(m_strProgPath + @"\SkinTemp\");
                Status = GameProc();
                if (Status > 1000)
                {
                    Point pt = new Point();
                    CaptureJpg("订单失败");
                }
            }
            catch (Exception ess)
            {
                WriteToFile(ess.ToString());
            }
            #endregion

            #region 处理返回码和异常截图
            if (MZH && Status != 1000)
            {
                if (Status > 2000 && Status < 3000)
                    Status += 2000;
            }
            string tmp;
            if (Status == 1000)
            {
                //picNum = 3;
                tmp = string.Format("截图成功,共{0}张\r\n", picNum);
                WriteToFile(tmp);
            }
            if (Status > 1000)
            {
                FileRW.DeleteTmpFiles(m_strCapturePath + OrdNo);
            }
            tmp = string.Format("移交状态={0}\r\n", Status);
            WriteToFile(tmp);
            tmp = string.Format("FStatus={0}\r\n", Status);
            #endregion

            #region①记录做单情况 ②连续失败5单重启电脑（频繁重启导致订单超时）
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

            #region 发送UDP数据
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
            #endregion

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

            #region 账号中文审核 M站审核
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

            //if (m_GameId == "100")
            //{
            //    try
            //    {
            //        int ssr = ReadQQ();
            //        if (ssr > 1000)
            //        {
            //            return ssr;
            //        }
            //    }
            //    //if (IDNumber.Length == 0 || QQLevel.Length == 0 || BoundMobilePhoneNumber.Length == 0 || emailband.Length == 0)
            //    //{
            //    //    WriteToFile("获取字段信息失败");
            //    //    return 2222;
            //    //}
            //    catch (Exception e)
            //    {
            //        WriteToFile(e.ToString());
            //    }
            //}
            #endregion

            AppInit();//IP地址 版本号
            for (int i = 0; i < 2; i++)
            {
                #region 关闭进程
                CloseGames();
                m_hGameWnd = User32API.GetDesktopWindow();
                for (int j = 0; j < 40; j++)
                {
                    int a = 5;
                    a = a * j;
                    Sleep(30);
                    KeyMouse.MouseMove(m_hGameWnd, 926 + a, 1007);
                }
                #endregion

                Status = GetHeroAndSkin();
                if (Status <= 1000)
                {
                    try
                    {
                        Status = LOLRead(m_strProgPath + @"\result.txt");//读取账号信息
                        if (Status == 2120)
                            continue;
                        if (intSkin > 0)
                            Status = SetSkin(m_strProgPath + @"\SkinTemp\");//皮肤拼图 
                        if (intHero > 0 && Status <= 1000)
                            Status = PinTuHero(m_strProgPath + @"\champion\", m_strCapturePath, "LOL5_01");//英雄拼图
                    } 
                    catch (Exception ex)
                    {
                        WriteToFile(ex.ToString());
                        return 2260;
                    }
                }
                if (Status == 2120)
                {
                    WriteToFile("清理残留程序,重新执行");
                    continue;
                }
                if (Status != 2120)
                {
                    CaptureJpg();
                    CloseGames();
                    break;
                }
            }
            return Status;
        }
        /// <summary>
        /// 关闭游戏
        /// </summary>
        public void CloseGames()
        {
#if DEBUG
            Game.RunCmd("taskkill /im  tgp_daemon.exe /F");
            Game.RunCmd("taskkill /im  LolClient.exe /F");
#else
            Game.RunCmd("taskkill /im  cmd.exe /F");
            //Game.RunCmd("taskkill /im  Client.exe /F");
            Game.RunCmd("taskkill /im  LOLAccount.exe /F");
#endif
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
            System.Diagnostics.Process.Start("shutdown", @"-r -t 0");
#endif
            return;
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
        //获取数据，发送服务器
        public string AutoVerifya()
        {
            return "S";
        }
        //获取数据，发送服务器
        public string AutoVerifyb()
        {
            return "S";
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
        /// <summary>
        /// 这个函数把文件的每一行读入list
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static List<string[]> ReadInfoFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                List<string[]> list = new List<string[]>();
                // 打开文件时 一定要注意编码 也许你的那个文件并不是GBK编码的
                using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("GBK")))
                {
                    while (!sr.EndOfStream) //读到结尾退出
                    {
                        string temp = sr.ReadLine();
                        //将每一行拆分，分隔符就是char 数组中的字符
                        string[] strArray = temp.Split(new char[] { '\t', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        //将拆分好的string[] 存入list
                        list.Add(strArray);
                    }
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// 这个函数把list中的每一行写入文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="list"></param>
        private static void WriteInfoTofile(string filePath, List<string[]> list)
        {
            // 打开文件时 一定要注意编码 也许你的那个文件并不是GBK编码的
            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("GBK")))
            {
                //一个string[] 是一行 ，一行中以tab键分隔
                foreach (string[] strArray in list)
                {
                    string line = string.Empty;
                    foreach (string temp in strArray)
                    {
                        if (!string.IsNullOrEmpty(temp))
                        {
                            line += temp;
                            line += "\t";
                        }
                    }
                    sw.WriteLine(line);
                }
            }
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
                objNewPic = new System.Drawing.Bitmap(objPic, intWidth, intHeight);
                objPic.Dispose();
                objPic = null;
                string newFile = strNewFile.Substring(0, strNewFile.LastIndexOf('.'));
                objNewPic.Save(newFile + ".bmp", ImageFormat.Bmp);
                objNewPic.Dispose();
            }
            catch (Exception exp)
            {
                WriteToFile(exp.ToString());
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
        /// 获取英雄与皮肤
        /// </summary>
        /// <returns></returns>
        public int GetHeroAndSkin()
        {
            for (int i = 0; i < 10; i++)
            {
                string result;
                #region 开启LOLAccount.exe
                Game.RunCmd("taskkill /im  LOLAccount.exe /F");
                //-------------------------------------------------------------------------
                string SendData = m_strAccount + " " + m_strPassword + " " + "KR";//韩服
                //-------------------------------------------------------------------------
                //-------------------------------------------------------------------------
                //string SendData = m_strAccount + " " + m_strPassword + " " + "NA";//美服
                //-------------------------------------------------------------------------
                int pid=Game.StartProcess(m_strProgPath + "\\LOLAccount.exe", SendData);
                Sleep(1000 * 5);
                for (int h = 0; h < 5; h++)
                {
                    Sleep(1000);
                    if (pid != 0)
                    {
                        WriteToFile("应用程序启动成功");
                        break;
                    }
                }

                if (pid == 0)
                {
                    WriteToFile("应用程序打开失败");
                    continue;
                }
                #endregion

                #region 加载资源
                for (int k = 0; k < 300; k++)
                {
                    Sleep(1000);
                    if (k + 5 % 20 == 0 || k == 0)
                        WriteToFile("等待返回信息，请稍等...");

                    if (File.Exists(m_strProgPath + @"\result.txt"))
                    {
                        FileInfo fi = new FileInfo(m_strProgPath + @"\result.txt");
                        if (fi.Length == 0)
                        {
                            WriteToFile("未获取到信息");
                            return 2120;
                        }
                        string msg = File.ReadAllText(m_strProgPath + @"\result.txt");
                        if (msg == "Wrong username or password.")
                        {
                            WriteToFile("账号密码错误");
                            CaptureJpg();
                            return 3000;
                        }
                        else if (msg == "account_state_transferred_out")
                        {
                            WriteToFile("服务器填写不正确");
                            CaptureJpg();
                            return 3333;
                        }
                        else if (msg.Contains("Account banned"))
                        {
                            WriteToFile("账号被限制登录");
                            WriteToFile(msg);
                            CaptureJpg();
                            return 3333;
                        }
                        else if (msg.Contains("invalid_credentials"))
                        {
                            WriteToFile("账号密码错误");
                            //WriteToFile(msg);
                            CaptureJpg();
                            return 3000;
                        }
                        else if (msg == "account disabled")
                        {
                            WriteToFile("帐户被禁用");
                            return 3360;
                        }
                        else if (msg.IndexOf("Summoner") >= 0 && msg.IndexOf("Level") >= 0 && msg.IndexOf("SoloQRank") >= 0 && msg.IndexOf("IpBalance") >= 0)
                        {
                            WriteToFile("获取信息完毕,分析信息...");
                            return 1000;
                        }
                        else
                        {
                            WriteToFile("获取信息有误");
                            try
                            {
                                File.Copy(m_strProgPath + @"\result.txt", "\\\\192.168.92.156\\vnc\\lol\\" + OrdNo);
                            }
                            catch { }
                            return 2120;
                        }
                    }
                }
                WriteToFile("获取信息超时");
                continue;
                #endregion
            }
            Game.RunCmd("多次获取失败...");
            return 2260;
        }
        /// <summary>
        /// 遍历文件
        /// </summary>
        /// <returns></returns>
        public static int TraversalFile(string dirPath)
        {
            int count = 0;
            //在指定目录查找文件
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo Dir = new DirectoryInfo(dirPath);
                try
                {
                    foreach (FileInfo file in Dir.GetFiles())//查找子目录 
                    {
                        string arrName = file.Name;
                        count++;
                        #region 处理图片
                        //if (file.Name.Contains("png"))
                        //{

                        //    using (FileStream fs = new FileStream(dirPath + file.Name, FileMode.Open, FileAccess.Read))
                        //    {
                        //        System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                        //        Bitmap map = new Bitmap(image);
                        //        arrName = file.Name.Replace(".jpg", "");
                        //        picResizePng(dirPath + file.Name, dirPath + arrName + ".png", image.Width, image.Height);
                        //    }
                        //    file.Delete();
                        //}
                        //else if (file.Name.Contains("png"))
                        //    arrName = file.Name.Replace(".png", "");
                        #endregion
                        locallist.Add(arrName); //给list赋值    
                        Console.WriteLine(arrName);
                    }
                    locallist.Sort();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return count;
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
        /// <summary>
        /// 读取账号游戏信息
        /// </summary>
        public int LOLRead(string path)
        {
            int skinPicName = 0;
            string msg = File.ReadAllText(m_strProgPath + "\\result.txt", UTF8Encoding.UTF8);
            JavaScriptSerializer js = new JavaScriptSerializer();   //实例化一个能够序列化数据的类
            JosnHelper list = js.Deserialize<JosnHelper>(msg);//msg为json字符串
            List<SkinList> ToJsonMy = list.SkinList;
            string strLv = list.Level;
            string strName = list.Summoner;
            string strCoinNum = list.IpBalance.Substring(0, list.IpBalance.IndexOf("."));
            string strRpNum = list.RpBalance.Substring(0, list.RpBalance.IndexOf("."));
            string strRunePages = list.RunePages;
            string strsoloRank = GetChinaRank(list.SoloQRank);
            string strRanked = GetChinaRank(list.PreviousSeasonRank);
            string strlastPlay = list.LastPlay.Substring(0, list.LastPlay.IndexOf("T")); ;
            int HeroNum = list.ChampionList.Count;
            int SkinNum = TraversalFile(m_strProgPath + @"\skin\");
            intHero = HeroNum;
            intSkin = SkinNum;
            int err = 0;
            #region  皮肤处理
            for (int i = 0; i < SkinNum; i++)
            {
                for (int j = 0; j < ToJsonMy.Count; j++)
                {
                    if (locallist[i].IndexOf(ToJsonMy[j].Id) >= 0)
                    {
                        try
                        {
                            skinPicName++;
                            Console.WriteLine(ToJsonMy[j].Name);//读取需要添加文字的图片
                            using (Bitmap bmp = new Bitmap(m_strProgPath + "\\skin\\" + locallist[i]))
                            {
                                Graphics g = Graphics.FromImage(bmp);
                                Font font = new Font("微软雅黑", 22, FontStyle.Bold);           //设置字体和大小
                                SolidBrush sbrush = new SolidBrush(Color.White);                //设置字体颜色
                                StringFormat format = new StringFormat();
                                format.Alignment = StringAlignment.Center;
                                g.DrawString(ToJsonMy[j].Name, font, sbrush, new Rectangle(0, 510, 308, 560), format);//文字在图片上的坐标x,y
                                if (!Directory.Exists(m_strProgPath + "\\SkinTemp"))
                                    Directory.CreateDirectory(m_strProgPath + "\\SkinTemp");
                                bmp.Save(m_strProgPath + "\\SkinTemp\\" + skinPicName + ".png", ImageFormat.Png);
                            }
                            break;
                        }
                        catch(Exception ex)
                        {
                            err++;
                            WriteToFile("下载图片为0字节，无法加载");
                            if (err > 3)
                            {
                                WriteToFile("皮肤下载不正确，大小0字节");
                                return 2120;
                            }
                        }
                    }
                }
            }
            WriteToFile("皮肤图片处理完毕..");
            #endregion

            string LOLTempPath = m_strProgPath + @"\LOLTemp\";
            try
            {
                if (File.Exists(m_strCapturePath + "temp.png"))
                    File.Delete(m_strCapturePath + "temp.png");
                using (Bitmap bmp = new Bitmap(LOLTempPath + "韩服排版.bmp"))//读取需要添加文字的图片  
                {  
                    Graphics g1 = Graphics.FromImage(bmp);
                    Font font1 = new Font("微软雅黑", 16, FontStyle.Bold);                       //设置字体和大小
                    SolidBrush sbrush1 = new SolidBrush(Color.FromArgb(218, 218, 218));                //设置字体颜色             
                    g1.DrawString(strName, font1, sbrush1, new PointF(353, 32));
                    g1.DrawString(strLv, font1, sbrush1, new PointF(353, 58));
                    g1.DrawString(strsoloRank, font1, sbrush1, new PointF(353, 84));
                    g1.DrawString(strRanked, font1, sbrush1, new PointF(353, 109));
                    g1.DrawString(HeroNum.ToString(), font1, sbrush1, new PointF(353, 136)); //文字在图片上的坐标x,y
                    g1.DrawString(strRpNum + "/" + strCoinNum, font1, sbrush1, new PointF(353, 162));
                    g1.DrawString(strlastPlay, font1, sbrush1, new PointF(353, 189));
                    if (!Directory.Exists(m_strCapturePath))
                        Directory.CreateDirectory(m_strCapturePath);
                    bmp.Save(m_strCapturePath + "temp.png");
                    PicAddWaterMark1(m_strCapturePath + "temp.png", LOLTempPath + (strsoloRank + ".jpg"), 29, 28, false);
                    WriteToFile("账号信息拼图");
                    g1.Dispose();
                    PinTu(m_strCapturePath, m_strCapturePath, "LOL1_01");
                }
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
            }
            picNum = 1;//计录截图张数
            return 1000;
        }
        /// <summary>
        /// 图片缩放
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="strNewFile"></param>
        /// <param name="intWidth"></param>
        /// <param name="intHeight"></param>
        /// <returns>参数:原文件完整名称,新文件完整名称,新的宽度,新的高度(若高度为0,按比例缩放)</returns>
        public bool picResizePng1(string strFile, string strNewFile, int intWidth, int intHeight)
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
            //waterBit.UnlockBits(waterBData);
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
        public string GetChinaRank(string englishrank)
        {
            string chinaRank = "无段位";
            if (englishrank.Contains("Challenger"))
                chinaRank = "最强王者";
            else if (englishrank.Contains("master"))
                chinaRank = "超凡大师";
            else if (englishrank.Contains("Diamond"))
                chinaRank = englishrank.Replace("Diamond", "璀璨钻石");
            else if (englishrank.Contains("Platinum"))
                chinaRank = englishrank.Replace("Platinum", "华贵铂金");
            else if (englishrank.Contains("Gold"))
                chinaRank = englishrank.Replace("Gold", "荣耀黄金");
            else if (englishrank.Contains("Silver"))
                chinaRank = englishrank.Replace("Silver", "不屈白银");
            else if (englishrank.Contains("Bronze"))
                chinaRank = englishrank.Replace("Bronze", "英勇青铜");
            return chinaRank;
        }
        public int GetTraversalFile(string dirPath, int width, int hight)
        {
            int count = 0;
            //在指定目录查找文件
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo Dir = new DirectoryInfo(dirPath);
                try
                {
                    foreach (FileInfo file in Dir.GetFiles())//查找子目录 
                    {
                        count++;
                        if (file.Name.Contains("png"))
                        {
                            picResizePng1(dirPath + file.Name, dirPath + file.Name, width, hight);
                            pngskin.Add(file.Name);
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return count;
        }
        /// <summary>
        /// 英雄拼图
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="savePath"></param>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public int PinTuHero(string folderPath, string savePath, string saveName)
        {
            WriteToFile("英雄拼图");
            picNum++;
            int num = GetTraversalFile(folderPath, 80, 80);//遍历缩放png
            if (num % 10 != 0)
                num = num / 10 + 1;
            else
                num = num / 10;
            int width = 83;//实际长度160
            int height = 83;
            picResize(m_strProgPath + "\\LOLTemp\\模板.bmp", folderPath + "模板.bmp", 880, num * height);
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            Bitmap AllPic = new Bitmap(folderPath + "模板.bmp");
            Graphics graph = Graphics.FromImage(AllPic);
            int i = 0;
            foreach (FileInfo file in folder.GetFiles("*.png"))
            {
                //Console.WriteLine(file.FullName);
                Image UnitPic = Bitmap.FromFile(file.FullName);
                graph.DrawImage(UnitPic, width * (i % 10), height * (i / 10));
                UnitPic.Dispose();
                i++;
            }
            graph.Dispose();
            if (m_strOrderType == "发布单")
                CreatWaterMark(savePath + saveName + ".jpg", AllPic);
            else
                AllPic.Save(savePath + saveName + ".jpg", ImageFormat.Jpeg);
            //AllPic.Save(savePath + saveName + ".jpg", ImageFormat.Jpeg);
            AllPic.Dispose();
            return 1000;
        }
        /// <summary>
        /// 皮肤拼图
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="savePath"></param>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public bool DeleteFiles(string path)
        {
            if (Directory.Exists(path) == false)
            {
                //MessageBox.Show("Path is not Existed!");
                WriteToFile("文件不存在");
                return false;
            }
            DirectoryInfo dir = new DirectoryInfo(path);
            FileInfo[] files = dir.GetFiles();
            try
            {
                foreach (var item in files)
                {
                    File.Delete(item.FullName);
                }
                if (dir.GetDirectories().Length != 0)
                {
                    foreach (var item in dir.GetDirectories())
                    {
                        if (!item.ToString().Contains("$") && (!item.ToString().Contains("Boot")))
                        {
                            // Console.WriteLine(item);
                            DeleteFiles(dir.ToString() + "\\" + item.ToString());
                        }
                    }
                }
                Directory.Delete(path);

                return true;
            }
            catch (Exception w)
            {
                // MessageBox.Show("Delete Failed!");
                WriteToFile(w.ToString());
                return false;

            }
        }
        /// <summary>
        /// 正常拼图
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="savePath"></param>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public int PinTu(string folderPath, string savePath, string saveName)
        {
            int width = 122;
            int height = 240;
            picResize(m_strProgPath + "\\LOLTemp\\模板.bmp", folderPath + "模板.bmp", 880, 240);
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            Bitmap AllPic = new Bitmap(folderPath + "模板.bmp");
            Graphics graph = Graphics.FromImage(AllPic);
            Image UnitPic = Bitmap.FromFile(m_strCapturePath + "temp.png");
            graph.DrawImage(UnitPic, 0, 0);

            graph.Dispose();
            UnitPic.Dispose();

            if (m_strOrderType == "发布单")
                CreatWaterMark(savePath + saveName + ".jpg", AllPic);
            else AllPic.Save(savePath + saveName + ".jpg", ImageFormat.Jpeg);
           
            AllPic.Dispose();
            Sleep(50);
            #region 名字打码
            //if (m_strOrderType == "发布单")
            //{   
            //    CreatWaterMark(savePath + saveName + ".jpg", AllPic);
            //    
            //    //Font font1 = new Font("微软雅黑", 18, FontStyle.Bold);                       //设置字体和大小
            //    //SolidBrush sbrush1 = new SolidBrush(Color.FromArgb(249, 204, 226));                //设置字体颜色             
            //    //graph.DrawString("██████", font1, sbrush1, new PointF(353, 29));
            //   
            //}
            //else
            //    AllPic.Save(savePath + saveName + ".jpg", ImageFormat.Jpeg);
            #endregion
            return 1000;
        }
        public int PinTuNew(List<string> pintulist, string PicPath, string saveName)
        {
            int num = pintulist.Count;
            if (num % 7 != 0)
                num = num / 7 + 1;
            else
                num = num / 7;
            int width = 122;
            int height = 222;
            picResize(m_strProgPath + "\\LOLTemp\\模板.bmp", PicPath + "模板.bmp", 880, num * height);
            Bitmap AllPic = new Bitmap(PicPath + "模板.bmp");
            Graphics graph = Graphics.FromImage(AllPic);
            int i = 0;
            foreach (string picname in pintulist)
            {
                int x = width * (i % 7);
                int y = height * (i / 7);
                Bitmap bt = new Bitmap(PicPath + "\\" + picname);
                graph.DrawImage(bt, x, y, width, height);
                bt.Dispose();
                i++;
            }

            graph.Dispose();

            for (int k = 1; k < 35; k++)
            {
                string ServerPicName = m_strCapturePath + saveName + "_0" + k + ".jpg";
                if (!File.Exists(ServerPicName))
                {
                    if (m_strOrderType == "发布单")
                       CreatWaterMark(ServerPicName, AllPic);
                    else
                       AllPic.Save(ServerPicName, ImageFormat.Jpeg);  
                    
                    AllPic.Dispose();
                    //Sleep(200);
                    for (int j = 0; j < 3; j++)
                    {
                        File.Delete(PicPath + "模板.bmp");
                        if (File.Exists(PicPath + "模板.bmp"))
                        {
                            Sleep(1000);
                            AllPic.Dispose();
                            continue;
                        }
                        break;
                    }                
                    break;
                }
            }
            return 1000;
        }
        public int SetSkin(string folderPath)
        {
            int num = GetTraversalFile(folderPath, 122, 222);//遍历缩放png
            if(num%35==0)
                num=num/35;
            else
                num=num/35+1;
            int max = 35;
            WriteToFile("皮肤拼图");
            for (int i = 0; i < num; i++)
            {
                picNum++;
                List<string> puntulist = new List<string>();//先定义list集合
                string[] arrPicName=new string[1];
                if (pngskin.Count - 35 * i < 35)
                    max = pngskin.Count - 35 * i;
                else
                    max = 35;
                for (int j = 0; j < max; j++)
                {
                    puntulist.Add (pngskin[i * 35 + j]);
                }
                Sleep(200);
                PinTuNew(puntulist, folderPath, "LOL3");
            }
            return 1000;
        }
        public void CreatWaterMark(string picnameandpath,Bitmap bit)
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
      
        private const int INTERNET_CONNECTION_MODEM = 1;
        private const int INTERNET_CONNECTION_LAN = 2;
        [System.Runtime.InteropServices.DllImport("winInet.dll")]
        private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);
        /// <summary>
        /// 判断本地的连接状态
        /// </summary>
        /// <returns>true:拨号上网;false:断网或者本地连接</returns>
        private static bool LocalConnectionStatus()
        {
            System.Int32 dwFlag = new Int32();
            if (!InternetGetConnectedState(ref dwFlag, 0))
            {
                Console.WriteLine("LocalConnectionStatus--未连网!");
                return false;
            }
            else
            {
                if ((dwFlag & INTERNET_CONNECTION_MODEM) != 0)
                {
                    WriteToFile("LocalConnectionStatus--采用调制解调器拨号上网。");
                    return true;
                }
                else if ((dwFlag & INTERNET_CONNECTION_LAN) != 0)
                {
                    WriteToFile("LocalConnectionStatus--采用网卡上网。");
                    return false;
                }
            }
            return false;
        }
    }
}
