namespace Common.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using Common.Constants;

    //TODO [CR RT]: Add class and methods documentation
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
            FileEditingHelper.CreateAccesibleFile(Directory.GetCurrentDirectory() + HelpersConstant.XmlParentDirectoryAndFile, Directory.GetCurrentDirectory() + HelpersConstant.XmlParentDirectory);
            TextWriter textWriter = File.CreateText(Directory.GetCurrentDirectory() + HelpersConstant.XmlParentDirectoryAndFile);
            xmlSerializer.Serialize(textWriter, list);
        }

        /// <summary>
        /// Generic Deserialize <see langword="method"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> Deserialize<T>()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(new List<T>().GetType());
            TextReader textReader = File.OpenText(Directory.GetCurrentDirectory() + HelpersConstant.XmlParentDirectoryAndFile);

            return (List<T>)xmlSerializer.Deserialize(textReader);

        }
    }
}
