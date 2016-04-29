using System;
using System.Collections;
using System.ComponentModel;
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
        /// <exception cref="InvalidOperationException">The parameter has no default value.</exception>
        public override object GetValue(IConsoleHost consoleHost, string[] args, PropertyInfo property)
        {
            if (!string.IsNullOrEmpty(Name) && Position > 0)
                throw new InvalidOperationException("Either the argument Name or Position can be set, but not both.");

            var value = GetPositionalArgumentValue(args);

            if (value == null)
                value = GetNamedArgumentValue(args, Name);

            if (value != null)
                return ConvertToType(value.ToString(), property.PropertyType);

            if (!IsInteractiveMode(args) && DefaultValue != null)
            {
                var defaultValueAsString = DefaultValue is IEnumerable
                    ? string.Join(",", ((IEnumerable) DefaultValue).OfType<object>().Select(o => o.ToString()))
                    : DefaultValue.ToString();

                return ConvertToType(defaultValueAsString, property.PropertyType);
            }

            var stringVal = consoleHost.ReadValue(GetFullParameterDescription(property));
            if (stringVal == "[default]")
            {
                if (DefaultValue != null)
                    return ConvertToType(DefaultValue.ToString(), property.PropertyType);

                throw new InvalidOperationException("The parameter has no default value.");
            }

            return ConvertToType(stringVal, property.PropertyType);
        }

        private bool IsInteractiveMode(string[] args)
        {
            return args.Length == 0;
        }

        private object GetPositionalArgumentValue(string[] args)
        {
            if (Position > 0)
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
            }

            return null; 
        }

        private string GetNamedArgumentValue(string[] args, string name)
        {
            var arg = args.FirstOrDefault(a => a.ToLowerInvariant().StartsWith("/" + name.ToLowerInvariant() + ":"));
            if (arg != null)
                return arg.Substring(arg.IndexOf(":", StringComparison.InvariantCulture) + 1);
            return null;
        }

        private string GetFullParameterDescription(PropertyInfo property)
        {
            var name = Name ?? property.Name;

            if (DefaultValue != null)
                name = "Type [default] to use default value: \"" + DefaultValue + "\"\n" + name; 

            var descriptionAttribute = property.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute != null)
                name = descriptionAttribute.Description + "\n" + name;

            return name + ": ";
        }
    }
}