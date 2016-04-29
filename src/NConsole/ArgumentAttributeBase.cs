using System;
using System.Linq;
using System.Reflection;

namespace NConsole
{
    /// <summary>The argument attribute base class.</summary>
    public abstract class ArgumentAttributeBase : Attribute
    {
        /// <summary>Gets the argument value.</summary>
        /// <param name="consoleHost">The command line host.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="property">The property.</param>
        /// <returns>The value.</returns>
        public abstract object GetValue(IConsoleHost consoleHost, string[] args, PropertyInfo property);

        /// <summary>Converts a string value to a specific type.</summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The type.</param>
        /// <returns>The value.</returns>
        protected object ConvertToType(string value, Type type)
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

            if (type.IsEnum)
                return Enum.Parse(type, value, true);

            if (type == typeof(string[]))
                return !string.IsNullOrEmpty(value) ? value.Split(',') : new string[] { };

            return value;
        }
    }
}