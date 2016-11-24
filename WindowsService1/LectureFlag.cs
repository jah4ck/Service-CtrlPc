using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCtrlPc
{
    public class LectureFlag
    {
        Trace MyTrace = new Trace();
        private string codeappli = "SERVICES";
        public void LectureFlagArr(string path)
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

        public int LectureFlagNfo(string path)
        {

            if (File.Exists(path + "nfo.flg"))
            {
                string LectureFlg = File.ReadAllText(path + @"nfo.flg");
                if (LectureFlg.Contains("True")||LectureFlg.Contains("1"))
                {
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            else
            {
                MyTrace.WriteLog("Le fichier arr.flg n'est pas présent", 2, codeappli);
                return 0;
            }

        }
    }
}
