namespace NConsole
{
    /// <summary>An abstraction of the command line.</summary>
    public interface IConsoleHost
    {
        /// <summary>Writes a message to the console.</summary>
        /// <param name="message">The message.</param>
        void WriteMessage(string message);

        /// <summary>Writes a message to the console with a new line at the end.<summary>
        /// <param name="message">The message.</param>
        void WriteMessageLine(string message);

        /// <summary>Writes an error message.</summary>
        /// <param name="message">The message.</param>
        void WriteError(string message);

        /// <summary>Reads a value from the console.</summary>
        /// <param name="message">The message.</param>
        /// <returns>The value.</returns>
        string ReadValue(string message);
    }
}