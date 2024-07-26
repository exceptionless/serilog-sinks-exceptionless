using Serilog;
using System;
using Exceptionless;
using Serilog.Sinks.SystemConsole.Themes;

namespace ConsoleDemo
{
    public class Program
    {
        public static void Main()
        {
            ExceptionlessClient.Default.Startup("API_KEY");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.Exceptionless(c => c.AddTags("Serilog Sample"))
                .CreateLogger();

            try
            {
                Log.Debug("Getting started");
                Log.Information("Hello {Name} from thread {ThreadId}", Environment.GetEnvironmentVariable("USERNAME"), Environment.CurrentManagedThreadId);
                Log.Warning("No coins remain at position {@Position}", new { Lat = 25, Long = 134 });

                Fail();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong");
            }

            Log.CloseAndFlush();
        }

        private static void Fail()
        {
            throw new DivideByZeroException();
        }
    }
}