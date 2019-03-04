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
using log4net.Config;
namespace deneme1
{
    public class logCompressDelete
    {

        public static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void fileCheck(string LogLocation, string saveLogDays)
        {

            string[] files = Directory.GetFiles(LogLocation, "*", SearchOption.AllDirectories);

           log.Info("Log silme islemi basladi..");
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


    public partial class log4netWinService : ServiceBase
    {
        static double periodsec = 120000;
        //double periodsec2 = periodsec;
        //periodsec2 =  Convert.ToDouble(period); wef

        private System.Timers.Timer timerTenSecond = new System.Timers.Timer(periodsec);
        
        static string period = ConfigurationManager.AppSettings["period"];
         
        ILog log = LogManager.GetLogger(typeof(log4netWinService));

        public log4netWinService()
        {
            InitializeComponent();
        }
      


        protected override void OnStart(string[] args)
        {

            log.Info("Started - Log4netWinService");
            timerTenSecond.Elapsed += new ElapsedEventHandler(TimerTenSecond_Elapsed);
            timerTenSecond.Enabled = true;
            

        }

        protected override void OnStop()
        {
            timerTenSecond.Enabled = false;
            log.Info("servis durdu");
        }

        private void TimerTenSecond_Elapsed(object sender, ElapsedEventArgs e)
        {
            log.Error("islemlere yeniden baslaniyor..");
            Timer_Tick();
        }
  

        private void Timer_Tick()
        {
            string zipname = ConfigurationManager.AppSettings["zipname"];
            string saveLogDays = ConfigurationManager.AppSettings["saveLogDays"];
            string saveRarDays = ConfigurationManager.AppSettings["saveRarDays"];

            try
            {
                //Console.Write("basladi");
                log.Info("disarda");
                var serverManager = new ServerManager();
                foreach (var site in serverManager.Sites)
                {
                    try
                    {
                        string sitePath = (site.LogFile.Directory + "\\W3SVC" + site.Id);
                        var actualPath = Environment.ExpandEnvironmentVariables(@sitePath);
                        sitePath = actualPath.ToString();
                        log.Info(site.ToString());
                        deneme1.logCompressDelete.logZip(sitePath, zipname);
                        deneme1.logCompressDelete.fileCheck(sitePath, saveLogDays);
                        deneme1.logCompressDelete.rarDel(Convert.ToDouble(saveRarDays), zipname);

                    }
                    catch (Exception ex)
                    {
                        log.Info(ex.ToString());
                        
                        continue;
                    }

                }
            }
            catch (Exception ex )
            {
                log.Info(ex.ToString());
                
            }
        }

 
     
    }
}

