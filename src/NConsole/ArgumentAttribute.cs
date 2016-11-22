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

        /// <summary>Gets or sets the argument description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the position of the unnamed argument.</summary>
        public int Position { get; set; }

        /// <summary>Gets or sets a value indicating whether the argument is required (default: true).</summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>Gets or sets a value indicating whether the argument accepts an input from a previous command (default: false).</summary>
        public bool AcceptsCommandInput { get; set; } = false;

        /// <summary>Gets the argument value.</summary>
        /// <param name="consoleHost">The command line host.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="property">The property.</param>
        /// <param name="command">The command.</param>
        /// <param name="input">The output from the previous command in the chain.</param>
        /// <returns>The value.</returns>
        /// <exception cref="System.InvalidOperationException">Either the argument Name or Position can be set, but not both.</exception>
        /// <exception cref="InvalidOperationException">Either the argument Name or Position can be set, but not both.</exception>
        /// <exception cref="InvalidOperationException">The parameter has no default value.</exception>
        public override object GetValue(IConsoleHost consoleHost, string[] args, PropertyInfo property, IConsoleCommand command, object input)
        {
            if (!string.IsNullOrEmpty(Name) && Position > 0)
                throw new InvalidOperationException("Either the argument Name or Position can be set, but not both.");

            string value = null;

            if (TryGetPositionalArgumentValue(args, out value))
                return ConvertToType(value, property.PropertyType);

            if (TryGetNamedArgumentValue(args, out value))
                return ConvertToType(value, property.PropertyType);

            if (AcceptsCommandInput && input != null)
                return input;

            if (!IsInteractiveMode(args) && !IsRequired)
                return property.CanRead ? property.GetValue(command) : null;

            value = consoleHost.ReadValue(GetFullParameterDescription(property, command));
            if (value == "[default]")
            {
                if (!IsRequired)
                    return property.CanRead ? property.GetValue(command) : null;

                throw new InvalidOperationException("The parameter '" + Name + "' is required.");
            }

            return ConvertToType(value, property.PropertyType);
        }

        private bool IsInteractiveMode(string[] args)
        {
            return args.Length == 0;
        }

        private bool TryGetPositionalArgumentValue(string[] args, out string value)
        {
            if (Position > 0)
            {
                var index = 0;
                foreach (var argument in args)
                {
                    if (argument.StartsWith("/") || argument.StartsWith("-") || argument.StartsWith("--"))
                        continue;

                    if (index == Position)
                    {
                        value = argument;
                        return true;
                    }

                    index++;
                }
            }

            value = null;
            return false;
        }

        private bool TryGetNamedArgumentValue(string[] args, out string value)
        {
            value = null;

            if (string.IsNullOrEmpty(Name))
                return false;

            var arg = args.FirstOrDefault(a => a.ToLowerInvariant().StartsWith("/" + Name.ToLowerInvariant() + ":"));
            if (arg != null)
            {
                value = arg.Substring(arg.IndexOf(":", StringComparison.Ordinal) + 1);
                return true;
            }

            return false;
        }

        private string GetFullParameterDescription(PropertyInfo property, IConsoleCommand command)
        {
            var name = Name ?? property.Name;

            if (IsRequired == false)
                name = "Type [default] to use default value: \"" + property.GetValue(command) + "\"\n" + name;

            if (!string.IsNullOrEmpty(Description))
                name = Description + "\n" + name;
            else
            {
                dynamic displayAttribute = property.GetCustomAttributes().SingleOrDefault(a => a.GetType().Name == "DisplayAttribute");
                if (displayAttribute != null && !string.IsNullOrEmpty(displayAttribute.Description))
                    name = displayAttribute.Description + "\n" + name;
                else
                {
                    dynamic descriptionAttribute = property.GetCustomAttributes().SingleOrDefault(a => a.GetType().Name == "DescriptionAttribute");
                    if (descriptionAttribute != null)
                        name = descriptionAttribute.Description + "\n" + name;
                }
            }

            return name + ": ";
        }
    }
}