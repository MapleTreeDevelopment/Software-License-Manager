using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Software_License_Manager.local
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class LocalLicenseGenerator
    {
        /// <summary>
        /// Erzeugt aus Hardware-ID, Vorname und Nachname einen (einfachen) Serial Key.
        /// </summary>
        public static string GenerateSerialKey(string hardwareId, string firstName, string lastName)
        {
            //Kombiniere Name und Hardware-ID
            string combined = $"{firstName} {lastName}|{hardwareId}";

            //Hash bilden (SHA-256)
            byte[] hashBytes = ComputeSha256(combined);

            //Serial Key formatieren (z. B. in Blöcken zu 5 Zeichen Base32)
            string base32 = ToBase32String(hashBytes);
            // z. B. "ABCDEFGHIJKLMNOPQRSTUVWXYZ12345"

            // In Blöcke trennen, z. B. 5er-Gruppen
            // "ABCDE-FGHIJ-KLMNO-PQRST-UVWXY-12345"
            string serialKey = ChunkString(base32, 5, '-');

            if (serialKey.Length > 6 * 6 - 1) // 6 Blöcke a 5 Zeichen + 5 Trenner => 35 + 5 = 41
            {
                serialKey = serialKey.Substring(0, 6 * 6 - 1);
                // ggf. sauberer: auf trennstellen achten
            }

            return serialKey;
        }

        private static byte[] ComputeSha256(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            }
        }

        /// <summary>
        /// Wandelt die Bytes in einen Base32-String um (ohne Sonderzeichen, nur A-Z + 2-7).
        /// </summary>
        private static string ToBase32String(byte[] data)
        {
            const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            // 32 Zeichen

            int bits = 0;
            int value = 0;
            StringBuilder sb = new StringBuilder();

            foreach (byte b in data)
            {
                value = value << 8 | b;
                bits += 8;
                while (bits >= 5)
                {
                    sb.Append(alphabet[value >> bits - 5 & 0x1F]);
                    bits -= 5;
                }
            }
            if (bits > 0)
            {
                sb.Append(alphabet[value << 5 - bits & 0x1F]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Teilt einen String in Blöcke mit <blockSize> Zeichen und trennt diese mit <separator>.
        /// Beispiel: ChunkString("ABCDEFGHIJ", 3, '-') => "ABC-DEF-GHI-J"
        /// </summary>
        private static string ChunkString(string input, int blockSize, char separator)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                // Bei jedem Blockanfang, außer beim allerersten, fügen wir den Trenner ein
                if (i > 0 && i % blockSize == 0)
                {
                    sb.Append(separator);
                }
                sb.Append(input[i]);
            }
            return sb.ToString();
        }
    }
}
