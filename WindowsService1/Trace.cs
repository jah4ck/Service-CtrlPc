using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCtrlPc
{
    public class Trace
    {
        public void WriteLog(string arg0, int arg1, string arg2)
        {
            try
            {
                SynchroHeure MySynchroHeure = new SynchroHeure();
                ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                Object Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
                DateTime dateTraitement = DateTime.Now;
                try
                {
                    dateTraitement = MySynchroHeure.GetNetworkTime();
                }
                catch (Exception err)
                {
                    dateTraitement = DateTime.Now;
                }
                ws.TraceLog(Guid.ToString(), dateTraitement, arg2, arg1, arg0);
            }
            catch (Exception err)
            {
                //ecrire dans le journal si erreur de communication
                ProcessStartInfo startinfo = new ProcessStartInfo();
                startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + arg2 + "";
                Process Trace = Process.Start(startinfo);
                Trace.WaitForExit();

                //ecrire l'erreur de communication dans le journal
                ProcessStartInfo startinfoCatch = new ProcessStartInfo();
                startinfoCatch.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                startinfoCatch.Arguments = "\"" + err.Message + "\" " + 1 + " " + arg2 + "";
                Process TraceCatch = Process.Start(startinfoCatch);
                TraceCatch.WaitForExit();

                
            }
            
        }
    }
}
