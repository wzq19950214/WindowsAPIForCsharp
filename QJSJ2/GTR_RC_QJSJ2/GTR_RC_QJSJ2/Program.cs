using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using GTR;

namespace GTR
{
    class Program
    {
        public static int AppRunTime = 0;

        public static bool bRelease = false;//�Ƿ�Ϊ���ϰ
        private static Thread Fun1Proc;
        public static uint pid = User32API.GetCurrentProcessId();

        public static void Fun1()
        {

            while (true)
            {
                AppRunTime++;
                Thread.Sleep(1000);
                if (AppRunTime > 0 && AppRunTime % 20 == 0)
                {
                    if (AppRunTime > 200)
                        break;
                    QJSJ2.udpdd.theUDPSend(18, QJSJ2.strAllog, QJSJ2.OrdNo);
                    QJSJ2.WriteToFile(QJSJ2.strAllog);
                    //Console.WriteLine(AppRunTime.ToString());
                }
            }
        }
        /// <summary>
        /// �Ӧ�ó��������ڵ㡣
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Fun1Proc = new Thread(new ThreadStart(Fun1));
            Fun1Proc.IsBackground = true;
            Fun1Proc.Start();
            
            QJSJ2.udpdd = new udpSockets();
            QJSJ2.the_nRC2Port = 0;
            if (args.Length > 0)
            {
                bRelease = true;
                QJSJ2.OrdNo = args[0];
                QJSJ2.the_nRC2Port = Convert.ToInt32(args[2]);
                Console.WriteLine("���");
                Console.WriteLine(args.Length);//3
                Console.WriteLine(args[0]);//�
                Console.WriteLine(args[1]);//0
                Console.WriteLine(args[2]);//Ŷ˿ں
                //string ConsoleTitle = null;
                //ConsoleTitle = String.IsNullOrEmpty(ConsoleTitle) ? Console.Title : ConsoleTitle;
                //IntPtr hWnd = User32API.FindWindow("ConsoleWindowClass", ConsoleTitle);
                //if (hWnd != IntPtr.Zero)
                //{
                //    User32API.ShowWindow(hWnd, 0);
                //}  

            }
            //string ConsoleTitle = null;
            //ConsoleTitle = String.IsNullOrEmpty(ConsoleTitle) ? Console.Title : ConsoleTitle;
            //IntPtr hWnd = User32API.FindWindow("ConsoleWindowClass", ConsoleTitle);
            //if (hWnd != IntPtr.Zero)
            //    User32API.ShowWindow(hWnd, 0);//���ؿ���̨
            QJSJ2.udpdd.udpInit("127.0.0.1", QJSJ2.the_nRC2Port);
            //QJSJ2.m_UDPPORT = QJSJ2.udpdd.localPort;
            QJSJ2 Sgs = new QJSJ2();
            try
            {
                Sgs.StartToWork();
            }
            catch(Exception e)
            {
                QJSJ2.WriteToFile(e.ToString());
            }
            Fun1Proc.Abort();
            QJSJ2.udpdd.Close();
            System.Environment.Exit(0);
        }
    }
}