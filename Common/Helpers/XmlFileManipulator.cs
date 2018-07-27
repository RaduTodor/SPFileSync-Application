namespace Common.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Constants;

    /// <summary>
    ///     This class is serializing and deserializing a list of objects
    /// </summary>
    public static class XmlFileManipulator
    {
        /// <summary>
        ///     Generic Serialize <see langword="method" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Serialize<T>(List<T> list)
        {
            var xmlSerializer = new XmlSerializer(list.GetType());
            FileEditingHelper.CreateAccesibleFile(
                Directory.GetCurrentDirectory() + HelpersConstants.XmlParentDirectoryAndFile,
                Directory.GetCurrentDirectory() + HelpersConstants.ParentDirectory);
            using (TextWriter textWriter =
                File.CreateText(Directory.GetCurrentDirectory() + HelpersConstants.XmlParentDirectoryAndFile))
            {             
                lock(textWriter)
                    xmlSerializer.Serialize(textWriter, list);                             
            }
        }

        /// <summary>
        ///     Generic Deserialize <see langword="method" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> Deserialize<T>()
        {
            var xmlSerializer = new XmlSerializer(new List<T>().GetType());
            using (TextReader textReader =
                File.OpenText(Directory.GetCurrentDirectory() + HelpersConstants.XmlParentDirectoryAndFile))
            {
                lock (textReader) 
                return (List<T>) xmlSerializer.Deserialize(textReader);
            }
        }
    }
}