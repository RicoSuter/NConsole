using System;
using System.Linq;
using System.Threading.Tasks;
using NConsole;

namespace SampleConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var processor = new CommandLineProcessor(new ConsoleHost(!args.Contains("/noninteractive")));
            processor.RegisterCommand<CloneCommand>("clone");
            processor.ProcessWithExceptionHandling(args);
            Console.ReadKey();
        }

        public class CloneCommand : IConsoleCommand
        {
            [Argument(Position = 1)]
            public string Repository { get; set; }

            [Switch(ShortName = "q", LongName = "quiet")]
            public bool Quiet { get; set; }

            [Argument(Name = "Test")]
            public string Test { get; set; }

            public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                host.WriteMessage(string.Format("Clone {{ Repository={0}, Quiet={1}, Test={2} }}", Repository, Quiet, Test));
                return null;
            }
        }
    }
}
