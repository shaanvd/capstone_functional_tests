using Serilog;

namespace Capstone_Project.Support
{
    public static class Logger
    {
        public static void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/Execution-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}