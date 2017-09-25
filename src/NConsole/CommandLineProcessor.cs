using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NConsole
{
    /// <summary>A command base command line processor.</summary>
    public class CommandLineProcessor
    {
        private readonly IConsoleHost _consoleHost;
        private readonly Dictionary<string, Type> _commands = new Dictionary<string, Type>();
        private readonly IDependencyResolver _dependencyResolver;

        /// <summary>Initializes a new instance of the <see cref="CommandLineProcessor" /> class.</summary>
        /// <param name="consoleHost">The command line host.</param>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        public CommandLineProcessor(IConsoleHost consoleHost, IDependencyResolver dependencyResolver = null)
        {
            _consoleHost = consoleHost;
            _dependencyResolver = dependencyResolver;

            RegisterCommand<HelpCommand>("help");
        }

        /// <summary>Gets the list of registered commands.</summary>
        public IReadOnlyDictionary<string, Type> Commands => _commands;

        /// <summary>Adds a command.</summary>
        /// <typeparam name="TCommandLineCommand">The type of the command.</typeparam>
        /// <param name="name">The name of the command.</param>
        public void RegisterCommand<TCommandLineCommand>(string name)
            where TCommandLineCommand : IConsoleCommand
        {
            RegisterCommand(name, typeof(TCommandLineCommand));
        }

        /// <summary>Adds a command.</summary>
        /// <typeparam name="TCommandLineCommand">The type of the command.</typeparam>
        public void RegisterCommand<TCommandLineCommand>()
            where TCommandLineCommand : IConsoleCommand
        {
            RegisterCommand(typeof(TCommandLineCommand));
        }

        /// <summary>Loads all commands from an assembly (command classes must have the CommandAttribute with a defined Name).</summary>
        /// <param name="assembly">The assembly.</param>
        public void RegisterCommandsFromAssembly(Assembly assembly)
        {
            var commandTypes = assembly.ExportedTypes.ToDictionary(t => t, t => t.GetTypeInfo().GetCustomAttribute<CommandAttribute>());
            foreach (var pair in commandTypes.Where(p => !string.IsNullOrEmpty(p.Value?.Name) && p.Key.GetTypeInfo().IsClass && !p.Key.GetTypeInfo().IsAbstract))
                RegisterCommand(pair.Value.Name, pair.Key);
        }

        /// <summary>Adds a command.</summary>
        /// <param name="commandType">Type of the command.</param>
        /// <exception cref="InvalidOperationException">The command has already been added.</exception>
        /// <exception cref="InvalidOperationException">The command class is missing the CommandAttribute attribute.</exception>
        public void RegisterCommand(Type commandType)
        {
            var commandAttribute = commandType.GetTypeInfo().GetCustomAttribute<CommandAttribute>(); 
            if (commandAttribute == null)
                throw new InvalidOperationException("The command class is missing the CommandAttribute attribute.");

            RegisterCommand(commandAttribute.Name, commandType);
        }

        /// <summary>Adds a command.</summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <exception cref="InvalidOperationException">The command has already been added.</exception>
        public void RegisterCommand(string name, Type commandType)
        {
            if (_commands.ContainsKey(name))
                throw new InvalidOperationException("The command '" + name + "' has already been added.");

            _commands.Add(name.ToLowerInvariant(), commandType);
        }
        
        /// <summary>Processes the command in the given command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <param name="input">The input for the first command.</param>
        /// <returns>The executed command.</returns>
        /// <exception cref="InvalidOperationException">The command could not be found.</exception>
        /// <exception cref="InvalidOperationException">No dependency resolver available to create a command without default constructor.</exception>
        public async Task<IList<CommandResult>> ProcessAsync(string[] args, object input = null)
        {
            var results = new List<CommandResult>();

            var commands = new List<string[]>();
            var commandArgs = new List<string>();
            foreach (var arg in args)
            {
                if (arg == "=")
                {
                    commands.Add(commandArgs.ToArray());
                    commandArgs = new List<string>();
                }
                else
                    commandArgs.Add(arg);
            }
            commands.Add(commandArgs.ToArray());

            foreach (var command in commands)
            {
                var result = await ProcessSingleAsync(command, results.LastOrDefault()?.Output);
                results.Add(result);
            }

            return results;
        }

        /// <summary>Processes the command in the given command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <param name="input">The input for the command.</param>
        /// <returns>The executed command.</returns>
        /// <exception cref="InvalidOperationException">The command could not be found.</exception>
        /// <exception cref="InvalidOperationException">No dependency resolver available to create a command without default constructor.</exception>
        public async Task<CommandResult> ProcessSingleAsync(string[] args, object input = null)
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
                        var value = argumentAttribute.GetValue(_consoleHost, args, property, command, input);
                        if (value != null)
                            property.SetValue(command, value);
                    }
                }

                var output = await command.RunAsync(this, _consoleHost);
                return new CommandResult
                {
                    Command = command,
                    Output = output
                };
            }
            else
                throw new InvalidOperationException("The command '" + commandName + "' could not be found.");
        }

        /// <summary>Processes the command in the given command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <param name="input">The output from the previous command.</param>
        /// <returns>The exeucuted command.</returns>
        /// <exception cref="InvalidOperationException">The command could not be found.</exception>
        /// <exception cref="InvalidOperationException">No dependency resolver available to create a command without default constructor.</exception>
        public IList<CommandResult> Process(string[] args, object input = null)
        {
            return ProcessAsync(args, input).GetAwaiter().GetResult();
        }

        /// <summary>Processes the command in the given command line arguments.</summary>
        /// <param name="args">The arguments.</param>
        /// <param name="input">The output from the previous command.</param>
        /// <returns>The exeucuted command.</returns>
        public IList<CommandResult> ProcessWithExceptionHandling(string[] args, object input = null)
        {
            try
            {
                return ProcessAsync(args, input).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                _consoleHost.WriteError(e.ToString());
                return null;
            }
        }

        /// <summary>Gets the name of the command to execute.</summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The command name.</returns>
        protected string GetCommandName(string[] args)
        {
            if (args.Length == 0)
            {
                _consoleHost.WriteMessage("Commands: \n");
                foreach (var command in Commands)
                    _consoleHost.WriteMessage("  " + command.Key + "\n");
                
                return _consoleHost.ReadValue("Command: ").ToLowerInvariant();
            }

            return args[0].ToLowerInvariant();
        }

        /// <exception cref="InvalidOperationException">No dependency resolver available to create a command without default constructor.</exception>
        private IConsoleCommand CreateCommand(Type commandType)
        {
            var constructors = commandType.GetTypeInfo().DeclaredConstructors;
            IConsoleCommand command;

            if (constructors.Any())
            {
                var constructor = constructors.First();

                if (constructor.GetParameters().Length > 0 && _dependencyResolver == null)
                    throw new InvalidOperationException("No dependency resolver available to create a command without default constructor.");

                var parameters = constructor.GetParameters()
                    .Select(param => _dependencyResolver.GetService(param.ParameterType))
                    .ToArray();

                command = (IConsoleCommand)constructor.Invoke(parameters);
            }
            else
            {
                if (_dependencyResolver == null)
                {
                    throw new InvalidOperationException($"Cannot create an instance of {commandType} because it does not " +
                                                        $"have any accessible constructors and no dependency resolver is available.");
                }

                command = (IConsoleCommand)_dependencyResolver.GetService(commandType);
            }

            return command;
        }
    }
}