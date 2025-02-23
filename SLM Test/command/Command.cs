using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLM_Test.command
{
    public class Command
    {
        public string Name { get; }
        public string Description { get; }
        // Action<string[]> enthält die Argumente, die nach dem Befehl eingegeben wurden
        public Action<string[]> Execute { get; }

        public Command(string name, string description, Action<string[]> execute)
        {
            Name = name;
            Description = description;
            Execute = execute;
        }
    }
}
