using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NConsole.Tests
{
    [TestClass]
    public class ArrayArgumentTests
    {
        public class MyArrayCommand : IConsoleCommand
        {
            [Argument(Name = "MyStrings")]
            public string[] MyStrings { get; set; }

            public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                return null;
            }
        }

        [TestMethod]
        public async Task When_argument_is_string_array_then_it_can_be_defined_as_comma_separated_string()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArrayCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new string[] { "test", "/mystrings:a,b,c" });
            var command = (MyArrayCommand)result.Last().Command;

            //// Assert
            Assert.AreEqual(3, command.MyStrings.Length);
            Assert.AreEqual("a", command.MyStrings[0]);
            Assert.AreEqual("b", command.MyStrings[1]);
            Assert.AreEqual("c", command.MyStrings[2]);
        }

        public class MyDefaultArrayCommand : IConsoleCommand
        {
            [Argument(Name = "MyStrings", IsRequired = false)]
            public string[] MyStrings { get; set; } = new string[] { };

            public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                return null;
            }
        }

        [TestMethod]
        public async Task When_array_has_empty_default_then_property_is_correctly_initialized()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyDefaultArrayCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new string[] { "test" });
            var command = (MyDefaultArrayCommand)result.Last().Command;

            //// Assert
            Assert.AreEqual(0, command.MyStrings.Length);
        }

        public class MyDefault2ArrayCommand : IConsoleCommand
        {
            [Argument(Name = "MyStrings", IsRequired = false)]
            public string[] MyStrings { get; set; } = new string[] { "a", "b", "c" };

            public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                return null; 
            }
        }

        [TestMethod]
        public async Task When_array_has_default_items_then_property_is_correctly_initialized()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyDefault2ArrayCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new string[] { "test" });
            var command = (MyDefault2ArrayCommand)result.Last().Command;

            //// Assert
            Assert.AreEqual(3, command.MyStrings.Length);
            Assert.AreEqual("a", command.MyStrings[0]);
            Assert.AreEqual("b", command.MyStrings[1]);
            Assert.AreEqual("c", command.MyStrings[2]);
        }
    }
}
