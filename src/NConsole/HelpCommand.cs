using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NConsole
{
    /// <summary>The help command to show the availble list of commands.</summary>
    public class HelpCommand : IConsoleCommand
    {
        /// <summary>Runs the command.</summary>
        /// <param name="processor">The processor.</param>
        /// <param name="host">The host.</param>
        public Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            foreach (var pair in processor.Commands)
            {
                if (pair.Key != "help")
                {
                    PrintCommand(host, pair);
                    host.ReadValue("Press <enter> key for next command...");
                }
            }

            return Task.FromResult(true);
        }

        private void PrintCommand(IConsoleHost host, KeyValuePair<string, Type> pair)
        {
            var commandType = pair.Value;

            host.WriteMessage("---------------------\n");
            host.WriteMessage("Command: ");
            host.WriteMessage(pair.Key + "\n");

            var descriptionAttribute = commandType.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
                host.WriteMessage("  " + descriptionAttribute.Description + "\n");

            host.WriteMessage("\nArguments: \n");
            foreach (var property in commandType.GetRuntimeProperties())
            {
                var argumentAttribute = property.GetCustomAttribute<ArgumentAttribute>();
                if (argumentAttribute != null)
                {
                    if (argumentAttribute.Position > 0)
                        host.WriteMessage("  Argument Position " + argumentAttribute.Position + "\n");
                    else
                        host.WriteMessage("  " + argumentAttribute.Name.ToLowerInvariant() + "\n");

                    var parameterDescriptionAttribute = property.GetCustomAttribute<DescriptionAttribute>();
                    if (parameterDescriptionAttribute != null)
                        host.WriteMessage("    " + parameterDescriptionAttribute.Description + "\n");
                }
            }
        }
    }
}
