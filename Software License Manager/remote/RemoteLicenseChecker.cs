using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Software_License_Manager.remote
{
    public static class RemoteLicenseChecker
    {
        private const string ConnectionString = "Server=localhost;Database=SLM;Uid=root;Pwd=;";

        /// <summary>
        /// Prüft, ob der gegebene Lizenzschlüssel und die aktuelle Hardware-ID
        /// in der licenses-Tabelle einen gültigen Eintrag haben.
        /// </summary>
        /// <param name="licenseKey">Der vom Nutzer eingegebene Lizenzschlüssel.</param>
        /// <param name="currentHardwareId">Die aktuelle, ermittelte Hardware-ID (z. B. CPU/Board/Kombi).</param>
        /// <returns>true, wenn gültige Lizenz gefunden, sonst false.</returns>
        public static bool CheckLicense(string licenseKey, string currentHardwareId)
        {
            // Falls nichts übergeben wurde, direkt ungültig
            if (string.IsNullOrWhiteSpace(licenseKey)) return false;

            // Hardware-ID hashen
            string hardwareHash = ComputeSha256(currentHardwareId ?? "");

            // Datenbankabfrage: Lizenz suchen
            //    Annahme: license_key ist eindeutig in der DB
            string sql = @"
            SELECT hardware_id_hash 
            FROM licenses 
            WHERE license_key = @licenseKey
            LIMIT 1";

            using (var connection = new MySqlConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@licenseKey", licenseKey);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            // Kein Eintrag für diesen Lizenzschlüssel
                            return false;
                        }

                        // Aus DB gelesener Wert
                        string storedHardwareHash = reader["hardware_id_hash"] as string;

                        // Wenn kein Hash hinterlegt ist (leerer String), könnte man es als 
                        // "Nicht-Hardware-gebundene Lizenz" interpretieren => direkt true
                        if (string.IsNullOrEmpty(storedHardwareHash))
                        {
                            return true;
                        }

                        //Hashvergleich
                        //Stimmen DB und aktuelle Hardware überein?
                        return string.Equals(storedHardwareHash, hardwareHash, StringComparison.OrdinalIgnoreCase);
                    }
                }
            }
        }

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
