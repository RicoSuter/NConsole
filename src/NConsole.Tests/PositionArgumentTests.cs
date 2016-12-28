using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NConsole.Tests
{
    [TestClass]
    public class PositionArgumentTests
    {
        public class MyCommand : IConsoleCommand
        {
            [Argument(Position = 1)]
            public string First { get; set; }

            public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                return First;
            }
        }

        [TestMethod]
        public async Task When_positional_argument_starts_with_slash_then_it_is_correctly_read()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new string[] { "test", "/foo/bar/test.cs" });
            var command = (MyCommand)result.Last().Command;

            //// Assert
            Assert.AreEqual("/foo/bar/test.cs", command.First);
        }
    }
}