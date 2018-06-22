using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NConsole.Tests
{
    public class EnumArgumentTests
    {
        [Fact]
        public async Task WhenArgumentIsEnumThenItShouldBeLoadedCorrectly()
        {
            //// Arrange
            var processor = new CommandLineProcessor(new ConsoleHost());
            processor.RegisterCommand<MyEnumCommand>("test");

            //// Act
            var result = await processor.ProcessAsync(new[] { "test", "/myenum:def" });
            var command = (MyEnumCommand)result.Last().Command; 

            //// Assert
            Assert.Equal(MyEnum.Def, command.MyEnum);
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
