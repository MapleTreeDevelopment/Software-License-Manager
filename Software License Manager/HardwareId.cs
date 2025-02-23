
namespace Software_License_Manager
{
    using Microsoft.Win32;
    using System;
    using System.Management;
    using System.Security.Cryptography;
    using System.Text;

    public static class HardwareId
    {
        /// <summary>
        /// Liefert einen eindeutigen Hashwert (SHA-256) aus mehreren Hardware- und Systemattributen:
        /// CPU-ID, Mainboard-Seriennummer, Festplattenseriennummer, MachineGuid.
        /// </summary>
        public static string GetCombinedHardwareId()
        {
            string cpuId = GetCpuId();
            string boardSerial = GetBaseBoardSerial();
            string driveSerial = GetDriveSerial();
            string machineGuid = GetMachineGuid();

            // Kombination aller Werte zu einem String
            string combined = $"{cpuId}|{boardSerial}|{driveSerial}|{machineGuid}";

            // Hash mit SHA-256
            return ComputeSha256(combined);
        }

        public static void PrintDetails()
        {
            string cpuId = GetCpuId();
            string boardSerial = GetBaseBoardSerial();
            string driveSerial = GetDriveSerial();
            string machineGuid = GetMachineGuid();
            Console.WriteLine($"CPU ID: {cpuId}\nBoard Serial: {boardSerial}\nDrive Serial: {driveSerial}\nMachine GUID: {machineGuid}");
        }

        /// <summary>
        /// Liest die CPU-ID (Win32_Processor.ProcessorId) aus oder "CPU-UNKNOWN", wenn nicht verfügbar.
        /// </summary>
        private static string GetCpuId()
        {
            try
            {
                using (var mc = new ManagementClass("Win32_Processor"))
                {
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        var prop = mo.Properties["ProcessorId"]?.Value;
                        if (prop != null)
                        {
                            return prop.ToString().Trim();
                        }
                    }
                }
            }
            catch { }
            return "CPU-UNKNOWN";
        }

        /// <summary>
        /// Liest die Mainboard-Seriennummer (Win32_BaseBoard.SerialNumber) aus oder "MB-UNKNOWN", wenn nicht verfügbar.
        /// </summary>
        private static string GetBaseBoardSerial()
        {
            try
            {
                using (var mc = new ManagementClass("Win32_BaseBoard"))
                {
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        var prop = mo.Properties["SerialNumber"]?.Value;
                        if (prop != null)
                        {
                            return prop.ToString().Trim();
                        }
                    }
                }
            }
            catch { }
            return "MB-UNKNOWN";
        }

        /// <summary>
        /// Liest die Festplattenseriennummer (Win32_PhysicalMedia.SerialNumber) aus oder "DRIVE-UNKNOWN".
        /// </summary>
        /// <remarks>
        /// Bei mehreren Platten wird hier nur die erste ausgelesen.
        /// Für RAID, VMs oder bestimmte Hersteller kann dies leer oder unzuverlässig sein.
        /// </remarks>
        private static string GetDriveSerial()
        {
            try
            {
                using (var mc = new ManagementClass("Win32_PhysicalMedia"))
                {
                    foreach (ManagementObject mo in mc.GetInstances())
                    {
                        var serial = mo.Properties["SerialNumber"]?.Value as string;
                        if (!string.IsNullOrEmpty(serial))
                        {
                            return serial.Trim();
                        }
                    }
                }
            }
            catch { }
            return "DRIVE-UNKNOWN";
        }

        /// <summary>
        /// Liest die Windows MachineGuid (HKLM\SOFTWARE\Microsoft\Cryptography\MachineGuid) aus oder "GUID-UNKNOWN".
        /// </summary>
        private static string GetMachineGuid()
        {
            try
            {
                const string key = @"SOFTWARE\Microsoft\Cryptography";
                using (var rk = Registry.LocalMachine.OpenSubKey(key))
                {
                    if (rk != null)
                    {
                        var guidObj = rk.GetValue("MachineGuid");
                        if (guidObj != null)
                        {
                            return guidObj.ToString().Trim();
                        }
                    }
                }
            }
            catch { }
            return "GUID-UNKNOWN";
        }

        /// <summary>
        /// Erzeugt einen SHA-256-Hash aus einem String und gibt ihn hex-kodiert zurück.
        /// </summary>
        private static string ComputeSha256(string rawData)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder sb = new StringBuilder();
                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
