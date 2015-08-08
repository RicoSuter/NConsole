using System;
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
            var processor = new CommandLineProcessor(new CommandLineHost(), new MyDependencyResolver());

            //// Act
            processor.AddCommand<MyCommand>("test");

            //// Assert
            Assert.IsTrue(processor.Commands.ContainsKey("test"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void When_adding_command_with_same_name_then_exception_is_thrown()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new CommandLineHost(), new MyDependencyResolver());
            processor.AddCommand<MyCommand>("Test");

            //// Act
            processor.AddCommand<MyCommand>("test"); // exception

            //// Assert
        }

        [TestMethod]
        public void When_adding_command_with_upper_case_then_it_is_converted_to_lower_case()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new CommandLineHost(), new MyDependencyResolver());

            //// Act
            processor.AddCommand<MyCommand>("Test");

            //// Assert
            Assert.IsFalse(processor.Commands.ContainsKey("Test"));
            Assert.IsTrue(processor.Commands.ContainsKey("test"));
        }

        [TestMethod]
        public async Task When_first_argument_is_existing_command_name_then_command_is_executed()
        {
            //// Arrange
            var resolver = new MyDependencyResolver(); 
            var processor = new CommandLineProcessor(new CommandLineHost(), resolver);
            processor.AddCommand<MyCommand>("test");

            //// Act
            await processor.ProcessAsync(new string[] { "test" });

            //// Assert
            Assert.AreEqual(true, resolver.MyState.State);
        }

        [TestMethod]
        public async Task When_running_command_with_parameter_then_they_are_correctly_set()
        {
            //// Arrange
            var resolver = new MyDependencyResolver();
            var processor = new CommandLineProcessor(new CommandLineHost(), resolver);
            processor.AddCommand<MyParameterCommand>("test");

            //// Act
            await processor.ProcessAsync(new string[] { "test", "/uint16:123" });

            //// Assert
            Assert.AreEqual(123.ToString(), resolver.MyState.State.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task When_dependency_resolver_is_missing_and_command_without_default_constructor_then_exception_is_thrown()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new CommandLineHost());
            processor.AddCommand<MyParameterCommand>("test");

            //// Act
            await processor.ProcessAsync(new string[] { "test" });

            //// Assert
        }
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
        public object State { get; set; }
    }

    public class MyCommand : ICommandLineCommand
    {
        private readonly MyState _state;

        public MyCommand(MyState state)
        {
            _state = state; 
        }

        public async Task RunAsync(CommandLineProcessor processor, ICommandLineHost host)
        {
            _state.State = true;
        }
    }

    public class MyParameterCommand : ICommandLineCommand
    {
        private readonly MyState _state;

        public MyParameterCommand(MyState state)
        {
            _state = state;
        }

        [Argument(Name = "UInt16")]
        public UInt16 UInt16 { get; set; }
        //public UInt32 UInt32 { get; set; }
        //public UInt64 UInt64 { get; set; }

        //public Int16 Int16 { get; set; }
        //public Int32 Int32 { get; set; }
        //public Int64 Int64 { get; set; }

        //public decimal Decimal { get; set; }

        //public bool Bool { get; set; }

        //public DateTime DateTime { get; set; }

        public async Task RunAsync(CommandLineProcessor processor, ICommandLineHost host)
        {
            _state.State = UInt16;
        }
    }
}
