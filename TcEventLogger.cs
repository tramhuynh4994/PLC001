using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCEVENTLOGGERLib;

namespace ConsoleApp4
{
    internal partial class PlcInstaller
    {

        private static void TcEventLogger_OnShutdown(object shutdownParm)
        {
            WriteLog("TcEventLogger: Shutdown");
        }

        private static void TcEventLogger_OnNewEvent(object evtObj)
        {
            WriteLog("TcEventLogger: New event");
            TcEvent tcevent = (TcEvent)evtObj;
            WriteLog(string.Format("{0}: {1}", tcevent.Class, tcevent.GetMsgString(tcevent.Id)));
        }

        private static void TcEventLogger_OnResetEvent(object evtObj)
        {
            WriteLog("TcEventLogger: Reset");
        }
    }
}
