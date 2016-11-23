using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ServiceCtrlPc
{
    public class LectureFileExecProgram
    {

        Trace MyTrace = new Trace();
        private string codeappli = "SERVICES";
        public LectureFileExecProgram()
        {
            //recup sur ftp toutes les 15 min du flag arr.flg
            
            MyTrace.WriteLog("Contrôle Exécution programme", 2, codeappli);
            try
            {
                string pathExecProg = @"c:\ProgramData\CtrlPc\SCRIPT\ExecProgram.csv";
                if (File.Exists(pathExecProg))
                {
                    string[] prog = File.ReadAllLines(pathExecProg);
                    if (prog.Length > 0)
                    {
                        foreach (string progLigne in prog)
                        {
                            if (progLigne.Length > 0)
                            {
                                string[] Colonne = progLigne.Split(';');
                                MyTrace.WriteLog("Demande Exécution : " + Colonne[0] + " Avec arguments : " + Colonne[1], 2, codeappli);
                                ExecProgram MyExecProgram = new ExecProgram(Colonne[0], Colonne[1]);
                            }
                        }
                    }
                }

            }
            catch (Exception err)
            {
                MyTrace.WriteLog(err.Message, 2, codeappli);
            }
        }
    }
}
