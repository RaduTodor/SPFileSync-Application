using Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace DataAccessLayer
{
    public class FilesGetter
    {
        public ConnectionConfiguration ConnectionConfiguration { get; set; }

        public string DownloadFile(string url, string directoryPath)
        {
            string serverTempdocPath = "";
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var credentials = ConnectionConfiguration.Connection.Credentials;

                request.Credentials = new NetworkCredential(credentials.UserName,credentials.Password);
                request.Timeout = 20000;
                request.AllowWriteStreamBuffering = false;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        serverTempdocPath = Path.Combine(directoryPath, ParseURLFileName(url));
                        using (FileStream fs = new FileStream(serverTempdocPath, FileMode.Create))
                        {
                            byte[] read = new byte[256];
                            int count = stream.Read(read, 0, read.Length);
                            while (count > 0)
                            {
                                fs.Write(read, 0, count);
                                count = stream.Read(read, 0, read.Length);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return "";
            }
            return serverTempdocPath;
        }

        public static string ParseURLFileName(string url)
        {
            url = url.Replace("%20", " ");
            return url.Split('/').Last();
        }
    }
}
