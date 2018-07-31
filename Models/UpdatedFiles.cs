namespace Models
{
    using System.Collections.Generic;
    public class UpdatedFiles
    {
        private static int _instanceCounter = 0;
        private static UpdatedFiles _singleInstance;
        private static readonly object LockObject = new object();

        private UpdatedFiles()
        {
            Files = new List<UpdatedFileModel>();
        }

        public static UpdatedFiles Instance
        {
            get
            {
                lock (LockObject)
                {
                    if (_singleInstance == null) _singleInstance = new UpdatedFiles();
                }

                return _singleInstance;
            }
        }

        public List<UpdatedFileModel> Files { get; set; }

        public string GetFileDetailsAt(int index)
        {
            return
                $"{Files[index].FileName} | {Files[index].UpdatedDate:g} from {Files[index].Url}";
        }
    }
}