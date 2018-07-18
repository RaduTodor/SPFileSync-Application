namespace Models
{
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class Credentials
    {
        private const string EncodingString = "ABCDEFGH";

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
            var cryptic = new DESCryptoServiceProvider();
            cryptic.Key = Encoding.ASCII.GetBytes(EncodingString);
            cryptic.IV = Encoding.ASCII.GetBytes(EncodingString);

            byte[] data = null;
            var empty = "";
            using (var stream = new MemoryStream())
            {
                var cryptoStream = new CryptoStream(stream, cryptic.CreateEncryptor(), CryptoStreamMode.Write);
                data = Encoding.ASCII.GetBytes(value);
                cryptoStream.Write(data, 0, data.Length);
                cryptoStream.Close();
                return Encoding.UTF8.GetString(stream.GetBuffer());
            }
        }

        private string Decrypt(string value)
        {
            var cryptic = new DESCryptoServiceProvider();

            cryptic.Key = Encoding.ASCII.GetBytes(EncodingString);
            cryptic.IV = Encoding.ASCII.GetBytes(EncodingString);

            using (var stream = GenerateStreamFromString(value))
            {
                var cryptoStream = new CryptoStream(stream,
                    cryptic.CreateDecryptor(), CryptoStreamMode.Read);

                var reader = new StreamReader(cryptoStream);
                var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(reader.ReadToEnd()));
                return Encoding.UTF8.GetString(memoryStream.ToArray());
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
