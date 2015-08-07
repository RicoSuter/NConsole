using System.Threading.Tasks;

namespace NConsole
{
    /// <summary>A command line command.</summary>
    public interface ICommandLineCommand
    {
        /// <summary>Runs the command.</summary>
        /// <param name="processor">The processor.</param>
        /// <param name="host">The host.</param>
        Task RunAsync(CommandLineProcessor processor, ICommandLineHost host);
    }
}