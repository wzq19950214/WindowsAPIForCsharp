using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using GTR;

namespace GTR
{
    class Program
    {
        public static int AppRunTime = 0;

        public static bool bRelease = false;//�Ƿ�Ϊ���ϰ�
        private static Thread Fun1Proc;
        public static uint pid = User32API.GetCurrentProcessId();
        [DllImport("user32.dll", EntryPoint = "GetKeyState")]
        public static extern int GetKeyState(int nVirtKey);
        public static void Fun1()
        {
            while (true)
            {
                if ((GetKeyState(162) & 0x8000) != 0 && (GetKeyState('Q') & 0x8000) != 0)
                {

                    Application.Run(new RC2APP.SplashTip("��������", System.Drawing.Color.Red, 1500, "����", 100));
                    Environment.Exit(0);
                }
                Thread.Sleep(100);
            }
        }
        /// <summary>
        /// Ӧ�ó��������ڵ㡣
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Fun1Proc = new Thread(new ThreadStart(Fun1));
            // C#���̱߳��ʱ��Thread�����и�IsBackground���ԣ�����Ϊtrue���ɣ������̹߳ر�ʱ�����߳���֮�ر�
            Fun1Proc.IsBackground = true;

            Fun1Proc.Start();

            MY.udpdd = new udpSockets();
            MY.the_nRC2Port = 0;//����
            if (args.Length > 0)
            {
                bRelease = true;
                MY.OrdNo = args[0];
                MY.the_nRC2Port = Convert.ToInt32(args[2]);
                Console.WriteLine("����");
                Console.WriteLine(args.Length);//3
                Console.WriteLine(args[0]);//������
                Console.WriteLine(args[1]);//0
                Console.WriteLine(args[2]);//�˿ں�
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
                System.Environment.Exit(-1);//�˳�����̨
            }
            new Thread(() =>
             {
                 MessageBox.Show("�����쳣�˳�");
                 Thread.Sleep(2000);
             }).Start();
            Thread.Sleep(2000);


            Fun1Proc.Abort();//��ֹ�߳�
            MY.udpdd.Close();//�ر�UDP ��������������̨ ��ΪUDP�Ƚ�����
            System.Environment.Exit(0);//�˳�����̨
        }
    }
}
