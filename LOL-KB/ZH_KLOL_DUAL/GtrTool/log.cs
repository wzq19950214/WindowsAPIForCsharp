using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace GTR
{
    class MyStr
    {
        public static string GetCut(string strSource)
        {
            char[] anychar = new char[1];
            anychar[0]='\0';
            int len = strSource.IndexOfAny(anychar);
            if (len > 0)
            {
                return strSource.Substring(0,len);
            }
            return strSource;
        }
        public static string FindStr(string strSource, string strKeyBegin, string strKeyEnd)
        {
            int iStart = 0;
            if (strKeyBegin != "")
            {
                iStart = strSource.IndexOf(strKeyBegin);
                if (iStart == -1)  { return ""; }
            }
            strSource = strSource.Substring(iStart);
            int iEnd = strSource.Length;
            if (strKeyEnd != "")
            {
                iEnd = strSource.IndexOf(strKeyEnd);
                if (iEnd == -1) { return ""; }
            }
            iStart = strKeyBegin.Length;
            if (iEnd - iStart < 0) { return ""; }
            
            return strSource.Substring(iStart, iEnd - iStart);
        }
        public static int firstIndexOf(string str, string pattern)
        {
            for (int i = 0; i < (str.Length - pattern.Length); i++)
            {
                int j = 0;
                while (j < pattern.Length)
                {
                    if (str.Substring(i + j, 1) != pattern.Substring(j, 1)) break;
                    j++;
                }
                if (j == pattern.Length) return i;
            }
            return -1;
        }
        public static string FindStr2(string strSource, string strKeyBegin, string strKeyEnd)
        {
            int iStart = 0;
            if (strKeyBegin != "")
            {
                iStart = firstIndexOf(strSource, strKeyBegin);
                if (iStart == -1) { return ""; }
            }
            strSource = strSource.Substring(iStart);
            int iEnd = strSource.Length;
            if (strKeyEnd != "")
            {
                iEnd = firstIndexOf(strSource, strKeyEnd);
                if (iEnd == -1) { return ""; }
            }
            iStart = strKeyBegin.Length;
            if (iEnd - iStart < 0) { return ""; }



            return strSource.Substring(iStart, iEnd - iStart);
        }


        
    }
    class FileRW
    {
        private static string _strPath = Application.ProductName + ".log";

        public static void InitLog()
        {
            File.Delete(_strPath);
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
            //strLog=String.Format(strLog, arg);
            StreamWriter sw=null;

            try
            {
                //Console.WriteLine(strLog);
                //sw = File.AppendText(_strPath);
                sw = new StreamWriter(_strPath, true, Encoding.Default);
                sw.WriteLine("{0}[{1}] {2}", DateTime.Now.ToString("HH:mm:ss"), Application.ProductVersion, strLog);
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
        /// <summary>
        /// 清空指定的文件夹，但不删除文件夹
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);////递归删除子文件夹
                    }
                    Directory.Delete(d);
                }
            }
        }
         public static void DeleteTmpFiles(string strPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(strPath);
                if (dir.Exists)
                {
                    Directory.Delete(strPath, true);
                    //然后再新建一个空的文件夹
                    Directory.CreateDirectory(strPath);
                }
                else
                {
                    string tmp=string.Format("文件路径：{0} 不存在",strPath);
                    WriteToFile(tmp);
                }
            }
            catch (Exception e)
            {
                WriteToFile(e.ToString());
                WriteToFile("删除小图失败");
                return;
            }
        }
    }

}
