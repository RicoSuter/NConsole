using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NConsole.Tests
{
    [TestClass]
    public class EnumArgumentTests
    {
        [TestMethod]
        public async Task WhenArgumentIsEnumThenItShouldBeLoadedCorrectly()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyEnumCommand>("test");

            //// Act
            await processor.ProcessAsync(new string[] { "test", "/myenum:def" }); 

            //// Assert
            // No exception thrown
        }
    }

    public enum MyEnum
    {
        Abc,
        Def
    }

    public class MyEnumCommand : IConsoleCommand
    {
        [Argument(Name = "MyEnum")]
        public MyEnum MyEnum { get; set; }

        public bool IsCorrect { get; private set; }

        public async Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            if (MyEnum != MyEnum.Def)
                throw new Exception("Wrong enum loaded.");
        }
    }
}
