using NLog;

namespace Common.Helpers
{
    //TODO [CR BT] : Please rename the class with a more specific name. eg. TraceLogger, LoggerManager etc. Do not use 'My' in the name of classes/methods/properties
    //TODO [CR BT] : This class should have every method/property static which means that also the class should be static.
    //TODO [CR BT] : Add documentation for class.
    //TODO [CR BT] : Move usings into namespace.
    public class MyLogger
    {
        public static Logger Logger = LogManager.GetCurrentClassLogger();  
    }
}
