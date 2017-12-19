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

        public static bool bRelease = false;//是否为线上版
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
                    if (AppRunTime > 300)
                        break;
                    MY.udpdd.theUDPSend(18, "程序运行中，请稍等...", MY.OrdNo);
                    Console.WriteLine(AppRunTime.ToString());
                }
            }
        }
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Fun1Proc = new Thread(new ThreadStart(Fun1));
            // C#多线程编程时，Thread对象有个IsBackground属性，设置为true即可，在主线程关闭时，子线程随之关闭
            Fun1Proc.IsBackground = true;

            Fun1Proc.Start();

            MY.udpdd = new udpSockets();
            MY.the_nRC2Port = 0;//测试
            if (args.Length > 0)
            {
                bRelease = true;
                MY.OrdNo = args[0];
                MY.the_nRC2Port = Convert.ToInt32(args[2]);
                Console.WriteLine("参数");
                Console.WriteLine(args.Length);//3
                Console.WriteLine(args[0]);//订单号
                Console.WriteLine(args[1]);//0
                Console.WriteLine(args[2]);//端口号
            }
            MY.udpdd.udpInit("127.0.0.1", MY.the_nRC2Port);
            MY.m_UDPPORT = MY.udpdd.localPort;
            MY lol = new MY();
            try
            {
                lol.StartToWork();
            }
            catch (Exception e)
            {
                System.Environment.Exit(-1);//退出控制台
            }
            Fun1Proc.Abort();//中止线程
            MY.udpdd.Close();//关闭UDP 【部分两个控制台 因为UDP先结束了
            System.Environment.Exit(0);//退出控制台
        }
    }
}
