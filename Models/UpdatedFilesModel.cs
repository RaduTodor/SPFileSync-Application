namespace Models
{
    using System;
    using System.Collections.Generic;
    //TODO [CR RT]: Create a separae model and use a list of that model instead of multiple lists.
    public class UpdatedFilesModel
    {
        private static int instanceCounter = 0;
        private static UpdatedFilesModel singleInstance;
        private static readonly object lockObject = new object();

        private UpdatedFilesModel()
        {
            UpdatedFilesUrl = new List<string>();
            UpdatedFilesName = new List<string>();
            UpdatedFilesLocation = new List<string>();
            UpdatedFilesUpdateMoment = new List<DateTime>();
        }

        public static UpdatedFilesModel Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (singleInstance == null) singleInstance = new UpdatedFilesModel();
                }

                return singleInstance;
            }
        }
        //TODO [CR RT]: If the name of the class it's called UpdateFileModel do not use in the name of parameters UpdatedFiles. If you call it like this it looks weird: UpdatedFilesModel.UpdatedFilesUrl. If you just name it Url it will look more fine: UpdatedFilesModel.Url. When you name a parameter always check the name of the class to look like a story
        public List<string> UpdatedFilesUrl { get; set; }

        public List<string> UpdatedFilesName { get; set; }

        public List<string> UpdatedFilesLocation { get; set; }

        public List<DateTime> UpdatedFilesUpdateMoment { get; set; }

        public void RemoveElementAt(int index)
        {
            UpdatedFilesUrl.RemoveAt(index);
            UpdatedFilesName.RemoveAt(index);
            UpdatedFilesLocation.RemoveAt(index);
            UpdatedFilesUpdateMoment.RemoveAt(index);
        }

        public string GetFileDetailsAt(int index)
        {
            return
                $"{UpdatedFilesName[index]} | {UpdatedFilesUpdateMoment[index]:g} from {UpdatedFilesUrl[index]}";
        }
    }
}