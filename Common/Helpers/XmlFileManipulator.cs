namespace Common.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Common.Constants;

    /// <summary>
    /// This class is serializing and deserializing a list of objects
    /// </summary>
    public static class XmlFileManipulator
    {
        /// <summary>
        /// Generic Serialize <see langword="method"/> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Serialize<T>(List<T> list)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(list.GetType());
            Directory.GetCurrentDirectory();
            FileEditingHelper.CreateAccesibleFile(Directory.GetCurrentDirectory() + HelpersConstants.XmlParentDirectoryAndFile, Directory.GetCurrentDirectory() + HelpersConstants.XmlParentDirectory);
            TextWriter textWriter = File.CreateText(Directory.GetCurrentDirectory() + HelpersConstants.XmlParentDirectoryAndFile);
            xmlSerializer.Serialize(textWriter, list);
            textWriter.Close();
        }

        /// <summary>
        /// Generic Deserialize <see langword="method"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> Deserialize<T>()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(new List<T>().GetType());
            TextReader textReader = File.OpenText(Directory.GetCurrentDirectory() + HelpersConstants.XmlParentDirectoryAndFile);

            return (List<T>)xmlSerializer.Deserialize(textReader);

        }
    }
}
