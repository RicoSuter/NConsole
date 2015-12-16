using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NConsole.Tests
{
    [TestClass]
    public class ArrayArgumentTests
    {
        [TestMethod]
        public async Task When_argument_is_string_array_then_it_can_be_defined_as_comma_separated_string()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArrayCommand>("test");

            //// Act
            var command = (MyArrayCommand)await processor.ProcessAsync(new string[] { "test", "/mystrings:a,b,c" }); 

            //// Assert
            Assert.AreEqual(3, command.MyStrings.Length);
            Assert.AreEqual("a", command.MyStrings[0]);
            Assert.AreEqual("b", command.MyStrings[1]);
            Assert.AreEqual("c", command.MyStrings[2]);
        }
    }

    public class MyArrayCommand : IConsoleCommand
    {
        [Argument(Name = "MyStrings")]
        public string[] MyStrings { get; set; }

        public async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
        }
    }
}
