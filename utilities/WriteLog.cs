using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace utilities
{
    /// <summary>
    /// 写日志
    /// </summary>
    public class WriteLog
    {
        private static Object LockSobj = new object();
        private static Object LockEobj = new object();
        public static void WriteDataLog(string strMsg)
        {
            string FileName = "\\ServiceLog\\ServiceLog_" + DateTime.Now.ToString("yyyyMMdd") + "\\" +DateTime.Now.ToString("HHmmss") + ".log";
            string stmp = Assembly.GetExecutingAssembly().Location;
            stmp = stmp.Substring(0, stmp.LastIndexOf('\\')) + FileName;
            string codk = stmp.Substring(0, stmp.LastIndexOf('\\'));
            try
            {
                if (!System.IO.Directory.Exists(codk))
                {
                    System.IO.Directory.CreateDirectory(codk);
                }
                if (!File.Exists(stmp))
                {
                    FileStream fs1 = new FileStream(stmp, FileMode.Create, FileAccess.Write);//创建写入文件    
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.WriteLine(">>>>〖" + DateTime.Now.ToString() + "〗："+ "\r\n" + strMsg);
                    sw.Close();
                    fs1.Close();

                }
                else
                {
                    FileStream fs2 = new FileStream(stmp, FileMode.Append, FileAccess.Write);//创建写入文件    
                    StreamWriter sww = new StreamWriter(fs2);
                    sww.WriteLine(">>>>〖" + DateTime.Now.ToString() + "〗：" + "\r\n" + strMsg);
                    sww.Close();
                    fs2.Close();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
        /// <summary>
        /// 写日系统日志
        /// </summary>
        /// <param name="SystemInfoStr">系统信息</param>
        public static void WriteSystemLog(string SystemInfoStr)
        {
            try
            {
                lock (LockSobj)
                {
                    //获取以当前系统日期为名的文件
                    string FileName = "\\SystemLog\\SystemLog_" + DateTime.Now.ToString("yyyyMMdd") + ".log";//为避免加密程序加密，此处采用.log而非.txt
                    string stmp = Assembly.GetExecutingAssembly().Location;
                    stmp = stmp.Substring(0, stmp.LastIndexOf('\\')) + FileName;
                    string codk = stmp.Substring(0, stmp.LastIndexOf('\\'));
                    //判断SystemLog文件夹是否创建，如果未创建，则创建该文件夹
                    if (!System.IO.Directory.Exists(codk))
                    {
                        System.IO.Directory.CreateDirectory(codk);
                    }
                    if (!File.Exists(stmp))
                    {
                        FileStream fs1 = new FileStream(stmp, FileMode.Create, FileAccess.Write);//创建写入文件    
                        StreamWriter sw = new StreamWriter(fs1);
                        sw.WriteLine(">>>>〖" + DateTime.Now.ToString() + "〗：" + SystemInfoStr);
                        sw.Close();
                        fs1.Close();

                    }
                    else
                    {
                        FileStream fs2 = new FileStream(stmp, FileMode.Append, FileAccess.Write);//创建写入文件    
                        StreamWriter sww = new StreamWriter(fs2);
                        sww.WriteLine(">>>>〖" + DateTime.Now.ToString() + "〗：" + SystemInfoStr);
                        sww.Close();
                        fs2.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }



        /// <summary>
        /// 写错误日志
        /// </summary>
        /// <param name="ErrorInfoStr">错误信息</param>
        public static void WriteErrorLog(string ErrorInfoStr)
        {
            try
            {
                lock (LockEobj)
                {
                    //获取以当前系统日期为名的文件
                    string FileName = "\\SystemLog\\ErrorLog_" + DateTime.Now.ToString("yyyyMMdd") + ".log";//为避免加密程序加密，此处采用.log而非.txt
                    string stmp = Assembly.GetExecutingAssembly().Location;
                    stmp = stmp.Substring(0, stmp.LastIndexOf('\\')) + FileName;
                    string codk = stmp.Substring(0, stmp.LastIndexOf('\\'));
                    //判断SystemLog文件夹是否创建，如果未创建，则创建该文件夹
                    if (!System.IO.Directory.Exists(codk))
                    {
                        System.IO.Directory.CreateDirectory(codk);
                    }
                    if (!File.Exists(stmp))
                    {
                        FileStream fs1 = new FileStream(stmp, FileMode.Create, FileAccess.Write);//创建写入文件    
                        StreamWriter sw = new StreamWriter(fs1);
                        sw.WriteLine(">>>>[" + DateTime.Now.ToString() + "]：" + ErrorInfoStr);
                        sw.Close();
                        fs1.Close();
                    }
                    else
                    {
                        FileStream fs2 = new FileStream(stmp, FileMode.Append, FileAccess.Write);//创建写入文件    
                        StreamWriter sww = new StreamWriter(fs2);
                        sww.WriteLine(">>>>〖" + DateTime.Now.ToString() + "〗：" + ErrorInfoStr);
                        sww.Close();
                        fs2.Close();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }



        /// <summary>
        /// 修改AppSettings中配置
        /// </summary>
        /// <param name="key">key值</param>
        /// <param name="value">相应值</param>
        public static bool SetConfigValue(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                    config.AppSettings.Settings[key].Value = value;
                else
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
