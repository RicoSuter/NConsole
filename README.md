NConsole is a .NET library to parse command line arguments and execute commands. If the command name or some non-optional parameters are missing, the library automatically switches to an interactive mode where the user is prompted to enter the missing parameters. Using descriptive attributes on the command class and its parameters, a nicely formatted help page can be generated.  

To use the library, install the [NuGet package 'NConsole'](https://www.nuget.org/packages/NConsole/). 

Features: 

- Interactive mode when parameters are missing
- Customizable parameter reading
- Automatically generated help page
- Support for optional parameters
- Dependency injection to inject services into command instances

## Usage

The usage is simple: Create an instance of the `CommandLineProcessor` class, register the commands and execute a command by calling the `ProcessAsync()` method with the command line arguments. 

    namespace MyApplication
    {
        class Program
        {
            static void Main(string[] args)
            {
                var processor = new CommandLineProcessor(new CommandLineHost());
                processor.AddCommand<SumCommand>("sum");
                processor.ProcessAsync(args).Wait();
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

The command `sum` can now be executed using the following command line call: 

    MyApplication.exe sum /firstvalue:5 /secondvalue:6
    Output: 11
    
The `SecondValue` has a default value and is therefore optional. This parameter is not required: 

    MyApplication.exe sum /firstvalue:5
    Output: 15

To show a list of all available commands with their parameters execute: 

    MyApplication.exe help
