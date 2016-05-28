using System;
using System.Linq;
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
            var result = await processor.ProcessAsync(new string[] { "test", "/myenum:def" });
            var command = (MyEnumCommand)result.Last().Command; 

            //// Assert
            Assert.AreEqual(MyEnum.Def, command.MyEnum);
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

        public async Task<object> RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            return null; 
        }
    }
}
