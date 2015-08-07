using System;

namespace NConsole
{
    /// <summary>A command line host implementation which uses System.Console.</summary>
    public class CommandLineHost : ICommandLineHost
    {
        /// <summary>Writes a message to the console.</summary>
        /// <param name="message">The message.</param>
        public void WriteMessage(string message)
        {
            Console.Write(message);
        }

        /// <summary>Reads a value from the console.</summary>
        /// <param name="message">The message.</param>
        /// <returns>The value.</returns>
        public string ReadValue(string message)
        {
            Console.Write(message + ": ");
            return Console.ReadLine();
        }
    }
}