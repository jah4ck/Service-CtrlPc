using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCtrlPc
{
    public class TransfertFile
    {
        Trace MyTrace = new Trace();
        private string codeappli = "SERVICES";
        public TransfertFile()
        {
            Download MyDownload = new Download();
            string login = @"pdv3f33\dev1";
            string password = "Stine1!?";
            string ftphost = "86.247.64.225";
            string a = login.Trim();
            string b = password.Trim();
            try
            {
                string[] LstFile = File.ReadAllLines(@"c:\ProgramData\CtrlPc\SCRIPT\LstFileUpload.csv");
                foreach (string item in LstFile)
                {
                    string path = item.Substring(0, item.LastIndexOf(";"));
                    string file = item.Substring(item.LastIndexOf(";") + 1);

                    string ftpfilepath = @"/CtrlPc/" + path + @"/" + file + "";
                    string ftpfullpath = @"ftp://" + ftphost + ftpfilepath;
                    MyTrace.WriteLog("Transfert fichier : "+ftpfullpath, 2, codeappli);
                    MyTrace.WriteLog("Vers : " + @"c:\ProgramData\CtrlPc\" + path + @"\", 2, codeappli);
                    MyDownload.FtpDownload(new Uri(ftpfullpath), new NetworkCredential(a, b), new DirectoryInfo(@"c:\ProgramData\CtrlPc\" + path + @"\"));
                }
            }
            catch (Exception err)
            {
                MyTrace.WriteLog(err.Message, 1, codeappli);
            }
        }
    }
}
