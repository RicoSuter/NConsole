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
        private readonly PropertyInfo _consoleForegroundColorProperty;

        /// <summary>Initializes a new instance of the <see cref="ConsoleHost"/> class.</summary>
        public ConsoleHost()
        {
            _consoleWriteMethod = _consoleType.GetRuntimeMethod("Write", new[] { typeof(string) });
            _consoleReadLineMethod = _consoleType.GetRuntimeMethod("ReadLine", new Type[] { });
            _consoleForegroundColorProperty = _consoleType.GetRuntimeProperty("ForegroundColor");
        }

        /// <summary>Writes a message to the console.</summary>
        /// <param name="message">The message.</param>
        public void WriteMessage(string message)
        {
            _consoleWriteMethod.Invoke(null, new object[] { message });
        }

        /// <summary>Writes an error message to the console.</summary>
        /// <param name="message">The message.</param>
        public void WriteError(string message)
        {
            var color = _consoleForegroundColorProperty.GetValue(null);
            _consoleForegroundColorProperty.SetValue(null, 12); // red
            _consoleWriteMethod.Invoke(null, new object[] { message });
            _consoleForegroundColorProperty.SetValue(null, color);
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