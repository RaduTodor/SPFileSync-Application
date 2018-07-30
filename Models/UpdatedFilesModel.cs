namespace Models
{
    using System;
    using System.Collections.Generic;

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