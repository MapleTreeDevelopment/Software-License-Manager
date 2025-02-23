using System;
using System.Security.Cryptography;
using System.Text;
using MySql.Data.MySqlClient;

namespace Software_License_Manager.remote
{

    public static class RemoteLicenseGenerator
    {

        private const string ConnectionString = "Server=localhost;Database=SLM;Uid=root;Pwd=;";

        /// <summary>
        /// Erstellt einen Lizenzschlüssel und schreibt ihn zusammen mit der
        /// Hardware-ID und dem Benutzernamen (Vorname + Nachname) in die Datenbank.
        /// </summary>
        /// <param name="hardwareId">
        /// Die eindeutige Hardware-ID des Zielrechners. Wenn du sie nicht festkoppeln willst,
        /// kannst du einen leeren String übergeben oder in der DB "hardware_id_hash" leer lassen.
        /// </param>
        /// <param name="firstName">Vorname</param>
        /// <param name="lastName">Nachname</param>
        /// <returns>Der neu erzeugte Lizenzschlüssel, den du dem Nutzer geben kannst.</returns>
        public static string GenerateAndStoreLicense(string hardwareId, string firstName, string lastName)
        {
            //Benutzername kombinieren
            string userName = $"{firstName} {lastName}".Trim();

            // Serial Key erzeugen
            string licenseKey = GenerateSimpleSerialKey();

            // Hardware-ID in SHA-256 hashen
            string hardwareIdHash = string.IsNullOrEmpty(hardwareId)
                ? ""
                : ComputeSha256(hardwareId);

            // In die MySQL-Datenbank eintragen
            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();

                string sql = @"
                INSERT INTO licenses (license_key, hardware_id_hash, user_name, created_at)
                VALUES (@licenseKey, @hwIdHash, @userName, @createdAt)";

                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@licenseKey", licenseKey);
                    cmd.Parameters.AddWithValue("@hwIdHash", hardwareIdHash);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@createdAt", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }

            //Den erzeugten Lizenzschlüssel zurückgeben
            return licenseKey;
        }

        /// <summary>
        /// Erzeugt einen sehr einfachen (zufälligen) "Serial Key" (z. B. 25 Zeichen A-Z/0-9 in 5er-Blöcken).
        /// In der Praxis evtl. ersetzen durch dein eigenes Format oder
        /// die bereits gezeigte Base32-Methode.
        /// </summary>
        private static string GenerateSimpleSerialKey()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int length = 25; // Gesamtlänge
            var random = new Random();

            // Einfach 25 zufällige Zeichen
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }

            // In 5er-Blöcken mit '-'
            // "ABCDE-FGHIJ-KLMNO-PQRST-UVWXY"
            for (int i = 5; i < sb.Length; i += 6)
            {
                sb.Insert(i, '-');
            }
            return sb.ToString();
        }

        /// <summary>
        /// SHA-256-Hash einer Zeichenkette. Ausgegeben als Hexstring in Kleinbuchstaben.
        /// </summary>
        private static string ComputeSha256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
