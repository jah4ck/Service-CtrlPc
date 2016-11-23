using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCtrlPc
{
    public class Shutdown
    {
        Trace MyTrace = new Trace();
        private string codeappli = "SERVICES";
        public Shutdown()
        {
            string pathCmd = @"c:\ProgramData\CtrlPc\SCRIPT\";
            if (File.Exists(pathCmd + "shutdownPc.cmd"))
            {
                if (!File.Exists(pathCmd + "stop.flg"))
                {
                    MyTrace.WriteLog("Exécution du script d'arrêt "+ pathCmd + "shutdownPc.cmd", 2, codeappli);
                    Process process = Process.Start(pathCmd + "shutdownPc.cmd");
                }
                else
                {
                    MyTrace.WriteLog("Arrêt annulé flag stop présent", 2, codeappli);
                }

            }
        }
    }
}
