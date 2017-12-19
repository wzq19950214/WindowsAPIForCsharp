using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;


namespace GTR
{
   
    //public struct UDP_DATA
    //{
    //    public int type;				//��������	(  0: ����  10:״̬  20:�쳣  )
    //    public char[] szOrderNo;		//�������  
    //    public char[] szData;		//��ϸ���� 
    //}
    public enum TRANSTYPE : int
    {
        TRANS_NONE = 0,				//��ʼ״̬
        TRANS_REQUEST_ORDER = 1,				//���������󶩵�����					( ROBOT -> RC2 )
        TRANS_ORDER_DATA = 2,				//RC2���������ݴ����������				( RC2 -> ROBOT )
        TRANS_ORDER_DATA_RET = 3,				//�����˷����յ��������ݵ�ȷ����Ϣ		( ROBOT -> RC2 )

        TRANS_GAME_DATA = 10,				//�����˷��ض�������Ϸ����				( ROBOT -> RC2 ) 
        TRANS_GAME_DATA_RET = 11,				//RC2ȷ���յ������˵���Ϸ����			( RC2 -> ROBOT )

        TRANS_GOODS_STAUTS = 12,				//�����˷���ִ�е�����״̬				( ROBOT -> RC2 )
        TRANS_GOODS_STATUS_RET = 13,				//���߻������յ� TRANS_GOODS_STAUTS ��Ϣ( RC2 -> ROBOT )

        TRANS_ORDER_NEW_LOG = 18,				//�����˸���RC2������־ [�˼�¼������վ����ͳ��]
        TRANS_ORDER_NEW_LOG_RET = 19,				//RC2���߻������յ���־ [�˼�¼������վ����ͳ��]

        TRANS_REQ_IDCODE_RESULT = 30,				//����������GTR������֤��               ( ROBOT -> RC2 ) 
        TRANS_RES_IDCODE_RESULT = 31,				//���ʹ��������֤��ĵ������˳���      ( RC2 -> ROBOT ) 
        TRANS_IDCODE_INPUT_RESULT = 32,				//������������֤���Ľ�����͸��ͻ���  ( ROBOT -> RC2 )

        TRANS_AGREE_SENDMAIL = 35,				//���뷢���ʼ�
        TRANS_SEND_MAIL = 36,				//ͬ�ⷢ���ʼ�

        TRANS_INSERT_ORDER = 40,				//������붩��
        TRANS_AGREE_INSERT = 41,				//ͬ����붩��

        TRANS_ORDER_END = 50,				//����������ɣ������ƽ���				( ROBOT -> RC2 )
        TRANS_ORDER_END_RET = 51,				//���߻������յ� TRANS_ORDER_END ��Ϣ	( RC2 -> ROBOT )

        TRANS_ORDER_CANCEL = 52,				//���볷��								( ROBOT -> RC2 )
        TRANS_ORDER_CANCEL_RET = 53,				//���߻������յ� TRANS_ORDER_CANCEL ��Ϣ( RC2 -> ROBOT )

        TRANS_ORDER_ABOLISH = 54,				//ȡ������								( ROBOT -> RC2 )
        TRANS_ORDER_ABOLISH_RET = 55,				//���߻������յ� TRANS_ORDER_ABOLISH ��Ϣ( RC2 -> ROBOT )

        TRANS_ORDER_OP = 71,				//ת�˹�����								( ROBOT -> RC2 )
        TRANS_ORDER_OP_RET = 81,				//���߻������յ� TRANS_ORDER_OP ��Ϣ	( RC2 -> ROBOT )

        TRANS_ORDER_OP_SUCESS = 82,				//ת�˹��������								( ROBOT -> RC2 )
        TRANS_ORDER_OP_SUCESS_RET = 72,				//���߻������յ� TRANS_ORDER_OP_SUCESS ��Ϣ	( RC2 -> ROBOT )

        TRANS_RC2_TO_UDPSVR = 101,				// RC2��UDPServer����QQ��������
        TRANS_UDPSVR_TO_ROBOT = 102,				// UDPServer���ֻ��ű�����������ָ��QQ����������
        TRANS_ROBOT_TO_UDPSVR = 103,				// �ֻ��ű���������UDPServer����ָ��QQ�Ŷ�Ӧ����������
        TRANS_UDPSVR_TO_RC2 = 104,				// UDPServer��RC2����ָ��QQ����������
        // ��Ӧ�����ݸ�ʽ
        // UDPDATA->type=����
        // UDPDATA->szOrder=������
        // UDPDATA->data="FQQ=100000\r\nFToken=999999\r\n" ���಻�䡣
        // Add By ZLC 20130130 END

    };
    public struct UdpData
    {
        public string strOrderNO;
        public string strData;
        public int udpSignal;
    }

    // ���� UdpState��
    public class UdpState
    {
        public UdpClient udpClient = null;
        public IPEndPoint ipEndPoint = null;
        public const int BufferSize = 8192;
        public byte[] buffer = new byte[BufferSize];
        public int counter = 0;
    }
    class udpSockets
    {
        public const int UDP_DATA_LEN = 8192;
        public const int UDP_ORDERNO_LEN = 50;
        public const int UDP_INFO_LEN = 8192-50-4;
        public  Thread UdpRecProc;
        public UdpData udpdata;
        private ManualResetEvent receiveDone = new ManualResetEvent(false);

//        public void UdpRecFun()
//        {
//            while (true)
//            {
//                Thread.Sleep(50);
//                theUDPRec();

//                if (udpdata.udpSignal > 0 && udpdata.udpSignal < 256) 
//                {
//                    GTR_dnf.m_strOrderData = udpdata.strData;
//#if DEBUG
//                    Console.WriteLine(udpdata.udpSignal.ToString());
//                    Console.WriteLine(udpdata.strOrderNO);
//                    Console.WriteLine(udpdata.strData);
//#endif
//                    switch (udpdata.udpSignal)
//                    {
//                        case 2:
//                            //TRANS_ORDER_DATA ��������
//                            break;
//                        case 31:
//                            //TRANS_RES_IDCODE_RESULT �����
//                            break;
//                        case 36:
//                            //TRANS_SEND_MAIL ͬ���ʼ�
//                            GTR_dnf.IsAskMail = true;
//                            break;
//                        case 40:
//                            //TRANS_INSERT_ORDER ���붩��
//                            theUDPSend(18, "������붩��:", GTR_dnf.OrdNo);
//                            break;
//                    }

//                }

//            }
//        }

        UdpState recS;

        //�������ݻ�����
        public byte[] m_recvData = new byte[UDP_DATA_LEN];
        //public byte[] m_sendData = new byte[UDP_DATA_LEN];
        //public UDP_DATA udpdata;
        public static IPEndPoint RemoteIpEndPoint;//�����κ�
        public UdpClient theUDPSocket;
        //public static UdpClient theUDPSocket;
        public IPEndPoint IpSendPoint;// ����
        public int localPort = 6801;
        public void udpInit(string hostname, int port)
        {
            Random ran = new Random();
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    theUDPSocket = new UdpClient(localPort);
                    //theUDPSocket = new UdpClient(GTR_dnf.m_UDPPORT);
                    break;
                }
                catch (Exception e)
                {
                    localPort = ran.Next(6000, 8888);
                    //GTR_dnf.m_UDPPORT = ran.Next(6000, 9999);
                }
            }
            
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            //RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, GTR_dnf.m_UDPPORT);
            //theUDPSocket = new UdpClient(RemoteIpEndPoint);
            IpSendPoint = new IPEndPoint(IPAddress.Parse(hostname), port);
            recS = new UdpState();
            recS.udpClient = theUDPSocket;
            recS.ipEndPoint = RemoteIpEndPoint;
            UdpRecProc = new Thread(new ThreadStart(ReceiveMsg));
            UdpRecProc.IsBackground = true;
            UdpRecProc.Start();
            
            //theUDPSocket.Client.Blocking = false; //����Ϊ������ģʽ
            //RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);
            //UdpRecProc = new Thread(new ThreadStart(UdpRecFun));
            //UdpRecProc.Start();
        }
        public void Close()
        {
            theUDPSocket.Close();
        }

        public void theUDPSend(UdpData udpdata)
        {
            theUDPSend(udpdata.udpSignal, udpdata.strData, udpdata.strOrderNO);
        }
        public void theUDPSend(int type,string senddata,string orderno)
        {
            byte[] m_sendData = new byte[UDP_DATA_LEN];
            m_sendData[0] = Convert.ToByte(type);// Convert.ToByte(type.ToString());
            m_sendData[1] = Convert.ToByte(type / 256);
            byte[] ord,sdata;
            ord =Encoding.Default.GetBytes(orderno);
            sdata = Encoding.Default.GetBytes(senddata);
            if (ord.GetLength(0) > 0)
                Array.Copy(ord, 0, m_sendData, 4, ord.GetLength(0));
            if (sdata.GetLength(0) > 0)
                Array.Copy(sdata, 0, m_sendData, 54, sdata.GetLength(0));
            try
            {
                theUDPSocket.Send(m_sendData, UDP_DATA_LEN, IpSendPoint);
            }
            catch (Exception err)
            {
                throw err;
            }
            return;
        }

        // ���ջص�����
        //public void ReceiveCallback(IAsyncResult iar)
        //{
        //    UdpState udpState = iar.AsyncState as UdpState;
        //    if (iar.IsCompleted)
        //    {
        //        try
        //        {
        //            m_recvData = udpState.udpClient.EndReceive(iar, ref recS.ipEndPoint);
        //            int type = 0;
        //            for (int i = 3; i >= 0; i--)
        //            {
        //                type = type * 256 + Convert.ToInt32(m_recvData[i]);
        //            }
        //            udpdata.udpSignal = type;
                    
        //            udpdata.strOrderNO = MyStr.GetCut(Encoding.Default.GetString(m_recvData, 4, UDP_ORDERNO_LEN));
                    

        //            int num = 0;
        //            for (int i = 54; i < m_recvData.Length; i++)
        //            {
                        
                       
        //                if (m_recvData[i] == 0)
        //                {
        //                    num=i;
        //                    //Console.ReadKey();
        //                    break;
        //                }
                        
        //            }
                    
                    

        //            udpdata.strData = MyStr.GetCut(Encoding.Default.GetString(m_recvData, UDP_ORDERNO_LEN + 4, num - 54));
        //            QQlogin.Form1.m_strOrderData = udpdata.strData;
        //            //FileRW.WriteToFile("222\r\n");
        //            switch (udpdata.udpSignal)
        //            {
        //                case 2:
        //                    //TRANS_ORDER_DATA ��������
        //                    break;
        //                case 31:
        //                    //TRANS_RES_IDCODE_RESULT �����
        //                    break;
        //                case 36:
        //                    //TRANS_SEND_MAIL ͬ���ʼ�
        //                    QQlogin.Form1.IsAskMail = true;
        //                    break;
        //                case 40:
        //                    //TRANS_INSERT_ORDER ���붩��
        //                    theUDPSend(18, "������붩��:", QQlogin.Form1.OrdNo);
        //                    break;
        //                case 51:
        //                    QQlogin.Form1.bYiJiao = true;
        //                    break;
        //            }
                   
        //            receiveDone.Set();
        //        }
        //        catch (Exception Err)
        //        {
        //            //FileRW.WriteToFile("111\r\n");
        //            //throw Err;
        //            //uint IOC_IN = 0x80000000;
        //            //uint IOC_VENDOR = 0x18000000;
        //            //uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
        //            //udpState.udpClient.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
        //        }
        //    }
        //}
        //
        public void ReceiveMsg()
        {
            //uint IOC_IN = 0x80000000;
            //uint IOC_VENDOR = 0x18000000;
            //uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            //theUDPSocket.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            while (true)
            {
                try
                {
                    lock (this)
                    {
                        //theUDPSocket.BeginReceive(new AsyncCallback(ReceiveCallback), recS);
                        receiveDone.WaitOne();
                        Thread.Sleep(100);
                    }
                }
                catch //(Exception err)
                {
                    //throw err;
                }
            }
        }
        //public UdpData theUDPRec()
        //{
        //    udpdata.udpSignal = 0;
        //    udpdata.strOrderNO = "";
        //    udpdata.strData = "";
        //    int buffSizeCurrent;
        //    buffSizeCurrent = theUDPSocket.Client.Available;//ȡ�û�������ǰ�����ݵĸ���  
        //    if (buffSizeCurrent > 0)
        //    {
        //        m_recvData = theUDPSocket.Receive(ref RemoteIpEndPoint);
        //        //value = Encoding.ASCII.GetString(receiveBytes);
        //        //byte[] ord = new byte[UDP_ORDERNO_LEN];
        //        //byte[] sdata = new byte[UDP_INFO_LEN];
        //        //Array.Copy(m_recvData, 4, ord, 0, UDP_ORDERNO_LEN);
        //        //Array.Copy(m_recvData, UDP_ORDERNO_LEN + 4, sdata, 0, UDP_INFO_LEN);
        //        int type=0;
        //        for (int i = 3; i >= 0; i--)
        //        {
        //            type = type * 256 + Convert.ToInt32(m_recvData[i]);
        //        }
        //        udpdata.udpSignal = type;
        //        //udpdata.strOrderNO = Encoding.Default.GetString(ord);
        //        //udpdata.strData = Encoding.Default.GetString(sdata);
        //        udpdata.strOrderNO = MyStr.GetCut(Encoding.Default.GetString(m_recvData, 4, UDP_ORDERNO_LEN));
        //        udpdata.strData = MyStr.GetCut(Encoding.Default.GetString(m_recvData, UDP_ORDERNO_LEN + 4, UDP_INFO_LEN));
                
        //    }

        //    return udpdata;
        //}
        //public int getUdpType()
        //{
        //    return theUDPRec().udpSignal;
        //}
        //public string getUdpData()
        //{
        //    return theUDPRec().strData;
        //}


        //������Ϣ
        public void send(int type, string senddata, string orderno, int port, string hostname)
        {
            IPEndPoint SendPoint = new IPEndPoint(IPAddress.Parse(hostname), port);
            //UdpClient UDPSocket = new UdpClient(hostname,port);
            byte[] m_sendData = new byte[UDP_DATA_LEN];
            m_sendData[0] = Convert.ToByte(type);// Convert.ToByte(type.ToString());
            m_sendData[1] = Convert.ToByte(type / 256);
            byte[] ord, sdata;
            ord = Encoding.Default.GetBytes(orderno);
            sdata = Encoding.Default.GetBytes(senddata);
            if (ord.GetLength(0) > 0)
                Array.Copy(ord, 0, m_sendData, 4, ord.GetLength(0));
            if (sdata.GetLength(0) > 0)
                Array.Copy(sdata, 0, m_sendData, 54, sdata.GetLength(0));
            try
            {
                theUDPSocket.Send(m_sendData, UDP_DATA_LEN, SendPoint);
            }
            catch
            { 
            }
            
        }
    }
}