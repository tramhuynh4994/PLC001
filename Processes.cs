using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    internal partial class PlcInstaller
    {
        /// <summary>
        /// Find and close all VS2013 instances 
        /// </summary>
        /// <returns></returns>
        static bool CloseVisualStudioInstances()
        {
            Process[] processes = Process.GetProcessesByName("devenv").Where(p => p.MainModule.FileName.Contains("Microsoft Visual Studio 12.0")).ToArray();
            if (processes.Length == 0)
            {
                return true;
            }
            else
            {
                WriteLog(string.Format("Found {0} VS2013 instances. Close them all", processes.Length));
                foreach (Process process in processes)
                {
                    try
                    {
                        WriteLog(process.MainModule.FileName);
                        process.CloseMainWindow(); // Attempt to close the main window
                        if (!process.WaitForExit(5000)) // Wait for up to 5 seconds for the process to exit
                        {
                            process.Kill(); // If it doesn't exit, forcefully terminate it
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog($"Error closing process {process.ProcessName}: {ex.Message}");
                        return false;
                    }
                }
            }
            return true;
        }

        static bool RunRubyFile(string rubyFile)
        {
            try
            {
                if (string.IsNullOrEmpty(_driverType) || _driverType!="1" && _driverType!="2")
                {
                    WriteLog("Driver type invalid (Must be 1 or 2). Return exit code -1");
                    Environment.Exit(-1);
                    return false;
                }
                else
                {
                    // Set up process start info
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = @"C:\ruby\bin\ruby.exe"; // Ruby executable
                    startInfo.Arguments = rubyFile; // Path to the Ruby script file
                    startInfo.RedirectStandardInput = true;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;

                    // Start the process
                    Process process = new Process();
                    process.StartInfo = startInfo;
                    process.Start();

                    // Provide user input to the Ruby script
                    process.StandardInput.WriteLine(_driverType);
                    process.StandardInput.Close();

                    // Read and display the output of the Ruby script
                    string output = process.StandardOutput.ReadToEnd();
                    int exitCode = process.ExitCode; // Get the exit code
                    WriteLog($"Exit Code: {exitCode}");
                    WriteLog(string.Format("Exit code: {0}", exitCode));
                    WriteLog(output); 
                    process.WaitForExit();
                    if(exitCode == 0) 
                    {
                        WriteLog(string.Format("Sucessfully execute ruby file"));
                        return true;
                    }
                    else
                    {
                        WriteLog("Fail to execute ruby file");
                        return false;   
                    }
 
                }
                
            }catch(Exception ex)
            {
                WriteLog(ex.ToString());
                return false;   
            }
        }
        public static void WriteLog(string str)
        {
            WriteLog(_workingDir,_logfile,str);
            Console.WriteLine(str);
        }
        public static void WriteLog(string logfolder, string filename, string strLog)
        {
            try
            {
                string functionName = string.Empty; 
                System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
                System.Diagnostics.StackFrame stackFrame = stackTrace.GetFrame(2); // Adjust the index as needed
                functionName = stackFrame?.GetMethod()?.Name ?? "Unknown";
                if (!Directory.Exists(logfolder))
                    Directory.CreateDirectory(logfolder);
                string str1 = logfolder + "\\" + filename;
                DateTime dateTime = DateTime.Today;
                string str2 = dateTime.ToString("MM-dd-yyyy");
                string str3 = str1 + "Log-" + str2 + ".log";
                FileInfo fileInfo = new FileInfo(str3);
                DirectoryInfo directoryInfo = new DirectoryInfo(fileInfo.DirectoryName);
                if (!directoryInfo.Exists)
                    directoryInfo.Create();
                StreamWriter streamWriter1 = new StreamWriter(fileInfo.Exists ? (Stream)new FileStream(str3, FileMode.Append) : (Stream)fileInfo.Create());
                StreamWriter streamWriter2 = streamWriter1;
                dateTime = DateTime.Now;
                string str4 = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "    :[" +functionName+"]  "+ strLog;
                streamWriter2.WriteLine(str4);
                streamWriter1.Close();
            }
            catch (Exception ex)
            {

            }

        }
        public static bool InstallNet(string batfile)
        {
            string batchFilePath = batfile; // Replace with the actual path
            string workingDirectory = _workingDir; // Replace with the actual working directory

            Process process = new Process();
            process.StartInfo.FileName = batchFilePath;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            int exitCode = process.ExitCode;

            WriteLog(string.Format("Exit code: {0}", exitCode));
            if (exitCode == 0)
            {
                WriteLog("Sucessfully install .NET or .NET already exist");
                return true;
            }
            else
            {
                WriteLog("Fail to install .NET");
                WriteLog(output);
                return false; 
            }
            
            
        }
    }
}
