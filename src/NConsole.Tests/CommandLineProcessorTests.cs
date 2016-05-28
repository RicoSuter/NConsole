using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NConsole.Tests
{
    [TestClass]
    public class CommandLineProcessorTests
    {
        [TestMethod]
        public void When_adding_a_command_then_the_command_is_in_the_commands_list()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost(), new MyDependencyResolver());

            //// Act
            processor.RegisterCommand<MyCommand>("test");

            //// Assert
            Assert.IsTrue(processor.Commands.ContainsKey("test"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void When_adding_command_with_same_name_then_exception_is_thrown()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost(), new MyDependencyResolver());
            processor.RegisterCommand<MyCommand>("Test");

            //// Act
            processor.RegisterCommand<MyCommand>("test"); // exception

            //// Assert
        }

        [TestMethod]
        public void When_adding_command_with_upper_case_then_it_is_converted_to_lower_case()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost(), new MyDependencyResolver());

            //// Act
            processor.RegisterCommand<MyCommand>("Test");

            //// Assert
            Assert.IsFalse(processor.Commands.ContainsKey("Test"));
            Assert.IsTrue(processor.Commands.ContainsKey("test"));
        }

        [TestMethod]
        public void When_first_argument_is_existing_command_name_then_command_is_executed()
        {
            //// Arrange
            var resolver = new MyDependencyResolver();
            var processor = new CommandLineProcessor(new ConsoleHost(), resolver);
            processor.RegisterCommand<MyCommand>("test");

            //// Act
            var result = processor.Process(new string[] { "test" });
            var command = (MyCommand)result.Last().Command;

            //// Assert
            Assert.IsNotNull(command);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When_dependency_resolver_is_missing_and_command_without_default_constructor_then_exception_is_thrown()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyCommand>("test");

            //// Act
            await processor.ProcessAsync(new string[] { "test" }); // exception

            //// Assert
        }

        public class MyDependencyResolver : IDependencyResolver
        {
            public MyDependencyResolver()
            {
                MyState = new MyState();
            }

            public MyState MyState { get; set; }

            public object GetService(Type serviceType)
            {
                return MyState;
            }
        }

        public class MyState
        {
        }

        public class MyCommand : IConsoleCommand
        {
            private readonly MyState _state;

            public MyCommand(MyState state)
            {
                _state = state;
            }

            public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
            {
                return null;
            }
        }
    }
}
