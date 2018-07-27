namespace Common.Helpers
{
    using NLog;
    /// <summary>
    ///     An instance of LoggerManager class is used to log everything in the application.
    /// </summary>
    public static class LoggerManager
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();
    }
}