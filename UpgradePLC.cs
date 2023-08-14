using EnvDTE80;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;
using TwinCAT.Ads;

namespace ConsoleApp4
{
    internal partial class PlcInstaller
    {
        static bool Upgrade()
        {
            MessageFilter.Register();

            /*
             * Close all VS2013 instances 
             */
            WriteLog(string.Format("Try to close all VS2013 instances"));
            CloseVisualStudioInstances();
            /*
             * Change TwinCat to config mode 
             */
            WriteLog(string.Format("Change twincat to config mode"));
            bool _setConfigMode = setConfigMode();
            if( !_setConfigMode )
            {
                WriteLog("Fail to set config");
                Environment.Exit(2);
                return false; 
            }
            /*
             * Run ruby file 
             */
            WriteLog(string.Format("Run ruby file to config driver {0}", _rubyFile));
            bool ruby = RunRubyFile(_rubyFile);
            if( !ruby )
            {
                WriteLog("Fail to execute Ruby file at " + _rubyFile); 
                return false;
            }
            else
            {
                WriteLog("Sucessfully executing ruby file"); 
            }

            WriteLog(string.Format("Upgrade PLC"));
            try
            {
                //Check if the inputs valid 
                if (!File.Exists(_vsFilePath))
                {
                    WriteLog("Visual studio sln doesnt exist at " + _vsFilePath);
                    Environment.Exit(1);
                    return false;
                }
               

                //Open solution 
                WriteLog("Opening sln at " + _vsFilePath);
                Type t = System.Type.GetTypeFromProgID("VisualStudio.DTE.12.0");
                dte = (DTE2)Activator.CreateInstance(t);
                dte.SuppressUI = true;
                dte.MainWindow.Visible = false;
                if (dte.ActiveSolutionProjects != null)
                {
                   // Console.WriteLine("Another project is running. Proceed to close");
                }
                dynamic solution = dte.Solution;
                solution.Open(_vsFilePath);
                dynamic project = solution.Projects.Item(1);
                WriteLog("Done opening sln at " + _vsFilePath);
                sysManager = project.Object;
                //Find Device2 and config 
                ConfigDevice2(); 
                //Build and activate solution 
                WriteLog("Build solution and Activate configuration");
                //sysManager.ActivateConfiguration();
                //Restart Twincat 
                WriteLog("StartRestartTwinCAT");
                //sysManager.StartRestartTwinCAT();

                //Double check Twincat config 
                string currentState=GetAdsState();
                WriteLog(string.Format("Current state = {0}", currentState));
                if (currentState != "RUN")
                {
                    WriteLog("Current state != Run"); 
                    return false;
                }
                string currentProj = dte.ActiveSolutionProjects.ToString();
                WriteLog(string.Format("Current proj: {0}", currentProj));
                //Dispo solution and dte object 
                dte.Quit();
                WriteLog("Done");
                MessageFilter.Revoke();
                return true; 
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
                Environment.Exit(200);
                return false;   

            }
            MessageFilter.Revoke();
        }

        /// <summary>
        /// Browse the try item and get the device #2 
        /// Scan available devices 
        /// Setup the device 
        /// </summary>
        static void ConfigDevice2()
        {
            try
            {
                ITcSmTreeItem ioDevices = sysManager.LookupTreeItem("TIID");
                string foundDevices = ioDevices.ProduceXml();
                WriteLog(foundDevices);
                File.WriteAllText(@"C:\ATM\Config.xml", foundDevices);
            }
            catch (Exception e)
            {

                WriteLog(e.ToString());
            }
        }
    }
}
