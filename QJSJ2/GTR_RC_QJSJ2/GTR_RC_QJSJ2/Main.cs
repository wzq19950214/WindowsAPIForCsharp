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
using System.Collections;
using System.Diagnostics;

namespace GTR
{
    class QJSJ2
    {

        public static int m_sign;
        //��������
        public static string m_strOrderData;
        //�˹���Ԥ����RC2����
        public static string  m_strRC2Data;
        //�˹���Ԥ������
        public static string m_strDeliverId="";
        //�����˹���Ԥ
        public static bool m_bHumanAgree = false;
        //ȷ���˹���Ԥ
        public static bool m_bHuman = false;
        //udpsockets
        public static udpSockets udpdd;
        //��֤�뷵��
        public static string mouseP = "";
        //��֤��ش����
        public int yzmTimes = 0;
        //udp�˿�
        public static int m_UDPPORT = 6902;
        //�ű��˿�
        public static int the_nRC2Port = 0;
        //��������-���׵�/������
        public static string m_strOrderType;
        //������
        public static string OrdNo = "���Զ���"; //"MZH-160607000000001";
        //����״̬
        public int Status;
        //��־
        public static string strAllog = "...";

        //��ͼĬ�ϳߴ�
        public const int Lwidth = 1280;
        public const int Lheight = 1200;
        public const int nZHPicWidth = 880;
        //ƴͼ�Ƿ�����
        public bool bPicFull = false;
        public Point ptBigPic;
        //װ����ͼ�ߴ�
        public Point ptMAX;
        static int PicNum = 1;
        static string strLastPicID = "";


        //ͬ���˹���Ԥ
        public static bool m_bHumanFinish = false;
        //�����־
        public static bool IsAnswer =false;
        //�ʼı�־
        public static bool IsAskMail = false;

        public static bool IntoOrder = false;
        //�ƽ���־
        public static bool bYiJiao = false;
        //������Ϸ��־
        public static bool IfEnter = false;
        //Mվ�㶩����־
        public static bool MZH = false;

        //���ھ��
        public static IntPtr m_hGameWnd;
        //��������·��
        private string m_strProgPath = System.Windows.Forms.Application.StartupPath;
        //ƥ��ͼƬ·��
        public static string m_strPicPath = System.Windows.Forms.Application.StartupPath + @"\pic\";
        //�쳣��ͼ����·��
        public static string LocalPicPath ="E:\\piccQQSJ\\";

        //������ϸ����

        public string m_GameTitle;
        public string m_strGameStartFile = @"D:\game\FullClient\FullClient\SUN2.exe";//@"E:\FullClient\SUN2.exe"; 
        public string m_strAccount = "";//�˺�
        public string m_strPassword;//����
        public string m_strGameName = "�漣����2";
        public string m_strArea;//��
        public string m_strServer;//��
        public string m_strSellerRoleName;//���ҽ�ɫ��
        public string m_strSecondPwd;//��������
        public string m_strSellerItemId;//������Ʒ
        public string m_strSellerItemNum;//��Ŀ
        public string m_strBuyerRoleName;//��ҽ�ɫ��
        public string m_strBuyerItemNum;//������Ŀ
        public long BackPackNum;//�����������
        public bool Totransfer = false;//�Ƿ�����
        public int  ToTrNum = 0;//���ͽ������
        public bool isSeller = false;
        public string m_strCapturePath = System.Windows.Forms.Application.StartupPath + @"\pic\";

        public static StringBuilder strb = new StringBuilder();

        public static string ServerAddr = "";
        //�Ƿ������Ϸ��ʶ
        public static bool InGame = false;
        //�ж��Ƿ����ֵ�
        public static bool IfTrading = false;
        /// <summary>
        ///���������
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
                    CaptureBmpInRect("ʧ��");
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
            tmp = string.Format("�ƽ�״̬={0}\r\n", Status);
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
                            WriteToFile("�ƽ��ɹ�");
                            break;
                        }
                        Sleep(300);
                    }
                    if (bYiJiao)
                        break;
                    WriteToFile("�ƽ�ʧ��");
                }
            }
            else
                WriteToFile("�˿�Ϊ0");

            CloseGames();
            string adslpath = System.Windows.Forms.Application.StartupPath + @"\adsl.ini";
            StringBuilder retVal = new StringBuilder(256);
            User32API.GetPrivateProfileString("��¼����", "ADSL��������", "", retVal, 256, adslpath);
            int num = 0;
            if (retVal.ToString() != "")
                num = int.Parse(retVal.ToString());
            if (num > 100)
            {
                User32API.WritePrivateProfileString("��¼����", "ADSL��������", "0", adslpath);
                Sleep(2500);
            }
            else
            {
                if ((Status > 1000 && Status < 3000) || Status > 4000)
                {
                    string strNum = string.Format("{0}", num + 18);
                    User32API.WritePrivateProfileString("��¼����", "ADSL��������", strNum, adslpath);
                }
            }

            StringBuilder retVal1 = new StringBuilder(256);
            User32API.GetPrivateProfileString("��¼����", "����ʧ��", "", retVal1, 255, adslpath);
            int num1 = int.Parse(retVal1.ToString());
            if (Status==3120)
            {
                if (num1 == 3)
                {
                    User32API.WritePrivateProfileString("��¼����", "����ʧ��", "0", adslpath);
                    RestartPC();//��������
                }
                else
                {
                    string strNum1 = string.Format("{0}", num1 + 1);
                    User32API.WritePrivateProfileString("��¼����", "����ʧ��", strNum1, adslpath);
                }
            }
            else
            {
                User32API.WritePrivateProfileString("��¼����", "����ʧ��", "0", adslpath);
            }

            m_hGameWnd = User32API.FindWindow(null, "�漣����SUN");
            if (m_hGameWnd != IntPtr.Zero)
            {
                RestartPC();
            }

            return;

        }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns>����״̬</returns>
        public int GameProc()
        {
            if (!KeyMouse.InitKeyMouse())
            {
                WriteToFile("��������ʧ��");
                return 3120;
            }
            int n = OrdNo.IndexOf("-");
            if (n > 0 || OrdNo == "���Զ���" || MZH)
                m_strOrderType = "������";
            else
                m_strOrderType = "���׵�";
            if (!RequestOrderData())
                return 3120;
            if (!ReadOrderDetail())
                return 3120;
            if (Regex.IsMatch(m_strAccount, @"[\u4e00-\u9fa5]"))
            {
                WriteToFile("�˺ź�������");
                return 3000;
            }
            if (Regex.IsMatch(m_strPassword, @"[\u4e00-\u9fa5]"))
            {
                WriteToFile("���뺬������");
                return 3000;
            }
            Sleep(2000);
            AppInit();
            //�����־�ļ�
            System.IO.File.WriteAllText(m_strProgPath + @"\�����־.log", string.Empty);
            //��ʾ����
            WriteToFile("����������" + m_strArea + "   ���ҽ�ɫ����" + m_strSellerRoleName);
            for (int i = 0; i < 5; i++)
            {
                if (i == 3)
                {
                    WriteToFile("�޷�������Ϸ������");
                    WriteToFile("�������Ӵ������Ϸά����");
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
        /// �ж϶���
        /// </summary>
        /// <returns></returns>
        public int Offline() {
            if(isSeller){
                return 4010;
            }
            Point at = new Point(-1,-2);
            for (int a = 0; a < 3;a++ ) {
                at = ImageTool.fPic(m_strPicPath+"����.bmp",380,320,650,450);
                if(at.X>0){
                    WriteToFile("������");
                    CaptureBmpInRect("����");
                    KeyMouse.MouseClick(at.X+185,at.Y+110,1,1,1000);
                    return 3030;
                }
            }
            return 1;
        }
        /// <summary>
        /// ������Ϸ
        /// </summary>
        /// <returns></returns>
        public int RunGame()
        {
            //�ж���ҽ�ɫ���Ƿ��д�д��o��0 ��ֱ��ת3930
            if (m_strBuyerRoleName.IndexOf("0") >= 0 || m_strBuyerRoleName.IndexOf("O") >= 0) {
                WriteToFile("�����:" + m_strBuyerRoleName);
                WriteToFile("�޷��ж���ҽ�ɫ��");
                WriteToFile("�Զ�����ϵͳ�����ɣ�ת�˹�");
                return 3930;
            }
            //�ж��˺�λ���Ƿ�Ϸ�
            if (m_strAccount.Length > 10 || m_strAccount.Length < 4)
            {
                WriteToFile("�����˺�:" + m_strAccount);
                WriteToFile("�˺Ų��Ϸ�");
                WriteToFile("�Զ�����ϵͳ�����ɣ�ת�˹�");
                return 3930;
            }
            Point pt = new Point(-1, -2);
            Point at = new Point(-1, -2);
            int aS = 0;
            int error = 0;
            Game.RunCmd("taskkill /im  iexplore.exe /F");
            m_hGameWnd = IntPtr.Zero;
            m_GameTitle = "UpdateClient Beta 1.0.8.0";
            WriteToFile("���Դ򿪿ͻ���");
            strAllog = "�ȴ�������Ϸ";
            string[] Sever = new string[] {"��˹Ī��","���֮��","�������"};
            for (int i = 0; i < 100; i++)
            {
                m_hGameWnd = User32API.FindWindow(null, m_GameTitle);
                if (m_hGameWnd == IntPtr.Zero)
                {
                    //����Ϸ
                    Game.StartProcess(m_strGameStartFile, "start");
                    Sleep(1000*10);
                }
                else
                {
                    WriteToFile("�Ѵ򿪿ͻ���");
                    for (int j = 0; j < 5;j++ ) {
                        pt = ImageTool.fPic(m_strPicPath + "ͬ��.bmp", 0, 0, 0, 0);
                        if (pt.X > 0)
                        {
                            //���ͬ�ⰴť
                            KeyMouse.MouseClick(pt.X-15,pt.Y+8,1,1,500);
                            //���ѡ������
                            for (int Se = 0; Se < Sever.Length;Se++ ) {
                                if (m_strArea == Sever[Se])
                                {
                                    aS = Se;
                                    KeyMouse.MouseClick(pt.X-130*Se,pt.Y-30,1,1,500);
                                    break;
                                }
                            }
                            //�����ʼ��Ϸ
                            for (int a = 0; a < 10;a++ ) {
                                at = ImageTool.fPic(m_strPicPath + "��ʼ��Ϸ.bmp", 0, 0, 0, 0);
                                if (at.X > 0)
                                {
                                    KeyMouse.MouseClick(at.X + 5, at.Y + 3, 1, 1, 1000);
                                     at = ImageTool.fPic(m_strPicPath + "��ʼ��Ϸ.bmp", 0, 0, 0, 0);
                                     if (at.X > 0)
                                     {
                                         KeyMouse.MouseClick(pt.X - 15, pt.Y + 8, 1, 1, 500);
                                         KeyMouse.MouseClick(pt.X - 15, pt.Y + 8, 1, 1, 500);
                                         KeyMouse.MouseClick(at.X + 5, at.Y + 3, 1, 1, 1000);
                                     }
                                    return 1;
                                }
                                else {
                                    WriteToFile("���ڸ���");
                                    Sleep(2000);
                                }
                            }
                            break;
                        }
                        Sleep(1000);
                   }
                   if (error < 3)
                   {

                       at = ImageTool.fPic(m_strPicPath + "���´���.bmp", 0, 0, 0, 0);
                       if (at.X > 0)
                       {
                           KeyMouse.MouseClick(650, 552, 1, 1, 1000);
                           Game.RunCmd("taskkill /im  iexplore.exe /F");
                       }
                       Game.RunCmd("taskkill /im  SUN2.exe /F");
                       WriteToFile("δ�ҵ�����Ϸ��ʶ,�ؿ���Ϸ");
                       Sleep(2000);
                       error++;
                       continue;
                   }
                   WriteToFile("������Ϸʧ��");
                   return 3120;
                }
            }

            WriteToFile("������Ϸʧ��");
            return 3120;
        }
        /// <summary>
        /// �˺�����
        /// </summary>
        /// <returns></returns>
        public int Login() {
            strAllog = "�ȴ�������Ϸ";
            for (int i = 0; i < 40;i++ ) {
                m_hGameWnd = User32API.FindWindow(null, "�漣����SUN");
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
                        WriteToFile("������δ��Ӧ,���¿�����Ϸ");
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
                    WriteToFile("�����˺Ž���ʧ��");
                    return 3120;
                }
            }
            strAllog = "�ȴ��˺�����";
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            for (int a = 0; a < 40;a++ ) {
                at = ImageTool.fPic(m_strPicPath+"��¼��Ϸ.bmp",0,0,0,0);
                if(at.X>0){
                    Sleep(1000);
                    for (int a1 = 0; a1 < 3;a1++ ) {
                        //�����˺�
                        WriteToFile("�����˺�:" + m_strAccount);
                        KeyMouse.MouseClick(at.X + 132, at.Y - 60, 1, 1, 200);
                        KeyMouse.MouseClick(at.X + 132, at.Y - 60, 1, 1, 200);
                        KeyMouse.SendBackSpaceKey(15);
                        KeyMouse.SendKeys(m_strAccount, 200);

                        Sleep(500);
                        //��������
                        WriteToFile("��������:" + m_strPassword.Length + "λ");
                        KeyMouse.MouseClick(at.X + 132, at.Y - 27, 1, 1, 200);
                        KeyMouse.MouseClick(at.X + 132, at.Y - 27, 1, 1, 200);
                        KeyMouse.SendBackSpaceKey(15);
                        KeyMouse.SendKeys(m_strPassword, 200);

                        Sleep(500);
                        //��ͼ
                        CaptureBmpInRect("�˺�����");
                        KeyMouse.MouseClick(at.X+5,at.Y+3,1,1,1000);
                        //�жϷ�ͣ
                        bt = ImageTool.fPic(m_strPicPath + "��ͣ.bmp", 0, 0, 0, 0);
                        if(bt.X>0){
                            WriteToFile("�˺��ѷ�ͣ");
                            CaptureBmpInRect("��ͣ");
                            KeyMouse.MouseClick(bt.X + 71, bt.Y + 108, 1, 1, 1000);
                            return 2300;
                        }

                        bool judge = LoginError(a1);
                        if (judge)
                        {
                            Sleep(2000);
                            for (int a2 = 0; a2 < 5;a2++ ) {
                                at = ImageTool.fPic(m_strPicPath +"�������б�.bmp", 0, 0, 0, 0);
                                if(at.X>0){
                                    //���������
                                    KeyMouse.MouseClick(at.X-114,at.Y+46,1,1,200);
                                    KeyMouse.MouseClick(at.X - 114, at.Y + 46, 1, 2, 500);

                                    //���ѡ�����
                                    KeyMouse.MouseClick(at.X + 157, at.Y + 93, 1, 1, 500);
                                    for (int i = 0; i < 5;i++ ) {
                                        at = ImageTool.fPic(m_strPicPath  + "���������.bmp",375,320,655,490);
                                        if(at.X>0){
                                            WriteToFile("��¼���");
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
                            WriteToFile("�˺��������");
                            return 2000;
                        }
                        if(!judge){
                            continue;
                        }

                    }

                }
                Sleep(2000);
            }
            WriteToFile("�����˺Ž���ʧ��");
            return 3120;
        }
        /// <summary>
        /// �ж��˺��������
        /// </summary>
        /// <returns></returns>
        public bool LoginError(int e) {
            Point at = new Point(-1,-2);

            for (int a = 0; a < 3;a++ ) {
                at = ImageTool.fPic(m_strPicPath+"�������.bmp",0,0,0,0);
                if(at.X>0){
                    WriteToFile("�������,�ٴ�����");
                    CaptureBmpInRect("����" + e);
                    KeyMouse.MouseClick(at.X+71,at.Y+108,1,1,1000);
                    Sleep(4000);
                    return false;
                }
                at = ImageTool.fPic(m_strPicPath + "�˺Ų�����.bmp", 0, 0, 0, 0);
                if (at.X > 0)
                {
                    WriteToFile("�û�������,�ٴ�����");
                    CaptureBmpInRect("����" + e);
                    KeyMouse.MouseClick(at.X + 71, at.Y + 108, 1, 1, 1000);
                    Sleep(4000);
                    return false;
                }
                at = ImageTool.fPic(m_strPicPath + "������δ��Ӧ.bmp", 0, 0, 0, 0);
                if (at.X > 0)
                {
                    WriteToFile("������δ��Ӧ,�ٴ�����");
                    CaptureBmpInRect("����" + e);
                    KeyMouse.MouseClick(at.X + 127, at.Y + 107, 1, 1, 1000);
                    Sleep(4000);
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// ѡ���ɫ
        /// </summary>
        /// <returns></returns>
        public int Role() {
            strAllog = "ѡ���ɫ";
            Sleep(8000);
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            for (int a = 0; a < 40;a++ ) {

                at = ImageTool.fPic(m_strPicPath + "ѡ���ɫ.bmp", 0, 110, 300, 180);
                if (at.X > 0)
                {
                    CaptureBmpInRect("��ɫ");
                    bt = ImageTool.fPic(m_strPicPath + "������ɫ.bmp", 0, 0, 0, 0);
                    if(bt.X<=0){
                        bt = ImageTool.fPic(m_strPicPath + "������ɫ1.bmp", 0, 0, 0, 0);
                    }
                    if (bt.Y > at.Y + 125)
                    {
                        WriteToFile("�˺��ж����ɫ");
                        CaptureBmpInRect1("��ɫ�б�",at.X-109,at.Y+20,at.X+178,bt.Y-20);
                        PinTu("��ɫ,����,��ɫ�б�,��ɫ���", "��ɫ��", true);
                        //���ʹ���ѡ���ɫ
                        string Path = m_strProgPath + "\\����\\��ɫ��.bmp";
                        string strResult = "";
                        strResult = RequestSafeCardInfo(1, Path,"", 60);
                        //�жϴ�����
                        WriteToFile("���ⷵ��:"+strResult);
                        if (strResult == "error")
                        {
                            WriteToFile("����Ա��������");
                            return 3230;
                        }
                        if (strResult == "")
                        {
                            WriteToFile("����Ա��������");
                            return 3230;
                        }
                        else {
                            int num = Int32.Parse(strResult);
                            if (num == 0)
                            {
                                WriteToFile("�������");
                                return 3230;
                            }
                            else {
                                WriteToFile("ѡ���ɫ"+num);
                                KeyMouse.MouseClick(at.X, at.Y + 50*num, 1, 1, 500);
                                CaptureBmpInRect("��ɫ");
                                KeyMouse.MouseClick(at.X, at.Y + 50 * num, 1, 2, 500);
                                return 1;
                            }
                        }
                    }
                    else {
                        WriteToFile("�˺�ֻ��Ψһ��ɫ");
                        KeyMouse.MouseClick(at.X,at.Y+45,1,1,500);
                        KeyMouse.MouseClick(at.X, at.Y + 50, 1, 2, 200);
                        KeyMouse.MouseClick(at.X, at.Y + 50, 1, 2, 500);
                        return 1;
                    }
                    
                }
                Sleep(2000);
            }
            WriteToFile("�����ɫѡ��ҳ��ʧ��");
            return 3120;
        }
        /// <summary>
        /// ������Ϸ���ж�
        /// </summary>
        /// <returns></returns>
        public int EnterGame() {
            strAllog = "���ڽ�����Ϸ";
            Point at = new Point(-1,-2);
            Sleep(1000*10);
            for (int j = 0; j < 40; j++)
            {
                at = ImageTool.fPic(m_strPicPath + "������Ϸ.bmp", 0, 0, 0, 0);
                if (at.X > 0)
                {
                    WriteToFile("��½�ɹ�");
                    Sleep(3000);
                    TanC();
                    CaptureBmpInRect("��½�ɹ�");
                    return 1;
                }
                at = ImageTool.fPic(m_strPicPath + "������Ϸ1.bmp", 0, 0, 0, 0);
                if (at.X > 0)
                {
                    WriteToFile("��½�ɹ�");
                    Sleep(3000);
                    TanC();
                    CaptureBmpInRect("��½�ɹ�");
                    return 1;
                }
                at = ImageTool.fPic(m_strPicPath + "������Ϸ2.bmp", 0, 0, 0, 0);
                if (at.X > 0)
                {
                    WriteToFile("��½�ɹ�");
                    Sleep(3000);
                    TanC();
                    CaptureBmpInRect("��½�ɹ�");
                    return 1;
                }
                if(j==25||j==26){
                    TanC();
                }
                Sleep(1000);
            }

            
            
            WriteToFile("������Ϸʧ��");
            return 3120;
        }
        public void TanC() {
            Point at = new Point(-1, -2);
            for (int i = 0; i < 10;i++ ) {
                KeyMouse.SendEscKey();
                Sleep(500);
                at = ImageTool.fPic(m_strPicPath + "�˳���Ϸ.bmp", 425, 215, 650, 445);
                if(at.X>0){
                    KeyMouse.MouseClick(at.X+120,at.Y-174,1,1,500);
                    break;
                }
            }
        }
        /// <summary>
        /// �򿪱�����ȡ���
        /// </summary>
        /// <returns></returns>
        public int BackPack() {

            strAllog = "��ȡ�����������";
            Point at = new Point(-1,-2);
            RECT rt = new RECT(0, 0, 0, 0);
            string Num = null;//��ȡ�Ľ��
            long num = 0;//���ת����int
            KeyMouse.Sendkey('i');
            Sleep(2000);
            for (int a = 0; a < 20; a++)
            {
                at = ImageTool.fPic(m_strPicPath+"������ʶ.bmp",810,520,1010,580,5);
                if(at.X<0&&a>10){
                    KeyMouse.Sendkey('i');
                    at = ImageTool.fPic(m_strPicPath + "������ʶ.bmp", 810, 520, 1010, 580, 5);
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
                    CaptureBmpInRect("����");
                    if(Num ==null){
                        WriteToFile("��ȡ���ʧ��");
                        return 2140;
                    }
                    WriteToFile("��������:" + m_strBuyerItemNum);

                    num = Int64.Parse(Num);
                    WriteToFile("�������:" + Num);
                    if (num >= Int64.Parse(m_strBuyerItemNum))
                    {
                        BackPackNum = num;
                        WriteToFile("��ҳ���");
                        KeyMouse.Sendkey('i'); 
                    }
                    else {
                        WriteToFile("��Ҳ���"); 
                        KeyMouse.Sendkey('i');

                        //ǰ���ֿ�ȡǮ
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
                    WriteToFile("��ȡ���ʧ��..");
                    return 2140;
                }
            }

            Sleep(2000);
            //�жϵص�,�жϽ���Ƿ��㹻����
            int dt = 0;
            for (int i = 0; i < 10;i++ ) {
                KeyMouse.MouseMove(100,100);
                at = ImageTool.fPic(m_strPicPath + "������ʶ.bmp", 810, 520, 1010, 580, 5);
                if(at.X>0){
                    KeyMouse.Sendkey('i'); 
                }
                at = ImageTool.fPic(m_strPicPath + "�ֵ�.bmp", 740, 0, 1027, 200);
                if (at.X < 0)
                {
                    WriteToFile("���ڽ��׵�ͼ.�жϽ���Ƿ��㹻����");

                    int TrNum= TransNum();
                    if (TrNum==3120) {
                        WriteToFile("�޷���ȡ����������");
                        return 3120;
                    }
                    ToTrNum = TrNum;
                    if ((num - TrNum) < Int64.Parse(m_strBuyerItemNum))
                    {
                        WriteToFile("��Ҳ��㴫��");
                        return 2140;
                    }
                    else
                    {
                        WriteToFile("����㹻����");
                        Totransfer = true;
                        return 1;
                    }
                }
                else {
                    dt++;
                }
                if(dt>5){
                    WriteToFile("�ڽ��׵�ͼ");
                    IfTrading = true;
                    return 1;
                }
            }

            WriteToFile("��ȡ������Ҵ���");
            return 3120;
           
        }
        /// <summary>
        /// ��ȡ�ֿ���
        /// </summary>
        /// <returns></returns>
        public int WareHouse(long MoneyNum) {  
            Sleep(1000);
            Point at = new Point(-1,-2);

            //ǰ���ֿ�
            #region
            WriteToFile("����ǰ���ֿ�");
            at = ImageTool.fPic(m_strPicPath + "�ֵ�.bmp", 740, 0, 1027, 200);
            if (at.X < 0)
            {
                WriteToFile("���ڽ��׵�ͼ");
                return -1;
            }
           
            for (int a = 0; a < 10; a++)
            {
                KeyMouse.Sendkey('m');
                Sleep(400);
                at = ImageTool.fPic(m_strPicPath + "��ͼ.bmp", 0, 645, 375, 790);
                if (at.X > 0)
                {
                    Sleep(1000);
                    //����ֿ�λ��
                    at = ImageTool.fPic(m_strPicPath + "�ֿ�����.bmp", 400, 400, 600, 600);
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
                    WriteToFile("�򿪵�ͼʧ��");
                    return -1;
                }
            }
            #endregion
            

            //�򿪲ֿ�
            #region
            for (int b = 0; b < 10; b++)
            {
                at = ImageTool.fPic(m_strPicPath + "�ֿ��ʶ.bmp", 400, 480, 630, 640);
                if (at.X > 0)
                {
                    WriteToFile("�򿪲ֿ�ɹ�");
                    break;
                }
                if (b == 9)
                {
                    WriteToFile("δ�ܴ򿪲ֿ�");
                    return -1;
                }
                Sleep(1000);
            }
            #endregion

            //��ȡ�ֿ���
            string Num = "";//�ֿ��еĽ��
            for (int i = 0; i < 5; i++)
            {
                at = ImageTool.fPic(m_strPicPath + "ȡ�����.bmp", 400, 470, 600, 590);
                if (at.X > 0)
                {
                    for (int n = 0; n < 5; n++)
                    {
                        //��ȡ�ֿ��еĽ��
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
                        WriteToFile("δ����ȷ��ȡ�ֿ���");
                        return -1;
                    }
                    WriteToFile("�ֿ����:" + Num);
                    CaptureBmpInRect("�ֿ����");

                    if ((Int64.Parse(Num) + MoneyNum) >= Int64.Parse(m_strBuyerItemNum))
                    {
                        WriteToFile("����㹻");
                        break;
                    }
                    else {
                        return -1;
                    }
                }
            }
            //�ж��Ƿ�����  
            bool Lock = false;//�ֿ�������ʶ
            #region
            for (int i = 0; i < 5; i++)
            {
                at = ImageTool.fPic(m_strPicPath + "�ֿ�����.bmp", 400, 170, 630, 270);
                if (at.X > 0)
                {
                    Lock = true;
                    WriteToFile("�ֿ�Ϊ����״̬");
                    KeyMouse.MouseClick(at.X - 50, at.Y + 417, 1, 1, 1000);
                    break;
                }
                at = ImageTool.fPic(m_strPicPath + "�ֿ⿪��.bmp", 400, 170, 630, 270);
                if (at.X > 0)
                {
                    WriteToFile("�ֿ�Ϊ����״̬");
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
                strAllog = "���ڳ��Կ����ֿ���";
                //������������
                #region
                for (int c = 0; c < 5; c++)
                {
                    at = ImageTool.fPic(m_strPicPath + "�ֿ�����1.bmp", 360, 300, 660, 500);
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


                //�����������
                int x=0, y=0;
                #region
                for (int d = 0; d < 5; d++)
                {
                    at = ImageTool.fPic(m_strPicPath + "�ֿ�����2.bmp", 400, 280, 630, 520);
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

                //������������
                #region
                RECT rt = new RECT(405, 355, 540, 515);
                int error = 0;
                if (m_strSecondPwd.Length > 8 || m_strSecondPwd.Length<4) {
                    return -1;
                }
                //���ֿ������������
                ArrayList b = new ArrayList();
                foreach (char c in m_strSecondPwd)
                {
                    b.Add(c.ToString());
                }

                m_hGameWnd = User32API.GetDesktopWindow();
                for (int j = 0; j < 20; j++)
                {

                    Sleep(1000);
                    //����������
                    KeyMouse.MouseMove(rt.left, rt.top - 50);
                    for (int i = 0; i < b.Count; i++)
                    {
                        Sleep(500);
                        if (b[i].ToString()=="1") {
                            List<Point> l1 = ReadStorPaswd("1", 424, 373, 38, 34);
                            if (l1.Count > 1) {
                                at = ImageTool.FindText(m_hGameWnd, "X", Color.FromArgb(217, 119, 0), "����", 12, FontStyle.Bold, 0, 0, rt, 30);
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
                                at = ImageTool.FindText(m_hGameWnd, "8", Color.FromArgb(217, 119, 0), "����", 12, FontStyle.Bold, 0, 0, rt, 30);
                                if (at.X > 0)
                                {
                                    l1.Remove(at);
                                    KeyMouse.MouseClick(l1[0].X, l1[0].Y, 1, 1, 1000);
                                    KeyMouse.MouseMove(rt.left, rt.top - 50);
                                    continue;
                                }
                            }
                        }
                        at = ImageTool.FindText(m_hGameWnd, b[i].ToString(), Color.FromArgb(217, 119, 0), "����", 12, FontStyle.Bold, 0, 0, rt, 30);
                        if (at.X > 0)
                        {
                            KeyMouse.MouseClick(at.X, at.Y, 1, 1, 1000);
                            KeyMouse.MouseMove(rt.left, rt.top - 50);
                        }

                    }
                    CaptureBmpInRect("�ֿ�����"+error);
                    KeyMouse.MouseClick(x + 100, y + 185, 1, 1, 2000);

                    //�������
                    at = ImageTool.fPic(m_strPicPath + "�ֿ��������.bmp", 380, 325, 660, 885);
                    if(at.X<0){
                        at = ImageTool.fPic(m_strPicPath + "�ֿ��������.bmp", 380, 325, 660, 885);
                    }
                    if(at.X>0){
                        KeyMouse.MouseClick(at.X+130,at.Y+108,1,1,500);
                        CaptureBmpInRect("�ֿ��������" + error);
                        error++;
                    }

                    //�ж��Ƿ���
                    at = ImageTool.fPic(m_strPicPath + "�ֿ⿪��.bmp", 380, 325, 660, 885);
                    if (at.X > 0)
                    {
                        Sleep(2000);
                        KeyMouse.MouseClick(at.X + 80, at.Y + 105, 1, 1, 1000);
                        at = ImageTool.fPic(m_strPicPath + "�ֿ⿪��.bmp", 400, 170, 630, 270);
                        if (at.X > 0)
                        {
                            WriteToFile("�ѿ����ֿ�");
                            break;
                        }
                    }
                    //��������������
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

            //�Ӳֿ�ȡ�����
            string InputNum = "";//����Ľ��
            long OutNum = 0;//��Ҫȡ���Ľ��
            string BegNum = "";//�������
            for (int i = 0; i < 5;i++ ) {
                at = ImageTool.fPic(m_strPicPath + "ȡ�����.bmp", 400, 470, 600, 590);
                if (at.X > 0) {
                    if ((Int64.Parse(Num) + MoneyNum) >= Int64.Parse(m_strBuyerItemNum))
                    {
                        //ȡ�����
                        #region
                        for (int j = 0; j < 5; j++)
                        {
                            OutNum = Int64.Parse(m_strBuyerItemNum) - MoneyNum;
                            KeyMouse.MouseClick(at.X + 170, at.Y + 57, 1, 1, 500);
                            KeyMouse.MouseClick(at.X + 170, at.Y + 57, 1, 1, 500);
                            KeyMouse.SendKeys(OutNum.ToString(), 200);
                            KeyMouse.MouseMove(at.X, at.Y - 20);
                            Sleep(500);
                            //��ȡ����Ľ��
                            InputNum = CheckPic(at.X, at.Y + 47, at.X + 200, at.Y + 67, 1);
                            if (InputNum == OutNum.ToString())
                            {
                                WriteToFile("ȡ����ң�" + InputNum);

                                for (int m = 0; m < 5; m++)
                                {
                                    KeyMouse.MouseClick(at.X + 155, at.Y + 85, 1, 1, 500);
                                    BegNum = CheckPic(at.X + 430, at.Y + 45, at.X + 590, at.Y + 67, 1);
                                    if ((Int64.Parse(BegNum)) >= (Int64.Parse(m_strBuyerItemNum)))
                                    {
                                        BackPackNum = Int64.Parse(BegNum);
                                        WriteToFile("�������:" + BegNum);
                                        WriteToFile("ȡǮ�ɹ�");
                                        CaptureBmpInRect("ȡǮ�ɹ�");
                                        return 1;
                                    }
                                    else if (Int64.Parse(BegNum) == MoneyNum)
                                    {
                                        continue;
                                    }
                                }

                                WriteToFile("ȡǮʧ��");
                                CaptureBmpInRect("ȡǮʧ��");
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
                        WriteToFile("ȡ�����ʧ��");
                        CaptureBmpInRect("ȡǮʧ��");
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
        /// �����б��ȡ����
        /// </summary>
        /// <param name="str">��Ҫ����������</param>
        /// <param name="left">�б�������</param>
        /// <param name="top">�б�������</param>
        /// <param name="width">���</param>
        /// <param name="height">�߶�</param>
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
                    pt = ImageTool.FindText(m_hGameWnd, str, Color.FromArgb(217, 119, 0), "����", 12, FontStyle.Bold, 0, 0, rt, 30);
                    if (pt.X > 0)
                    {
                        l.Add(pt);
                    }
                }
            }
            return l;
        }
        /// <summary>
        /// ��ȡ���ͷ���
        /// </summary>
        /// <returns></returns>
        public int TransNum() {
            RECT rt = new RECT(740, 0, 1027, 200);
            Point at = new Point(-1, -2);
            m_hGameWnd = User32API.FindWindow(null, "�漣����SUN");
            string[,] str = new string[,] { 
                            { "���ָ�", "3000" }, 
                            { "��ɪ��", "6000" }, 
                            { "�¿���", "35000" }, 
                            { "�����", "2000" },
                            { "����֮·", "3000" },
                            { "���¼����", "4000" },
                            { "��������ˮ·", "5000" },
                            { "����ɳĮ", "20000" },
                            { "����Ͽ��", "25000" },
                            { "����֮��", "30000" },
                            { "����Ĺ��", "3000" },
                            { "����֮��", "35000" },
                            { "��֮ͭ��", "45000" },
                            { "����в�", "55000" }
            };

            for (int j = 0; j < 3;j++) {
                for (int i = 0; i < 14; i++)
                {
                    at = ImageTool.FindText(m_hGameWnd, str[i, 0], Color.FromArgb(197, 178, 137), "����", 12, FontStyle.Regular, 0, 0, rt, 20);
                    if (at.X > 0)
                    {
                        int n = int.Parse(str[i, 1]);
                        WriteToFile("��ǰ���ڵ�ͼ:" + str[i, 0]);
                        WriteToFile("������Ҫ���:"+str[i, 1]);
                        return n;
                    }
                    Sleep(200);
                }
                Sleep(1000);
            }
            
            return 3120;
        }
        /// <summary>
        /// ƥ���ȡ����
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="type">1����,2��������</param>
        /// <returns>ƥ�䵽������</returns>
        public string CheckPic(int left, int top, int right, int bottom,int type)
        {
            
            Point at = new Point(-1, -2);
            m_hGameWnd = User32API.GetDesktopWindow();
            //m_hGameWnd = User32API.FindWindow(null, "�漣����SUN");
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
                at = ImageTool.FindText(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "����", 12, FontType, 0, 0, rt, 38);
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
                    at = ImageTool.FindText(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "����", 12, FontType, 0, 0, rt, 38);
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
        /// ǰ�����׵ص�
        /// </summary>
        /// <returns></returns>
        public int GoTrading() {
            strAllog = "ǰ�����׵ص�";
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            Point ct = new Point(-1, -2);
            bool su = false;
            int cs = 0;
            //�����ֵ�
            #region
            if (!IfTrading)
            {
                WriteToFile("ǰ�����׵�ͼ");
                for (int a = 0; a < 20; a++)
                {
                    KeyMouse.Sendkey('m');
                    Sleep(1000);
                    at = ImageTool.fPic(m_strPicPath + "���͵�.bmp", 0, 0, 940, 750);
                    if (at.X > 0)
                    {
                        ct = new Point(at.X,at.Y);
                        KeyMouse.MouseClick(at.X, at.Y, 1, 1, 500);
                        for (int c = 0; c < 3; c++)
                        {
                            Sleep(500);
                            at = ImageTool.fPic(m_strPicPath + "��ͼ.bmp", 0, 645, 375, 790);
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
                                ct = ImageTool.fPic(m_strPicPath + "���͵�.bmp", 0, 0, 940, 750);
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
                                ct = ImageTool.fPic(m_strPicPath + "���͵�.bmp", 0, 0, 940, 750);
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
                        WriteToFile("ǰ�����׵ص㳬ʱ");
                        return 3040;
                    }
                }
            }
            #endregion

            //���ֵ�
            #region
            su = false;
            if (IfTrading)
            {
                WriteToFile("�Ѿ��ڽ��׵�ͼ");
                for (int a = 0; a < 20; a++)
                {
                    KeyMouse.Sendkey('m');
                    Sleep(500);
                    at = ImageTool.fPic(m_strPicPath + "���͵�.bmp", 0, 0, 940, 750);
                    if (at.X > 0)
                    {
                        KeyMouse.MouseClick(at.X, at.Y, 1, 1, 500);
                        for (int c = 0; c < 3;c++ ) {
                            Sleep(500);
                            at = ImageTool.fPic(m_strPicPath + "��ͼ.bmp", 0, 645, 375, 790);
                            if (at.X > 0)
                            {
                                KeyMouse.SendEscKey();
                            }
                        }
                        for (int b = 0; b < 10; b++)
                        {
                            Sleep(500);
                            at = ImageTool.fPic(m_strPicPath + "���͵�Ŀ¼.bmp", 0, 0, 0, 0);
                            if (at.X > 0)
                            {
                                KeyMouse.MouseClick(at.X + 108, at.Y + 420, 1, 1, 500);
                                WriteToFile("���ｻ�׵ص�");
                                CaptureBmpInRect("���׵�ͼ");
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
                        WriteToFile("ǰ�����׵ص㳬ʱ");
                        return 3040;
                    }
                }
            }
            #endregion
            

            WriteToFile("�޷����ｻ�׵ص�");
            return 3040;
        }
        /// <summary>
        /// ���͵�Ŀ¼
        /// </summary>
        /// <returns></returns>
        public bool Transfer() {
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            Point ct = new Point(-1, -2);
            for (int a = 0; a < 5;a++ ) {
                at = ImageTool.fPic(m_strPicPath+"���͵�Ŀ¼.bmp",320,180,720,220);
                if(at.X>0){
                    Sleep(1000);
                    at = ImageTool.fPic(m_strPicPath+"���͵�Ŀ¼.bmp",320,180,720,220);
                    if (at.X > 0) {
                        WriteToFile("���ִ��͵�Ŀ¼");
                        for (int b = 0; b < 5; b++)
                        {
                            KeyMouse.MouseMove(at.X,at.Y);
                            bt = ImageTool.fPic(m_strPicPath + "�����ֵ�.bmp", at.X - 55, at.Y - 6, at.X + 237, at.Y + 447);
                            if(bt.X<0){
                                bt = ImageTool.fPic(m_strPicPath + "�����ֵ�1.bmp", at.X - 55, at.Y - 6, at.X + 237, at.Y + 447);
                            }
                            if (bt.X > 0)
                            {
                                WriteToFile("�������");
                                CaptureBmpInRect("����");


                                KeyMouse.MouseClick(bt.X + 3, bt.Y + 3, 1, 1, 200);
                                KeyMouse.MouseClick(bt.X + 3, bt.Y + 3, 1, 2, 500);

                                for (int c = 0; c < 10; c++)
                                {
                                    ct = ImageTool.fPic(m_strPicPath + "ȷ������.bmp", 0, 0, 0, 0);
                                    if (ct.X > 0)
                                    {
                                        KeyMouse.MouseClick(ct.X + 43, ct.Y + 90, 1, 1, 500);
                                        for (int d = 0; d < 10; d++)
                                        {
                                            Sleep(3000);
                                            ct = ImageTool.fPic(m_strPicPath + "�ֵ�.bmp", 760, 30, 1027, 200);
                                            if (ct.X > 0)
                                            {
                                                WriteToFile("���ｻ�׵�ͼ");
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
        /// ��Ϸ�����趨
        /// </summary>
        /// <returns></returns>
        public int SetUp() {
            strAllog = "�޸�ϵͳ����";
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);

            for (int a = 0; a < 10;a++ ) {
                KeyMouse.SendF12();
                Sleep(1000);
                at = ImageTool.fPic(m_strPicPath+"����.bmp",0,0,0,0);
                if(at.X>0){
                        KeyMouse.MouseClick(at.X - 24, at.Y + 37, 1, 1, 500);
                    for (int b = 0; b < 5;b++ ) {
                        bt = ImageTool.fPic(m_strPicPath + "�ܾ�����.bmp",at.X-173,at.Y,at.X+260,at.Y+568,5);
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
        /// ����˽����Ϣ
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
            strAllog = "�ȴ����׿����";
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            int k = 0;
            int bz = 0;
            int Time = 180;
            int number = 0;
            int nums = 0;
            thread_fun = new Thread(fun);
            thread_fun.Start();
            //����������������Ϣ
            WriteToFile("����˽����Ϣ");
            WriteToFile("˽������:"+m_strBuyerRoleName+" �����뵽2���ֵ´��͵㴦���ҷ��ͽ�������");
            string[] str = new string[3] {"/���Ļ�",m_strBuyerRoleName,"�����뵽2���ֵ´��͵㴦���ҷ��ͽ�������" };
            bool ToCl = false;
            for (int a = 0; a < 200;a++ ) {
                int time = Time - waittime;
                if (time <= 0)
                {
                    WriteToFile("�ȴ���ҷ����׳�ʱ");
                    thread_fun.Abort();
                    return 3100;
                }
                if(number==5&&nums==0){
                    bool su = Friend();
                    nums++;
                    Sleep(2000);
                    //if(!su){
                    //    WriteToFile("����˽��ʧ��");
                    //    return 3100;
                    //}
                }
                KeyMouse.SendEnterKey();
                at = ImageTool.fPic(m_strPicPath+"�����.bmp",0,580,390,755,5);
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
                            KeyMouse.SendKG();//����ո�
                        }
                        else
                        {
                            return 3120;
                        }
                    }
                    KeyMouse.SendEnterKey();
                    bt = ImageTool.fPic(m_strPicPath + "�����߻򲻴���.bmp", at.X + 534, at.Y - 214, at.X + 976, at.Y - 31);
                    if(bt.X>0){
                       if(bz==0){
                           WriteToFile("��Ҳ�����,����ʱ��");
                           bz++;
                       }
                        Time = 120;
                    }
                    for (int i = 0; i < 32;i++ ) {
                        bt = ImageTool.fPic(m_strPicPath + "���ױ�ʶ.bmp", 0,0,0,0);
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

            WriteToFile("�޷�����˽����Ϣ");
            return 3120;
        
        }
        /// <summary>
        /// ��˽��
        /// </summary>
        public bool Friend(){
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            Point ct = new Point(-1, -2);
            for (int a = 0; a < 10;a++ ) {
                
                KeyMouse.Sendkey('f');
                KeyMouse.MouseMove(0,0);
                at = ImageTool.fPic(m_strPicPath+"�༭����Ϣ.bmp",300,400,660,600);
                if(at.X>0){
                    KeyMouse.MouseClick(at.X+3,at.Y+5,1,1,3000);
                    for (int b = 0; b < 3;b++ ) {
                        
                        bt = ImageTool.fPic(m_strPicPath + "������Ϣ.bmp", 700, 200, 1000, 400);
                        if(bt.X>0){
                            KeyMouse.MouseClick(at.X+220, at.Y-385, 1, 1, 1000);

                            for (int i = 0; i < 20;i++ ) {
                                KeyMouse.MouseClick(bt.X + 104, bt.Y - 158, 1, 1, 200);
                                KeyMouse.MouseClick(bt.X + 104, bt.Y - 158, 1, 1, 500);
                                KeyMouse.SendBackSpaceKey(10);

                                //��ҽ�ɫ��
                                bool su = false;
                                su = ToClipboard(m_strBuyerRoleName);
                                if (su)
                                {
                                    KeyMouse.SendCtrlV();
                                }

                                KeyMouse.MouseClick(bt.X + 84, bt.Y - 60, 1, 1, 200);
                                KeyMouse.MouseClick(bt.X + 84, bt.Y - 60, 1, 1, 500);

                                //����
                                string content = "�����뵽2���ֵ´��͵㴦���ҷ��ͽ�������";
                                su = ToClipboard(content);
                                if (su)
                                {
                                    KeyMouse.SendCtrlV();
                                }

                                //����
                                KeyMouse.MouseClick(bt.X, bt.Y, 1, 1, 500);

                                Sleep(1000);
                                ct = ImageTool.fPic(m_strPicPath + "������.bmp", 370, 325, 650, 500);
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
            // bt = ImageTool.fPic(m_strPicPath + "������Ϣ.bmp", 700, 200, 1000, 400);
            // if (bt.X > 0) {
            //     KeyMouse.SendEscKey();
            // }
            //at = ImageTool.fPic(m_strPicPath + "�༭����Ϣ.bmp", 300, 400, 660, 600);
            //if(at.X>0){
            //    KeyMouse.SendEscKey();
            //}
            return false;
        }
        /// <summary>
        /// ���ݸ��Ƶ�ճ����
        /// </summary>
        /// <returns></returns>
        public bool ToClipboard(string str)
        {
            //���� �����ݸ��Ƶ�ճ����
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
                    WriteToFile("�޷���ȷ��������");
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        public int Trading() {
            Point at = new Point(-1,-2);
            Point bt = new Point(-1, -2);
            Point ct = new Point(-1, -2);
            Point dt = new Point(-1, -2);
            Point st = new Point(-1, -2);
            WriteToFile("��ҽ�ɫ��:��" + m_strBuyerRoleName + "��");
            WriteToFile("���׽��:��" + m_strBuyerItemNum + "��");
            strAllog = "�ȴ�����";
            string Num = null;//��ȡ�Ľ��
            long num = 0;//���ת����int
            for (int a = 0; a < 180;a++ ) {
                if(a==179){
                    WriteToFile("�ȴ���ҷ����׳�ʱ");
                    return 3100;
                }
                at = ImageTool.fPic(m_strPicPath+"���ױ�ʶ.bmp",0,0,0,0);
                if(at.X>0){
                    WriteToFile("�ѽ�����ҷ���Ľ���");
                    //ƥ�������
                    Sleep(2000);
                    CaptureBmpInRect("���׿�");
                    bool PlName = PlayerName(at.X - 82, at.Y - 3, at.X + 148, at.Y + 386);
                    if (PlName)
                    {
                        WriteToFile("ƥ��������ɹ�,��������");
                    }
                    else {
                        WriteToFile("ƥ�������ʧ��,ȡ������");
                        return 3120;
                    }

                    #region
                   
                    for (int b = 0; b < 10;b++ ) {
                        if(b==9){
                            st = ImageTool.fPic(m_strPicPath + "����ȡ��.bmp", 0, 0, 0, 0);
                            if (st.X > 0)
                            {
                                WriteToFile("����ʧ�ܻ�ȡ��");
                                return 3120;
                            }
                            WriteToFile("�޷�������");
                            return 3120;
                        }
                        KeyMouse.MouseClick(at.X-56,at.Y+150,1,1,500);
                        //������
                        bt = ImageTool.fPic(m_strPicPath + "������.bmp", 0, 0, 0, 0);
                        if(bt.X>0){
                            KeyMouse.MouseClick(bt.X+117,bt.Y+90,1,1,500);
                            KeyMouse.SendKeys(m_strBuyerItemNum, 200);
                            for (int n = 0; n < 20; n++)
                            {
                                //��ȡ����Ľ��
                                Num = CheckPic(bt.X -36, bt.Y+82, bt.X + 139, bt.Y + 103, 2);
                                if (Num == null || Num == "0")
                                {
                                    Sleep(1000);
                                    continue;
                                }
                                break;
                            }
                            //&&&&&&&&&&&&&&&&&&&&&&&&&
                            CaptureBmpInRect("������");
                            if (Num == null)
                            {
                                WriteToFile("��ȡ���ʧ��");
                                return 3120;
                            }

                            num = Int64.Parse(Num);
                            WriteToFile("����:" + Num);
                            if (num == Int64.Parse(m_strBuyerItemNum))
                            {

                                WriteToFile("���������ȷ");
                                KeyMouse.SendEnterKey();
                                //----------------
                                for (int b1 = 0; b1 < 10;b1++ ) {
                                    if (b1 > 8)
                                    {
                                        st = ImageTool.fPic(m_strPicPath + "����ȡ��.bmp", 0, 0, 0, 0);
                                        if (st.X > 0)
                                        {
                                            WriteToFile("����ʧ�ܻ�ȡ��");
                                            CaptureBmpInRect("����δ�ɹ�");
                                            return 3120;
                                        }
                                        WriteToFile("δ�ҵ���ȷ�������Ұ�ť");
                                        return 3120;
                                    }
                                    st = ImageTool.fPic(m_strPicPath + "�������.bmp", 360, 310, 650, 470);
                                    if (st.X > 0)
                                    {
                                        KeyMouse.MouseClick(st.X, st.Y + 105, 1, 1, 500);
                                    }
                                    //ȷ����ҵ�����
                                    bt = ImageTool.fPic(m_strPicPath + "ȷ������.bmp", 0, 0, 0, 0);
                                    if(bt.X>0){
                                        WriteToFile("ȷ������");
                                        KeyMouse.MouseClick(bt.X+54,bt.Y+103,1,1,500);

                                        //$$$$$$$$$$$$$$$$$
                                        for (int c = 0; c < 10;c++ ) {
                                            if(c==9){
                                                st = ImageTool.fPic(m_strPicPath + "�������.bmp", 360, 310, 650, 470);
                                                if (st.X > 0)
                                                {
                                                    KeyMouse.MouseClick(st.X, st.Y + 105, 1, 1, 500);
                                                }
                                                st = ImageTool.fPic(m_strPicPath + "������Ʒ.bmp", 360,310,650,470);
                                                if (st.X > 0)
                                                {
                                                    KeyMouse.MouseClick(st.X+135,st.Y+105,1,1,500);
                                                }
                                                st = ImageTool.fPic(m_strPicPath + "����ȡ��.bmp", 0, 0, 0, 0);
                                                if (st.X > 0)
                                                {
                                                    WriteToFile("����ʧ�ܻ�ȡ��");
                                                    return 3120;
                                                }
                                                WriteToFile("δ�ҵ��ύ���װ�ť");
                                                return 3120;
                                            }
                                            //׼���ύ����
                                            ct = ImageTool.fPic(m_strPicPath + "�����ύ.bmp", at.X-82,at.Y-3,at.X+148,at.Y+386);
                                            if(ct.X>0){
                                                //�˶������������
                                                CaptureBmpInRect("�˶Խ��");
                                                long SureNum = 0;
                                                string SureNums = "";

                                                for (int m = 0; m < 100;m++ ) {
                                                    SureNums = CheckPic1(at.X - 10, at.Y + 140, at.X + 135, at.Y + 161);
                                                    if(m==99){
                                                        WriteToFile("�˶��������:" + SureNums);
                                                        WriteToFile("�˶Խ�Ҳ���ȷ");
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
                                                WriteToFile("�˶��������:" + SureNum);
                                                if (SureNum == Int64.Parse(m_strBuyerItemNum))
                                                {
                                                    WriteToFile("�˶Խ����ȷ");
                                                }
                                                else {
                                                    WriteToFile("�˶Խ�Ҳ���ȷ");
                                                    return 3120;
                                                }
                                                //�ύ����
                                                WriteToFile("�ύ����");
                                                KeyMouse.MouseClick(ct.X+3,ct.Y+3,1,1,500);
                                                strAllog = "�ȴ�����ύ����";

                                                for (int d = 0; d < 61;d++ ) {
                                                    if(d>30){
                                                        st = ImageTool.fPic(m_strPicPath + "�������.bmp", 360, 310, 650, 470);
                                                        if (st.X > 0)
                                                        {
                                                            KeyMouse.MouseClick(st.X, st.Y + 105, 1, 1, 500);
                                                        }
                                                        st = ImageTool.fPic(m_strPicPath + "����ȡ��.bmp", 0, 0, 0, 0);
                                                        if (st.X > 0)
                                                        {
                                                            WriteToFile("����ʧ�ܻ�ȡ��");
                                                            CaptureBmpInRect("����δ�ɹ�");
                                                            return 3120;
                                                        }
                                                        WriteToFile("�ȴ�����ύ��ʱ");
                                                        return 3100;
                                                    }
                                                    //���Ҳ�ύ.������ɽ��װ�ť
                                                    dt = ImageTool.fPic(m_strPicPath + "��ɽ���.bmp", at.X - 82, at.Y - 3, at.X + 148, at.Y + 386);
                                                    if(dt.X>0){
                                                        WriteToFile("����Ѿ��ύ����");

                                                        WriteToFile("���������ͬ�⽻��");
                                                        Sleep(100);
                                                        if (OrdNo != "���Զ���" && !IsAskMail)
                                                        {
                                                            udpdd.theUDPSend(35, "", OrdNo);
                                                            for (int j = 0; j < 10; j++)
                                                            {
                                                                Sleep(1000);
                                                                if (IsAskMail)
                                                                {
                                                                    WriteToFile("������ͬ�⽻��");
                                                                    Sleep(500);
                                                                    break;
                                                                }
                                                                if (j >= 9)
                                                                {
                                                                    WriteToFile(OrdNo);
                                                                    WriteToFile(m_sign.ToString());
                                                                    WriteToFile("�ȴ�������ͬ�⽻�׳�ʱ");
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
                                                        strAllog = "�ȴ����ȷ���������";
                                                        for (int s = 0; s < 20;s++ ) {
                                                            //�жϽ��׿��Ƿ񻹴���
                                                            at = ImageTool.fPic(m_strPicPath + "���ױ�ʶ.bmp", 0, 0, 0, 0);
                                                            if(at.X<0){
                                                                WriteToFile("�����ȷ���������");
                                                                break;
                                                            }
                                                            if(s>15){
                                                                WriteToFile("�ȴ����ȷ��������ɳ�ʱ");
                                                                return 3100;
                                                            }
                                                            Sleep(2000);
                                                        }

                                                        
                                                        //+++++++++++
                                                        //##############
                                                        for (int s = 0; s < 5; s++) {
                                                            st = ImageTool.fPic(m_strPicPath + "�������.bmp", 0, 0, 0, 0); 
                                                            if(st.X>0){
                                                                //��������жϱ���������
                                                                int SNum1 = RemainBackPack();
                                                                if (SNum1 == 1)
                                                                {
                                                                    WriteToFile("���׳ɹ�");
                                                                    CaptureBmpInRect("���׳ɹ�");
                                                                    return 1000;
                                                                }
                                                                else
                                                                {
                                                                    WriteToFile("δ��⵽���׳ɹ�,�˹����");
                                                                    CaptureBmpInRect("����δ�ɹ�");
                                                                    return 4010;
                                                                }
                                                            }
                                                            st = ImageTool.fPic(m_strPicPath + "�������1.bmp", 0, 0, 0, 0);
                                                            if (st.X > 0)
                                                            {//��������жϱ���������
                                                                int SNum2 = RemainBackPack();
                                                                if (SNum2 == 1)
                                                                {
                                                                    WriteToFile("���׳ɹ�");
                                                                    CaptureBmpInRect("���׳ɹ�");
                                                                    return 1000;
                                                                }
                                                                else
                                                                {
                                                                    WriteToFile("δ��⵽���׳ɹ�,�˹����");
                                                                    CaptureBmpInRect("����δ�ɹ�");
                                                                    return 4010;
                                                                }
                                                            }
                                                            Sleep(200);
                                                        }
                                                        //δ�ҵ��������
                                                        WriteToFile("δ�ҵ����׳ɹ���־");
                                                        WriteToFile("��ʣ�����ж��Ƿ��Ѿ�����");
                                                        //δ�ҵ�������ɱ�ʶ,�жϱ���������
                                                        int SNum = RemainBackPack();
                                                        if (SNum == 1)
                                                        {
                                                            WriteToFile("�Ѿ�����");
                                                            CaptureBmpInRect("���׳ɹ�");
                                                            return 1000;
                                                        }
                                                        else {
                                                            WriteToFile("δ��⵽���׳ɹ�,�˹����");
                                                            CaptureBmpInRect("����δ�ɹ�");
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
                                st = ImageTool.fPic(m_strPicPath + "����ȡ��.bmp", 0, 0, 0, 0);
                                if (st.X > 0)
                                {
                                    WriteToFile("����ʧ�ܻ�ȡ��");
                                    return 3120;
                                }
                                WriteToFile("����������");
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
            WriteToFile("����ʧ��");
            return 3120;
        }
        /// <summary>
        /// ƥ�������
        /// </summary>
        /// <returns></returns>
        public bool PlayerName(int left, int top, int right, int bottom)
        {
            m_hGameWnd = User32API.FindWindow(null, "�漣����SUN");
            if (m_hGameWnd == IntPtr.Zero)
            {
                m_hGameWnd = User32API.GetDesktopWindow();
            }

            Point at = new Point(-1,-2);
            RECT rt = new RECT(left,top,right,bottom);
            bool b = false;
            for (int a = 0; a < 10;a++ ) {
                at = ImageTool.FindText(m_hGameWnd,m_strBuyerRoleName+"����Ʒ", Color.FromArgb(255, 133, 50), "����", 12, FontStyle.Bold, 1, 1, rt, 38);
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
        /// ����ǰƥ��������
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
                at = ImageTool.FindText1(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "����", 12, FontType, 0, 0, rt, 38);
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
                    at = ImageTool.FindText1(m_hGameWnd, a[j], Color.FromArgb(255, 255, 255), "����", 12, FontType, 0, 0, rt, 38);
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
        /// �򿪱�����ȡ���
        /// </summary>
        /// <returns></returns>
        public int RemainBackPack()
        {

            strAllog = "��ȡ���׺󱳰��������";
            Point at = new Point(-1, -2);
            RECT rt = new RECT(0, 0, 0, 0);
            string Num = null;//��ȡ�Ľ��
            long num = 0;//���ת����int
            long RemainNum = 0;//ʣ����
            KeyMouse.Sendkey('i');
            for (int a = 0; a < 7; a++)
            {
                at = ImageTool.fPic(m_strPicPath + "������ʶ.bmp", 700, 340, 1025, 600);
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
                    CaptureBmpInRect("ʣ����");
                    if (Num == null)
                    {
                        WriteToFile("��ȡ���׺���ʧ��");
                        return 4010;
                    }


                    RemainNum = BackPackNum - Int64.Parse(m_strBuyerItemNum);
                    if (Totransfer)
                    {
                        RemainNum = RemainNum - ToTrNum;
                        num = Int64.Parse(Num);
                        WriteToFile("����ʹ���˽�ң�" + ToTrNum);
                        WriteToFile("Ӧʣ����:" + RemainNum);
                        WriteToFile("ʵ��ʣ����:" + Num);
                        if (RemainNum == num)
                        {
                            
                            WriteToFile("����������ȷ");
                            KeyMouse.Sendkey('i');
                            return 1;
                        }
                        else
                        {
                            WriteToFile("������������ȷ");
                            return 4010;
                        } 
                    }
                    else {
                        num = Int64.Parse(Num);
                        WriteToFile("Ӧʣ����:" + RemainNum);
                        WriteToFile("ʵ��ʣ����:" + Num);
                        if (RemainNum == num)
                        {
                           
                            WriteToFile("����������ȷ");
                            KeyMouse.Sendkey('i');
                            return 1;
                        }
                        else
                        {
                            WriteToFile("������������ȷ");
                            return 4010;
                        } 
                    }
                   

                   
                }
                Sleep(500);
                if (a == 6)
                {
                    WriteToFile("��ȡ���׺���ʧ��");
                    return 4010;
                }
            }

            WriteToFile("��ȡ������Ҵ���");
            return 3120;

        }
        /// <summary>
        /// StopGTR����ֵ
        /// </summary>
        /// <returns></returns>
        public bool AskArgess()
        {
            if (OrdNo == "���Զ���")
            {
                WriteToFile("���Զ���,ѡ��1");
                return true;
            }
            string stradd = System.Windows.Forms.Application.StartupPath;
            int ipos = stradd.LastIndexOf("\\");
            stradd = stradd.Substring(0, ipos);

            stradd = stradd + "\\RCClient1.ini";
            if (!File.Exists(stradd))
            {
                string stp = string.Format("��ַ[{0}]������", stradd);
                WriteToFile(stp);
                return false;
            }
            string strTemp = FileRW.ReadFile(stradd);

            ServerAddr = MyStr.FindStr(strTemp, "ServerAddr=", "\r\n");

            string str = string.Format("��������ַ:[{0}]", ServerAddr);
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
                str = string.Format("StopGTR����ֵ[{0}]", Re);
                WriteToFile(str);
                return true;
            }
            else
            {
                str = string.Format("StopGTR����ֵ[{0}]", Re);
                WriteToFile(str);
                return false;
            }
        }
        /// <summary>
        /// �ر���Ϸ
        /// </summary>
        public void CloseGames()
        {
            ////�ر���Ϸ
            Point at = new Point(-1,-2);
            m_hGameWnd = User32API.FindWindow(null, "�漣����SUN");
            if(m_hGameWnd!=IntPtr.Zero){

                for (int i = 0; i < 2; i++)
                {
                    at = ImageTool.fPic(m_strPicPath + "�ر���Ϸ.bmp", 0, 0, 0, 0);
                    if (at.X > 0)
                    {
                        KeyMouse.MouseClick(at.X + 3, at.Y + 3, 1, 1, 500);
                        break;
                    }
                    at = ImageTool.fPic(m_strPicPath + "�ر���Ϸ1.bmp", 0, 0, 0, 0);
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
                    at = ImageTool.fPic(m_strPicPath + "�˳���Ϸ.bmp", 425,215,650,445);
                    if (at.X > 0)
                    {
                        KeyMouse.MouseClick(at.X + 3, at.Y + 3, 1, 1, 500);
                        for (int k = 0; k < 3;k++ ) {
                            at = ImageTool.fPic(m_strPicPath + "�˳���Ϸ1.bmp", 370,320,655,490);
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
            

            WriteToFile("�رղ�������");
            Game.RunCmd("taskkill /im  iexplore.exe /F");


        }
        /// <summary>
        /// ���󶩵�����
        /// </summary>
        /// <returns></returns>
        public static bool RequestOrderData()
        {
            string tmp = string.Format("FExeProcID={0}\r\nFRobotPort={1}\r\n", Program.pid, m_UDPPORT);
            udpdd.theUDPSend((int)TRANSTYPE.TRANS_REQUEST_ORDER, tmp, OrdNo);
            if (the_nRC2Port == 0)
            {//������
                m_strOrderData = FileRW.ReadFile("info.txt");
            }
            else
            { //��������ȡ
                m_strOrderData = "";
                string tmp1 = string.Format("FExeProcID={0}\r\nFRobotPort={1}\r\n", Program.pid, m_UDPPORT);
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



            string strlog = string.Format("��Ϸ��[{0}]", m_strGameName);
            WriteToFile(strlog);
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



            return true;
        }
        /// <summary>
        /// ��ʼ��
        /// </summary>
        public void AppInit()
        {
            //Kernel32API.WinExec("rasphone -d �������",6);
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
            tmp = string.Format("IP:{0},�汾��:{1},�ű��˿�{2}", Game.GetLocalIp(), ApplicationVersion.ToString(), m_UDPPORT);
            WriteToFile(tmp);
            return;
        }
        /// <summary>
        ///�߳���ͣ
        /// </summary>
        /// <param name="time">ʱ�䵥λ����</param>
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
        /// ��ͼ
        /// </summary>
        public void CaptureJpg()
        {
            WriteToFile("���������");
            string tmp = SetPicPathBmp(LocalPicPath, OrdNo);
            Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
            User32API.MakeSureDirectoryPathExists(m_strCapturePath);
            RECT rect = new RECT(0, 0, 1200, 890);
            Game.CaptureBmp(bm, rect, tmp);
            return;
        }
        /// <summary>
        /// ���ý�ͼ·��
        /// </summary>
        /// <param name="str">�ļ���·��</param>
        /// <param name="strPicID">ͼƬ���</param>
        /// <returns>ͼƬ·��</returns>
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
        /// bmp��ͼ
        /// </summary>
        /// <param name="strPicName">����</param>
        public void CaptureBmpInRect(string strPicName)
        {
            IntPtr desk = User32API.GetDesktopWindow();
            m_hGameWnd =User32API.FindWindow(null, "�漣����SUN");
            RECT rt = new RECT(0, 0, 0, 0);
            if (m_hGameWnd == IntPtr.Zero || strPicName == "ʧ��")
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
        /// �����ɫͼ
        /// </summary>
        /// <param name="strPicName">����</param>
        /// <param name="left">�߽磺��</param>
        /// <param name="top">�߽磺��</param>
        /// <param name="right">�߽磺��</param>
        /// <param name="bottom">�߽磺��</param>
        public void CaptureBmpInRect1(string strPicName, int left, int top, int right, int bottom)
        {
            string PicName = m_strProgPath + "\\����\\";
            Bitmap bm = ImageTool.GetScreenCapture(User32API.GetDesktopWindow());
            User32API.MakeSureDirectoryPathExists(strPicName);
            strPicName = PicName + strPicName + ".bmp";
            RECT rect = new RECT(left, top, right, bottom);
            Game.CaptureBmp(bm, rect, strPicName);

            //�������ҽ�ɫ������ͼƬ
            Bitmap bmp = new Bitmap(right - left+21, 20);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.White, new Rectangle(0, 0, right - left+20, 20));
            g.DrawString("���ҽ�ɫ����" + m_strSellerRoleName, new Font("����", 11, FontStyle.Regular), Brushes.Black, new PointF(0, 0));
            bmp.Save(PicName + "��ɫ.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            CutImage(PicName + "��ɫ���ģ��.bmp", PicName + "��ɫ���.bmp", right - left, bottom - top, 0, 0);
        }
        ///   <summary>   
        ///   ��ͼƬ�н�ȡ����������ͼ   
        ///   </summary>   
        ///   <param   name= "OriginalFile "> ԭʼͼƬ·�� </param>   
        ///   <param   name= "SaveFile "> ������ͼ·�� </param>   
        ///   <param   name= "width "> ��ȡͼƬ��� </param>   
        ///   <param   name= "height "> ��ȡͼƬ�߶� </param>   
        ///   <param   name= "CoordX "> ��ͼԭͼƬX���� </param>   
        ///   <param   name= "CoordY "> ��ȡԭͼƬY���� </param>   
        public static void CutImage(string OriginalFile, string SaveFile, int width, int height, int CoordX, int CoordY)
        {
            //�����ͼ   
            Image FromImage = Image.FromFile(OriginalFile);
            int x = 0;   //��ȡX����   
            int y = 0;   //��ȡY����   
            //ԭͼ��������ͼƬ��   ֮��       
            //������С��0(��ԭͼ��С��Ҫ���ɵ�ͼ)ʱ����ͼ���Ϊ��С��   ��ԭͼ���   X������Ϊ0     
            //���������0(��ԭͼ�����Ҫ���ɵ�ͼ)ʱ����ͼ���Ϊ����ֵ   ��width X������Ϊ   sX��CoordX֮���С��      
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

            //������ͼλͼ   
            Bitmap bitmap = new Bitmap(width, height);
            //������ͼ����   
            Graphics graphic = Graphics.FromImage(bitmap);
            //��ȡԭͼ��Ӧ����д����ͼ��   
            graphic.DrawImage(FromImage, 0, 0, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
            //����ͼ��������ͼ   
            Image SaveImage = Image.FromHbitmap(bitmap.GetHbitmap());
            //����ͼ��   
            SaveImage.Save(SaveFile, ImageFormat.Bmp);
            //�ͷ���Դ   
            SaveImage.Dispose();
            bitmap.Dispose();
            graphic.Dispose();
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
        /// ƴͼ
        /// </summary>
        /// <param name="strAllPic">ͼƬ���Ƽ���</param>
        /// <param name="strPicID">ƴͼID</param>
        /// <param name="bVerbical">true������bmp��false������jpg</param>
        /// <returns></returns>
        public int PinTu(string strAllPic, string strPicID, bool bVerbical)
        {
            string m_strProgPath1 =m_strProgPath + "\\����\\";
            string[] arrPicName;
            int num = SplitString(strAllPic, ",", out arrPicName);

            for (int i = 0; i < num + 1; i++)
            {
                if (arrPicName[i] == "����")
                {
                    CreatePlate("����", "", nZHPicWidth);
                    continue;
                }

                CreatePlate(m_strProgPath1 + arrPicName[i], "", nZHPicWidth);
            }
            string strPic = m_strProgPath1 + "ģ��.bmp";
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
                if (!File.Exists(m_strProgPath1 + arrPicName[i] + ".bmp"))
                    continue;
                Bitmap sbmp = new Bitmap(m_strProgPath1 + arrPicName[i] + ".bmp", true);
                //ImageTool.CreatBmpFromByte(bbmp, sbmp, ref x, ref y, ref z, "");
                ImageTool.BmpInsert(bbmp, sbmp, ref x, ref y, ref z, "");
                sbmp.Dispose();

                //ɾ��Сͼ
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
            File.Delete(m_strProgPath1 + "ģ��.bmp");
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
                if (ptMAX.X == 0 && ptMAX.Y == 0)
                {
                    WriteToFile("��ͼΪ��\r\n");
                    bPicFull = false;
                    return false;
                }

                if (ptBigPic.Y > 0)//����
                {
                    ptMAX.X = Math.Max(ptBigPic.X, ptMAX.X);
                    ptMAX.Y += ptBigPic.Y;
                }
                else
                    ptMAX.X += ptBigPic.X;

                if (width != nZHPicWidth)  //ȡ���������
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

            return true;
        }
        /// <summary>
        /// �����˹�����
        /// </summary>
        /// <param name="CodeType">���ݸ�ʽ 3</param>
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
                WriteToFile("���Զ���,ѡ��1");
                return "1";
            }
            IsAnswer = false;
            string strSendData;
            WriteToFile("������֤��");
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
                    WriteToFile("�ȴ���ɫ����");
            }
            WriteToFile("�ȴ���ɫ���س�ʱ");
            return "error";
        }
        /// <summary>
        ///post��������web�ӿ�
        /// </summary>
        /// <param name="url">�ӿ�url</param>
        /// <param name="postData">����</param>
        /// <returns>�ӿڷ��ؽ��</returns>
        public static string PostUrlData(string url, string postData)
        {

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            System.Net.HttpWebRequest objWebRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            objWebRequest.Method = "POST";
            objWebRequest.ContentType = "application/x-www-form-urlencoded";
            objWebRequest.ContentLength = byteArray.Length;
            Stream newStream = objWebRequest.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length); //д����� 
            newStream.Close();
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)objWebRequest.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);//Encoding.Default
            string textResponse = sr.ReadToEnd(); // ���ص�����
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

