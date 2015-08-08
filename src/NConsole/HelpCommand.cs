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
        /// <summary>Gets or sets the command to show the help for.</summary>
        [Argument(Position = 1, DefaultValue = "")]
        public string Command { get; set; }

        /// <summary>Runs the command.</summary>
        /// <param name="processor">The processor.</param>
        /// <param name="host">The host.</param>
        public Task RunAsync(CommandLineProcessor processor, IConsoleHost host)
        {
            if (string.IsNullOrEmpty(Command))
            {
                foreach (var pair in processor.Commands)
                {
                    PrintCommand(host, pair);
                    host.WriteMessage("\n");
                }
            }
            else if(processor.Commands.ContainsKey(Command))
                PrintCommand(host, processor.Commands.Single(c => c.Key == Command));

            return Task.FromResult(true);
        }

        private void PrintCommand(IConsoleHost host, KeyValuePair<string, Type> pair)
        {
            var commandType = pair.Value;
            host.WriteMessage(pair.Key + "\n");

            var descriptionAttribute = commandType.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
                host.WriteMessage(descriptionAttribute.Description + "\n");

            foreach (var property in commandType.GetRuntimeProperties())
            {
                var parameter = property.Name.ToLowerInvariant();

                host.WriteMessage("  " + parameter + " (" + property.PropertyType.Name + ")" + "\n");

                var parameterDescriptionAttribute = property.GetCustomAttribute<DescriptionAttribute>();
                if (parameterDescriptionAttribute != null)
                    host.WriteMessage("    " + parameterDescriptionAttribute.Description + "\n");
            }
        }
    }
}
