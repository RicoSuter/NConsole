using System;
using System.Linq;
using System.Reflection;

namespace NConsole
{
    /// <summary>Attribute to define a command line argument.</summary>
    public class ArgumentAttribute : ArgumentAttributeBase
    {
        /// <summary>Gets or sets the argument name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the position of the unnamed argument.</summary>
        public int Position { get; set; }

        /// <summary>Gets or sets the default value of the argument. 
        /// Setting a default value makes the argument optional.</summary>
        public object DefaultValue { get; set; }

        /// <summary>Gets the argument value.</summary>
        /// <param name="consoleHost">The command line host.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="property">The property.</param>
        /// <returns>The value.</returns>
        /// <exception cref="System.InvalidOperationException">Either the argument Name or Position can be set, but not both.</exception>
        /// <exception cref="InvalidOperationException">Either the argument Name or Position can be set, but not both.</exception>
        public override object GetValue(IConsoleHost consoleHost, string[] args, PropertyInfo property)
        {
            if (!string.IsNullOrEmpty(Name) && Position > 0)
                throw new InvalidOperationException("Either the argument Name or Position can be set, but not both.");

            if (!string.IsNullOrEmpty(Name))
                return LoadNamedArgument(consoleHost, args, property);
            else
                return LoadPositionalArgument(args);
        }

        private object LoadPositionalArgument(string[] args)
        {
            var index = 0;
            foreach (var argument in args)
            {
                if (argument.StartsWith("/") || argument.StartsWith("-") || argument.StartsWith("--"))
                    continue;

                if (index == Position)
                    return argument;

                index++;
            }
            return null;
        }

        private object LoadNamedArgument(IConsoleHost consoleHost, string[] args, PropertyInfo property)
        {
            var value = GetParameterValue(args, Name);
            if (value != null)
                return ConvertToType(value, property.PropertyType);
            else
            {
                if (DefaultValue != null)
                    return DefaultValue;
                else
                    return ConvertToType(consoleHost.ReadValue(Name), property.PropertyType);
            }
        }

        private string GetParameterValue(string[] args, string name)
        {
            var arg = args.FirstOrDefault(a => a.ToLowerInvariant().StartsWith("/" + name.ToLowerInvariant() + ":"));
            if (arg != null)
                return arg.Substring(arg.IndexOf(":", StringComparison.InvariantCulture) + 1);
            return null;
        }
    }
}