using SLM_Test.command;
using Software_License_Manager;

namespace SLM_Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            CommandManager commandManager = new CommandManager();

            Console.WriteLine("Willkommen zur Befehls-Konsole!");
            Console.WriteLine("Gib 'help' ein, um verfügbare Befehle zu sehen.");

            bool exitRequested = false;
            while (!exitRequested)
            {
                Console.Write("\n> ");
                string input = Console.ReadLine()?.Trim() ?? "";

                if (string.IsNullOrEmpty(input))
                    continue;

                // Befehl + Argumente aufsplitten
                string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                string commandName = parts[0];
                string[] commandArgs = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

                // Prüfen, ob der Befehl existiert
                if (commandManager.GetCommands().ContainsKey(commandName))
                {
                    // Ausführen
                    // Falls du async benötigst, könntest du hier Task.Run() oder async/await einsetzen
                    commandManager.GetCommands()[commandName].Execute(commandArgs);
                }
                else
                {
                    Console.WriteLine($"Unbekannter Befehl: {commandName}. Tippe 'help' für Hilfe.");
                }

                if (commandName.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    commandName.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    exitRequested = true;
                }
            }
        }
    }
}
