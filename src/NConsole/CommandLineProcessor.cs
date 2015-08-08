using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NConsole
{
    /// <summary>A command base command line processor.</summary>
    public class CommandLineProcessor
    {
        private readonly ICommandLineHost _commandLineHost;
        private readonly Dictionary<string, Type> _commands = new Dictionary<string, Type>();
        private readonly IDependencyResolver _dependencyResolver;

        /// <summary>Initializes a new instance of the <see cref="CommandLineProcessor" /> class.</summary>
        /// <param name="commandLineHost">The command line host.</param>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        public CommandLineProcessor(ICommandLineHost commandLineHost, IDependencyResolver dependencyResolver = null)
        {
            _commandLineHost = commandLineHost;
            _dependencyResolver = dependencyResolver; 

            AddCommand<HelpCommand>("help");
        }

        /// <summary>Gets the list of registered commands.</summary>
        public IReadOnlyDictionary<string, Type> Commands { get { return _commands; } }

        /// <summary>Adds a command.</summary>
        /// <typeparam name="TCommandLineCommand">The type of the command.</typeparam>
        /// <param name="name">The name of the command.</param>
        public void AddCommand<TCommandLineCommand>(string name)
            where TCommandLineCommand : ICommandLineCommand
        {
            AddCommand(name, typeof(TCommandLineCommand));
        }

        /// <summary>Adds a command.</summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <exception cref="InvalidOperationException">The command has already been added.</exception>
        public void AddCommand(string name, Type commandType)
        {
            if (_commands.ContainsKey(name))
                throw new InvalidOperationException("The command '" + name + "' has already been added.");
            _commands.Add(name.ToLowerInvariant(), commandType);
        }

        /// <summary>Processes the command in the given command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <exception cref="InvalidOperationException">No dependency resolver available to create a command without default constructor.</exception>
        public async Task ProcessAsync(string[] args)
        {
            var commandName = GetCommandName(args);
            if (_commands.ContainsKey(commandName))
            {
                var commandType = _commands[commandName];
                var command = CreateCommand(commandType);

                foreach (var property in commandType.GetRuntimeProperties())
                {
                    var argumentAttribute = property.GetCustomAttribute<ArgumentAttributeBase>();
                    if (argumentAttribute != null)
                    {
                        var value = argumentAttribute.Load(_commandLineHost, args, property);
                        property.SetValue(command, value);
                    }
                }

                await command.RunAsync(this, _commandLineHost);
            }
            else
                _commandLineHost.WriteMessage("Command '" + commandName + "' could not be found.\n");
        }

        /// <summary>Gets the name of the command to execute.</summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The command name.</returns>
        protected string GetCommandName(string[] args)
        {
            if (args.Length == 0)
                return _commandLineHost.ReadValue("Command").ToLowerInvariant();
            return args[0].ToLowerInvariant();
        }

        /// <exception cref="InvalidOperationException">No dependency resolver available to create a command without default constructor.</exception>
        private ICommandLineCommand CreateCommand(Type commandType)
        {
            var constructor = commandType.GetConstructors().First();

            if (constructor.GetParameters().Length > 0 && _dependencyResolver == null)
                throw new InvalidOperationException("No dependency resolver available to create a command without default constructor.");

            var parameters = constructor.GetParameters()
                .Select(param => _dependencyResolver.GetService(param.ParameterType))
                .ToArray();

            return (ICommandLineCommand) constructor.Invoke(parameters);
        }
    }
}
