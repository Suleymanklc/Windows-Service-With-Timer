using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;
using System.Threading;
using Microsoft.Web.Administration;
using System.IO;
using System.Configuration;
using Ionic.Zip;
using System.Xml.Linq;
using log4net;
namespace deneme1
{
    public class logCompressDelete
    {

        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void fileCheck(string LogLocation, string saveLogDays)
        {

            string[] files = Directory.GetFiles(LogLocation, "*", SearchOption.AllDirectories);

            Console.WriteLine("tamam icerde");
            foreach (string file in files)
            {
                try
                {
                    Console.WriteLine(file);

                    FileInfo fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddDays(Convert.ToDouble(saveLogDays)))
                    {
                        fi.Delete();

                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    continue;
                }
            }

        }
        public static void logZip(string LogLocation, string zipname)
        {
            try
            {
                using (var zip = new ZipFile())
                {

                    log.Info("logZip started..");
                    string name = System.Environment.MachineName;
                    DateTime currentDateTime = DateTime.Now;
                    String dateStr = currentDateTime.ToString("yyyy-MM-dd HH-mm");
                    string zipPath = zipname + "_" + name + "_" + dateStr + ".gz";
                    zip.AddDirectory(LogLocation);
                    zip.Save(zipPath);
                    log.Info("logZip finished..");
                }
            }

            catch (Exception ex)
            {
                log.Error(ex);

                //     ServiceStart();
                // throw new Exception($"Cannot zip");
            }

        }


        public static void logDirsDel()
        {
            //var config = XDocument.Load(File.OpenRead("ConsoleApplication3.exe.config"));
            //var sections = config.Descendants("appSettings");
            string str = "";
            string saveLogDays = ConfigurationManager.AppSettings["saveLogDays"];
            foreach (var key in ConfigurationSettings.AppSettings)
            {
                str = Convert.ToString(key);



                if (str.Contains("LogLoc"))
                {

                    string LogLocation = ConfigurationManager.AppSettings[str];
                    log.Info(LogLocation);
                    logDel(Convert.ToDouble(saveLogDays), LogLocation);

                }
            }

        }
        public static void logDel(double day, string sourcePath)
        {
            log.Info("log directory deleting started..");
            try
            {


                DirectoryInfo baseDir = new DirectoryInfo(sourcePath);
                DirectoryInfo[] subDirectories = baseDir.GetDirectories();
                if (subDirectories != null && subDirectories.Length > 0)
                {
                    foreach (DirectoryInfo subDirectory in subDirectories)
                    {

                        //  string[] files = Directory.GetFiles(string.Concat(sourcePath, @"\", subDirectory.Name));
                        DirectoryInfo[] subDirectories2 = subDirectory.GetDirectories();

                        foreach (DirectoryInfo subDirectory2 in subDirectories2)
                        {
                            try
                            {
                                string subdir = string.Concat(sourcePath, @"\", subDirectory.Name, @"\", subDirectory2.Name);
                                File.SetAttributes(subdir, FileAttributes.Normal);
                                Directory.Delete(subdir, false);
                                Directory.Delete(subdir, recursive: true);
                            }

                            catch (Exception ex)
                            {
                                log.Error(ex);
                                continue;

                            }

                        }
                    }
                }
                log.Info("log directory deleting finished..");

            }

            catch (Exception ex)
            {
                log.Error(ex);
                // ServiceStart();

                // throw new Exception($"Cannot zip");

            }

        }

        public static void rarDel(double saveRarDays, string sourcePath)
        {

            try
            {
                log.Info("rarDel started..");



                string[] files = Directory.GetFiles(sourcePath);


                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastWriteTime < DateTime.Now.AddDays(saveRarDays))
                    {
                        fi.Delete();

                    }
                }

                log.Info("rarDel finished..");
            }

            catch (Exception ex)
            {
                log.Error(ex);
                //ServiceStart();


            }




        }
    }
}


namespace deneme
{


    public partial class Service1 : ServiceBase
    {
        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Service1()
        {
            InitializeComponent();
        }
        private System.Threading.Timer myTimer;


        protected override void OnStart(string[] args)
        {
            log.Info("servis basladi");
            startTimer();
        }

        private void startTimer()
        {
            try
            {
                if (myTimer == null)
                {
               
                    int timerIntervalSecs = 60*60*8;
                    TimeSpan tsInterval = new TimeSpan(0, 0, timerIntervalSecs);
                    myTimer = new System.Threading.Timer(
                        new System.Threading.TimerCallback(Timer_Tick)
                        , null, tsInterval, tsInterval);
                }
            }
            catch (Exception ex)
            {
                stopTimer();
                log.Info("servis basladi");
            }
        }
        private void stopTimer()
        {
            try
            {
                if (myTimer != null)
                {
              
                    myTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    myTimer.Dispose();
                    myTimer = null;
                }
            }
            catch (Exception )
            {
                log.Info("servis basladi");
            }
        }

        private void Timer_Tick(object state)
        {
           
            try
            {


                var serverManager = new ServerManager();
                foreach (var site in serverManager.Sites)
                {
                    string sitePath = (site.LogFile.Directory + "WSVC" + site.Id);
                    
                }
            }
            catch (Exception )
            {
                log.Info("servis basladi");
            }
        }

 
        protected override void OnStop()
        {
            stopTimer();
            log.Info("servis durdu");
        }
    }
}

