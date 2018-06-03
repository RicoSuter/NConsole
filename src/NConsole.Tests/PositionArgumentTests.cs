using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NConsole.Tests
{
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

        [Fact]
        public async Task When_positional_argument_starts_with_slash_then_it_is_correctly_read()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/foo/bar/test.cs" });
            var command = (MyCommand)result.Last().Command;

            //// Assert
            Assert.Equal("/foo/bar/test.cs", command.First);
        }
    }
}