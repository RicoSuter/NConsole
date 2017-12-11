using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NConsole.Tests
{
    [TestClass]
    public class UnexpectedArgumentsTests
    {
        [TestMethod]
        public async Task When_running_command_with_full_datetime_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            ////Act
            var result = await processor.ProcessAsync(new string[] { "test", "first", "/datetime:2014-5-3 12:13:14", "/TestSwitch" });
            var command = (MyArgumentCommand)result.Last().Command;

            //// Assert
            Assert.AreEqual(14, command.DateTime.Second);
            Assert.AreEqual(13, command.DateTime.Minute);
            Assert.AreEqual(12, command.DateTime.Hour);
            Assert.AreEqual(3, command.DateTime.Day);
            Assert.AreEqual(5, command.DateTime.Month);
            Assert.AreEqual(2014, command.DateTime.Year);
        }

        [TestMethod]
        public async Task When_running_command_with_incorrect_full_datetime_parameter_then_exception_is_thrown()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            ////Act
            try
            {
                var result = await processor.ProcessAsync(new string[] { "test", "first", "/datetime:2014-5-3", "12:13:14", "/TestSwitch" });
                var command = (MyArgumentCommand)result.Last().Command;
                Assert.Fail();
            }
            catch (UnusedArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "Unrecognised arguments are present.");
            }
        }

        [TestMethod]
        public async Task When_running_command_with_unexpected_arguments_then_exception_is_thrown()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyArgumentCommand>("test");

            ////Act
            try
            {
                var result = await processor.ProcessAsync(new string[] { "test", "first", "second", "third" });
                var command = (MyArgumentCommand)result.Last().Command;
                Assert.Fail();
            }
            catch (UnusedArgumentException ex)
            {
                Assert.AreEqual(ex.Message, "Unrecognised arguments are present.");
            }
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