using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NConsole.Tests
{
    public class UnexpectedArgumentsTests
    {
        [Fact]
        public async Task When_running_command_with_full_datetime_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            ////Act
            var result = await processor.ProcessAsync(new[] { "test", "first", "/datetime:2014-5-3 12:13:14", "/TestSwitch" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.Equal(14, command.DateTime.Second);
            Assert.Equal(13, command.DateTime.Minute);
            Assert.Equal(12, command.DateTime.Hour);
            Assert.Equal(3, command.DateTime.Day);
            Assert.Equal(5, command.DateTime.Month);
            Assert.Equal(2014, command.DateTime.Year);
        }

        [Fact]
        public async Task When_running_command_with_incorrect_full_datetime_parameter_then_exception_is_thrown()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            ////Act
            async Task ProcessAsyncThrowsException()
            {
                var result = await processor.ProcessAsync(new [] { "test", "first", "/datetime:2014-5-3", "12:13:14", "/TestSwitch" });
                var command = (MyArgumentCommand)result.Last().Command;
            }

            //// Assert
            var exception = await Assert.ThrowsAsync<UnusedArgumentException>(async () => await ProcessAsyncThrowsException());
            Assert.Equal("Unrecognised arguments are present: [Used arguments (3) != Provided arguments (4) -> Check [12:13:14]]", exception.Message);
        }

        [Fact]
        public async Task When_running_command_with_unexpected_arguments_then_exception_is_thrown()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            async Task ProcessAsyncThrowsException()
            {
                var result = await processor.ProcessAsync(new [] { "test", "first", "second", "/third:third", "/Fourth" });
                var command = (MyArgumentCommand)result.Last().Command;
            }

            //// Assert
            var exception = await Assert.ThrowsAsync<UnusedArgumentException>(async () => await ProcessAsyncThrowsException());
            Assert.Equal("Unrecognised arguments are present: [Used arguments (1) != Provided arguments (4) -> Check [second, /third:third, /Fourth]]", exception.Message);
        }

        public class MyArgumentCommand : IConsoleCommand
        {
            [Argument(Name = "DateTime", IsRequired = false)]
            public DateTime DateTime { get; set; } = DateTime.Parse("2015-1-1");

            [Argument(Position = 1, IsRequired = false)]
            public string First { get; set; }

            [Switch(LongName = "TestSwitch", ShortName = "t")]
            public bool Switch { get; set; }

            public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                return null;
            }
        }
    }
}