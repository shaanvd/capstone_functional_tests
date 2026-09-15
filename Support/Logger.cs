using Serilog;

namespace CapstoneProject.Support
{
    public static class Logger
    {
        public static void Initialize()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("Logs/Execution-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}