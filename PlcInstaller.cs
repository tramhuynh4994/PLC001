using EnvDTE80;
using NDesk.Options;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using TCatSysManagerLib;
using TCEVENTLOGGERLib;
using TwinCAT.Ads;

namespace ConsoleApp4
{
    internal partial class PlcInstaller
    {
        //PLC Path: C:\AEM\System\PLC\Release\HDMx_PNB_5.7.IR6
        static string _workingDir = @"C:\SkyzoneFiles\PLCInstaller\"; 
        static string _vsFilePath = @"C:\PLCInstaller\HelloWorld\HelloWorld.sln";//@"C:\AEM\System\PLC\Release\HDMx_PNB_5.7.IR6\HDMx_PNB_5.7.IR6\HDMx_PNB.sln"; 
        static string _rubyFile = @"C:\AEM\System\PLC\Release\AutoConfig_Trinamic_LiChuan.rb";
        //static string _asmId = "10.250.0.100.1.1";
        static string _logfile = "plc-installer";
        static bool _help = false;        
        static DTE2 dte; //interface to interact with visual studio 
        static TcAdsClient _client;
        static TcEventLog tcEventLogger = new TcEventLog();
        static string _driverType = "1";//string.Empty;
        static ITcSysManager sysManager; 
        [STAThread]
        static void Main(string[] args)
        {
            /*
             * Setup events for TwincatEventLogger
             */
            WriteLog(string.Format("Create and subscribe to twincat event logs"));
            tcEventLogger.OnResetEvent += TcEventLogger_OnResetEvent;
            tcEventLogger.OnNewEvent += TcEventLogger_OnNewEvent;
            tcEventLogger.OnShutdown += TcEventLogger_OnShutdown;
            /*
             * Setup cmd options 
             */
            WriteLog(string.Format("Setup help options"));
            SetupOptions(args);

            /*
            * Setup .NET 
            */
            string batfile = _workingDir + "DotNetInstall_4.7.2.bat";
            bool installnet = InstallNet(batfile);
            WriteLog(".NET installed " + installnet);
            if (!installnet)
            {
                Environment.Exit(1);
            }
            /*
             * Setup AdsClient 
             */
            WriteLog(string.Format("Setup ads client at port: {0}", (int)AmsPort.SystemService));
            _client = new TcAdsClient();
            _client.Connect((int)AmsPort.SystemService);

            /*
             * Upgrade PLC 
             */
            bool upgraded = Upgrade();

            /*
            * Clean up resources 
            */
            WriteLog(string.Format("Clean up resources"));
            _client.Dispose();
            WriteLog("Done");
            //Console.ReadLine();

            if (upgraded)
            {
                WriteLog("Upgrade done. Exit");
                Environment.Exit(0);
            }
            else
            {
                WriteLog("Upgrade fail. Exit");
                Environment.Exit(-1);
            }
           
        }

 

      
    


    }
}
