﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Net;
using Microsoft.Win32;

namespace ServiceCtrlPc
{
    public partial class Service1 : ServiceBase
    {

        //appel du statut du service
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);


        //structure du service
        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };
        

        public Service1()
        {
            InitializeComponent();  
            
        }
        Trace MyTrace = new Trace();
        private string codeappli = "SERVICES";
        private int bcl1 =0;
        private int bcl2 = 0;
        private int bcl3 = 0;
        private System.Timers.Timer TMroutine1= new System.Timers.Timer();
        private System.Timers.Timer TMroutine2= new System.Timers.Timer();
        private System.Timers.Timer TMroutine3= new System.Timers.Timer();

        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("Démarrage du service");
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Set up a timer to trigger every minute.
            //TMroutine1 = new System.Timers.Timer();
            TMroutine1.Interval = 900000; // 15 min
            TMroutine1.Elapsed += new System.Timers.ElapsedEventHandler(this.Routine1);
            TMroutine1.Start();

            //TMroutine2 = new System.Timers.Timer();
            TMroutine2.Interval = 120000; // 2 min
            TMroutine2.Elapsed += new System.Timers.ElapsedEventHandler(this.Routine2);
            TMroutine2.Start();

            //TMroutine3 = new System.Timers.Timer();
            TMroutine3.Interval = 60000; //  1 min
            TMroutine3.Elapsed += new System.Timers.ElapsedEventHandler(this.Routine3);
            TMroutine3.Start();

            EventLog.WriteEntry("Ecriture journal.log");
            try
            {
                
                      
                MyTrace.WriteLog("START : Démarage du service ServiceCtrlPc", 2, codeappli);                
                string path = @"c:\ProgramData\CtrlPc\";                
                MyTrace.WriteLog("START : Récupération de fichiers à télécharger", 2, codeappli);
                try
                {
                    ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                    Object Guid = null;
                    EventLog.WriteEntry("Récupération fichier de maj");
                    Guid =Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
                    MyTrace.WriteLog("START : Appel du WS --> GetDownloadFile(" + Guid.ToString()+","+ DateTime.Now+")", 2, codeappli);
                    string FileDownload=ws.GetDownloadFile(Guid.ToString(), DateTime.Now);
                    if (FileDownload.Length > 0)
                    {
                        string[] LstFileDownload = FileDownload.Split('\r');
                        foreach (string ligne in LstFileDownload)
                        {
                            MyTrace.WriteLog("START : Téléchargement de " + ligne, 2, codeappli);
                            string argument = ligne.Replace(";", " ");
                            ExecProgram MyExecProgram = new ExecProgram("DownloadFile.exe", argument);
                            MyTrace.WriteLog("START : Téléchargement terminé de " + ligne, 2, codeappli);
                            //mise a jour de la bdd via ws
                            string[] colonne = ligne.Split(new Char[] { ';' });
                            MyTrace.WriteLog("START : Appel du WS --> SetDownloadFile(" + Guid.ToString() + "," + DateTime.Now + "," + colonne[0] + "," + colonne[1] + ")", 2, codeappli);
                            ws.SetDownloadFile(Guid.ToString(), DateTime.Now, colonne[0], colonne[1]);
                        }
                    }
                    else
                    {
                        MyTrace.WriteLog("START : Aucun fichier à télécharger ", 2, codeappli);
                    }
                    
                }
                catch (Exception err)
                {
                    MyTrace.WriteLog(err.Message, 1, codeappli);
                }
                EventLog.WriteEntry("fin récupération fichier de maj");
                EventLog.WriteEntry("Génération des fichiers de paramètrage");
                try
                {
                    MyTrace.WriteLog("START : Génération des fichiers de paramètrage", 2, codeappli);
                    ExecProgram MyExecProgram = new ExecProgram("GeneFileParam.exe","0");
                }
                catch (Exception err)
                {
                    MyTrace.WriteLog("START : Génération des fichier ko --> " + err.Message, 1, codeappli);
                }

                MyTrace.WriteLog("START : Contrôle du flag arr.flg", 2, codeappli);
                LectureFlag MyLectureFlag = new LectureFlag();
                MyLectureFlag.LectureFlagArr(path + @"FLAG\");
            }
            
            catch (Exception err)
            {
                EventLog.WriteEntry(err.Message);
            }
        }

        public void Routine1(object sender, System.Timers.ElapsedEventArgs args)
        {
            TMroutine1.Stop();
            if (bcl1 > 1000)
            {
                bcl1 = 0;
            }
            bcl1++;
            int id = bcl1;
            //Exécution des program et création des fichiers de param et téléchargement des maj
            //création fichier param
            MyTrace.WriteLog("RT1 : "+id.ToString()+" : Début routine 1", 2, codeappli);
            SynchroHeure MySynchroHeure = new SynchroHeure();
            try
            {
                ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                Object Guid = null;
                Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
                MyTrace.WriteLog("RT1 : " + id.ToString() + " : Création de fichiers de paramètrage", 2, codeappli);
                ExecProgram MyExecProgram = new ExecProgram("GeneFileParam.exe", "0");
            }
            catch (Exception err)
            {
                MyTrace.WriteLog("RT1 : " + id.ToString() + " : Erreur lors de la création des fichier de paramètrage --> " + err.Message, 1, codeappli);
            }
            //téléchargement des fichiers :
            try
            {
                ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                Object Guid = null;
                Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
                MyTrace.WriteLog("RT1 : " + id.ToString() + " : Controle des fichiers à télécharger", 2, codeappli);
                DateTime dateTraitement = DateTime.Now;
                try
                {
                    dateTraitement=MySynchroHeure.GetNetworkTime();
                }
                catch (Exception err)
                {
                    MyTrace.WriteLog("RT1 : " + id.ToString() + " : Récupération heure serveur KO --> " + err.Message, 1, codeappli);
                    dateTraitement = DateTime.Now;
                }
                string FileDownload = ws.GetDownloadFile(Guid.ToString(), dateTraitement);
                if (FileDownload.Length > 0)
                {
                    string[] LstFileDownload = FileDownload.Split(Environment.NewLine.ToCharArray());
                    foreach (string ligne in LstFileDownload)
                    {
                        
                        string argument = ligne.Replace(";", " ");
                        if (argument.Length>3)
                        {
                            MyTrace.WriteLog("RT1 : " + id.ToString() + " : Téléchargement de " + ligne, 2, codeappli);
                            ExecProgram MyExecProgram = new ExecProgram("DownloadFile.exe", argument);
                            MyTrace.WriteLog("RT1 : " + id.ToString() + " : Téléchargement terminé de " + ligne, 2, codeappli);
                            //mise a jour de la bdd via ws
                            string[] colonne = ligne.Split(new Char[] { ';' });
                            try
                            {
                                dateTraitement = MySynchroHeure.GetNetworkTime();
                            }
                            catch (Exception err)
                            {
                                MyTrace.WriteLog("RT1 : " + id.ToString() + " : Récupération heure serveur KO --> " + err.Message, 1, codeappli);
                                dateTraitement = DateTime.Now;
                            }
                            //Vérification si le fichier a bien été téléchargé
                            //string pathControle = ligne.Replace(";", @"\");
                            if (File.Exists(@"c:\ProgramData\CtrlPc\" + colonne[0]+@"\"+colonne[1]))
                            {
                                MyTrace.WriteLog("RT1 : " + id.ToString() + " :Téléchargement réussi : " + colonne[0] + @"\" + colonne[1], 2, codeappli);
                                MyTrace.WriteLog("RT1 : " + id.ToString() + " : Appel du WS --> SetDownloadFile(" + Guid.ToString() + "," + dateTraitement + "," + colonne[0] + "," + colonne[1] + ")", 2, codeappli);
                                ws.SetDownloadFile(Guid.ToString(), dateTraitement, colonne[0], colonne[1]);
                            }
                            else
                            {
                                MyTrace.WriteLog("RT1 : " + id.ToString() + " : Téléchargement KO : " + colonne[0] + @"\" + colonne[1], 1, codeappli);
                            }
                        }
                        else
                        {
                            MyTrace.WriteLog("RT1 : " + id.ToString() + " : Aucun fichier à télécharger ", 2, codeappli);
                        }
                    }
                }
                else
                {
                    MyTrace.WriteLog("RT1 : " + id.ToString() + " : Aucun fichier à télécharger ", 2, codeappli);
                }
            }
            catch (Exception err)
            {
                MyTrace.WriteLog("RT1 : " + id.ToString() + " : Erreur lors du téléchargement --> " + err.Message, 1, codeappli);
                MyTrace.WriteLog("RT1 : " + id.ToString() + " : Erreur lors du téléchargement détail--> " + err.StackTrace, 1, codeappli);
            }
            
            MyTrace.WriteLog("RT1 : " + id.ToString() + " : Fin routine 1", 2, codeappli);
            TMroutine1.Start();
        }
        public void Routine2(object sender, System.Timers.ElapsedEventArgs args)
        {
            TMroutine2.Stop();
            if (bcl2 > 1000)
            {
                bcl2 = 0;
            }
            bcl2++;
            int id = bcl2;
            MyTrace.WriteLog("RT2 : " + id.ToString() + " : Début routine 2", 2, codeappli);
            //Maj de la date de dernière connexion
            try
            {
                ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                Object Guid = null;
                Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
                MyTrace.WriteLog("RT2 : " + id.ToString() + " : MAJ de la date de dernière connexion", 2, codeappli);
                ws.SetDateDerniereConnexion(Guid.ToString());
            }
            catch (Exception err)
            {
                MyTrace.WriteLog("RT2 : " + id.ToString() + " : Erreur lors de l'ajout de la date de dernière connexion " + err.Message, 1, codeappli);
                throw;
            }

            //Vérification du flag d'arrêt prioritaire sur l'exception
            try
            {
                ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                Object Guid = null;
                Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
                MyTrace.WriteLog("RT2 : " + id.ToString() + " : Controle présence demande d'arrêt via WS", 2, codeappli);
                MyTrace.WriteLog("RT2 : " + id.ToString() + " : Appel du WS --> GetArret(" + Guid.ToString() + ")", 2, codeappli);
                string flagArr = ws.GetArret(Guid.ToString());
                if (flagArr.Contains("1") || flagArr.Contains("True"))
                {
                    MyTrace.WriteLog("RT2 : " + id.ToString() + " : Demande d'arrêt de la station =>" + flagArr, 2, codeappli);
                    Shutdown MyShutdown = new Shutdown();
                }
            }
            catch (Exception err)
            {
                MyTrace.WriteLog("RT2 : " + id.ToString() + " : Erreur lors du controle d'arret --> " + err.Message, 1, codeappli);
                MyTrace.WriteLog("RT2 : " + id.ToString() + " : Lecture du fichier arr.flg ", 2, codeappli);
                LectureFlag MyLectureFlag = new LectureFlag();
                try
                {
                    MyLectureFlag.LectureFlagArr(@"C:\ProgramData\CtrlPc\FLAG\");
                }
                catch (Exception err2)
                {
                    MyTrace.WriteLog("RT2 : " + id.ToString() + " : Erreur lors de la lecture du flag arr.flg --> " + err2.Message, 1, codeappli);
                }


            }

            //Controle de l'exception
            int exception=0;
            try
            {
                ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                Object Guid = null;
                Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
                MyTrace.WriteLog("RT2 : " + id.ToString() + " : Controle de demande d'exception", 2, codeappli);

                string result = ws.GetException(Guid.ToString());
                if (result.Contains("True")||result.Contains("1"))
                {
                    exception = 1;
                    MyTrace.WriteLog("RT2 : " + id.ToString() + " : exception => " + result, 2, codeappli);
                    MyTrace.WriteLog("RT2 : " + id.ToString() + " : Pas de contrôle d'arrêt", 2, codeappli);
                }
                else
                {
                    exception = 0;
                    MyTrace.WriteLog("RT2 : " + id.ToString() + " : exception => " + result, 2, codeappli);
                    MyTrace.WriteLog("RT2 : " + id.ToString() + " : Pas d'exception donc contrôle des planning", 2, codeappli);
                }
            }
            catch (Exception err)
            {
                MyTrace.WriteLog("RT2 : " + id.ToString() + " : Erreur lors de la récupération de l'exception => " + err.Message, 1, codeappli);
                MyTrace.WriteLog("RT2 : " + id.ToString() + " : Lecture du fichier nfo.flg ", 2, codeappli);
                LectureFlag MyLectureFlag = new LectureFlag();
                try
                {
                    exception=MyLectureFlag.LectureFlagNfo(@"C:\ProgramData\CtrlPc\FLAG\");
                }
                catch (Exception err2)
                {
                    MyTrace.WriteLog("RT2 : " + id.ToString() + " : Erreur lors de la lecture du flag nfo.flg --> " + err2.Message, 1, codeappli);
                }
            }
            if (exception == 0)
            {
                SynchroHeure MySynchroHeure = new SynchroHeure();
                //controle planning
                MyTrace.WriteLog("RT2 : " + id.ToString() + " : Vérification du planning", 2, codeappli);
                try
                {
                    ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
                    Object Guid = null;
                    Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
                    DateTime dateTraitement = DateTime.Now;
                    try
                    {
                        dateTraitement = MySynchroHeure.GetNetworkTime();
                    }
                    catch (Exception err)
                    {
                        MyTrace.WriteLog("RT2 : " + id.ToString() + " : Récupération heure serveur KO --> " + err.Message, 1, codeappli);
                        dateTraitement = DateTime.Now;
                    }
                    MyTrace.WriteLog("RT2 : " + id.ToString() + " : Appel du WS --> GetPlageHoraire(" + Guid.ToString() + "," + dateTraitement + ")", 2, codeappli);
                    string stop = ws.GetPlageHoraire(Guid.ToString(), dateTraitement);
                    if (stop.Contains("0") || stop.Contains("False"))
                    {
                        MyTrace.WriteLog("RT2 : " + id.ToString() + " : Demande d'arrêt envoyé par WS --> " + stop, 2, codeappli);
                        Shutdown MyShutdown = new Shutdown();
                    }
                    else
                    {
                        MyTrace.WriteLog("RT2 : " + id.ToString() + " : Pas de demande d'arrêt de la part du WS", 2, codeappli);
                    }
                }
                catch (Exception err)
                {
                    MyTrace.WriteLog("RT2 : " + id.ToString() + " : Erreur lors de la vérification du planning via WS--> " + err.Message, 1, codeappli);
                    //vérification dans fichier
                    try
                    {
                        MyTrace.WriteLog("RT2 : " + id.ToString() + " : Contrôle du planning via fichier planning", 2, codeappli);
                        ControleHoraireLocal MyControleHoraireLocal = new ControleHoraireLocal();
                    }
                    catch (Exception err2)
                    {
                        MyTrace.WriteLog("RT2 : " + id.ToString() + " : Erreur lors du contrôle en local du planning --> " + err2.Message, 1, codeappli);
                    }

                }
            }
            MyTrace.WriteLog("RT2 : " + id.ToString() + " : Fin routine 2", 2, codeappli);
            TMroutine2.Start();

        }

        public void Routine3(object sender, System.Timers.ElapsedEventArgs args)
        {
            TMroutine3.Stop();
            if (bcl3>1000)
            {
                bcl3 =0;
            }
            bcl3++;
            int id = bcl3;
            MyTrace.WriteLog("RT3 : " + id.ToString() + " : Début Routine 3", 2, codeappli);
            SynchroHeure MySynchroHeure = new SynchroHeure();

            ReferenceWSCtrlPc.WSCtrlPc ws = new ReferenceWSCtrlPc.WSCtrlPc();
            Object Guid = null;
            Guid = Registry.GetValue(@"HKEY_USERS\.DEFAULT\Software\CtrlPc\Version", "GUID", null);
            DateTime dateTraitement = DateTime.Now;
            try
            {
                dateTraitement = MySynchroHeure.GetNetworkTime();
            }
            catch (Exception err)
            {
                MyTrace.WriteLog("RT3 : " + id.ToString() + " : Récupération heure serveur KO --> " + err.Message, 1, codeappli);
                dateTraitement = DateTime.Now;
            }
            try
            {
                MyTrace.WriteLog("RT3 : " + id.ToString() + " : Appel du WS --> GetExecProgram(" + Guid.ToString()+", "+dateTraitement+")", 2, codeappli);
                string ExecProgram=ws.GetExecProgram(Guid.ToString(), dateTraitement);
                if (ExecProgram.Length > 0 && !ExecProgram.Contains("0;0"))
                {
                    string[] ExecProgramLine = ExecProgram.Split(Environment.NewLine.ToCharArray());
                    foreach (string ligne in ExecProgramLine)
                    {
                        if (ligne.Length > 0 && ligne.Contains(";"))
                        {
                            MyTrace.WriteLog("RT3 : " + id.ToString() + " : Parse de la ligne suivante : " + ligne, 2, codeappli);
                            string[] colonne = ligne.Split(';');
                            try
                            {
                                string path = @"c:\ProgramData\CtrlPc\SCRIPT\";
                                CtrlProcess MyCtrlProcess = new CtrlProcess();
                                if (File.Exists(path + colonne[1])&& !MyCtrlProcess.CtrlProcessRunning(colonne[1]))
                                {
                                    MyTrace.WriteLog("RT3 : " + id.ToString() + " : Process non trouvé pour l'aplpication : " + colonne[1], 2, codeappli);
                                    dateTraitement = DateTime.Now;
                                    try
                                    {
                                        dateTraitement = MySynchroHeure.GetNetworkTime();
                                    }
                                    catch (Exception err)
                                    {
                                        MyTrace.WriteLog("RT3 : " + id.ToString() + " : Récupération heure serveur KO --> " + err.Message, 1, codeappli);
                                        dateTraitement = DateTime.Now;
                                    }
                                    MyTrace.WriteLog("RT3 : " + id.ToString() + " : Appel du WS --> SetExecProgram(" + Convert.ToInt32(colonne[0]) + "," + dateTraitement + ")", 2, codeappli);
                                    ws.SetExecProgram(Convert.ToInt32(colonne[0]), dateTraitement);
                                    ExecProgram MyExecProgram = new ExecProgram(colonne[1], "0");
                                    
                                    
                                }
                                else
                                {
                                    MyTrace.WriteLog("RT3 : " + id.ToString() + " : Le program n'est^pas présent : " + ligne, 1, codeappli);
                                }
                            }
                            catch (Exception err)
                            {
                                MyTrace.WriteLog("RT3 : " + id.ToString() + " : " + err.Message, 1, codeappli);
                            }
                        }
                        
                    }
                }
                else
                {
                    MyTrace.WriteLog("RT3 : " + id.ToString() + " : Aucun program à exécuter", 2, codeappli);
                }
            }
            catch (Exception err)
            {
                MyTrace.WriteLog("RT3 : " + id.ToString() + " : ERREUR --> " + err.Message, 1, codeappli);
                MyTrace.WriteLog("RT3 : " + id.ToString() + " : Lecture du fichier d'exécution de program", 2, codeappli);
                try
                {
                    LectureFileExecProgram MyStartRoutine1 = new LectureFileExecProgram();
                }
                catch (Exception err2)
                {
                    MyTrace.WriteLog("RT3 : " + id.ToString() + " : Lecture du fichier csv --> " + err2.Message, 1, codeappli);
                }
            }
            
            MyTrace.WriteLog("RT3 : " + id.ToString() + " : Fin Routine 3", 2, codeappli);
            TMroutine3.Start();
        }

        protected override void OnStop()
        {            
            MyTrace.WriteLog("Arrêt du service ServiceCtrlPc", 2, codeappli);
        }       
    }
}
 