NConsole is a simple .NET library which parses command line arguments and executes the desired commands with the provided parameters. If the command name or some non-optional parameters are missing, the library automatically switches to an interactive mode where the user is prompted to enter the missing values. 

## Usage

    namespace MyApplication
    {
        class Program
        {
            static void Main(string[] args)
            {
                var processor = new CommandLineProcessor(new CommandLineHost());
                processor.AddCommand<SumCommand>("sum");
                processor.Process(args);
            }
        }

        [Description("Calculates the sum of two values.")]
        public class SumCommand : ICommandLineCommand
        {
            [Description("The first value.")]
            public int FirstValue { get; set; }

            [Description("The second value.")]
            [DefaultValue(10)]
            public int SecondValue { get; set; }

            public void Run(CommandLineProcessor processor, ICommandLineHost host)
            {
                host.WriteMessage((FirstValue + SecondValue).ToString());
            }
        }
    }

The command can now be called this way: 

    MyApplication.exe sum /firstvalue:5 /secondvalue:6
    
Output: 11
    
With the optional parameter: 

    MyApplication.exe sum /firstvalue:5
    
Output: 15
