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
        private int statusParam = 3;//fichier
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
                                Int32.TryParse(colonne[1], out statusParam);
                                Int32.TryParse(colonne[2], out type);
                            }
                        }
                    }
                }
            }
            SynchroHeure MySynchroHeure = new SynchroHeure();
            DateTime dateTraitement = DateTime.Now;
            try
            {
                dateTraitement = MySynchroHeure.GetNetworkTime();
            }
            catch (Exception err)
            {
                dateTraitement = DateTime.Now;
            }
            if (status==3 && statusParam==3)//mode journal
            {
                try
                {
                    ProcessStartInfo startinfo = new ProcessStartInfo();
                    if (arg1 == 1 && (type == 3 || type == 1))
                    {
                        startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                        startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\"" + "JOURNAL" + "\"";
                        Process Trace = Process.Start(startinfo);
                        Trace.WaitForExit();
                    }
                    if (arg1 == 2 && (type == 3 || type == 2))
                    {
                        startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                        startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\"" + "JOURNAL" + "\"";
                        Process Trace = Process.Start(startinfo);
                        Trace.WaitForExit();
                    }
                   
                }
                catch (Exception err)
                {
                    string NameDate = dateTraitement.ToString("yyyyMMdd");
                    string Date = dateTraitement.ToString("dd/MM/yyyy HH:mm:ss");
                    using (StreamWriter writer = File.AppendText(@"C:\ProgramData\CtrlPc\LOG\JOURNAL_ERREUR__" + NameDate + ".log"))
                    {
                        writer.WriteLine(Date + "     " + arg2 + "     " + "ERREUR : " + err.Message);
                        
                    }
                }
                
            }
            else if (status==2 && statusParam==2) //mode WS
            {
                try
                {
                    Object Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
                    ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                    string result = "OK";
                    if (arg1 == 1 && (type == 3 || type == 1))
                    {
                        result = ws.TraceLogNew(Guid.ToString(), dateTraitement, arg2, arg1, arg0);
                    }
                    if (arg1 == 2 && (type == 3 || type == 2))
                    {
                        result = ws.TraceLogNew(Guid.ToString(), dateTraitement, arg2, arg1, arg0);
                    }
                    if (result=="RELICA")
                    {
                        ProcessStartInfo startinfo = new ProcessStartInfo();
                        if (arg1 == 1 && (type == 3 || type == 1))
                        {
                            startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                            startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\"" + "RELICA" + "\"";
                            Process Trace = Process.Start(startinfo);
                            Trace.WaitForExit();
                        }
                        if (arg1 == 2 && (type == 3 || type == 2))
                        {
                            startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                            startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\"" + "RELICA" + "\"";
                            Process Trace = Process.Start(startinfo);
                            Trace.WaitForExit();
                        }

                    }
                }
                catch (Exception err)
                {
                    ProcessStartInfo startinfo = new ProcessStartInfo();
                    if (arg1 == 1 && (type == 3 || type == 1))
                    {
                        startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                        startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\"" + "JOURNAL" + "\"";
                        Process Trace = Process.Start(startinfo);
                        Trace.WaitForExit();
                        startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\""+err.Message+"\"";
                        Trace = Process.Start(startinfo);
                        Trace.WaitForExit();
                    }
                    if (arg1 == 2 && (type == 3 || type == 2))
                    {
                        startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                        startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\""+"JOURNAL"+"\"";
                        Process Trace = Process.Start(startinfo);
                        Trace.WaitForExit();
                    }
                }
            }

            if (status==3 && statusParam==2) //mode relica
            {
                try
                {
                    ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                    Object Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);

                    ProcessStartInfo startinfo = new ProcessStartInfo();
                    if (arg1 == 1 && (type == 3 || type == 1))
                    {
                        startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                        startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\"" + "RELICA" + "\"";
                        Process Trace = Process.Start(startinfo);
                        Trace.WaitForExit();
                        ws.SetIncrementeRelica(Guid.ToString());
                    }
                    if (arg1 == 2 && (type == 3 || type == 2))
                    {
                        startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                        startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\"" + "RELICA" + "\"";
                        Process Trace = Process.Start(startinfo);
                        Trace.WaitForExit();
                        ws.SetIncrementeRelica(Guid.ToString());
                    }

                    
                }
                catch (Exception err)
                {
                    ProcessStartInfo startinfo = new ProcessStartInfo();
                    if (arg1 == 1 && (type == 3 || type == 1))
                    {
                        startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                        startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\"" + "RELICA" + "\"";
                        Process Trace = Process.Start(startinfo);
                        Trace.WaitForExit();
                        startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\"" + err.Message + "\"";
                        Trace = Process.Start(startinfo);
                        Trace.WaitForExit();
                    }
                    if (arg1 == 2 && (type == 3 || type == 2))
                    {
                        startinfo.FileName = @"c:\ProgramData\CtrlPc\SCRIPT\TraceLog.exe";
                        startinfo.Arguments = "\"" + arg0 + "\" " + arg1 + " " + "\"" + "RELICA" + "\"";
                        Process Trace = Process.Start(startinfo);
                        Trace.WaitForExit();
                    }
                }
            }
                       
            
        }
    }
}
