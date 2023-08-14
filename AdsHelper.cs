using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace ConsoleApp4
{
    internal partial class PlcInstaller
    {
        public static string GetAdsState()
        {
            string _state = string.Empty;
            try
            {
                StateInfo mode = new StateInfo();
                mode = _client.ReadState();
                _state = mode.AdsState.ToString();
                WriteLog(string.Format("Current ads state: {0}",_state ));
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
            }
            return _state;
        }
        public static bool setConfigMode()
        {
            try
            {
                string state =GetAdsState();
                if (state == "Config")
                {
                    WriteLog(string.Format("Current state is CONFIG. Return TRUE"));
                    return true; 
                }
                StateInfo mode = new StateInfo();
                WriteLog(string.Format("Change to config mode. Sleep for 20secs then re-check"));
                mode.AdsState = AdsState.Reconfig;
                _client.WriteControl(mode);
                Thread.Sleep(10000);
                string stateAfter = GetAdsState();
                if ((stateAfter == "Config"))
                {
                    WriteLog(string.Format("ADS mode is == Config. Return TRUE"));
                    return true;
                }
                else
                {
                    WriteLog(string.Format("ADS mode is != Config. Return FALSE"));
                    return false;
                }

            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
                return false;
            }

        }

    }
}
