namespace Common.Constants
{
    public static class DefaultExceptionMessages
    {
        public const string ErrorMetadataReadExceptionMessage =
            "There was an error while reading from local metadata files";

        public const string FileDownloadExceptionMessage =
            "There was an error while downloading the file from url: {0}";

        public const string CreateFileExceptionMessage =
            "There was an error while creating or granting access to a file";

        public const string CsomAddExceptionMessage =
            "There was an error while trying to add a reference listItem with csom tehnology";

        public const string CsomChangeExceptionMessage =
            "There was an error while trying to change a reference listItem with csom tehnology";

        public const string CsomRemoveExceptionMessage =
            "There was an error while trying to remove a reference listItem with csom tehnology";

        public const string RestAddExceptionMessage =
            "There was an error while trying to add a reference listItem with rest calls";

        public const string RestChangeExceptionMessage =
            "There was an error while trying to change a reference listItem with rest calls";

        public const string RestRemoveExceptionMessage =
            "There was an error while trying to remove a reference listItem with rest calls";

        public const string GetRequestExceptionMessage =
            "There was an error while trying get list {0} with a request from sharepoint {1}";

        public const string ClientContextOperationExceptionMessage =
            "There was an error while creating a client context or while executing a querry from it";

        public const string CurrentUserExceptionMessage =
            "There was an error while creating a client context or while executing a querry from it";

        public const string LoginExceptionMessage = "There was an error while trying to login to sharepoint site";

        public const string NoInternetAccessExceptionMessage = "The internet access is no longer availabe. The application will retry the sync in {0} minutes";

        public const string ConfigurationSyncFinishedUnssuccesful =
            "The sync process of configuration with sharepoint site {0} has finished but there was an error";

        public const string AccessFileUrlExceptionMessage =
            "There was an error while accessing the file from url: {0}";

        public const string FileNotAvailableExceptionMessage =
            "The file from url: {0} is no longer available";
    }
}