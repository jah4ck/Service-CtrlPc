using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCtrlPc
{
    public class ExecProgram
    {
        Trace MyTrace = new Trace();
        private string codeappli = "SERVICES";
        public ExecProgram(string Prog,string argument)
        {
            string path = @"c:\ProgramData\CtrlPc\SCRIPT\";
            try
            {
                CtrlProcess MyCtrlProcess = new CtrlProcess();
                if (File.Exists(path+Prog)&& !MyCtrlProcess.CtrlProcessRunning(Prog))
                {
                    MyTrace.WriteLog("Exécution du programme "+Prog, 2, codeappli);
                    Process Exec = new Process();
                    Exec.StartInfo.FileName = path + Prog;
                    Exec.StartInfo.Arguments = argument;
                    Exec.StartInfo.UseShellExecute = false;
                    Exec.StartInfo.RedirectStandardOutput = true;
                    Exec.Start();
                    MyTrace.WriteLog(Exec.StandardOutput.ReadToEnd(), 2, codeappli);
                    MyTrace.WriteLog("Attente fin du process " + Prog, 2, codeappli);
                    Exec.WaitForExit();
                    MyTrace.WriteLog("Exécution du programme Terminé" + Prog, 2, codeappli);

                }
            }
            catch (Exception err)
            {
                MyTrace.WriteLog(err.Message, 1, codeappli);
            }
            
        }
    }
}
