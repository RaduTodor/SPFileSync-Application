using Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace DataAccessLayer
{
    //TODO [CR RT]: Add class and methods documentation
    //TODO [CR RT]: Rename class e.g. FileOperationProvider. Rename DownloadFile mehtod to simply Download
    //TODO [CR RT]: Make ConnectionConfiguration private, initialize it from ctor. It is not used from outside of the class -> incapsulation violation
    public class FilesGetter
    {
        public ConnectionConfiguration ConnectionConfiguration { get; set; }

        //TODO [CR RT]: Change return type to void, the returned value is not used
        //TODO [CR RT]: Change naming of fs, serverTempdocPath;
        //TODO [CR RT]: Add null chack for stream
        //TODO [CR RT]: Extract request creation in separate method
        public string DownloadFile(string url, string directoryPath)
        {
            string serverTempdocPath = "";
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var credentials = ConnectionConfiguration.Connection.Credentials;

                request.Credentials = credentials;
                //TODO [CR RT]: Extract constant, magic number
                request.Timeout = 20000;
                request.AllowWriteStreamBuffering = false;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        serverTempdocPath = Path.Combine(directoryPath, ParseURLFileName(url));
                        using (FileStream fs = new FileStream(serverTempdocPath, FileMode.Create))
                        {
                            //TODO [CR RT]: Extract constant, magic number. Why 256?
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
                //TODO [CR RT]: Use string.Empty
                //TODO [CR RT]: Log exception in document log
                Console.WriteLine(exc.Message);
                return "";
            }
            return serverTempdocPath;
        }

        ////TODO [CR RT]: Change naming ParseURLFileName -> ParseUrlFileName
        ////TODO [CR RT]: This is a Helper/Utility method -> extract it into Common. It has no direct relation with the rest of the logic from the class.
        public static string ParseURLFileName(string url)
        {
            url = url.Replace("%20", " ");
            return url.Split('/').Last();
        }
    }
}
