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
using System.Text.RegularExpressions;//����
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
        #region ��ʼ��ͨ�ű���
        //��������
        public static string m_strOrderData;
        //udpsockets
        public static udpSockets udpdd;
        //��֤��ش����
        public int yzmTimes = 0;
        //�ű��˿�
        public static int m_UDPPORT = 6801;
        //RC����˿�
        public static int the_nRC2Port = 1;
        //��������-���׵�/������
        public static string m_strOrderType;
        //������
        public static string OrdNo = "���Զ���"; //"MZH-160607000000001";
        //����״̬
        public int Status;
        #endregion

        #region ��ʼ���������

        //��ͼĬ�ϳߴ�
        bool zhongduan = false;
        private static Thread jiankong;
        Point ptMove = new Point(-25, -10);
        public const int Lwidth = 1280;
        public const int Lheight = 1200;
        public const int nZHPicWidth = 880;
        //ƴͼ�Ƿ�����
        public bool bPicFull = false;
        public Point ptBigPic;
        //�Ƿ�����¹���
        bool isTest = false;
        //װ����ͼ�ߴ�
        public Point ptMAX;
        static int PicNum = 1;
        static string strLastPicID = "";
        string m_strLastName;
        int m_nPicNum;
        static int picNum = 0;
        public int time = 0;
        public int QWE = 0;
        //ƴͼ�Ҳ���ͼƬ����
        int C = 0;
        //struct CoinStruct
        //{
        //    public int x;
        //    public String no;
        //}
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
        //��֤�루jpg��
        string mousea;
        //���ھ��
        public static IntPtr m_hGameWnd;
        //�л����
        public static IntPtr m_hChangeWnd;
        //��������·��
        private string m_strProgPath = System.Windows.Forms.Application.StartupPath;
        //ƥ��ͼƬ·��
        public static string m_strPicPath = System.Windows.Forms.Application.StartupPath + @"\�λ�����\";
        //�쳣��ͼ����·��
        public static string LocalPicPath = "E:\\�λ�����2�˺Ž�ͼ\\";
        public int AccType;
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
        public string IsNeedRecognition;
        public static StringBuilder strb = new StringBuilder();
        #endregion
        //���������
        public void StartToWork()
        {
            #region ����������

            User32API.WinExec("rasphone -h �������", 2); //��
            Sleep(1000);
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
                    WriteToFile("���󲦺�ʧ�ܲ����в���");
                    Game.tskill("rasphone");
                    Sleep(500);
                    if (User32API.FindWindow(null, "���ӵ� ������� ʱ����") == IntPtr.Zero)
                        break;
                }
                if (User32API.FindWindow(null, "��������") != IntPtr.Zero)
                {
                    WriteToFile("����ʧ�ܲ����в���");
                    Game.tskill("rasphone");
                    if (User32API.FindWindow(null, "��������") == IntPtr.Zero)
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
                CaptureJpg("����ʧ��");
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
                tmp = string.Format("��ͼ�ɹ�,��{0}��\r\n", picNum);
                WriteToFile(tmp);
            }

            CloseGames();
            tmp = string.Format("�ƽ�״̬={0}\r\n", Status);
            WriteToFile(tmp);
            tmp = string.Format("FStatus={0}\r\n", Status);

            #region//�ټ�¼������� ������ʧ��5���������ԣ�Ƶ���������¶�����ʱ��
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
            try
            {
                jiankong.Abort();
            }
            catch { };
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

            #region �˺��������
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
            #endregion

            #region ���������ж�
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
                WriteToFile("δ��д��ȷ�������ʽ��Ĭ�Ϻ�׺@163.com");
                AccType = 0;
            }
            try
            {
                m_strAccount = m_strAccount.Substring(0, m_strAccount.IndexOf("@"));
            }
            catch
            { }
            AppInit();//IP��ַ �汾��
            jiankong = new Thread(new ThreadStart(fun1));
            jiankong.Start();
            #endregion

            for (int i = 0; i < 3; i++)
            {
                //-------------------------------------
                //��������
                //Sleep(3000);
                //GameCarPic();

                #region ƴͼ����
                //PinTu("��������,ʦ�ż���,��������,����,��������,�ٻ�������", "MHXY1", false);
                //PinTu("��������,�ٻ�������,��������", "MHXY2", false);
                //PinTu("��������,��������", "MHXY2", false);
                //PinTu("װ��1,װ��2,����,װ��3,װ��4,����,װ��5,װ��6", "MHXY3", false);
                //PinTu("װ����װ", "MHXY3", false);
                //PinTu("����", "MHXY4", false);
                //PinTu("����", "MHXY4", false);
                //PinTu("�ҵĳɾ�", "MHXY5", false);
                //PinTu("�������ɾ�,�������ɾ�1", "MHXY5", false);
                //PinTu("����", "MHXY6", false);
                //PinTu("��������,���＼��", "MHXY6", false);
                #endregion

                #region ��ͼ����
                //Point pa = new Point(-1, -2);
                //pa = ImageTool.fPic(m_strPicPath + "װ���ϱ�.bmp", 0, 0, 0, 0, 30);//����
                //pa = ImageTool.fPic(m_strPicPath + "װ���±�.bmp", 0, 0, 0, 0, 30);//����
                //pa = ImageTool.fPic(m_strPicPath + "���3.bmp", 0, 0, 0, 0, 40);//����
                //if (pa.X > 0)
                //{
                //    WriteToFile("���³ɹ�");
                //    KeyMouse.MouseClick(m_hGameWnd, pa.X + 15, pa.Y + 15, 1, 1, 1000);
                //    return 1;
                //}
                #endregion

                #region ���ֲ���
                //m_strArea = "����ǰ��ر������������еĿͻ���";
                //Point pa = new Point(-1, -2);
                //RECT rtA = new RECT(583, 763, 880, 900);
                //for (int a = 0; a < 10; a++)
                //{
                //    pa = ImageTool.FindText(m_hGameWnd, m_strArea, Color.FromArgb(255, 255, 0), "����", 16, FontStyle.Regular, 0, 0, rtA, 30);
                //    if (pa.X > 0)
                //    {
                //        KeyMouse.MouseClick(pa.X + 5, pa.Y + 5, 1, 1, 500);
                //        break;
                //    }
                //    Sleep(200);
                //}
                #endregion

                //--------------------------------------

                #region ����һ���Զ���¼��Ϸ
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
        // ����TGP
        public int RunGame()
        {
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            for (int i = 0; i < 5; i++)
            {
                m_GameTitle = "�λ����� ONLINE";
                m_hGameWnd = User32API.FindWindow(null, m_GameTitle);
                //���Ϊ�գ�����ָ��·������Ϸ
                if (m_hGameWnd == IntPtr.Zero)
                {
                    myapp.RunBat(m_strGameStartFile);
                    WriteToFile(m_strGameStartFile);
                    m_GameTitle = "�λ����� ONLINE";
                    m_hGameWnd = User32API.FindWindow(null, m_GameTitle);
                    Sleep(1000);
                    if (m_hGameWnd == IntPtr.Zero)
                        continue;
                }

                //Ѱ�ҵ�¼����
                for (int y = 0; y < 60; y++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "�������¼.bmp", 0, 0, 0, 0, 80);
                    pb = ImageTool.fPic(m_strPicPath + "�������¼1.bmp", 0, 0, 0, 0, 80);
                    if (pa.X > 0 || pb.X > 0)
                    {
                        pa = pa.X > 0 ? pa : pb;
                        WriteToFile("���³ɹ�");
                        KeyMouse.MouseClick(m_hGameWnd, pa.X + 15, pa.Y + 15, 1, 1, 1000);
                        return 1;
                    }
                    if (y % 8 == 0)
                    {
                        WriteToFile("�ȴ���Ϸ����");
                    }
                    Sleep(2000);
                }

            }
            WriteToFile("��Ϸ�������ʧ��");
            return 2260;
        }
        //�����ʺ�����      
        public int EnterAccPwd()
        {
            //--------------------------------------------------

            // -------------------------------------------------
            #region ��ʼ������
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            Point pc = new Point(-1, -2);
            RECT rtC = new RECT(560, 525, 704, 550);
            RECT rtCC = new RECT(580, 570, 886, 679);
            m_hGameWnd = User32API.GetDesktopWindow();
            #endregion
            for (int i = 0; i < 10; i++)
            {
                pa = ImageTool.fPic(m_strPicPath + "�����.bmp", 347, 418, 508, 516);
                if (pa.X > 0)
                {
                    KeyMouse.MouseClick(pa.X + 618, pa.Y + 375, 1, 1, 1000);
                    break;
                }
                if(i%4==0)
                    WriteToFile("�ȴ��������");
                Sleep(1000);
            }
            if (pa.X < 0)
            {
                CaptureJpg("�Ҳ��������");
                WriteToFile("����������ʧ��");
                return 2260;
            }

            for (int i = 0; i < 5; i++)
            {
                Sleep(3000);
                pa = ImageTool.fPic(m_strPicPath + "�˺Ž����־.bmp", 798, 509, 852, 557);
                if (pa.X > 0)
                {
                   
                    pc = ImageTool.FindText(m_hGameWnd, "����ǰ��ر������������еĿͻ���", Color.FromArgb(255, 255, 0), "����", 16, FontStyle.Regular, 0, 0, rtCC, 30);
                    if (pc.X > 0)
                    {
                        WriteToFile("�رղ��Է����ѣ����Ե�½����");
                        KeyMouse.MouseClick(793, 660, 1, 1, 500);
                    }
                    for (int j = 1; j < 4; j++)
                    {

                        #region �����˺����벢��½
                        KeyMouse.MouseClick(pa.X - 23, pa.Y + 11, 1, 1, 500); //����˺������
                        KeyMouse.SendBackSpaceKey(30);
                        WriteToFile("�����˺�����");
                        KeyMouse.SendKeys(m_strAccount, 200);
                        Sleep(500);
                        KeyMouse.MouseClick(pa.X - 67, pa.Y + 35 + AccType * 18, 1, 1, 500);  //--�˺ź�׺����ѡ�����,Ĭ��163.com
                        KeyMouse.MouseClick(pa.X - 23, pa.Y + 51, 1, 1, 500);
                        KeyMouse.SendBackSpaceKey();
                        //WriteToFile("��������");
                        KeyMouse.SendKeys(m_strPassword, 300);
                        CaptureJpg("�˺�����");
                        WriteToFile("�����˺ţ�[" + m_strAccount + "]" + "����[" + m_strPassword.Length.ToString() + "]λ���");
                        Sleep(1000);
                        KeyMouse.MouseClick(pa.X - 30, pa.Y + 122, 1, 1, 500);//�����¼

                        #endregion

                        #region �˺ŵ�¼�쳣�ж�
                        for (int k = 0; k < 7; k++)
                        {
                            pb = ImageTool.fPic(m_strPicPath + "���벻��ȷ.bmp", 718, 584, 781, 633);
                            if (pb.X > 0)
                                break;
                            pc = ImageTool.FindText(m_hGameWnd, "���ʺ��Ѿ�������", Color.FromArgb(255, 255, 255), "����", 16, FontStyle.Regular, 0, 0, rtC, 30);
                            if (pc.X > 0)
                            {
                                WriteToFile("���ʺ��Ѿ�������");
                                return 3700;
                            }

                            Sleep(300);
                        }
                        if (pb.X > 0)
                        {
                            WriteToFile("��" + j.ToString() + "���ʺ��������");
                            if (j == 3)
                            {
                                CaptureJpg("�����3��");
                                return 3000;
                            }
                            Sleep(500);
                            KeyMouse.MouseClick(pb.X - 17, pb.Y + 66, 1, 1, 500);
                            continue;
                        }

                        #endregion

                        #region ���������־�жϣ�return 1
                        for (int k = 0; k < 3; k++)
                        {
                            pb = ImageTool.fPic(m_strPicPath + "���������־.bmp", 411, 440, 919, 713);
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
                CaptureJpg("�Ҳ����ʺŽ����־");
                WriteToFile("�˺Ž������ʧ��");
                return 2260;
            }
            WriteToFile("δ֪�����¼ʧ��");
            CaptureJpg("δ֪�����¼ʧ�ܣ��Ҳ�������ֵ");
            return 2120;
        }
        public int SelectServer()
        {
            #region ��ʼ������
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
            //���ֲ���
            // -------------------------------------------------

            for (int i = 0; i < 5; i++)
            {
                pb = ImageTool.fPic(m_strPicPath + "���������־.bmp", 411, 440, 919, 713);
                if (pb.X > 0)
                {
                    KeyMouse.MouseMove(m_hGameWnd, 0, 0);
                    WriteToFile("ѡ������");
                    #region Ѱ�����н�ɫ������
                    for (int a = 0; a < 10; a++)
                    {
                        pa = ImageTool.FindText(m_hGameWnd, m_strServer, Color.FromArgb(0, 0, 0), "����", 14, FontStyle.Regular, 0, 0, rtH, 30);
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
                        //--��������δ���
                        #region Ѱ�Ҵ���
                        for (int a = 0; a < 10; a++)
                        {
                            pa = ImageTool.FindText(m_hGameWnd, m_strArea, Color.FromArgb(255, 255, 255), "����", 14, FontStyle.Regular, 0, 0, rtA, 30);
                            if (pa.X > 0)
                            {
                                KeyMouse.MouseClick(pa.X + 5, pa.Y + 5, 1, 1, 500);
                                break;
                            }
                            Sleep(200);
                        }
                        if (pa.X < 0)
                        {
                            WriteToFile("�Ҳ���������" + m_strArea);
                            CaptureJpg("�Ҳ���������" + m_strArea);
                            return 2260;
                        }
                        #endregion

                        #region Ѱ�Ҿ�������
                        for (int a = 0; a < 10; a++)
                        {
                            pa = ImageTool.FindText(m_hGameWnd, m_strServer, Color.FromArgb(0, 0, 0), "����", 14, FontStyle.Regular, 0, 0, rtS, 30);
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
                            WriteToFile("�Ҳ���������" + m_strServer);
                            CaptureJpg("�Ҳ���������" + m_strServer);
                            return 2260;
                        }
                        #endregion
                    }

                    #region �����������쳣�ж�
                    WriteToFile("���������쳣����");
                    for (int a = 0; a < 20; a++)
                    {
                        //��������æ
                        pa = ImageTool.fPic(m_strPicPath + "��������æ.bmp", 555, 508, 666, 561);
                        if (pa.X > 0)
                            break;

                        //��������æ
                        pa = ImageTool.fPic(m_strPicPath + "�Ŷ��ж�����.bmp", 556, 501, 861, 588);
                        if (pa.X > 0)
                        {
                            WriteToFile("�Ŷ��ж�����");
                            return 2370;
                        }

                        //�������ȴ�����
                        pa = ImageTool.fPic(m_strPicPath + "�ȴ����ӷ�����.bmp", 558, 583, 805, 635);
                        if (pa.X > 0)
                        {
                            WriteToFile("�ȴ����ӷ�����");
                            WaitTime++;
                            Sleep(5000);
                            if (WaitTime > 4)
                            {
                                WriteToFile("���ӷ�������ʱ");
                                return 2370;
                            }
                            continue;
                        }
                        //���Է����汾
                        pa = ImageTool.FindText(m_hGameWnd, "����ǰ��ر������������еĿͻ���", Color.FromArgb(255, 255, 0), "����", 16, FontStyle.Regular, 0, 0, rtCC, 30);
                        if (pa.X > 0)
                        {
                            WriteToFile("�˺����ڷ�����Ϊ���Է�,�޷���¼�߰汾�ͻ���,ת�˹�");
                            KeyMouse.MouseClick(793, 660, 1, 1, 500);
                            return 3333;
                        }
                        //�������޽�ɫ
                        pa = ImageTool.fPic(m_strPicPath + "�޽�ɫ.bmp", 356, 443, 433, 471);
                        if (pa.X > 0)
                        {
                            WriteToFile("�������޽�ɫ");
                            return 3010;
                        }
                        #region �ж������������־��return 1
                        pa = ImageTool.fPic(m_strPicPath + "��ɫ�����־1.bmp", 612, 231, 832, 394);
                        pc = ImageTool.fPic(m_strPicPath + "��ɫ�����־2.bmp", 469, 748, 546, 789);
                        if (pa.X > 0 || pc.X > 0)
                            return 1;
                        #endregion
                        Sleep(300);
                    }
                    if (pa.X > 0)
                    {
                        KeyMouse.MouseClick(pa.X + 162, pa.Y + 135, 1, 1, 1000);
                        WriteToFile("��������æ,����ѡ������");
                        Sleep(2000);
                        continue;
                    }
                    #endregion
                }
                Sleep(1000);
            }
            if (pb.X < 0)
            {
                WriteToFile("����������ʧ");
                CaptureJpg("����������ʧ");
                return 2260;
            }
            WriteToFile("��ν���,��������æ,���ӷ�������ʱ");
            CaptureJpg("��������æ");
            return 2370;
        }
        public int SelectRole()
        {
            #region ��ʼ������
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            Point pc = new Point(-1, -2);
            RECT rtR = new RECT(547, 757, 651, 781);
            RECT rtA = new RECT(566, 543, 869, 587);
            m_hGameWnd = User32API.GetDesktopWindow();
            #endregion

            #region �㿨Ƿ�Ѳ�ѯ
            WriteToFile("���㿨�Ƿ�Ƿ��");
            for (int a = 0; a < 10; a++)
            {
                pa = ImageTool.FindText(m_hGameWnd, "����ʺ�Ƿ��300��", Color.FromArgb(255, 255, 255), "����", 16, FontStyle.Regular, 0, 0, rtA, 30);
                if (pa.X > 0)
                {
                    WriteToFile("�㿨Ƿ���Ѵ�����,�޷�������Ϸ");
                    return 3500;
                }
                pa = ImageTool.FindText(m_hGameWnd, "����ʺ�Ƿ��", Color.FromArgb(255, 255, 255), "����", 16, FontStyle.Regular, 0, 0, rtA, 30);
                if (pa.X > 0)
                {
                    string qianfei = CheckPic1(rtA.left, rtA.top, rtA.right, rtA.bottom);
                    WriteToFile("�ʺŵ㿨��Ƿ��" + qianfei);
                    if (Convert.ToInt32(qianfei) > 300)
                    {
                        WriteToFile("�㿨Ƿ���Ѵ�����,�޷�������Ϸ");
                        return 3500;
                    }
                    KeyMouse.MouseClick(pa.X + 153, pa.Y + 84, 1, 1, 500);
                    break;
                }
                Sleep(200);
            }
            #endregion

            #region Ѱ��ָ����ɫ��
            for (int i = 0; i < 6; i++)
            {
                if (i != 0)
                    KeyMouse.MouseClick(680 + 100 * (i - (i / 3) * 3), 500 + 120 * (i / 3), 1, 1, 500);
                for (int a = 0; a < 15; a++)
                {
                    pa = ImageTool.FindText(m_hGameWnd, m_strSellerRole, Color.FromArgb(255, 255, 255), "����", 14, FontStyle.Regular, 0, 0, rtR, 30);
                    if (pa.X > 0)
                    {
                        WriteToFile("��ʵ��ɫ��" + pa.X.ToString() + "," + pa.Y.ToString());
                        if (!ImageTool.CheckRGB(pa.X - 4, pa.Y, pa.X - 1, pa.Y + 17, 255, 255, 255, 30))//ƥ���Ƿ����ǰ׺
                        {
                            Sleep(300);
                            if (!ImageTool.CheckRGB(pa.X + ImageTool.checkrole + 1, pa.Y, pa.X + ImageTool.checkrole + 4, pa.Y + 17, 255, 255, 255, 30))//ƥ���Ƿ����ǰ׺
                            {
                                KeyMouse.MouseClick(680 + 100 * (i - (i / 3) * 3), 500 + 120 * (i / 3), 1, 2, 500);
                                WriteToFile("�ҵ�ָ����ɫ��" + m_strSellerRole);
                                return 1;
                            }
                        }
                        WriteToFile("����ǰ׺���ߺ�׺");
                    }
                    Sleep(200);
                }
            }

            WriteToFile("δ�ҵ�ָ��������,��Ĭ�Ͻ�ɫ");
            KeyMouse.MouseClick(680, 500, 1, 2, 500);
            return 1;
            #endregion
        }
        public int GameCarPic()
        {

            #region �ж��Ƿ������Ϸ���˶���������ɫ
            for (int i = 0; i < 60; i++)
            {
                Sleep(500);
                if (WinTitle().Contains(m_strArea) && WinTitle().Contains(m_strServer))
                {
                    WriteToFile("������Ϸ�ɹ����˶�������ȷ");
                    if (WinTitle().Contains(m_strSellerRole))
                        WriteToFile("�˶��û�����ȷ");
                    break;
                }
                if (i % 19 == 0)
                    WriteToFile("�ȴ�������Ϸ�˶�����");
                if (i == 59)
                {
                    WriteToFile("������Ϸ��ʱ��˶�����ʧ��");
                    WriteToFile(WinTitle());
                    return 2260;
                }
                
            }
            User32API.SetForegroundWindow(m_hGameWnd);
            #endregion

            #region ��ʼ������
            Point pa = new Point(-1, -2);
            Point pb = new Point(-1, -2);
            Point pc = new Point(-1, -2);
            Point pzb = new Point(-1, -2);
            Point pzb1 = new Point(-1, -2);
            RECT rt = new RECT(775, 301, 856, 321);
            #endregion

            #region �������ԡ����ܡ��澭���� ��ͼ �ٻ�������

            #region �ȴ����ﴰ�ڼ���,�������Խ�ͼ
            for (int i = 0; i < 3; i++)
            {
                Sleep(2000);
                KeyMouse.SendAltAny('w');
                for (int j = 0; j < 6; j++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "��������.bmp", rt);
                    if (pa.X > 0)
                        break;
                    Sleep(500);
                }
                if (pa.X > 0)
                    break;
            }
            if (pa.X < 0)
            {
                WriteToFile("�Ҳ���[��������]");
                CaptureJpg("�Ҳ���[��������]");
                return 2260;
            }
            KeyMouse.MouseMove(600, 366);
            Sleep(1000);
            CaptureBmpInRect("��������", pa.X - 106, pa.Y - 4, pa.X + 174, pa.Y + 402, pa.X - 59, pa.Y + 29, pa.X + 61, pa.Y + 45);
            #endregion

            #region �������ܽ�ͼ
            for (int i = 0; i < 3; i++)
            {
                MyClicklast(pa.X - 123, pa.Y + 211);
                for (int k = 0; k < 6; k++)
                {
                    pb = ImageTool.fPic(m_strPicPath + "��������.bmp", rt);
                    if (pb.X > 0)
                        break;
                    Sleep(300);
                }
                if (pb.X > 0)
                    break;
            }
            if (pb.X < 0)
            {
                WriteToFile("�Ҳ���[��������]");
                return 2260;
            }

            CaptureBmpInRect("��������", pa.X - 106, pa.Y - 4, pa.X + 174, pa.Y + 402);
            #endregion

            #region �������ܽ�ͼ
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 6; k++)
                {
                    MyClicklast(pa.X - 123, pa.Y + 280);
                    pb = ImageTool.fPic(m_strPicPath + "��������.bmp", rt);
                    if (pb.X > 0)
                        break;
                    Sleep(300);
                }
                if (pb.X > 0)
                    break;
            }
            if (pb.X < 0)
            {
                WriteToFile("�Ҳ���[��������]");
                return 2260;
            }
            CaptureBmpInRect("��������", pa.X - 106, pa.Y - 4, pa.X + 174, pa.Y + 402);
            #endregion

            #region ʦ�ż��ܽ�ͼ
            for (int i = 0; i < 3; i++)
            {
                for (int k = 0; k < 6; k++)
                {
                    MyClicklast(pa.X - 123, pa.Y + 140);
                    pb = ImageTool.fPic(m_strPicPath + "ʦ�ż���.bmp", rt);
                    if (pb.X > 0)
                        break;
                    Sleep(300);
                }
                if (pb.X > 0)
                    break;
            }
            if (pb.X < 0)
            {
                WriteToFile("�Ҳ���[ʦ�ż���]");
                return 2260;
            }
            CaptureBmpInRect("ʦ�ż���", pa.X - 106, pa.Y - 4, pa.X + 174, pa.Y + 402);
            #endregion

            #region �澭������ͼ
            for (int i = 0; i < 3; i++)
            {
                MyClicklast(pb.X + 62, pb.Y + 348);
                for (int k = 0; k < 6; k++)
                {
                    pc = ImageTool.fPic(m_strPicPath + "�澭����.bmp", 600, 300, 690, 330);
                    if (pc.X > 0)
                        break;
                    Sleep(300);
                }
                if (pc.X > 0)
                    break;
            }
            if (pc.X < 0)
            {
                WriteToFile("�Ҳ���[�澭����]");
                return 2260;
            }
            CaptureBmpInRect("�澭����", pc.X - 277, pc.Y - 1, pc.X + 349, pc.Y + 461);
            Sleep(500);

            #endregion

            #region �ȴ��ٻ��޴��ڼ���,���Խ�ͼ
            for (int i = 0; i < 3; i++)
            {
                KeyMouse.SendAltAny('o');
                for (int j = 0; j < 6; j++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "�ٻ�������.bmp", 453,298,553,322);
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
                WriteToFile("�Ҳ���[�ٻ�������]");
                CaptureJpg("�Ҳ���[�ٻ�������]");
                return 2260;
            }
            CaptureBmpInRect("�ٻ�������", pa.X - 142, pa.Y - 3, pa.X + 226, pa.Y + 448);
            #endregion
            #endregion

            #region ���� ���� ����  ���� �ٻ��� װ�� ��װ ���� ���� ������װ

            #region ���� ���� ���� �ٻ��� ����
            for (int i = 0; i < 3; i++)
            {
                KeyMouse.SendAltAny('e');
                for (int j = 0; j < 6; j++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "��������.bmp", 595, 370, 686, 394);
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
                WriteToFile("�Ҳ���[��������]");
                CaptureJpg("�Ҳ���[��������]");
                return 2260;
            }
            CaptureBmpInRect("��������", pa.X - 230, pa.Y + 20, pa.X + 18, pa.Y + 325);
            MyClicklast(pa.X - 140, pa.Y + 35);
            Sleep(1000);
            CaptureBmpInRect("�ٻ�������", pa.X - 230, pa.Y + 20, pa.X + 18, pa.Y + 325);
            MyClicklast(pa.X - 78, pa.Y + 35);
            Sleep(1000);
            CaptureBmpInRect("��������", pa.X - 230, pa.Y + 20, pa.X + 18, pa.Y + 325);
            MyClicklast(pa.X + 69, pa.Y + 34);
            Sleep(1000);
            CaptureBmpInRect("��������", pa.X + 16, pa.Y + 20, pa.X + 300, pa.Y + 325);
            MyClicklast(pa.X + 150, pa.Y + 34);
            Sleep(1000);
            CaptureBmpInRect("��������", pa.X + 16, pa.Y + 20, pa.X + 300, pa.Y + 325);
            CaptureJpg("��������");

            #endregion

            #region װ����ͼ װ����װ

            //------------װ��-----------
            MyClicklast(pa.X -206 , pa.Y + 34);
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    MyClicklast1(405 + i * 190, 514 + 61 * j);
                    for(int z=0;z<10;z++)
                    {
                        pzb = ImageTool.fPic(m_strPicPath + "װ���ϱ�.bmp", 280, 250, 1000, 800, 30);
                        pzb1 = ImageTool.fPic(m_strPicPath + "װ���±�.bmp", pzb.X > 0 ? pzb.X : 280, pzb.Y > 0 ? pzb.Y : 250, 1000, 800, 30);
                        if(pzb.X>0&&pzb1.X>0)
                        {
                            CaptureBmpInRect("װ��" + (i * 3 + j + 1).ToString(), pzb.X+1, pzb.Y+1, pzb1.X + 19, pzb1.Y + 64);
                            break;
                        } 
                        Sleep(500);
                    }
                    if(pzb.X<0&&pzb1.X<0)
                    {
                        WriteToFile((i*3+j+1).ToString()+"����װ��");
                        CaptureJpg((i * 3 + j + 1).ToString() + "����װ��");
                    }
                    else if (pzb.X < 0 && pzb1.X > 0)
                    {
                        WriteToFile("�Ҳ���"+(i*3+j+1).ToString()+"���±�");
                        CaptureJpg("�Ҳ���" + (i * 3 + j + 1).ToString() + "���±�");
                    }
                    else if (pzb.X > 0 && pzb1.X < 0)
                    {
                        WriteToFile("�Ҳ���" + (i * 3 + j + 1).ToString() + "���ϱ�");
                        CaptureJpg("�Ҳ���" + (i * 3 + j + 1).ToString() + "���ϱ�");
                    }
                }
            }
            //---------------------------

            MyClicklast(pa.X - 63, pa.Y + 279);
            Sleep(1000);
            CaptureBmpInRect("װ����װ", pa.X - 233, pa.Y - 72, pa.X + 46, pa.Y + 261);
            MyClicklast(pa.X - 63, pa.Y + 279);

            #endregion

            #region ���½�ͼ
            for (int i = 0; i < 3; i++)
            {
                MyClicklast(pa.X - 142, pa.Y + 307);
                for (int k = 0; k < 6; k++)
                {
                    pb = ImageTool.fPic(m_strPicPath + "����.bmp", 300, 360, 681, 408);
                    if (pb.X > 0)
                        break;
                    Sleep(500);
                }
                if (pb.X > 0)
                    break;
            }
            if (pb.X < 0)
            {
                WriteToFile("�Ҳ���[����]");
                return 2260;
            }
            CaptureBmpInRect("����", pb.X - 259, pb.Y - 5, pb.X + 295, pb.Y + 331);
            Sleep(500);
            #endregion 

            #region ������ͼ
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
                        pa = ImageTool.fPic(m_strPicPath + "��������.bmp", 595, 373, 686, 394);
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
                    pb = ImageTool.fPic(m_strPicPath + "����.bmp", 618, 379, 660, 405);
                    if (pb.X > 0)
                        break;
                    Sleep(300);
                }
                if (pb.X > 0)
                    break;
            }
            if (pa.X < 0)
            {
                WriteToFile("�Ҳ���[��������]");
                return 2260;
            }
            if (pb.X < 0)
            {
                WriteToFile("�Ҳ���[����]");
                return 2260;
            }
            CaptureBmpInRect("����", pb.X - 253, pb.Y - 86, pb.X + 289, pb.Y + 314);
            MyClicklast(pb.X - 34, pb.Y + 36);
            Sleep(500);
            #endregion

            #endregion

            #region �ɳ����� �ɾͼ���

            #region �ɳ�����
            for (int i = 0; i < 3; i++)
            {
                Sleep(2000);
                KeyMouse.SendAltAny('n');
                for (int j = 0; j < 6; j++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "��������.bmp", 550,300,700,360);
                    if (pa.X > 0)
                        break;
                    Sleep(500);
                }
                if (pa.X > 0)
                    break;
            }
            if (pa.X < 0)
            {
                WriteToFile("�Ҳ���[��������]");
                CaptureJpg("�Ҳ���[��������]");
                return 2260;
            }
            CaptureBmpInRect("�ҵĳɾ�", pa.X - 249, pa.Y - 5, pa.X + 317, pa.Y + 437);
            Sleep(500);
            #endregion

            #region �ɾͼ���
            MyClicklast(pa.X + 312, pa.Y + 166);
            Sleep(1000);
            MyClicklast(pa.X - 167, pa.Y + 53);//���
            Sleep(1000);
            MyClicklast(pa.X - 161, pa.Y + 82);//�������ɾ�
            Sleep(2000);
            CaptureBmpInRect("�������ɾ�", pa.X - 249, pa.Y - 5, pa.X + 317, pa.Y + 437);
            MyClicklast(pa.X - 74, pa.Y + 319);//�������ɾ�����
            Sleep(2000);
            CaptureBmpInRect("�������ɾ�1", pa.X - 249, pa.Y - 5, pa.X + 317, pa.Y + 437);
            #endregion

            #endregion

            #region ���� �������� ���＼��

            #region ����
            for (int i = 0; i < 3; i++)
            {
                Sleep(2000);
                KeyMouse.SendAltAny('p');
                for (int j = 0; j < 6; j++)
                {
                    pa = ImageTool.fPic(m_strPicPath + "����.bmp", 548, 324, 631, 348);
                    if (pa.X > 0)
                        break;
                    Sleep(500);
                }
                if (pa.X > 0)
                    break;
            }
            if (pa.X < 0)
            {
                WriteToFile("�Ҳ���[����]");
                CaptureJpg("�Ҳ���[����]");
                return 2260;
            }
            CaptureBmpInRect("����", pa.X - 142, pa.Y - 5, pa.X + 176, pa.Y + 186);
            Sleep(500);
            #endregion

            #region �������� ���＼��
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
                        pa = ImageTool.fPic(m_strPicPath + "��������.bmp", 595, 373, 686, 394);
                        if (pa.X > 0)
                            break;
                        Sleep(500);
                    }
                    if (pa.X < 0)
                        continue;
                }
                MyClicklast(pa.X - 78, pa.Y + 35);
                Sleep(1000);
                MyClicklast(pa.X - 146, pa.Y + 226);//��������
                CaptureJpg("��������");              
            }

            Sleep(500);
            #endregion

            #endregion

            #region ƴͼ
            WriteToFile("��ʼƴͼ");
            PinTu("��������,ʦ�ż���,��������,����,��������,�ٻ�������","MHXY1",false);
            PinTu("��������,�ٻ�������,��������", "MHXY2", false);
            PinTu("��������,��������", "MHXY2", false);
            PinTu("װ��1,װ��2,����,װ��3,װ��4,����,װ��5,װ��6", "MHXY3", false);
            PinTu("װ����װ", "MHXY3", false);
            PinTu("����", "MHXY4", false);
            PinTu("����", "MHXY4", false);
            PinTu("�ҵĳɾ�", "MHXY5", false);
            PinTu("�������ɾ�", "MHXY5", false);
            PinTu("�������ɾ�1", "MHXY5", false);
            PinTu("����", "MHXY6", false);
            PinTu("��������,���＼��", "MHXY6", false);
            PinTu("�澭����", "MHXY7", false);
            #endregion

            return 1000;

        }
        /// <summary>
        /// �ر���Ϸ
        /// </summary>
        public void CloseGames()
        {
            Game.RunCmd("taskkill /im  my.exe /F");
            Game.RunCmd("taskkill /im  mhtab.exe /F");
            Game.RunCmd("taskkill /im  mhmain.exe /F");
            Game.RunCmd("taskkill /im  xyqsvc.exe /F");
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
                try
                {
                    WriteToFile(m_RegInfos);
                }
                catch { WriteToFile("�����������ʧ��"); }
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
            System.Diagnostics.Process.Start("shutdown", @"/r");
#endif
            return;
        }
        /// <summary>
        /// �����˹�����
        /// </summary>
        /// <param name="CodeType">���ݸ�ʽ</param>
        /// <param name="ImagePath">ͼƬ·��</param>
        /// <param name="Explain"></param>
        /// <param name="time">���ⳬʱʱ�䣺��λ��</param>
        /// <returns>������</returns>
        public string RequestSafeCardInfo(int CodeType, string ImagePath, string Explain, int time)
        {
            #region ˵��
            //���󶩵����� & ���ն������� & ����ȷ��
            //	TRANS_REQ_IDCODE_RESULT  = 30,    //����������GTR������֤��               ( ROBOT -> RC2 ) 
            //TRANS_RES_IDCODE_RESULT  = 31,    //���ʹ��������֤��ĵ������˳���      ( RC2 -> ROBOT ) 
            //TRANS_IDCODE_INPUT_RESULT = 32,   //������������֤���Ľ�����͸��ͻ���  ( ROBOT -> RC2 )
            // 
            //30 ���ݸ�ʽ:
            //FCodeType=  ��������(����Ϊ��)
            //1. ������֤��.
            //2. �ܱ���֤��.
            //3. ������֤��.
            //FImageName= ��֤��ͼƬ�ļ���ȫ·��(����Ϊ��)
            //FQuestion=  һЩ˵���ı�(��Ϊ��) 
            //FTimeout=   ��ʱֵ(��λ��)
            //FSmsMobile=%s\r\n
            //FSmsValue=%s\r\n
            //FSmsAddress=%s\r\n
            #endregion
            if (OrdNo == "���Զ���")
            {
                Console.Write("�������ܱ���");
                return Console.ReadLine();
            }
            IsAnswer = false;
            string strSendData;
            WriteToFile("������֤��...");
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
                    tmp = string.Format("���ⷵ��:{0}", m_strOrderData);
                    WriteToFile(tmp);
                    return m_strOrderData;
                }
                Sleep(1000);
                if (i % 20 == 15)
                    WriteToFile("�ȴ���֤��...");
            }
            WriteToFile("�ȴ���֤�볬ʱ...");
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
            //WriteToFile("�Զ����ⷵ��:" + code);
        }
        //��ȡ���ݣ����ͷ�����(M)
        public string AutoVerifya()
        {
            return "";
        }
        //��ȡ���ݣ����ͷ�����
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
            newStream.Write(byteArray, 0, byteArray.Length); //д����� 
            newStream.Close();
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);//Encoding.Default
            string textResponse = sr.ReadToEnd(); // ���ص�����
            return textResponse;
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
                LocalPicPath = "C:\\�λ�����2�˺Ž�ͼ\\";
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
                //���·�����쳣��·����Ϊ "Z:\\jiaoben\\"
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
                    WriteToFile("���Ӵﵽ���");
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
                    WriteToFile("���Ӵﵽ���");
                }
            }
            else
                return;
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
        /// bmp��ͼ
        /// </summary>
        /// <param name="bm">bmpͼƬ��</param>
        /// <param name="strPicName">��ͼ����</param>
        /// <param name="left">�߽磺��</param>
        /// <param name="top">�߽磺��</param>
        /// <param name="right">�߽磺��</param>
        /// <param name="bottom">�߽磺��</param>
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
        /// bmp��ͼ��ˮӡ����
        /// </summary>
        /// <param name="bm">bmpͼƬ��</param>
        /// <param name="strPicName">��ͼ����</param>
        /// <param name="left">�߽磺��</param>
        /// <param name="top">�߽磺��</param>
        /// <param name="right">�߽磺��</param>
        /// <param name="bottom">�߽磺��</param>
        /// <param name="waterleft">ˮӡ�߽�</param>
        /// <param name="watertop">ˮӡ�߽�</param>
        /// <param name="waterright">ˮӡ�߽�</param>
        /// <param name="waterbottom">ˮӡ�߽�</param>
        /// <param name="waterleft1">ˮӡ�߽�1</param>
        /// <param name="watertop1">ˮӡ�߽�1</param>
        /// <param name="waterright1">ˮӡ�߽�1</param>
        /// <param name="waterbottom1">ˮӡ�߽�1</param>
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
        /// ˮӡ���
        /// </summary>
        /// <param name="filePic">ԭͼ</param>
        /// <param name="rect">ˮӡ�߽�</param>
        /// <returns></returns>
        public bool WaterMark(string filePic, RECT rect)
        {
            if (filePic.IndexOf(".bmp") < 0)
                filePic += ".bmp";
            if (!File.Exists(filePic))
            {

                FileRW.WriteToFile(filePic + "<< �ļ������ڣ�");
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
        /// ƴͼ
        /// </summary>
        /// <param name="strAllPic">ͼƬ���Ƽ���</param>
        /// <param name="strPicID">ƴͼID</param>
        /// <param name="bVerbical">true������bmp��false������jpg</param>
        /// <returns></returns>
        /// 
        public int PinTu(string strAllPic, string strPicID, bool bVerbical)
        {
            string[] arrPicName;
            int num = SplitString(strAllPic, ",", out arrPicName);

            for (int i = 0; i < num + 1; i++)
            {
                if (arrPicName[i] == "����")
                {
                    CreatePlate("����", "", nZHPicWidth);
                    continue;
                }

                CreatePlate(m_strCapturePath + arrPicName[i], "", nZHPicWidth);
            }
            string strPic = m_strCapturePath + "ģ��.bmp";
            if (bVerbical)
            {
                if (!CreatePlate("����ͼƬ", strPic, 0))
                    return 1;
            }
            else
            {
                if (!CreatePlate("����ͼƬ", strPic, nZHPicWidth))
                    return 1;
            }
            Bitmap bbmp = new Bitmap(strPic, true);
            int x = 0, y = 0, z = 0;
            for (int i = 0; i < num + 1; i++)
            {

                if (arrPicName[i] == "����")
                {
                    //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "����");
                    ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "����");
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

                //ɾ��Сͼ
                if (Program.bRelease)
                    File.Delete(m_strCapturePath + arrPicName[i] + ".bmp");


            }
            string PicName = SetPicPath(m_strCapturePath, strPicID);
            if (bVerbical)
                //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "").Save(m_strCapturePath + strPicID + ".bmp", ImageFormat.Bmp);
                ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "").Save(m_strCapturePath + strPicID + ".bmp", ImageFormat.Bmp);
            else
            {
                //if (OrdNo.IndexOf("MZH") == 0 || OrdNo == "���Զ���")
                //{
                //    Bitmap sbmp = new Bitmap(m_strPicPath + "ˮӡ.bmp", true);
                //    ImageTool.BmpInsert(bbmp, sbmp, ref x, ref y, ref z, "ˮӡ");
                //    sbmp.Dispose();
                //}

                string strJpg = PicName.Replace(".bmp", ".jpg");
                //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "").Save(strJpg, ImageFormat.Jpeg);
                ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "").Save(strJpg, ImageFormat.Jpeg);
                picNum++;
            }
            bbmp.Dispose();
            File.Delete(m_strCapturePath + "ģ��.bmp");
            return 1;
        }
        /// <summary>
        /// ��ȡ�ַ���
        /// </summary>
        /// <param name="strScr">ԭ�ַ���</param>
        /// <param name="strFG">�ָ���</param>
        /// <param name="strArray">�õ��ַ�������</param>
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
        /// ͼƬ����
        /// </summary>
        /// <param name="filePic">������ʾ</param>
        /// <param name="strPicID">ͼƬID</param>
        /// <param name="width">��</param>
        /// <returns></returns>
        public bool CreatePlate(string filePic, string strPicID, int width)
        {
            if (filePic == "����ͼƬ")
            {
                bPicFull = true;
                goto NEXT_STEP;
            }
            if (filePic == "����")
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

                FileRW.WriteToFile(filePic + "<< �ļ������ڣ�");
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
                //WriteToFile("��Ȳ���,����\r\n");
                ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                ptBigPic.X = 0;
                ptBigPic.Y += ptMAX.Y + 1;
                ptMAX.Y = 0;
                //WriteToFile("ptBigPic.y=%d\r\n",ptBigPic.y);
            }

            if ((Lheight - ptBigPic.Y) < Sheight)
            {
                //WriteToFile("Lheight-ptBigPic.y={0}-{1}",Lheight,ptBigPic.Y);
                WriteToFile("�߶Ȳ���\r\n");
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
                WriteToFile("��ͼΪ��\r\n");
                C = C + 1;
                bPicFull = false;
                return false;
            }

            if (ptBigPic.Y > 0)//����
            {
                ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                ptMAX.Y += ptBigPic.Y;
            }
            else
            {
                if (ptBigPic.X < 130)
                    ptMAX.X = ptBigPic.X + 130;//��ͼƬС��ˮӡ�ߴ�ʱ���������ˮӡ�ĳߴ磨ˮӡ���Ϊ130��
                else
                    ptMAX.X += ptBigPic.X;
            }

            if (width != nZHPicWidth || MZH)  //ȡ���������
                width = ptMAX.X;
            //byte[] pBMPData = new byte[(width + 3) / 4 * 4 * ptMAX.Y * 4];
            byte[] pBMPData = new byte[width * 4 * ptMAX.Y];




            try
            {
                if (filePic == "����ͼƬ")
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
        /// ��ȡ���ڱ���
        /// </summary>
        /// <returns></returns>
        public string WinTitle()
        {
            m_hGameWnd = User32API.FindWindow("MHXYMainFrame", null);//���ݴ���������ȡ���
            string winTitle = User32API.GetWindowText(m_hGameWnd).Trim();//��ȡ���ڱ���
            return winTitle;
        }
        /// <summary>
        /// ƴͼpng
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
                if (arrPicName[i] == "����")
                {
                    CreatePlatePng("����", "", nZHPicWidth);
                    continue;
                }

                CreatePlatePng(picPath + arrPicName[i], "", nZHPicWidth);
            }
            string strPic = picPath + "ģ��.bmp";
            if (bVerbical)
            {
                if (!CreatePlatePng("����ͼƬ", strPic, 0))
                    return 1;
            }
            else
            {
                if (!CreatePlatePng("����ͼƬ", strPic, nZHPicWidth))
                    return 1;
            }
            Bitmap bbmp = new Bitmap(strPic, true);
            int x = 0, y = 0, z = 0;
            for (int i = 0; i < num + 1; i++)
            {

                if (arrPicName[i] == "����")
                {
                    //ImageTool.CreatBmpFromByte(bbmp, bbmp, ref x, ref y, ref z, "����");
                    try
                    {
                        ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "����");//��һ��BmpInsert
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
                    ImageTool.BmpInsert(bbmp, sbmp, ref x, ref y, ref z, "");//�ڶ���BmpInsert
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

                //ɾ��Сͼ
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
                    ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "").Save(picPath + strPicID + ".bmp", ImageFormat.Bmp);//������BmpInsert
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
                    if (m_strOrderType == "������")
                    {
                        CreatWaterMark(strJpg, ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, ""));
                    }
                    else
                        ImageTool.BmpInsert(bbmp, bbmp, ref x, ref y, ref z, "").Save(strJpg, ImageFormat.Jpeg);//�����BmpInsert 
                }
                catch (Exception ex)
                {
                    WriteToFile("BmpInsert5");
                    WriteToFile(ex.ToString());
                }
                picNum++;
            }
            bbmp.Dispose();
            File.Delete(picPath + "ģ��.bmp");
            File.Delete(picPath + "RoleList.png");
            File.Delete(picPath + "TierInfo.png");
            return 1;
        }
        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <returns></returns>
        public int TraversalFile(string dirPath)
        {
            List<string> list = new List<string>();//�ȶ���list����
            int count = 0;
            //��ָ��Ŀ¼�����ļ�
            if (Directory.Exists(dirPath))
            {
                DirectoryInfo Dir = new DirectoryInfo(dirPath);
                try
                {
                    foreach (FileInfo file in Dir.GetFiles())//������Ŀ¼ 
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

                        list.Add(arrName); //��list��ֵ                    
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
        /// ͼƬ����png
        /// </summary>
        /// <param name="filePic">������ʾ</param>
        /// <param name="strPicID">ͼƬID</param>
        /// <param name="width">��</param>
        /// <returns></returns>
        public bool CreatePlatePng(string filePic, string strPicID, int width)
        {
            if (filePic == "����ͼƬ")
            {
                bPicFull = true;
                goto NEXT_STEP;
            }
            if (filePic == "����")
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

                FileRW.WriteToFile(filePic + "<< �ļ������ڣ�");
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
                //WriteToFile("��Ȳ���,����\r\n");
                ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                ptBigPic.X = 0;
                ptBigPic.Y += ptMAX.Y + 1;
                ptMAX.Y = 0;
                //WriteToFile("ptBigPic.y=%d\r\n",ptBigPic.y);
            }

            if ((Lheight - ptBigPic.Y) < Sheight)
            {
                //WriteToFile("Lheight-ptBigPic.y={0}-{1}",Lheight,ptBigPic.Y);
                WriteToFile("�߶Ȳ���\r\n");
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
                WriteToFile("��ͼΪ��\r\n");
                C = C + 1;
                bPicFull = false;
                return false;
            }

            if (ptBigPic.Y > 0)//����
            {
                ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                ptMAX.Y += ptBigPic.Y;
            }
            else
            {
                if (ptBigPic.X < 130)
                    ptMAX.X = ptBigPic.X + 130;//��ͼƬС��ˮӡ�ߴ�ʱ���������ˮӡ�ĳߴ磨ˮӡ���Ϊ130��
                else
                    ptMAX.X += ptBigPic.X;
            }

            if (width != nZHPicWidth || MZH)  //ȡ���������
                width = ptMAX.X;
            //byte[] pBMPData = new byte[(width + 3) / 4 * 4 * ptMAX.Y * 4];
            byte[] pBMPData = new byte[width * 4 * ptMAX.Y];

            try
            {
                if (filePic == "����ͼƬ")
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
                    string winTitle = User32API.GetWindowText(hwdl2).Trim();//��ȡ���ڱ���
                    //WriteToFile(winTitle);
                    if (winTitle.Contains("�������"))
                    {

                        if (myapp.FindControl(hwdl2, null, "������������µ�¼").Count > 0)
                        {
                            WriteToFile("���������ж�");
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
            //m_hGameWnd = User32API.FindWindow(null, "�漣����SUN");
            RECT rt = new RECT(left, top, right, bottom);
            Point[] ST = new Point[15];
            string num = null;
            int n = 0;
            FontStyle FontType = new FontStyle();
            for (int j = 0; j < 10; j++)
            {
                rt = new RECT(left, top, right, bottom);
                string[] a = new string[10] { "8", "0", "4", "2", "3", "1", "5", "6", "7", "9" };
                //ImageTool.FindText(m_hGameWnd, "����ʺ�Ƿ��300��", Color.FromArgb(255, 255, 255), "����", 16, FontStyle.Regular, 0, 0, rtA, 30);
                at = ImageTool.FindText(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "����", 16, FontStyle.Regular, 0, 0, rt, 30);
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
                    //at = ImageTool.FindText1(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "����", 12, FontType, 0, 0, rt, 38);
                    at = ImageTool.FindText(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "����", 16, FontStyle.Regular, 0, 0, rt, 30);
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
        /// �л����ڽ�
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="time"></param>
        public void MyMouserClick(int x, int y, int time)
        {
            for (int i = 0; i < 3; i++)
            {
                m_hChangeWnd = User32API.FindWindow(null, "�˺�GTR");
                if (m_hChangeWnd != IntPtr.Zero)
                {
                    //User32API.ShowWindow(m_hChangeWnd, ShowWindowCmd.SW_RESTORE);
                    //User32API.SetForegroundWindow(m_hChangeWnd);
                    //��ʾ����
                    User32API.ShowWindow(m_hChangeWnd, ShowWindowCmd.SW_NORMAL);

                    //ǰ����ʾ
                    User32API.SetForegroundWindow(m_hChangeWnd);
                }
                Sleep(200);
            }
            if (m_hChangeWnd == IntPtr.Zero)
            {
                WriteToFile("RC���������");
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
        /// ģ���˹��ƶ����
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
                m_hChangeWnd = User32API.FindWindow(null, "�˺�GTR");
                if (m_hChangeWnd != IntPtr.Zero)
                {
                    //User32API.ShowWindow(m_hChangeWnd, ShowWindowCmd.SW_RESTORE);
                    //User32API.SetForegroundWindow(m_hChangeWnd);
                    //��ʾ����
                    User32API.ShowWindow(m_hChangeWnd, ShowWindowCmd.SW_NORMAL);

                    //ǰ����ʾ
                    User32API.SetForegroundWindow(m_hChangeWnd);
                }
                Sleep(200);
            }
            if (m_hChangeWnd == IntPtr.Zero)
            {
                WriteToFile("RC���������");
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
                WriteToFile("�������X:" + screenPoint.X.ToString() + "Y:" + screenPoint.Y.ToString() + "Ŀ������X" + x.ToString() + "Y:" + y.ToString());
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
                        WriteToFile("��걻����");
                        continue;
                    }
                    WriteToFile("�Ƶ���");
                    KeyMouse.PressMouseKey();
                    KeyMouse.PressMousekeyDouble(2);
                    Sleep(3000);
                    WriteToFile("���");

                    break;
                }
                myapp.mouse_event(1, IntMoveX, IntMoveY, 0, 0);
                //myapp.mouse_event(1, 0, IntMove, 0, 0);
                Sleep(200 + 50 * i);
            }

        }
        /// <summary>
        /// �������
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
                screenPoint = Control.MousePosition;//����������Ļ���Ͻǵ�����
                ptX = x - screenPoint.X;
                ptY = y - screenPoint.Y;
                if (Math.Abs(screenPoint.X - x) < 5 && Math.Abs(screenPoint.Y - y) < 5)
                    break;
            }
            if (Math.Abs(screenPoint.X - x) < 5 && Math.Abs(screenPoint.Y - y) < 5)
                KeyMouse.MouseClick(x, y, tb, tc, time);
            else
                WriteToFile("�ƶ����ʧ��");
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
                WriteToFile("���X:" + screenPoint.X.ToString() + "Ŀ��X" + x.ToString());
                if (Math.Abs(ptX) < 5)
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        screenPoint = Control.MousePosition;
                        ptY = y - screenPoint.Y;
                        IntMove = ptY > 0 ? 5 : -5;
                        WriteToFile("���Y:" + screenPoint.Y.ToString() + "Ŀ��Y" + y.ToString());
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
            //-----------����--------
            //REC = new RECT(0, 0, 650, 537);
            //-----------------------
            while (true)
            {
                pt = ImageTool.fPic(m_strPicPath + "���.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "���1.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
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
                        WriteToFile("�����ƶ����");
                    Console.WriteLine("{0},{1}", 0, my);
                }
                Sleep(100);
                ts = DateTime.Now - datest;
                if ((int)ts.TotalSeconds % 20 == 0)
                    WriteToFile("�������λ��");
                if (ts.TotalSeconds > 120)
                {
                    WriteToFile("��ʱ");
                    return false;
                }
            }

            while (true)
            {
                pt = ImageTool.fPic(m_strPicPath + "���.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "���1.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                else if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "���3.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
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
                        WriteToFile("�����ƶ����");
                    Console.WriteLine("{0},{1}", mx, 0);
                }
                Sleep(100);
                ts = DateTime.Now - datest;
                if ((int)ts.TotalSeconds % 20 == 0)
                    WriteToFile("�������λ��");
                if (ts.TotalSeconds > 120)
                {
                    WriteToFile("��ʱ");
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
            //-----------����--------
            //REC = new RECT(0, 0, 650, 537);
            //-----------------------
            while (true)
            {
                pt = ImageTool.fPic(m_strPicPath + "���.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "���1.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
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
                        WriteToFile("�����ƶ����");
                    Console.WriteLine("{0},{1}", 0, my);
                }
                Sleep(100);
                ts = DateTime.Now - datest;
                if ((int)ts.TotalSeconds % 20 == 0)
                    WriteToFile("�������λ��");
                if (ts.TotalSeconds > 120)
                {
                    WriteToFile("��ʱ");
                    return false;
                }
            }

            while (true)
            {
                pt = ImageTool.fPic(m_strPicPath + "���.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "���1.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
                else if (pt.X < 0)
                    pt = ImageTool.fPic(m_strPicPath + "���3.bmp", REC.left, REC.top, REC.right, REC.bottom, 40);
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
                        WriteToFile("�����ƶ����");
                    Console.WriteLine("{0},{1}", mx, 0);
                }
                Sleep(100);
                ts = DateTime.Now - datest;
                if ((int)ts.TotalSeconds % 20 == 0)
                    WriteToFile("�������λ��");
                if (ts.TotalSeconds > 120)
                {
                    WriteToFile("��ʱ");
                    return false;
                }
            }
            return true;

        }
        public void CreatWaterMark(string picnameandpath, Bitmap bit)
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
    }
}
