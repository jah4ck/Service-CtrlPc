using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCtrlPc
{
    public class Routine1
    {
        Trace MyTrace = new Trace();
        private string codeappli = "SERVICES";
        public void LectureFlag(string path)
        {

            if (File.Exists(path + "arr.flg"))
            {
                string LectureFlg = File.ReadAllText(path + @"arr.flg");
                if (LectureFlg.Contains("True"))
                {
                    MyTrace.WriteLog("Demande d'arrêt de l'ordinateur", 2, codeappli);
                    Shutdown MyShutDown = new Shutdown();
                }
                else
                {
                    MyTrace.WriteLog("Pas de demande d'arrêt", 2, codeappli);
                }

            }
            else
            {
                MyTrace.WriteLog("Le fichier arr.flg n'est pas présent", 2, codeappli);
            }

        }
    }
}
