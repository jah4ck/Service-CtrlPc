using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCtrlPc
{
    public class TransfertOneFile
    {
        public TransfertOneFile(string pathFTP, string pathStation)
        {
            Download MyDownload = new Download();
            string login = @"pdv3f33\dev1";
            string password = "Stine1!?";

            string ftphost = "86.247.64.225";

            string ftpfullpath = @"ftp://" + ftphost + pathFTP;

            string a = login.Trim();
            string b = password.Trim();
            MyDownload.FtpDownload(new Uri(ftpfullpath), new NetworkCredential(a, b), new DirectoryInfo(pathStation));

        }
    }
}
