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
        private IDependencyResolver _dependencyResolver;

        /// <summary>Initializes a new instance of the <see cref="CommandLineProcessor" /> class.</summary>
        /// <param name="commandLineHost">The command line host.</param>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        public CommandLineProcessor(ICommandLineHost commandLineHost, IDependencyResolver dependencyResolver)
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
        public async Task ProcessAsync(string[] args)
        {
            var commandName = GetCommandName(args);
            if (_commands.ContainsKey(commandName))
            {
                var commandType = _commands[commandName];
                var command = CreateCommand(commandType);

                foreach (var property in commandType.GetRuntimeProperties())
                {
                    var parameter = property.Name.ToLowerInvariant();
                    var value = GetParameterValue(args, property, parameter);
                    property.SetValue(command, value);
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

        /// <summary>Gets the parameter value for the given key.</summary>
        /// <param name="args">The arguments.</param>
        /// <param name="key">The key.</param>
        /// <returns>The parameter value.</returns>
        protected virtual string GetParameterValue(string[] args, string key)
        {
            var arg = args.FirstOrDefault(a => a.StartsWith("/" + key + ":"));
            if (arg != null)
                return arg.Substring(arg.IndexOf(":", StringComparison.InvariantCulture) + 1);
            return null;
        }

        private ICommandLineCommand CreateCommand(Type commandType)
        {
            var constructor = commandType.GetConstructors().First();

            var parameters = constructor.GetParameters()
                .Select(param => _dependencyResolver.GetService(param.ParameterType))
                .ToArray();

            return (ICommandLineCommand) constructor.Invoke(parameters);
        }

        private object GetParameterValue(string[] args, PropertyInfo property, string key)
        {
            var value = GetParameterValue(args, key);
            if (value != null)
                return ConvertToType(value, property.PropertyType);
            else
            {
                var defaultValueAttribute = property.GetCustomAttribute<DefaultValueAttribute>();
                if (defaultValueAttribute != null)
                    return defaultValueAttribute.Value;
                else
                    return ConvertToType(_commandLineHost.ReadValue(key), property.PropertyType);
            }
        }

        private object ConvertToType(string value, Type type)
        {
            if (type == typeof(Int16))
                return Int16.Parse(value);
            if (type == typeof(Int32))
                return Int32.Parse(value);
            if (type == typeof(Int64))
                return Int64.Parse(value);

            if (type == typeof(UInt16))
                return UInt16.Parse(value);
            if (type == typeof(UInt32))
                return UInt32.Parse(value);
            if (type == typeof(UInt64))
                return UInt64.Parse(value);

            if (type == typeof(Decimal))
                return Decimal.Parse(value);

            if (type == typeof(Boolean))
                return Boolean.Parse(value);

            if (type == typeof(DateTime))
                return DateTime.Parse(value);

            return value;
        }
    }
}
