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
using System.Text.RegularExpressions;//����
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Principal;
using System.Web.Script.Serialization;
//using System.Threading.Tasks;
namespace GTR
{
    class LOL
    {
        #region ��ʼ��ͨ�ű���
        //��������
        public static string m_strOrderData;
        //udpsockets
        public static udpSockets udpdd;
        //��֤�뷵��
        public static string mouseP = "";
        //��֤��ش����
        public int yzmTimes = 0;
        //udp�˿�
        public static int m_UDPPORT = 6801;
        //�ű��˿�
        public static int the_nRC2Port = 0;
        //��������-���׵�/������
        public static string m_strOrderType;
        //������
        public static string OrdNo = "���Զ���"; //"MZH-160607000000001";
        //����״̬
        public int Status;
        public int IsStop = 1;
        public static IntPtr ChangerHWND;

        #endregion

        #region ��ʼ���������
        //�����־
        public static bool IsAnswer;
        //�ʼı�־
        public static bool IsAskMail;
        //�ƽ���־
        public static bool bYiJiao = false;
        //������Ϸ��־
        public static bool IfEnter = false;
        //Mվ�㶩����־
        public static bool MZH = false;
        //��վ�Ƿ�̬���Ա�־
        public string IsNeedRecognition;
        static int PicNum = 1;
        static string strLastPicID = "";
        string m_strLastName;
        int m_nPicNum;
        static int picNum = 0;
        public int time = 0;
        public int QWE = 0;
        //ӵ�з���ҳ��
        public int RenusNum;
        //ӵ�е�Ӣ������
        public Int64 intHero = 0;
        //ӵ�е�Ƥ������
        public Int64 intSkin = 0;
        //ӵ�еĽ��
        public Int32 intCoin = 0;
        public Int32 intLevel = 0;
        public bool IsNewGame = false;
        //�����ͼĿ¼
        public bool IsSendCapPath = false;
        string[] FileArr;
        //��֤�루jpg��
        string mousea;
        //���ھ��
        public static IntPtr m_hGameWnd;
        //��������·��
        private string m_strProgPath = System.Windows.Forms.Application.StartupPath;
        //ƥ��ͼƬ·��
        public static string m_strPicPath = System.Windows.Forms.Application.StartupPath + @"\Ӣ������\";
        //�쳣��ͼ����·��
        public static string LocalPicPath = "D:\\LOL����\\";
        //[WebMethod]
        //public string Project(string paramaters)
        //{

        //    return paramaters;

        //}
        //������ϸ����
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
        static List<string> locallist = new List<string>();//�ȶ���list����
        static List<string> pngskin = new List<string>();//�ȶ���list����
        #endregion
        //���������
        public void StartToWork()
        {

            #region  IP ��ͼ·�� ����
            m_strCapturePath = "E:\\ƴͼ";
            if (m_strCapturePath == "")
            {
                WriteToFile("�Ҳ�����ͼ��š��쳣·����");
            }
            WriteToFile(m_strCapturePath);
            Sleep(1000);
            User32API.WinExec("rasphone -h �������", 2); //��
            Sleep(3000);
            if (LocalConnectionStatus())
            {
                User32API.WinExec("rasphone -h �������", 2); //��
                Sleep(3000);
            }
            User32API.WinExec("rasphone -d �������", 2); //��
            for (int i = 0; i < 8; i++)
            {
                if (User32API.FindWindow(null, "�������ӵ� �������...") != IntPtr.Zero)
                {
                    Sleep(3000);
                    WriteToFile("���������...");
                }
                if (User32API.FindWindow(null, "���ӵ� ������� ʱ����") != IntPtr.Zero)
                {
                    WriteToFile("���󲦺�ʧ��,ֹͣ����");
                    Game.tskill("rasphone");
                    Sleep(500);
                    if (User32API.FindWindow(null, "���ӵ� ������� ʱ����") == IntPtr.Zero)
                        RestartPC();
                }
                if (User32API.FindWindow(null, "��������") != IntPtr.Zero)
                {
                    WriteToFile("����ʧ�ܣ�ֹͣ����");
                    Game.tskill("rasphone");
                    RestartPC();
                    return;
                }
            }
            WriteToFile("���ſ�����ӳɹ�");

            #endregion

            #region ��Ҫ���ܿ�
            try
            {
                DeleteFolder(LocalPicPath, 7);
                DeleteFiles(m_strProgPath + @"\champion\");
                DeleteFiles(m_strProgPath + @"\SkinTemp\");
                Status = GameProc();
                if (Status > 1000)
                {
                    Point pt = new Point();
                    CaptureJpg("����ʧ��");
                }
            }
            catch (Exception ess)
            {
                WriteToFile(ess.ToString());
            }
            #endregion

            #region ����������쳣��ͼ
            if (MZH && Status != 1000)
            {
                if (Status > 2000 && Status < 3000)
                    Status += 2000;
            }
            string tmp;
            if (Status == 1000)
            {
                //picNum = 3;
                tmp = string.Format("��ͼ�ɹ�,��{0}��\r\n", picNum);
                WriteToFile(tmp);
            }
            if (Status > 1000)
            {
                FileRW.DeleteTmpFiles(m_strCapturePath + OrdNo);
            }
            tmp = string.Format("�ƽ�״̬={0}\r\n", Status);
            WriteToFile(tmp);
            tmp = string.Format("FStatus={0}\r\n", Status);
            #endregion

            #region�ټ�¼������� ������ʧ��5���������ԣ�Ƶ���������¶�����ʱ��
            StringBuilder retVal = new StringBuilder(256);
            User32API.GetPrivateProfileString("��¼����", "ADSL��������", "", retVal, 256, m_strProgPath + "\\adsl.ini");
            int num = 0;
            if (retVal.ToString() != "")
                num = int.Parse(retVal.ToString());
            if (num > 100)
            {
                User32API.WritePrivateProfileString("��¼����", "ADSL��������", "0", m_strProgPath + "\\adsl.ini");
                Sleep(2500);
            }
            else
            {
                if ((Status > 1000 && Status < 3000) || Status > 4000)
                {
                    string strNum = string.Format("{0}", num + 20);
                    User32API.WritePrivateProfileString("��¼����", "ADSL��������", strNum, m_strProgPath + "\\adsl.ini");
                }
            }
            StringBuilder retVal1 = new StringBuilder(256);
            User32API.GetPrivateProfileString("��¼����", "����ʧ��", "", retVal1, 255, m_strProgPath + "\\adsl.ini");
            int num1 = int.Parse(retVal1.ToString());
            if ((Status > 1000 && Status < 3000 && Status != 3333) || (Status > 4000 && Status != 4050))
            {
                if (num1 == 5)
                {
                    User32API.WritePrivateProfileString("��¼����", "����ʧ��", "0", m_strProgPath + "\\adsl.ini");
                    RestartPC();//��������
                }
                else
                {
                    string strNum1 = string.Format("{0}", num1 + 1);
                    User32API.WritePrivateProfileString("��¼����", "����ʧ��", strNum1, m_strProgPath + "\\adsl.ini");
                }
            }
            #endregion

            #region ����UDP����
            if (the_nRC2Port != 0)
            {
                for (int j = 0; j < 2; j++)
                {
                    try
                    {
                        udpdd.theUDPSend((int)TRANSTYPE.TRANS_ORDER_END, tmp, OrdNo);//����UDP
                    }
                    catch (Exception ex)
                    {
                        WriteToFile(ex.ToString());
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        if (bYiJiao)
                        {
                            WriteToFile("�ƽ��ɹ�");

                            break;
                        }
                        Sleep(100);
                    }
                    if (bYiJiao)
                    {
                        break;
                    }
                    if (j == 1)
                        WriteToFile("�ƽ�ʧ��");
                }
            }
            else
            {
                WriteToFile("�˿�Ϊ0");
            }
            #endregion

            return;
        }
        /// ������
        /// <returns>����״̬</returns>
        public int GameProc()
        {
            #region �����������
            if (!KeyMouse.InitKeyMouse())
            {
                WriteToFile("��������ʧ��");
                return 2260;
            }
            #endregion

            #region �ж϶����������󶩵���Ϣ
            if (OrdNo.IndexOf("MZH") == 0)
                MZH = true;
            int n = OrdNo.IndexOf("-");
            if (n > 0 || OrdNo == "���Զ���" || MZH)
                m_strOrderType = "������";
            else
                m_strOrderType = "���׵�";
            if (!RequestOrderData())
                return 2260;
            if (!ReadOrderDetail())
                return 2260;
            #endregion

            #region �˺�������� Mվ���
            if (Regex.IsMatch(m_strAccount, @"[\u4e00-\u9fa5]"))
            {
                WriteToFile("�˺ź�������");
                WriteToFile(m_strAccount);
                return 3000;
            }
            if (Regex.IsMatch(m_strPassword, @"[\u4e00-\u9fa5]"))
            {
                WriteToFile("���뺬������");
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
            //    //    WriteToFile("��ȡ�ֶ���Ϣʧ��");
            //    //    return 2222;
            //    //}
            //    catch (Exception e)
            //    {
            //        WriteToFile(e.ToString());
            //    }
            //}
            #endregion

            AppInit();//IP��ַ �汾��
            for (int i = 0; i < 2; i++)
            {
                #region �رս���
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
                        Status = LOLRead(m_strProgPath + @"\result.txt");//��ȡ�˺���Ϣ
                        if (Status == 2120)
                            continue;
                        if (intSkin > 0)
                            Status = SetSkin(m_strProgPath + @"\SkinTemp\");//Ƥ��ƴͼ 
                        if (intHero > 0 && Status <= 1000)
                            Status = PinTuHero(m_strProgPath + @"\champion\", m_strCapturePath, "LOL5_01");//Ӣ��ƴͼ
                    } 
                    catch (Exception ex)
                    {
                        WriteToFile(ex.ToString());
                        return 2260;
                    }
                }
                if (Status == 2120)
                {
                    WriteToFile("�����������,����ִ��");
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
        /// �ر���Ϸ
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
        /// ���󶩵�����
        /// </summary>
        /// <returns></returns>
        public static bool RequestOrderData()
        {
            if (the_nRC2Port == 0)
            {//������
                m_strOrderData = FileRW.ReadFile("info.txt");
            }
            else
            { //��������ȡ
                m_strOrderData = "";
                string tmp = string.Format("FExeProcID={0}\r\nFRobotPort={1}\r\n", Program.pid, m_UDPPORT);
                udpdd.theUDPSend((int)TRANSTYPE.TRANS_REQUEST_ORDER, tmp, OrdNo);
                for (int i = 0; i < 30; i++)
                {
                    if (m_strOrderData != "")
                    {
                        tmp = string.Format("�˿ں�{0}������{1}���̺�{2}", the_nRC2Port, OrdNo, Program.pid);
                        WriteToFile(tmp);
                        Thread.Sleep(100);
                        return true;
                    }
                    Thread.Sleep(100);
                }
                WriteToFile("��������ʧ��\r\n");
                return false;
            }
            return true;
        }
        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <returns></returns>
        public bool ReadOrderDetail()
        {
            if (m_strOrderData == "")
            {
                WriteToFile(("==========> ��������Ϊ�� <==========\n"));
                return false;
            }
            string m_RegInfos = MyStr.FindStr(m_strOrderData, "<RegInfos>", "</RegInfos>");
            string strItem = MyStr.FindStr(m_RegInfos, "<Name>��Ϸ�˺�</Name>", "</RegInfoItem>");
            m_strAccount = MyStr.FindStr(strItem, "<Value>", "</Value>");
            if (m_strAccount == "")
            {
                strItem = MyStr.FindStr(m_RegInfos, "<Name>��Ϸ�ʺ�</Name>", "</RegInfoItem>");
                m_strAccount = MyStr.FindStr(strItem, "<Value>", "</Value>");
            }
            strItem = MyStr.FindStr(m_RegInfos, "<Name>��Ϸ����</Name>", "</RegInfoItem>");
            m_strPassword = MyStr.FindStr(strItem, "<Value>", "</Value>");
            strItem = MyStr.FindStr2(m_RegInfos, "<Name>��Ϸ��ɫ��</Name>", "</RegInfoItem>");
            m_strSellerRole = MyStr.FindStr(strItem, "<Value>", "</Value>");
            strItem = MyStr.FindStr(m_RegInfos, "<Name>�ֿ�����</Name>", "</RegInfoItem>");
            m_strSecondPwd = MyStr.FindStr(strItem, "<Value>", "</Value>");
            strItem = MyStr.FindStr(m_RegInfos, "<Name>�û����</Name>", "</RegInfoItem>");
            strItem = MyStr.FindStr(m_RegInfos, "<Name>IsNeedRecognition</Name>", "</RegInfoItem>");
            IsNeedRecognition = MyStr.FindStr(strItem, "<Value>", "</Value>");
            WriteToFile("�Ƿ���Ҫ��ȡQQ��Ϣ��" + IsNeedRecognition);
            if (m_strAccount == "")
            {
                WriteToFile("�ʺ�Ϊ��");
                return false;
            }
            if (m_strPassword == "")
            {
                WriteToFile("����Ϊ��");
                return false;
            }
            char[] acc = m_strAccount.ToCharArray();
            for (int i = 0; i < acc.Length; i++)
            {
                if (char.IsControl(acc[i]))
                {
                    WriteToFile("�˺ź��в��ɼ���ת���ַ�");
                    return false;
                }
            }
            char[] pwd = m_strPassword.ToCharArray();
            for (int i = 0; i < pwd.Length; i++)
            {
                if (char.IsControl(pwd[i]))
                {
                    WriteToFile("���뺬�в��ɼ���ת���ַ�");
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
            string strlog = string.Format("��Ϸ��[{0}]", m_strGameName);
            WriteToFile(strlog);
            int tt = m_strCapturePath.LastIndexOf("\\");
            if (m_strCapturePath == "")
                m_strCapturePath = "C:\\ƴͼ\\";
            else if (tt > 0)
                m_strCapturePath += "\\";
            return true;
        }
        public void AppInit()
        {
            string tmp;
            Version ApplicationVersion = new Version(Application.ProductVersion);
            tmp = string.Format("IP:{0},�汾��:{1},�ű��˿�{2}", Game.GetLocalIp(), ApplicationVersion.ToString(), m_UDPPORT);
            WriteToFile(tmp);
            return;
        }
        /// <summary>
        ///�߳���ͣ
        /// </summary>
        public static void Sleep(int time)
        {
            Thread.Sleep(time);
            return;
        }
        /// <summary>
        ///��־���
        /// </summary>
        /// <param name="tmp">��־����</param>
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
        /// ��������
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
            //WriteToFile("�Զ����ⷵ��:" + code);
        }
        //��ȡ���ݣ����ͷ�����
        public string AutoVerifya()
        {
            return "S";
        }
        //��ȡ���ݣ����ͷ�����
        public string AutoVerifyb()
        {
            return "S";
        }
        //�������ݣ�JSON��ʽ��
        public static string Post(string url, string postData)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            System.Net.HttpWebRequest objWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            objWebRequest.Method = "POST";
            objWebRequest.ContentType = "application/json";
            objWebRequest.ContentLength = byteArray.Length;
            Stream newStream = objWebRequest.GetRequestStream();
            // Send the data. 
            newStream.Write(byteArray, 0, byteArray.Length); //д����� 
            newStream.Close();
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);//Encoding.Default
            string textResponse = sr.ReadToEnd(); // ���ص�����
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
        /// ����������ļ���ÿһ�ж���list
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static List<string[]> ReadInfoFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                List<string[]> list = new List<string[]>();
                // ���ļ�ʱ һ��Ҫע����� Ҳ������Ǹ��ļ�������GBK�����
                using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("GBK")))
                {
                    while (!sr.EndOfStream) //������β�˳�
                    {
                        string temp = sr.ReadLine();
                        //��ÿһ�в�֣��ָ�������char �����е��ַ�
                        string[] strArray = temp.Split(new char[] { '\t', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        //����ֺõ�string[] ����list
                        list.Add(strArray);
                    }
                }
                return list;
            }
            return null;
        }
        /// <summary>
        /// ���������list�е�ÿһ��д���ļ�
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="list"></param>
        private static void WriteInfoTofile(string filePath, List<string[]> list)
        {
            // ���ļ�ʱ һ��Ҫע����� Ҳ������Ǹ��ļ�������GBK�����
            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("GBK")))
            {
                //һ��string[] ��һ�� ��һ������tab���ָ�
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
        //������֤���,0Ϊ����,1Ϊ��ȷ
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
            newStream.Write(byteArray, 0, byteArray.Length); //д����� 
            newStream.Close();
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);//Encoding.Default
            string textResponse = sr.ReadToEnd(); // ���ص�����
            return textResponse;
        }
        /// <summary>
        ///�����Ƿ���ȷ
        /// </summary>
        /// <param name="isTrue">��ȷ���0��ȷ��1����</param>
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
        /// ɾ��ָ������֮ǰ���ļ���
        /// </summary>
        /// <param name="path">�ļ���·��</param>
        /// <param name="time">ʱ�䣺��λ��</param>
        public void DeleteFolder(string path, int time)
        {
            WriteToFile("ɾ���ļ�");
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
        /// ��ͼ
        /// </summary>
        public void CaptureJpg()
        {
            //����ͼ·���Ƿ����쳣
            try
            {
                string tmp = SetPicPathBmp(LocalPicPath, OrdNo, "");
                if (!IsSendCapPath)
                {
                    WriteToFile("��ͼĿ¼��" + tmp);
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
            //���·�����쳣��·����Ϊ "Z:\\jiaoben\\"
            catch
            {
                LocalPicPath = "Z:\\jiaoben\\";
                string tmp = SetPicPathBmp(LocalPicPath, OrdNo, "");
                if (!IsSendCapPath)
                {
                    WriteToFile("�쳣��ͼĿ¼��" + tmp);
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
            //����ͼ·���Ƿ����쳣
            try
            {
                string tmp = SetPicPathBmp(LocalPicPath, OrdNo, picName);
                if (!IsSendCapPath)
                {
                    WriteToFile("��ͼĿ¼��" + tmp);
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
            //���·�����쳣��·����Ϊ "Z:\\jiaoben\\"
            catch
            {
                LocalPicPath = "Z:\\jiaoben\\";
                string tmp = SetPicPathBmp(LocalPicPath, OrdNo, picName);
                if (!IsSendCapPath)
                {
                    WriteToFile("��ͼĿ¼��" + tmp);
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
        //ͼƬ����bmp
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
        //ͼƬ����
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
        //ͼƬ����
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
        /// ���ý�ͼ·��-�����쳣��ͼ
        /// </summary>
        /// <param name="str">�ļ���·��</param>
        /// <param name="strPicID">ͼƬ���</param>
        /// <returns>ͼƬ·��</returns>
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
        /// ��ȡӢ����Ƥ��
        /// </summary>
        /// <returns></returns>
        public int GetHeroAndSkin()
        {
            for (int i = 0; i < 10; i++)
            {
                string result;
                #region ����LOLAccount.exe
                Game.RunCmd("taskkill /im  LOLAccount.exe /F");
                //-------------------------------------------------------------------------
                string SendData = m_strAccount + " " + m_strPassword + " " + "KR";//����
                //-------------------------------------------------------------------------
                //-------------------------------------------------------------------------
                //string SendData = m_strAccount + " " + m_strPassword + " " + "NA";//����
                //-------------------------------------------------------------------------
                int pid=Game.StartProcess(m_strProgPath + "\\LOLAccount.exe", SendData);
                Sleep(1000 * 5);
                for (int h = 0; h < 5; h++)
                {
                    Sleep(1000);
                    if (pid != 0)
                    {
                        WriteToFile("Ӧ�ó��������ɹ�");
                        break;
                    }
                }

                if (pid == 0)
                {
                    WriteToFile("Ӧ�ó����ʧ��");
                    continue;
                }
                #endregion

                #region ������Դ
                for (int k = 0; k < 300; k++)
                {
                    Sleep(1000);
                    if (k + 5 % 20 == 0 || k == 0)
                        WriteToFile("�ȴ�������Ϣ�����Ե�...");

                    if (File.Exists(m_strProgPath + @"\result.txt"))
                    {
                        FileInfo fi = new FileInfo(m_strProgPath + @"\result.txt");
                        if (fi.Length == 0)
                        {
                            WriteToFile("δ��ȡ����Ϣ");
                            return 2120;
                        }
                        string msg = File.ReadAllText(m_strProgPath + @"\result.txt");
                        if (msg == "Wrong username or password.")
                        {
                            WriteToFile("�˺��������");
                            CaptureJpg();
                            return 3000;
                        }
                        else if (msg == "account_state_transferred_out")
                        {
                            WriteToFile("��������д����ȷ");
                            CaptureJpg();
                            return 3333;
                        }
                        else if (msg.Contains("Account banned"))
                        {
                            WriteToFile("�˺ű����Ƶ�¼");
                            WriteToFile(msg);
                            CaptureJpg();
                            return 3333;
                        }
                        else if (msg.Contains("invalid_credentials"))
                        {
                            WriteToFile("�˺��������");
                            //WriteToFile(msg);
                            CaptureJpg();
                            return 3000;
                        }
                        else if (msg == "account disabled")
                        {
                            WriteToFile("�ʻ�������");
                            return 3360;
                        }
                        else if (msg.IndexOf("Summoner") >= 0 && msg.IndexOf("Level") >= 0 && msg.IndexOf("SoloQRank") >= 0 && msg.IndexOf("IpBalance") >= 0)
                        {
                            WriteToFile("��ȡ��Ϣ���,������Ϣ...");
                            return 1000;
                        }
                        else
                        {
                            WriteToFile("��ȡ��Ϣ����");
                            try
                            {
                                File.Copy(m_strProgPath + @"\result.txt", "\\\\192.168.92.156\\vnc\\lol\\" + OrdNo);
                            }
                            catch { }
                            return 2120;
                        }
                    }
                }
                WriteToFile("��ȡ��Ϣ��ʱ");
                continue;
                #endregion
            }
            Game.RunCmd("��λ�ȡʧ��...");
            return 2260;
        }
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <returns></returns>
        public static int TraversalFile(string dirPath)
        {
            int count = 0;
            //��ָ��Ŀ¼�����ļ�
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo Dir = new DirectoryInfo(dirPath);
                try
                {
                    foreach (FileInfo file in Dir.GetFiles())//������Ŀ¼ 
                    {
                        string arrName = file.Name;
                        count++;
                        #region ����ͼƬ
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
                        locallist.Add(arrName); //��list��ֵ    
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
        /// ������Ҫ�ϴ���jpg��ʽ��ͼƬ���»��ߡ�
        /// </summary>
        public void TraversalPicJpg()
        {
            if (Directory.Exists(m_strCapturePath))
            {
                DirectoryInfo Dir = new DirectoryInfo(m_strCapturePath);
                try
                {
                    foreach (FileInfo file in Dir.GetFiles("*.jpg"))//������Ŀ¼ 
                    {
                        string arrName = string.Empty;
                        if (file.Name.Contains("jpg") && file.Name.Contains("_"))
                        {
                            using (FileStream fs = new FileStream(m_strCapturePath + file.Name, FileMode.Open, FileAccess.Read))
                            {
                                System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                                WriteToFile("ͼƬ���ƣ�" + file.Name + ",ͼƬ��ȣ�" + image.Width + ",ͼƬ�߶ȣ�" + image.Height);
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
        /// ��ȡ�˺���Ϸ��Ϣ
        /// </summary>
        public int LOLRead(string path)
        {
            int skinPicName = 0;
            string msg = File.ReadAllText(m_strProgPath + "\\result.txt", UTF8Encoding.UTF8);
            JavaScriptSerializer js = new JavaScriptSerializer();   //ʵ����һ���ܹ����л����ݵ���
            JosnHelper list = js.Deserialize<JosnHelper>(msg);//msgΪjson�ַ���
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
            #region  Ƥ������
            for (int i = 0; i < SkinNum; i++)
            {
                for (int j = 0; j < ToJsonMy.Count; j++)
                {
                    if (locallist[i].IndexOf(ToJsonMy[j].Id) >= 0)
                    {
                        try
                        {
                            skinPicName++;
                            Console.WriteLine(ToJsonMy[j].Name);//��ȡ��Ҫ������ֵ�ͼƬ
                            using (Bitmap bmp = new Bitmap(m_strProgPath + "\\skin\\" + locallist[i]))
                            {
                                Graphics g = Graphics.FromImage(bmp);
                                Font font = new Font("΢���ź�", 22, FontStyle.Bold);           //��������ʹ�С
                                SolidBrush sbrush = new SolidBrush(Color.White);                //����������ɫ
                                StringFormat format = new StringFormat();
                                format.Alignment = StringAlignment.Center;
                                g.DrawString(ToJsonMy[j].Name, font, sbrush, new Rectangle(0, 510, 308, 560), format);//������ͼƬ�ϵ�����x,y
                                if (!Directory.Exists(m_strProgPath + "\\SkinTemp"))
                                    Directory.CreateDirectory(m_strProgPath + "\\SkinTemp");
                                bmp.Save(m_strProgPath + "\\SkinTemp\\" + skinPicName + ".png", ImageFormat.Png);
                            }
                            break;
                        }
                        catch(Exception ex)
                        {
                            err++;
                            WriteToFile("����ͼƬΪ0�ֽڣ��޷�����");
                            if (err > 3)
                            {
                                WriteToFile("Ƥ�����ز���ȷ����С0�ֽ�");
                                return 2120;
                            }
                        }
                    }
                }
            }
            WriteToFile("Ƥ��ͼƬ�������..");
            #endregion

            string LOLTempPath = m_strProgPath + @"\LOLTemp\";
            try
            {
                if (File.Exists(m_strCapturePath + "temp.png"))
                    File.Delete(m_strCapturePath + "temp.png");
                using (Bitmap bmp = new Bitmap(LOLTempPath + "�����Ű�.bmp"))//��ȡ��Ҫ������ֵ�ͼƬ  
                {  
                    Graphics g1 = Graphics.FromImage(bmp);
                    Font font1 = new Font("΢���ź�", 16, FontStyle.Bold);                       //��������ʹ�С
                    SolidBrush sbrush1 = new SolidBrush(Color.FromArgb(218, 218, 218));                //����������ɫ             
                    g1.DrawString(strName, font1, sbrush1, new PointF(353, 32));
                    g1.DrawString(strLv, font1, sbrush1, new PointF(353, 58));
                    g1.DrawString(strsoloRank, font1, sbrush1, new PointF(353, 84));
                    g1.DrawString(strRanked, font1, sbrush1, new PointF(353, 109));
                    g1.DrawString(HeroNum.ToString(), font1, sbrush1, new PointF(353, 136)); //������ͼƬ�ϵ�����x,y
                    g1.DrawString(strRpNum + "/" + strCoinNum, font1, sbrush1, new PointF(353, 162));
                    g1.DrawString(strlastPlay, font1, sbrush1, new PointF(353, 189));
                    if (!Directory.Exists(m_strCapturePath))
                        Directory.CreateDirectory(m_strCapturePath);
                    bmp.Save(m_strCapturePath + "temp.png");
                    PicAddWaterMark1(m_strCapturePath + "temp.png", LOLTempPath + (strsoloRank + ".jpg"), 29, 28, false);
                    WriteToFile("�˺���Ϣƴͼ");
                    g1.Dispose();
                    PinTu(m_strCapturePath, m_strCapturePath, "LOL1_01");
                }
            }
            catch (Exception ex)
            {
                WriteToFile(ex.ToString());
            }
            picNum = 1;//��¼��ͼ����
            return 1000;
        }
        /// <summary>
        /// ͼƬ����
        /// </summary>
        /// <param name="strFile"></param>
        /// <param name="strNewFile"></param>
        /// <param name="intWidth"></param>
        /// <param name="intHeight"></param>
        /// <returns>����:ԭ�ļ���������,���ļ���������,�µĿ��,�µĸ߶�(���߶�Ϊ0,����������)</returns>
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
                FileRW.WriteToFile(filePic + "<< �ļ������ڣ�");
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
            string chinaRank = "�޶�λ";
            if (englishrank.Contains("Challenger"))
                chinaRank = "��ǿ����";
            else if (englishrank.Contains("master"))
                chinaRank = "������ʦ";
            else if (englishrank.Contains("Diamond"))
                chinaRank = englishrank.Replace("Diamond", "����ʯ");
            else if (englishrank.Contains("Platinum"))
                chinaRank = englishrank.Replace("Platinum", "���󲬽�");
            else if (englishrank.Contains("Gold"))
                chinaRank = englishrank.Replace("Gold", "��ҫ�ƽ�");
            else if (englishrank.Contains("Silver"))
                chinaRank = englishrank.Replace("Silver", "��������");
            else if (englishrank.Contains("Bronze"))
                chinaRank = englishrank.Replace("Bronze", "Ӣ����ͭ");
            return chinaRank;
        }
        public int GetTraversalFile(string dirPath, int width, int hight)
        {
            int count = 0;
            //��ָ��Ŀ¼�����ļ�
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo Dir = new DirectoryInfo(dirPath);
                try
                {
                    foreach (FileInfo file in Dir.GetFiles())//������Ŀ¼ 
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
        /// Ӣ��ƴͼ
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="savePath"></param>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public int PinTuHero(string folderPath, string savePath, string saveName)
        {
            WriteToFile("Ӣ��ƴͼ");
            picNum++;
            int num = GetTraversalFile(folderPath, 80, 80);//��������png
            if (num % 10 != 0)
                num = num / 10 + 1;
            else
                num = num / 10;
            int width = 83;//ʵ�ʳ���160
            int height = 83;
            picResize(m_strProgPath + "\\LOLTemp\\ģ��.bmp", folderPath + "ģ��.bmp", 880, num * height);
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            Bitmap AllPic = new Bitmap(folderPath + "ģ��.bmp");
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
            if (m_strOrderType == "������")
                CreatWaterMark(savePath + saveName + ".jpg", AllPic);
            else
                AllPic.Save(savePath + saveName + ".jpg", ImageFormat.Jpeg);
            //AllPic.Save(savePath + saveName + ".jpg", ImageFormat.Jpeg);
            AllPic.Dispose();
            return 1000;
        }
        /// <summary>
        /// Ƥ��ƴͼ
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
                WriteToFile("�ļ�������");
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
        /// ����ƴͼ
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="savePath"></param>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public int PinTu(string folderPath, string savePath, string saveName)
        {
            int width = 122;
            int height = 240;
            picResize(m_strProgPath + "\\LOLTemp\\ģ��.bmp", folderPath + "ģ��.bmp", 880, 240);
            DirectoryInfo folder = new DirectoryInfo(folderPath);
            Bitmap AllPic = new Bitmap(folderPath + "ģ��.bmp");
            Graphics graph = Graphics.FromImage(AllPic);
            Image UnitPic = Bitmap.FromFile(m_strCapturePath + "temp.png");
            graph.DrawImage(UnitPic, 0, 0);

            graph.Dispose();
            UnitPic.Dispose();

            if (m_strOrderType == "������")
                CreatWaterMark(savePath + saveName + ".jpg", AllPic);
            else AllPic.Save(savePath + saveName + ".jpg", ImageFormat.Jpeg);
           
            AllPic.Dispose();
            Sleep(50);
            #region ���ִ���
            //if (m_strOrderType == "������")
            //{   
            //    CreatWaterMark(savePath + saveName + ".jpg", AllPic);
            //    
            //    //Font font1 = new Font("΢���ź�", 18, FontStyle.Bold);                       //��������ʹ�С
            //    //SolidBrush sbrush1 = new SolidBrush(Color.FromArgb(249, 204, 226));                //����������ɫ             
            //    //graph.DrawString("������������", font1, sbrush1, new PointF(353, 29));
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
            picResize(m_strProgPath + "\\LOLTemp\\ģ��.bmp", PicPath + "ģ��.bmp", 880, num * height);
            Bitmap AllPic = new Bitmap(PicPath + "ģ��.bmp");
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
                    if (m_strOrderType == "������")
                       CreatWaterMark(ServerPicName, AllPic);
                    else
                       AllPic.Save(ServerPicName, ImageFormat.Jpeg);  
                    
                    AllPic.Dispose();
                    //Sleep(200);
                    for (int j = 0; j < 3; j++)
                    {
                        File.Delete(PicPath + "ģ��.bmp");
                        if (File.Exists(PicPath + "ģ��.bmp"))
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
            int num = GetTraversalFile(folderPath, 122, 222);//��������png
            if(num%35==0)
                num=num/35;
            else
                num=num/35+1;
            int max = 35;
            WriteToFile("Ƥ��ƴͼ");
            for (int i = 0; i < num; i++)
            {
                picNum++;
                List<string> puntulist = new List<string>();//�ȶ���list����
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
            #region ��ɫ����
            Bitmap picbmp = bit;
            Rectangle srcRect = new Rectangle(0, 0, picbmp.Width >= 1000 ? 1000 : picbmp.Width, picbmp.Height >= 1000 ? 1000 : picbmp.Height);

            Bitmap mybm = new Bitmap(m_strPicPath + "����.bmp");
            int Width = mybm.Width;
            int height = mybm.Width;
            Bitmap bm = new Bitmap(Width, height);//��ʼ��һ����¼��ɫЧ����ͼƬ����
            Color pixel;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//��ȡ��ǰ���������ֵ
                    if (pixel.R == 8 && pixel.G == 8 && pixel.B == 8)
                        bm.SetPixel(x, y, Color.FromArgb(0, pixel.R, pixel.G, pixel.B));//��ͼ
                    else if (pixel.R == 42 && pixel.G == 42 && pixel.B == 42)
                        bm.SetPixel(x, y, Color.FromArgb(90, 242, 242, 242));//��ͼ
                    else if (pixel.R > 42 && pixel.G > 42 && pixel.B > 42)
                    {
                        int a = 90 - (pixel.R + pixel.G + pixel.B - 42 * 3) / 3;
                        bm.SetPixel(x, y, Color.FromArgb(a, 242, 242, 242));//��ͼ
                    }
                    else
                    {
                        int a = 90 + (pixel.R + pixel.G + pixel.B - 42 * 3) / 3;
                        bm.SetPixel(x, y, Color.FromArgb(a, 242, 242, 242));//��ͼ
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
        /// �жϱ��ص�����״̬
        /// </summary>
        /// <returns>true:��������;false:�������߱�������</returns>
        private static bool LocalConnectionStatus()
        {
            System.Int32 dwFlag = new Int32();
            if (!InternetGetConnectedState(ref dwFlag, 0))
            {
                Console.WriteLine("LocalConnectionStatus--δ����!");
                return false;
            }
            else
            {
                if ((dwFlag & INTERNET_CONNECTION_MODEM) != 0)
                {
                    WriteToFile("LocalConnectionStatus--���õ��ƽ��������������");
                    return true;
                }
                else if ((dwFlag & INTERNET_CONNECTION_LAN) != 0)
                {
                    WriteToFile("LocalConnectionStatus--��������������");
                    return false;
                }
            }
            return false;
        }
    }
}
