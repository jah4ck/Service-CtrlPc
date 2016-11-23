using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServiceCtrlPc
{
    public class ControleHoraireLocal
    {
        Trace MyTrace = new Trace();
        private string codeappli = "SERVICES";
        public ControleHoraireLocal()
        {
            string path=@"C:\ProgramData\CtrlPc\PLANNING\planning.csv";
            if (File.Exists(path))
            {
                string[] ligne = File.ReadAllLines(path);
                int count = 0;
                SynchroHeure MySynchroHeure = new SynchroHeure();
                DateTime heureactuelle = DateTime.Now;
                try
                {
                    heureactuelle = MySynchroHeure.GetNetworkTime();
                }
                catch (Exception err)
                {
                    MyTrace.WriteLog("Récupération heure serveur KO --> " + err.Message, 1, codeappli);
                    heureactuelle = DateTime.Now;
                }
                foreach (string lignedetail in ligne)
                {
                    if (lignedetail.Contains(";"))
                    {
                        string[] colonne = lignedetail.Split(new Char[] { ';' });
                        DateTime dateDebut = Convert.ToDateTime(colonne[0]);
                        DateTime dateFin = Convert.ToDateTime(colonne[1]);

                        if (heureactuelle >= dateDebut && heureactuelle <= dateFin)
                        {
                            count++;
                        }
                    }
                }
                if (count == 0)
                {
                    MyTrace.WriteLog("La date n'est pas comprise parmis les plages locales", 2, codeappli);
                    MyTrace.WriteLog("Appel de l'arrêt de l'ordinateur", 2, codeappli);
                    Shutdown MyShutdown = new Shutdown();
                }
            }
            else
            {
                MyTrace.WriteLog("Le planning n'est pas présent en locale", 2, codeappli);
            }
        }
    }
}
