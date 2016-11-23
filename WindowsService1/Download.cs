using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceCtrlPc
{
    public class Download
    {
        public FileInfo FtpDownload(Uri uri, NetworkCredential credentials, DirectoryInfo dirInfo)
        {
            FileInfo fi;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uri);
            request.Credentials = credentials;
            request.UseBinary = true;
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            // Utilisé pour envoyer la commande "QUIT" au serveur afin de fermer correctement la connexion
            request.KeepAlive = false;
            // Taille du tableau servant à stocker les morceaux du fichier.
            // On épargne ainsi les ressources du serveur en ne chargeant pas la totalité du fichier.
            // Cela permet de transférer rapidement des fichiers volumineux.
            byte[] buffer = new byte[2048];
            // Variable de mesure de la taille du morceau de fichier lu. Permet d'indiquer que le fichier a été lu et écrit en totalité.
            // Recupération de la réponse
            using (FtpWebResponse res = (FtpWebResponse)request.GetResponse())
            {
                fi = new FileInfo(string.Concat(dirInfo.FullName, uri.Segments[uri.Segments.Length - 1]));
                using (BinaryReader stream = new BinaryReader(res.GetResponseStream()))
                {
                    using (FileStream fs = File.Create(fi.FullName, buffer.Length, FileOptions.WriteThrough))
                    {
                        // Ecriture du flux dans le fichier block par block
                        int block;
                        while ((block = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, block);
                        }
                    }
                }
            }

            return fi;
        }
    }
}
