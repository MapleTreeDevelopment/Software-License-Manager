using System.Security.Cryptography;
using System.Text;

namespace Software_License_Manager
{

    public static class LocalLicenseChecker
    {
        /// <summary>
        /// Prüft, ob ein gegebener Lizenzschlüssel und eine aktuelle Hardware-ID 
        /// mit den gespeicherten Werten (z. B. lizenzKey, hashedHwId) übereinstimmen.
        /// </summary>
        /// <param name="enteredLicenseKey">
        /// Der vom Benutzer bzw. aus einer Konfig-Datei gelesene Lizenzschlüssel.
        /// </param>
        /// <param name="currentHardwareId">
        /// Die aktuell ermittelte Hardware-ID (z. B. über HardwareId.GetCombinedHardwareId()).
        /// </param>
        /// <param name="storedLicenseKey">
        /// Der original gespeicherte (oder erwartete) Lizenzschlüssel.
        /// </param>
        /// <param name="storedHashedHwId">
        /// Der SHA-256-Hash der Hardware-ID, die bei der Aktivierung erfasst wurde.
        /// </param>
        /// <returns>
        /// true, wenn alles übereinstimmt, ansonsten false.
        /// </returns>
        public static bool CheckLocalLicense(
            string enteredLicenseKey,
            string currentHardwareId,
            string storedLicenseKey,
            string storedHashedHwId)
        {
            // 1) Lizenzschlüssel vergleichen
            //    (Je nach Fall vielleicht Case-Insensitiv)
            if (!string.Equals(enteredLicenseKey, storedLicenseKey, StringComparison.OrdinalIgnoreCase))
            {
                // Ungültiger Lizenzschlüssel
                return false;
            }

            // 2) Hardware-ID hashen
            string hashedCurrentHwId = ComputeSha256(currentHardwareId);

            // 3) Vergleich mit gespeicherter Hardware-ID
            return hashedCurrentHwId.Equals(storedHashedHwId, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Einfache Hilfsmethode für SHA-256-Hash.
        /// </summary>
        private static string ComputeSha256(string text)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
