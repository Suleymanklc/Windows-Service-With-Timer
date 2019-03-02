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
            Log("servis durdu");
        }
    }
}
