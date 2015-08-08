using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NConsole;

namespace SampleConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var processor = new CommandLineProcessor(new CommandLineHost());
            processor.AddCommand<CloneCommand>("clone");
            processor.ProcessAsync(new string[] { "clone", "--quiet", "http://example.com/app.git" }).Wait();
            Console.ReadKey();
        }
        
        public class CloneCommand : ICommandLineCommand
        {
            [Argument(Position = 0)]
            public string Repository { get; set; }

            [Switch(ShortName = "q", LongName = "quiet")]
            public bool Quiet { get; set; }

            public async Task RunAsync(CommandLineProcessor processor, ICommandLineHost host)
            {
                host.WriteMessage(string.Format("Clone {{ Repository={0}, Quiet={1} }}", Repository, Quiet));
            }
        }
    }
}
