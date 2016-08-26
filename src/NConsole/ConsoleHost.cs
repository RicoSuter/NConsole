using System;
using System.Reflection;

namespace NConsole
{
    /// <summary>A command line host implementation which uses System.Console.</summary>
    public class ConsoleHost : IConsoleHost
    {
        private readonly Type _consoleType = Type.GetType("System.Console", true);
        private readonly MethodInfo _consoleWriteMethod;
        private readonly MethodInfo _consoleReadLineMethod;

        public ConsoleHost()
        {
            _consoleWriteMethod = _consoleType.GetRuntimeMethod("Write", new[] { typeof(string) });
            _consoleReadLineMethod = _consoleType.GetRuntimeMethod("ReadLine", new Type[] { });
        }

        /// <summary>Writes a message to the console.</summary>
        /// <param name="message">The message.</param>
        public void WriteMessage(string message)
        {
            _consoleWriteMethod.Invoke(null, new object[] { message });
        }

        /// <summary>Reads a value from the console.</summary>
        /// <param name="message">The message.</param>
        /// <returns>The value.</returns>
        public string ReadValue(string message)
        {
            _consoleWriteMethod.Invoke(null, new object[] { "\n" + message });
            return (string)_consoleReadLineMethod.Invoke(null, new object[] { });
        }
    }
}