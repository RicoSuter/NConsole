namespace NConsole
{
    /// <summary>An abstraction of the command line.</summary>
    public interface ICommandLineHost
    {
        /// <summary>Writes a message to the console.</summary>
        /// <param name="message">The message.</param>
        void WriteMessage(string message);

        /// <summary>Reads a value from the console.</summary>
        /// <param name="message">The message.</param>
        /// <returns>The value.</returns>
        string ReadValue(string message);
    }
}