using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NConsole.Tests
{
    public class NamedArgumentTests
    {
        [Fact]
        public async Task When_running_command_with_uint16_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/uint16:123" });
            var command = (MyArgumentCommand)result.Last().Command; 

            //// Assert
            Assert.True(123 == command.UInt16);
        }

        [Fact]
        public async Task When_running_command_with_uint32_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/uint32:123" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.True(123 == command.UInt32);
        }

        [Fact]
        public async Task When_running_command_with_uint64_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/uint64:123" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.True(123 == command.UInt64);
        }

        [Fact]
        public async Task When_running_command_with_int16_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/int16:123" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.True(123 == command.Int16);
        }

        [Fact]
        public async Task When_running_command_with_int32_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/int32:123" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.True(123 == command.Int32);
        }

        [Fact]
        public async Task When_running_command_with_int64_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/int64:123" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.True(123 == command.Int64);
        }

        [Fact]
        public async Task When_running_command_with_decimal_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/decimal:123" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.True(123 == command.Decimal);
        }

        [Fact]
        public async Task When_running_command_with_boolean_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/boolean:true" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.True(command.Boolean);
        }

        [Fact]
        public async Task When_running_command_with_datetime_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/datetime:2014-5-3" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.Equal(3, command.DateTime.Day);
            Assert.Equal(5, command.DateTime.Month);
            Assert.Equal(2014, command.DateTime.Year);
        }

        [Fact]
        public async Task When_running_command_with_string_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/string:abc" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.Equal("abc", command.String);
        }

        [Fact]
        public async Task When_running_command_with_quoted_string_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/string:abc def" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.Equal("abc def", command.String);
        }

        public class MyArgumentCommand : IConsoleCommand
        {
            [Argument(Name = "UInt16", IsRequired = false)]
            public UInt16 UInt16 { get; set; } = 0;

            [Argument(Name = "UInt32", IsRequired = false)]
            public UInt32 UInt32 { get; set; } = 0;

            [Argument(Name = "UInt64", IsRequired = false)]
            public UInt64 UInt64 { get; set; } = 0;


            [Argument(Name = "Int16", IsRequired = false)]
            public Int16 Int16 { get; set; } = 0;

            [Argument(Name = "Int32", IsRequired = false)]
            public Int32 Int32 { get; set; } = 0;

            [Argument(Name = "Int64", IsRequired = false)]
            public Int64 Int64 { get; set; } = 0;


            [Argument(Name = "Decimal", IsRequired = false)]
            public Decimal Decimal { get; set; } = 0;

            [Argument(Name = "Boolean", IsRequired = false)]
            public Boolean Boolean { get; set; } = false;

            [Argument(Name = "DateTime", IsRequired = false)]
            public DateTime DateTime { get; set; } = DateTime.Parse("2015-1-1");


            [Argument(Name = "String", IsRequired = false)]
            public string String { get; set; } = "";

            public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                return null; 
            }
        }
    }
}
