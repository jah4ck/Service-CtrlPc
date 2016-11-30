using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCtrlPc
{
    public class Trace
    {
        private int status = 3;//fichier
        private int type = 3;//tout
        public void WriteLog(string arg0, int arg1, string arg2)
        {
            if (File.Exists(@"c:\ProgramData\CtrlPc\SCRIPT\RemLog.nfo"))
            {
                using (FileStream filestream = new FileStream(@"c:\ProgramData\CtrlPc\SCRIPT\RemLog.nfo", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader read = new StreamReader(filestream))
                    {
                        string ligne;
                        while ((ligne = read.ReadLine()) != null)
                        {
                            if (ligne.Length > 2)
                            {
                                string[] colonne = ligne.Split(';');
                                Int32.TryParse(colonne[0], out status);
                                Int32.TryParse(colonne[1], out type);
                            }
                        }
                    }
                }
            }

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
                string result= ws.TraceLogNew(Guid.ToString(), dateTraitement, arg2, arg1, arg0);
                if (result=="RELICA")
                {
                    string NameDate = dateTraitement.ToString("yyyyMMdd");
                    string Date = dateTraitement.ToString("dd/MM/yyyy HH:mm:ss");
                    using (StreamWriter writer = new StreamWriter(@"C:\ProgramData\CtrlPc\LOG\RELICA_"+NameDate+".log"))
                    {
                        if (arg1==1)
                        {
                            writer.WriteLine(Date + "     " + arg2 + "     " + "ERREUR : " + arg0.ToString());
                        }
                        if (arg1 == 2)
                        {
                            writer.WriteLine(Date + "     " + arg2 + "     " + "INFO : " + arg0.ToString());
                        }
                    }

                }
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
