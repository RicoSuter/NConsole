# NConsole for .NET

**This library is deprecated and should not be used anymore, use e.g. [Spectre.Console](https://spectreconsole.net/) instead.**

[![Build status](https://ci.appveyor.com/api/projects/status/llcch712f3q1wswe?svg=true)](https://ci.appveyor.com/project/rsuter/nconsole)
[![NuGet Version](https://badge.fury.io/nu/nconsole.svg)](https://www.nuget.org/packages/NConsole/)

NConsole is a .NET library to parse command line arguments and execute commands. If the command name or some non-optional parameters are missing, the library automatically switches to an interactive mode where the user is prompted to enter the missing parameters. Using descriptive attributes on the command class and its parameters, a nicely formatted help page can be generated.

To use the library in your application, install [the NuGet package NConsole](https://www.nuget.org/packages/NConsole/).

Features:

- Interactive mode when parameters are missing
- Customizable parameter reading
- Automatically generated help page
- Support for optional parameters
- Dependency injection to inject services into command instances

## Usage

The usage is simple: Create an instance of the `CommandLineProcessor` class, register the commands and execute a command by calling the `Process()` method with the command line arguments.

```csharp
namespace MyApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<SumCommand>("sum");
            processor.Process(args);
        }
    }

    [Description("Calculates the sum of two values.")]
    public class SumCommand : IConsoleCommand
    {
        [Description("The first value.")]
        [Argument(Name = "FirstValue")]
        public int FirstValue { get; set; }

        [Description("The second value.")]
        [Argument(Name = "SecondValue", IsRequired = false)]
        public int SecondValue { get; set; } = 10;

        public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            host.WriteMessage((FirstValue + SecondValue).ToString());
            return null;
        }
    }
}
```

The command `sum` can now be executed using the following command line call:

    MyApplication.exe sum /firstvalue:5 /secondvalue:6
    Output: 11

The `SecondValue` has is not required (`IsRequired` set to ``false`). The default value is set via a C# property initializer. This parameter is not required:

    MyApplication.exe sum /firstvalue:5
    Output: 15

Execute the following command to show a list of all available commands:

    MyApplication.exe help
