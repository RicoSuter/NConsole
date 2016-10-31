using System;

namespace NConsole
{
    /// <summary>Attribute to define a command class.</summary>
    public class CommandAttribute : Attribute
    {
        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the description.</summary>
        public string Description { get; set; }
    }
}