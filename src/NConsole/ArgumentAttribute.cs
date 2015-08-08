using System;
using System.Linq;
using System.Reflection;

namespace NConsole
{
    public class ArgumentAttribute : ArgumentAttributeBase
    {
        public string Name { get; set; }

        public int Position { get; set; }

        public object Default { get; set; }

        public override object Load(ICommandLineHost commandLineHost, string[] args, PropertyInfo property)
        {
            if (!string.IsNullOrEmpty(Name))
                return LoadNamedArgument(commandLineHost, args, property);
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

                if (index == Position + 1)
                    return argument;

                index++;
            }
            return null;
        }

        private object LoadNamedArgument(ICommandLineHost commandLineHost, string[] args, PropertyInfo property)
        {
            var value = GetParameterValue(args, Name);
            if (value != null)
                return ConvertToType(value, property.PropertyType);
            else
            {
                if (Default != null)
                    return Default;
                else
                    return ConvertToType(commandLineHost.ReadValue(Name), property.PropertyType);
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