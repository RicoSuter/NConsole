using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NConsole.Tests
{
    [TestClass]
    public class NamedArgumentTests
    {
        [TestMethod]
        public async Task When_running_command_with_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            await processor.ProcessAsync(new string[] { "test", "/uint16:123" });

            //// Assert
            // No exception
        }
        
        public class MyArgumentCommand : IConsoleCommand
        {
            [Argument(Name = "UInt16")]
            public UInt16 UInt16 { get; set; }

            public async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                if (UInt16 != 123)
                    throw new Exception("UInt16 is not 123");
            }
        }
    }
}
