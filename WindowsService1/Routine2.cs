using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCtrlPc
{
    public class Routine2
    {
        Trace MyTrace = new Trace();
        private string codeappli = "SERVICES";
        public void ControlePlage(string[] plage)
        {
            MyTrace.WriteLog("Contrôle des plages horaires", 2, codeappli);
            DateTime datetime = DateTime.Now;
            try
            {
                MyTrace.WriteLog("Récupération de l'heure", 2, codeappli);
                SynchroHeure MySynchroHeure = new SynchroHeure();
                datetime = MySynchroHeure.GetNetworkTime();
            }
            catch (Exception err)
            {
                datetime = DateTime.Now;
                MyTrace.WriteLog(err.Message, 1, codeappli);
            }

            string jourSem = datetime.ToString("ddd");
            MyTrace.WriteLog("Jour semaine : "+ jourSem, 2, codeappli);
            int heureActuel = Int32.Parse(datetime.ToString("HHmm"));
            MyTrace.WriteLog("Heure : " + heureActuel, 2, codeappli);
            if ((jourSem.Contains("dim") && heureActuel > 1500) || jourSem.Contains("lun") || jourSem.Contains("mar") || jourSem.Contains("mer") || jourSem.Contains("jeu") || (jourSem.Contains("ven") && heureActuel < 1500))
            {
                //lecture ligne S
                foreach (string ligne in plage)
                {
                    
                    if (ligne.Contains("S"))
                    {
                        MyTrace.WriteLog("plage : " + ligne, 2, codeappli);
                        int heuredeb = Int32.Parse(ligne.Substring(2, 4));
                        int heurefin = Int32.Parse(ligne.Substring(7, 4));


                        if (heuredeb < heureActuel || heureActuel < heurefin)
                        {
                            MyTrace.WriteLog("Arrêt demandé : " + ligne, 2, codeappli);
                            Shutdown MyShutDown = new Shutdown();
                        }

                    }
                }
            }
            else
            {
                //lecture ligne W
                foreach (string ligne in plage)
                {
                    
                    if (ligne.Contains("W"))
                    {
                        MyTrace.WriteLog("plage : " + ligne, 2, codeappli);
                        int heuredeb = Int32.Parse(ligne.Substring(2, 4));
                        int heurefin = Int32.Parse(ligne.Substring(7, 4));


                        if (heuredeb < heureActuel || heureActuel < heurefin)
                        {
                            MyTrace.WriteLog("Arrêt demandé : " + ligne, 2, codeappli);
                            Shutdown MyShutDown = new Shutdown();
                        }
                    }
                }
            }
        }
    }
}
