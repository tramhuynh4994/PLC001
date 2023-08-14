using NDesk.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    internal partial class PlcInstaller
    {
        static void ShowHelp(OptionSet p)
        {
            WriteLog("Usage: PLC Installer [OPTIONS]");
            p.WriteOptionDescriptions(Console.Out);
            Environment.Exit(0);
        }

        private static void SetupOptions(string[] args)
        {
            OptionSet options = new OptionSet()
            .Add("h|help", delegate (string v) { _help = v != null; })
            .Add("f|vsPath=", delegate (string v) { _vsFilePath = v; })
            .Add("d|driverType (1 to Trinamic, 2 to enable Lichuan)=", delegate (string v) { _driverType = v; })
            .Add("r|Ruby file to execute", delegate (string v) { _rubyFile = v; });

            try
            {
                options.Parse(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }

            if (_help)
            {
                ShowHelp(options);
                return;
            }
        }
    }
}
