using System.Reflection;

namespace NConsole
{
    public class SwitchAttribute : ArgumentAttributeBase
    {
        public string ShortName { get; set; }

        public string LongName { get; set; }

        public override object Load(ICommandLineHost commandLineHost, string[] args, PropertyInfo property)
        {
            foreach (var argument in args)
            {
                if (argument == "-" + ShortName)
                    return true;

                if (argument == "--" + LongName)
                    return true;
            }

            return false; 
        }
    }
}
