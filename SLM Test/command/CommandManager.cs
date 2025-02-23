using Software_License_Manager;
using Software_License_Manager.local;
using Software_License_Manager.remote;

namespace SLM_Test.command
{
    public class CommandManager
    {

        private static Dictionary<string, Command> _commands = new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);

        public CommandManager()
        {
            RegisterCommands();
        }

        public Dictionary<string, Command> GetCommands()
        {
            return _commands;
        }

        private static void RegisterCommands()
        {
            //Der "help"-Befehl
            _commands["help"] = new Command(
                "help",
                "Zeigt eine Liste aller verfügbaren Befehle an.",
                args =>
                {
                    Console.WriteLine("Verfügbare Befehle:");
                    foreach (var cmd in _commands.Values)
                    {
                        Console.WriteLine($"{cmd.Name} - {cmd.Description}");
                    }
                }
            );

            _commands["hw"] = new Command(
                "hw",
                "Hardware befehle. Nutzung: hw <id/info>",
                args =>
                {
                    if (args.Length == 0)
                    {
                        Console.WriteLine("Bitte ein argument angeben");
                        return;
                    }
                    if (args[0] == "id")
                    {
                        string id = HardwareId.GetCombinedHardwareId();
                        Console.WriteLine($"Hardware ID: '{id}'");
                    }
                    if (args[0] == "info")
                    {
                        HardwareId.PrintDetails();
                    }
                }
            );

            _commands["license"] = new Command(
                "license",
                "license befehle. Nutzung: license <vorname> <nachname>",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Bitte zwei argumente angeben");
                        return;
                    }
                    string vorname = args[0];
                    string nachname = args[1];
                    Console.WriteLine(LocalLicenseGenerator.GenerateSerialKey(HardwareId.GetCombinedHardwareId(), vorname, nachname));
                }
            );

            _commands["rlicense"] = new Command(
                "rlicense",
                "rlicense befehle. Nutzung: rlicense <vorname> <nachname>",
                args =>
                {
                    if (args.Length < 2)
                    {
                        Console.WriteLine("Bitte zwei argumente angeben");
                        return;
                    }
                    string vorname = args[0];
                    string nachname = args[1];
                    Console.WriteLine(RemoteLicenseGenerator.GenerateAndStoreLicense(HardwareId.GetCombinedHardwareId(), vorname, nachname));
                }
            );



            //Befehl "exit"
            _commands["exit"] = new Command(
                "exit",
                "Beendet das Programm.",
                args =>
                {
                    
                }
            );

            //Befehl "quit"
            _commands["quit"] = new Command(
                "quit",
                "Beendet das Programm (Alternative zu exit).",
                args => { }
            );

        }

    }
}
