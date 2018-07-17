using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Models
{
    public class Credentials
    {
        private const string encodingString = "ABCDEFGH";

        public string UserName { get; set; }

        //[XmlIgnore]
        public string Password { get; set; }

        //[XmlAttribute("CryptedPassword")]
        //[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        //public string CryptedPassword
        //{
        //    get { return (Encrypt(Password)); }
        //    set { Password = Decrypt(value); }
        //}

        private string Encrypt(string value)
        {
            DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();
            cryptic.Key = ASCIIEncoding.ASCII.GetBytes(encodingString);
            cryptic.IV = ASCIIEncoding.ASCII.GetBytes(encodingString);

            byte[] data = null;
            string empty = "";
            using (var stream = new MemoryStream())
            {
                CryptoStream cryptoStream = new CryptoStream(stream, cryptic.CreateEncryptor(), CryptoStreamMode.Write);
                data = ASCIIEncoding.ASCII.GetBytes(value);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.Close();
                return System.Text.Encoding.UTF8.GetString(stream.GetBuffer());
            }
        }

        private string Decrypt(string value)
        {
            DESCryptoServiceProvider cryptic = new DESCryptoServiceProvider();

            cryptic.Key = ASCIIEncoding.ASCII.GetBytes(encodingString);
            cryptic.IV = ASCIIEncoding.ASCII.GetBytes(encodingString);

            using (var stream = GenerateStreamFromString(value))
            {
                CryptoStream cryptoStream = new CryptoStream(stream,
                    cryptic.CreateDecryptor(), CryptoStreamMode.Read);

                StreamReader reader = new StreamReader(cryptoStream);
                MemoryStream memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(reader.ReadToEnd()));
                return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        private Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
