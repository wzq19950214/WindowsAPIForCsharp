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
    //    public int type;				//数据类型	(  0: 数据  10:状态  20:异常  )
    //    public char[] szOrderNo;		//订单编号  
    //    public char[] szData;		//详细数据 
    //}
    public enum TRANSTYPE : int
    {
        TRANS_NONE = 0,				//初始状态
        TRANS_REQUEST_ORDER = 1,				//机器人请求订单数据					( ROBOT -> RC2 )
        TRANS_ORDER_DATA = 2,				//RC2将订单数据传输给机器人				( RC2 -> ROBOT )
        TRANS_ORDER_DATA_RET = 3,				//机器人返回收到订单数据的确认信息		( ROBOT -> RC2 )

        TRANS_GAME_DATA = 10,				//机器人返回订单的游戏数据				( ROBOT -> RC2 ) 
        TRANS_GAME_DATA_RET = 11,				//RC2确认收到机器人的游戏数据			( RC2 -> ROBOT )

        TRANS_GOODS_STAUTS = 12,				//机器人返回执行的物流状态				( ROBOT -> RC2 )
        TRANS_GOODS_STATUS_RET = 13,				//告诉机器人收到 TRANS_GOODS_STAUTS 消息( RC2 -> ROBOT )

        TRANS_ORDER_NEW_LOG = 18,				//机器人告诉RC2处理日志 [此记录用于主站数据统计]
        TRANS_ORDER_NEW_LOG_RET = 19,				//RC2告诉机器人收到日志 [此记录用于主站数据统计]

        TRANS_REQ_IDCODE_RESULT = 30,				//机器人请求GTR处理验证码               ( ROBOT -> RC2 ) 
        TRANS_RES_IDCODE_RESULT = 31,				//发送处理完的验证码的到机器人程序      ( RC2 -> ROBOT ) 
        TRANS_IDCODE_INPUT_RESULT = 32,				//机器人输入验证码后的结果发送给客户端  ( ROBOT -> RC2 )

        TRANS_AGREE_SENDMAIL = 35,				//申请发送邮件
        TRANS_SEND_MAIL = 36,				//同意发送邮件

        TRANS_INSERT_ORDER = 40,				//申请插入订单
        TRANS_AGREE_INSERT = 41,				//同意插入订单

        TRANS_ORDER_END = 50,				//订单处理完成，正常移交。				( ROBOT -> RC2 )
        TRANS_ORDER_END_RET = 51,				//告诉机器人收到 TRANS_ORDER_END 消息	( RC2 -> ROBOT )

        TRANS_ORDER_CANCEL = 52,				//申请撤单								( ROBOT -> RC2 )
        TRANS_ORDER_CANCEL_RET = 53,				//告诉机器人收到 TRANS_ORDER_CANCEL 消息( RC2 -> ROBOT )

        TRANS_ORDER_ABOLISH = 54,				//取消订单								( ROBOT -> RC2 )
        TRANS_ORDER_ABOLISH_RET = 55,				//告诉机器人收到 TRANS_ORDER_ABOLISH 消息( RC2 -> ROBOT )

        TRANS_ORDER_OP = 71,				//转人工操作								( ROBOT -> RC2 )
        TRANS_ORDER_OP_RET = 81,				//告诉机器人收到 TRANS_ORDER_OP 消息	( RC2 -> ROBOT )

        TRANS_ORDER_OP_SUCESS = 82,				//转人工操作完成								( ROBOT -> RC2 )
        TRANS_ORDER_OP_SUCESS_RET = 72,				//告诉机器人收到 TRANS_ORDER_OP_SUCESS 消息	( RC2 -> ROBOT )

        TRANS_RC2_TO_UDPSVR = 101,				// RC2向UDPServer请求QQ令牌数据
        TRANS_UDPSVR_TO_ROBOT = 102,				// UDPServer向手机脚本机器人请求指定QQ的令牌数据
        TRANS_ROBOT_TO_UDPSVR = 103,				// 手机脚本机器人向UDPServer发送指定QQ号对应的令牌数据
        TRANS_UDPSVR_TO_RC2 = 104,				// UDPServer向RC2发送指定QQ的令牌数据
        // 对应的数据格式
        // UDPDATA->type=信令
        // UDPDATA->szOrder=订单号
        // UDPDATA->data="FQQ=100000\r\nFToken=999999\r\n" 其余不变。
        // Add By ZLC 20130130 END

    };
    public struct UdpData
    {
        public string strOrderNO;
        public string strData;
        public int udpSignal;
    }

    // 定义 UdpState类
    public class UdpState
    {
        public UdpClient udpClient = null;
        public IPEndPoint ipEndPoint = null;
        public const int BufferSize = 4096;
        public byte[] buffer = new byte[BufferSize];
        public int counter = 0;
    }
    class udpSockets
    {
        private int UdpQuit = 0;
        public const int UDP_DATA_LEN = 4096;
        public const int UDP_ORDERNO_LEN = 50;
        public const int UDP_INFO_LEN = 4096 - 50 - 4;
        private static Thread UdpRecProc;
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
        //                    QJSJ2.m_strOrderData = udpdata.strData;
        //#if DEBUG
        //                    Console.WriteLine(udpdata.udpSignal.ToString());
        //                    Console.WriteLine(udpdata.strOrderNO);
        //                    Console.WriteLine(udpdata.strData);
        //#endif
        //                    switch (udpdata.udpSignal)
        //                    {
        //                        case 2:
        //                            //TRANS_ORDER_DATA 订单数据
        //                            break;
        //                        case 31:
        //                            //TRANS_RES_IDCODE_RESULT 答题答案
        //                            break;
        //                        case 36:
        //                            //TRANS_SEND_MAIL 同意邮寄
        //                            QJSJ2.IsAskMail = true;
        //                            break;
        //                        case 40:
        //                            //TRANS_INSERT_ORDER 插入订单
        //                            theUDPSend(18, "申请插入订单:", QJSJ2.OrdNo);
        //                            break;
        //                    }

        //                }

        //            }
        //        }

        UdpState recS;

        //接收数据缓冲区
        public byte[] m_recvData = new byte[UDP_DATA_LEN];
        //public byte[] m_sendData = new byte[UDP_DATA_LEN];
        //public UDP_DATA udpdata;
        public static IPEndPoint RemoteIpEndPoint;//接收任何
        public static UdpClient theUDPSocket;
        public IPEndPoint IpSendPoint;// 发送

        public void udpInit(string hostname, int port)
        {
            while (true)
            {
                try
                {
                    theUDPSocket = new UdpClient(QJSJ2.m_UDPPORT);
                    break;
                }
                catch (Exception err)
                {
                    FileRW.WriteError(err);
                    QJSJ2.m_UDPPORT++;
                }
            }
            RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
            //RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, QJSJ2.m_UDPPORT);
            //theUDPSocket = new UdpClient(RemoteIpEndPoint);
            IpSendPoint = new IPEndPoint(IPAddress.Parse(hostname), port);
            recS = new UdpState();
            recS.udpClient = theUDPSocket;
            recS.ipEndPoint = RemoteIpEndPoint;
            UdpRecProc = new Thread(new ThreadStart(ReceiveMsg));
            UdpRecProc.Start();

            //theUDPSocket.Client.Blocking = false; //设置为非阻塞模式
            //RemoteIpEndPoint = new IPEndPoint(IPAddress.Parse(hostname), port);
            //UdpRecProc = new Thread(new ThreadStart(UdpRecFun));
            //UdpRecProc.Start();
        }
        public void Close()
        {
            UdpQuit++;
            while (UdpQuit > 10)
            {
                UdpRecProc.Abort();
                theUDPSocket.Close();
                return;
            }
            return;
        }

        public void theUDPSend(UdpData udpdata)
        {
            theUDPSend(udpdata.udpSignal, udpdata.strData, udpdata.strOrderNO);
        }

        public void theUDPSend(int type, string senddata, string orderno)
        {
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
                theUDPSocket.Send(m_sendData, 4096, IpSendPoint);
            }
            catch (Exception err)
            {
                FileRW.WriteError(err);
                //throw err;
            }
            return;
        }
        public void theUDPSendToVS(int type, string senddata, string orderno)
        {
            IPEndPoint IpSendVSPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6800);
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
                theUDPSocket.Send(m_sendData, UDP_DATA_LEN, IpSendVSPoint);
            }
            catch (Exception err)
            {
                throw err;
            }
            return;
        }
        // 接收回调函数
        public void ReceiveCallback(IAsyncResult iar)
        {
            UdpState udpState = iar.AsyncState as UdpState;
            if (iar.IsCompleted)
            {
                //m_recvData = theUDPSocket.EndReceive(iar, ref RemoteIpEndPoint);
                try
                {
                    m_recvData = udpState.udpClient.EndReceive(iar, ref recS.ipEndPoint);
                    int type = 0;
                    for (int i = 3; i >= 0; i--)
                    {
                        type = type * 256 + Convert.ToInt32(m_recvData[i]);
                    }
                    udpdata.udpSignal = type;
                    udpdata.strOrderNO = MyStr.GetCut(Encoding.Default.GetString(m_recvData, 4, UDP_ORDERNO_LEN));
                    udpdata.strData = MyStr.GetCut(Encoding.Default.GetString(m_recvData, UDP_ORDERNO_LEN + 4, UDP_INFO_LEN));
                    //#if DEBUG        
                    Console.WriteLine(udpdata.udpSignal.ToString());
                    Console.WriteLine(udpdata.strOrderNO);
                    if (udpdata.strOrderNO != QJSJ2.OrdNo)
                    {

                        //return;
                       // FileRW.WriteToFile("收到" + udpdata.strOrderNO+"订单信息");
                    }
                    Console.WriteLine(udpdata.strData);
                    //#endif
                    QJSJ2.m_strOrderData = udpdata.strData;
                    QJSJ2.m_sign = udpdata.udpSignal;
                    switch (udpdata.udpSignal)
                    {
                        case 2:
                            //TRANS_ORDER_DATA 订单数据
                            break;
                        case 31:
                            //QJSJ2.m_strAnwer = udpdata.strData;
                            //TRANS_RES_IDCODE_RESULT 答题答案
                            break;
                        case 36:
                            //TRANS_SEND_MAIL 同意邮寄
                            QJSJ2.IsAskMail = true;
                            break;
                        case 40:
                            theUDPSend(41, "1", udpdata.strOrderNO);
                            //if (QJSJ2.OrdList.IndexOf(udpdata.strOrderNO) < 0)
                            //{
                            //    QJSJ2.OrdList.Add(udpdata.strOrderNO);
                            //}
                            //if (QJSJ2.InList.IndexOf(udpdata.strOrderNO) < 0)
                            //{
                            //    QJSJ2.InList.Add(udpdata.strOrderNO);
                            //}
                            //if (QJSJ2.IsAskMail)
                            //{
                            //    theUDPSend(35, "", udpdata.strOrderNO);
                            //}
                            break;
                        case 51:
                            QJSJ2.bYiJiao = true;
                            break;
                        case 81:
                            //接收人工干预详细信息
                            QJSJ2.m_strRC2Data = udpdata.strData;
                            QJSJ2.m_bHumanAgree = true;
                            break;
                        case 82:
                            //人工干预完成
                            QJSJ2.m_bHumanFinish = true;
                            break;
                    }
                    receiveDone.Set();
                }
                catch (Exception Err)
                {
                    FileRW.WriteError(Err);
                    //throw Err;
                    //uint IOC_IN = 0x80000000;
                    //uint IOC_VENDOR = 0x18000000;
                    //uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                    //udpState.udpClient.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                }
            }
        }
        //
        public void ReceiveMsg()
        {
            uint IOC_IN = 0x80000000;
            uint IOC_VENDOR = 0x18000000;
            uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            theUDPSocket.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
            while (UdpQuit == 0)
            {
                try
                {
                    lock (this)
                    {
                        theUDPSocket.BeginReceive(new AsyncCallback(ReceiveCallback), recS);
                        receiveDone.WaitOne();
                        Thread.Sleep(100);
                    }
                }
                catch (Exception err)
                {
                    FileRW.WriteError(err);
                    //throw err;
                }
            }
            UdpQuit = 999;
        }
        //public UdpData theUDPRec()
        //{
        //    udpdata.udpSignal = 0;
        //    udpdata.strOrderNO = "";
        //    udpdata.strData = "";
        //    int buffSizeCurrent;
        //    buffSizeCurrent = theUDPSocket.Client.Available;//取得缓冲区当前的数据的个数  
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


        //发送消息
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
            theUDPSocket.Send(m_sendData, 4096, SendPoint);

        }
    }
}