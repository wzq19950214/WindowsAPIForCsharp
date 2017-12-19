using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GTR;

namespace PicImage
{
    class Program
    {
        static void Main(string[] args)
        {
            //Bitmap aa=new Bitmap("E:\\主界面.bmp");
            //Enlarge(aa, aa.Height, aa.Width);
            //Enlarge("D:\\背景.bmp", 1204, 1204);
            network aaa = new network();
            aaa.Network(args);
        }
    }
    class  network
    {
        public static string filname = "";
        public static string Imgname = "";
        public static Bitmap bmbm;
        public static string strOrdNo = "";
        public static bool udppp = false;
        private static udpSockets udpdd = new udpSockets();
        public static int duankou = 0;
        public static bool bigpic = false;
        public void Network(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[4];
                args[0] = "D:\\背景.bmp";
                args[1] = "D:\\透明\\";
                args[2] = "1";
                args[3] = "测试订单";
                strOrdNo = args[3];
            }
            Imgname = args[0];
            filname = args[1];
            int tt = filname.LastIndexOf("\\");
            tt = filname.Length - tt - 1;
            if(tt>1)
                filname += "\\";
            if (args[2].ToString() != "1" && args.Length > 3 && args[3].ToString() != "测试订单")
            {
                udppp = true;
                int.TryParse(args[2], out duankou);
                strOrdNo = args[3];
            }
            try
            {
                //strOrdNo = args[3];
                if (udppp)
                    udpdd.udpInit("127.0.0.1", duankou);
            }
            catch { }
            WriteToFile("背景图片:" + Imgname + "\n路径名称:" + filname + "\n端口号：" + duankou.ToString() + "订单号：" + strOrdNo);
            try
            {
                bmbm = pic(Imgname, Color.FromArgb(140, 140, 140));
                if (bigpic)
                    bmbm = Enlarge(bmbm, 3000, 3000);
                TraversalFile(filname);
            }
            catch (Exception e)
            {
                WriteToFile(e.ToString());
            }
        }
        /// <summary>
        /// 遍历文件
        /// </summary>
        /// <returns></returns>
        public  int TraversalFile(string dirPath)
        {
            List<string> list = new List<string>();//先定义list集合
            int count = 0; int num = 0;
            num = GetFiles(dirPath);
            WriteToFile(num.ToString() + "张截图");
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
                            using (Bitmap aaa = new Bitmap(dirPath + file.Name))
                            {
                                if (aaa.Width > 200 || aaa.Height > 200)
                                {
                                    arrName = file.Name.Replace(".jpg", ".png");
                                    if (bigpic)
                                        pintu(bmbm, dirPath + file.Name, dirPath + arrName, 4,bigpic);
                                    else
                                        pintu(bmbm, dirPath + file.Name, dirPath + arrName, 4);
                                    WriteToFile("拼图转换" + file.Name);
                                }
                            }
                            //using (FileStream fs = new FileStream(dirPath + file.Name, FileMode.Open, FileAccess.Read))
                            //{
                            //    //System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                            //    //Bitmap map = new Bitmap(image);
                            //    arrName = file.Name.Replace(".jpg", ".png");
                            //    pintu(bmbm, dirPath + file.Name, dirPath + arrName, 5);
                            //    //if (IsGetZie)
                            //    //    picResizePng(dirPath + file.Name, dirPath + arrName + ".png", 122, 222);
                            //    //else
                            //    //    picResizePng(dirPath + file.Name, dirPath + arrName + ".png", image.Width, image.Height);
                            //}
                            if (file.Name=="CYHX7_01.jpg")
                                File.Delete(dirPath + file.Name);
                        }
                        list.Add(arrName); //给list赋值                    
                    }
                    WriteToFile("开始转换拼图\r\n");
                    foreach (FileInfo file1 in Dir.GetFiles())//查找子目录 
                    {
                        string arrName1 = string.Empty;
                        //count++;
                        if (file1.Name.Contains("png"))
                        {
                            using (FileStream fs = new FileStream(dirPath + file1.Name, FileMode.Open, FileAccess.Read))
                            {
                                Image image = Image.FromStream(fs);
                                Bitmap map = new Bitmap(image);
                                arrName1 = file1.Name.Replace(".png", ".jpg");
                                map.Save(dirPath + arrName1, ImageFormat.Jpeg);
                                map.Dispose();
                                image.Dispose();
                                WriteToFile("拼图完成" + file1.Name);
                            }
                            File.Delete(dirPath + file1.Name);
                        }
                        //list.Add(arrName1); //给list赋值                    
                    }
                }
                catch (Exception e)
                {
                    WriteToFile(e.ToString());
                }
            }
            return list.Count;
        }
        public int GetFiles(string dir)
        {
            int num = 0;
            try
            {
                string[] files = Directory.GetFiles(dir);//得到文件
                foreach (string file in files)//循环文件
                {
                    string exname = file.Substring(file.LastIndexOf(".") + 1);//得到后缀名

                    if (".jpg".IndexOf(file.Substring(file.LastIndexOf(".") + 1)) > -1)//如果后缀名为.txt文件
                    {
                        num++;
                    }
                }
            }
            catch
            {

            }
            return num;
        }
        public  void pintu(Bitmap bmmp, string Img,string name, int touming)
        {
            int aa = 0; int bb = 0;
            Image img = Image.FromFile(Img);
            Graphics g = Graphics.FromImage(img);
            //Bitmap aaa = pic(filename, Img);
            aa = img.Width; bb = img.Height;
            if (img.Width > 1000 && img.Height > 1000)
            {
                aa = 1000; bb = 1000;
            }
            else if (img.Width > 1000 && img.Height < 1000)
            {
                aa = 1000; bb = img.Height;
            }
            else if (img.Width < 1000 && img.Height > 1000)
            {
                aa = img.Width; bb = 1000;
            }
            Rectangle srcRect = new Rectangle(0, 0, aa, bb);
            BitmapData bigBData = bmmp.LockBits(srcRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Bitmap bmp2 = new Bitmap(aa, bb, bigBData.Stride, PixelFormat.Format32bppArgb, bigBData.Scan0);
            //bmp2.Save("D:\\3333.png");
            Image watermark = new Bitmap(bmp2);
            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();
            //colorMap.OldColor = System.Drawing.Color.FromArgb(255, 0, 0, 0);
            //colorMap.NewColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };
            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
            //设置透明度
            float transparency = 0.5F;
            if (touming >= 1 && touming <= 10)
                transparency = (touming / 10.0F);
            float[][] colorMatrixElements = {
                                                 new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                  new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                  new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
                                                 new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                              };
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            int xpos = 0;
            int ypos = 0;
            xpos = (int)(img.Width * (float).01);
            ypos = (int)(img.Height * (float).01);
            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            int quality = 100;
            qualityParam[0] = quality;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;
            img.Save(name,ImageFormat.Png);
            bmmp.UnlockBits(bigBData);
            //bmmp.Dispose();
            bmp2.Dispose();
            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
        }
        public void pintu(Bitmap bmmp, string Img, string name, int touming,bool datu)
        {
            int aa = 0; int bb = 0;
            Image img = Image.FromFile(Img);
            Graphics g = Graphics.FromImage(img);
            //Bitmap aaa = pic(filename, Img);
            aa = img.Width; bb = img.Height;
            Rectangle srcRect = new Rectangle(0, 0, aa, bb);
            BitmapData bigBData = bmmp.LockBits(srcRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Bitmap bmp2 = new Bitmap(aa, bb, bigBData.Stride, PixelFormat.Format32bppArgb, bigBData.Scan0);
            //bmp2.Save("D:\\3333.png");
            Image watermark = new Bitmap(bmp2);
            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();
            //colorMap.OldColor = System.Drawing.Color.FromArgb(255, 0, 0, 0);
            //colorMap.NewColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };
            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);
            //设置透明度
            float transparency = 0.5F;
            if (touming >= 1 && touming <= 10)
                transparency = (touming / 10.0F);
            float[][] colorMatrixElements = {
                                                 new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                 new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                  new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                  new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
                                                 new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                              };
            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);
            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            int xpos = 0;
            int ypos = 0;
            xpos = (int)(img.Width * (float).01);
            ypos = (int)(img.Height * (float).01);
            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            int quality = 100;
            qualityParam[0] = quality;
            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;
            img.Save(name, ImageFormat.Png);
            bmmp.UnlockBits(bigBData);
            //bmmp.Dispose();
            bmp2.Dispose();
            g.Dispose();
            img.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shuiyin"></param>
        /// <param name="RGB"></param>
        /// <returns></returns>
        public  Bitmap pic(string shuiyin,Color RGB)
        {
            #region 滤色处理
            //Bitmap mypicbm = new Bitmap(shuiyin);
            // Rectangle srcRect1 = new Rectangle(0, 0, mypicbm.Width, mypicbm.Height);
            //BitmapData bigBData1 = mypicbm.LockBits(srcRect1, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            //Bitmap sssbtm = new Bitmap(mypicbm.Width, mypicbm.Height, bigBData1.Stride, PixelFormat.Format32bppArgb, bigBData1.Scan0);//生成透明背景;
            //sssbtm.Save("D:\\22.png");
            Bitmap mybm = new Bitmap(shuiyin);
            //Bitmap mybm1 = new Bitmap(filename);
            int Width = mybm.Width;
            int height = mybm.Height;
            Bitmap bm = new Bitmap(Width, height);//初始化一个记录滤色效果的图片对象
            int x, y;
            Color pixel;
            //Color pic = Color.FromArgb(140, 140, 140);
            for (x = 0; x < Width; x++)
            {
                for (y = 0; y < height; y++)
                {
                    pixel = mybm.GetPixel(x, y);//获取当前坐标的像素值
                    if (pixel.R == 8 && pixel.G == 8 && pixel.B == 8)
                        bm.SetPixel(x, y, Color.FromArgb(0, 255, 255, 255));//绘图
                    else if ((pixel.R >= 37 && pixel.R <= 45) && (pixel.G >= 37 && pixel.G <= 45) && (pixel.B >= 37 && pixel.B <= 45))
                    {
                        //int a = 150 - (pixel.R + pixel.G + pixel.B - 42 * 3) / 3;
                        bm.SetPixel(x, y, Color.FromArgb(255, RGB.R, RGB.G, RGB.B));//绘图
                    }
                    else if (pixel.R == 42 && pixel.G == 42 && pixel.B == 42)
                        bm.SetPixel(x, y, Color.FromArgb(255, RGB.R, RGB.G, RGB.B));//绘图
                    else if ((pixel.R >= 31 && pixel.R <= 38) && (pixel.G >= 31 && pixel.G <= 38) && (pixel.B >= 31 && pixel.B <= 38))
                    {
                        //int a = 150 - (pixel.R + pixel.G + pixel.B - 42 * 3) / 3;
                        bm.SetPixel(x, y, Color.FromArgb(255, RGB.R, RGB.G, RGB.B));//绘图
                    }
                    else
                    {
                        //int a = 150 - (pixel.R + pixel.G + pixel.B - 42 * 3) / 3;
                        bm.SetPixel(x, y, Color.FromArgb(200, RGB.R, RGB.G, RGB.B));//绘图
                    }
                }
            }
            //Rectangle srcRect = new Rectangle(0, 0, mybm1.Width, mybm1.Height);
            //BitmapData bigBData = bm.LockBits(srcRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            //Bitmap bmp2 = new Bitmap(mybm1.Width, mybm1.Height, bigBData.Stride, PixelFormat.Format32bppArgb, bigBData.Scan0);
            //bmp2.Save("D:\\22.png");
            //bmp2.Dispose();
            //bm.Save("D:\\55.png", ImageFormat.Png);
            //mypicbm.Dispose();
            //mypicbm.UnlockBits(bigBData1);
            mybm.Dispose();
            return bm;
            //bm.Save("D:\\22.png");
            #endregion
        }

        public Bitmap Enlarge(Bitmap qwe, int weight, int height)
        {
            Bitmap b = new Bitmap(weight, height);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            g.DrawImage(qwe, 0, 0, weight, height);
            g.Dispose();
            qwe.Dispose();
            b.Save("D:\\555555.jpg",ImageFormat.Jpeg);
            return b;
            //byte[] byteImage1;
            //Bitmap qwe = new Bitmap("D:\\背景.bmp");
            //MemoryStream ms = new MemoryStream();
            //qwe.Save(ms, qwe.RawFormat);
            ////byte[] byteImage = new Byte[ms.Length];
            //byteImage1 = ms.ToArray();

            ////byte[] pBMPData = new byte[1024 * 4 * 1024];
            //CreatBmpFromByte1(byteImage1, 1024, 1024).Save("D:\\2223.bmp", ImageFormat.Bmp);


            //Image img = Image.FromFile(aaaa);
            //Graphics g = Graphics.FromImage(img);
            
        }
        public  Bitmap CreatBmpFromByte1(Byte[] tmpData, int weight, int height)
        {
            //Stream stream = new
            Bitmap bmp = new Bitmap(weight, height, PixelFormat.Format32bppRgb);
            Rectangle srcRect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData srcBData = bmp.LockBits(srcRect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            //unsafe
            //{
            //    srcBData.Scan0 = tmpData.;
            //}

            //修改模板颜色
            //for (int i = 1; i < height - 1; i++)
            //{
            //    for (int j = 1; j < weight - 1; j++)
            //    {
            //        tmpData[i * weight * 4 + j * 4] = 255;
            //        tmpData[i * weight * 4 + j * 4 + 1] = 255;
            //        tmpData[i * weight * 4 + j * 4 + 2] = 255;
            //    }
            //}
            //////////
            System.Runtime.InteropServices.Marshal.Copy(tmpData, 0, srcBData.Scan0, srcBData.Stride * srcBData.Height);
            bmp.UnlockBits(srcBData);
            return bmp;
            //bmp.Save("t.bmp");
        }

        /// <summary>
        ///日志输出
        /// </summary>
        /// <param name="tmp">日志内容</param>
        public static void WriteToFile(string tmp)
        {
            if (udppp)
            {
                //udpdd.send(18,tmp,strOrdNo,duankou,"127.0.0.1");
                udpdd.theUDPSend(18, tmp, strOrdNo);
                FileRW.WriteToFile(tmp);
            }
            else
            {
                FileRW.WriteToFile(tmp);
                Console.WriteLine(tmp);
            }
            return;
        }
    }
    class FileRW
    {
        private static string _strPath = Application.StartupPath + "\\" + Application.ProductName + ".log";
        public static void InitLog()
        {
            using (File.Create(_strPath))
            { }
        }
        public static void WriteToFile(string strLog)
        {
            if (_strPath.Trim() == "")
            {
                return;
            }

            if (!File.Exists(_strPath))
                InitLog();
            StreamWriter sw = null;

            try
            {
                //sw = File.AppendText(_strPath);
                sw = new StreamWriter(_strPath, true, Encoding.Default);
                sw.WriteLine("{0}[{1}] ===> {2}", DateTime.Now.ToString("HH:mm:ss"), Application.ProductVersion, strLog);
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                if (sw != null)
                    sw.Close();
            }
        }
        public static void WriteToFile(string strLog, string strPath)
        {
            StreamWriter Asw = null;
            try
            {
                Asw = new StreamWriter(strPath, false, Encoding.Default);
                Asw.Write(strLog);
            }
            catch (Exception e)
            {
                throw e;
                //return;
            }
            finally
            {
                if (Asw != null)
                    Asw.Close();
            }
        }
        public static void WriteError(Exception ex)
        {
            WriteToFile(ex.ToString());
        }
        public static void WriteError(string strex)
        {
            WriteToFile(strex);
        }
        public static string ReadFile(string strPathName)
        {
            StreamReader sr;
            try
            {
                //sr = File.OpenText(strPathName);
                sr = new StreamReader(strPathName, Encoding.Default);
                string tmp = sr.ReadToEnd();
                sr.Close();
                return tmp;
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }

            return "";

        }
    }
}
