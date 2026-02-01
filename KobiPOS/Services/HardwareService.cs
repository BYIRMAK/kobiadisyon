using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace KobiPOS.Services
{
    public class HardwareService
    {
        public static string GetHardwareID()
        {
            try
            {
                // On Windows, get CPU ID and Motherboard Serial
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var cpuId = GetCpuId();
                    var motherboardId = GetMotherboardId();
                    var combined = $"{cpuId}-{motherboardId}";
                    return GenerateHash(combined);
                }
                else
                {
                    // Fallback for non-Windows systems
                    var machineName = Environment.MachineName;
                    var userName = Environment.UserName;
                    var combined = $"{machineName}-{userName}";
                    return GenerateHash(combined);
                }
            }
            catch
            {
                // Ultimate fallback
                return GenerateHash(Environment.MachineName);
            }
        }

        private static string GetCpuId()
        {
            try
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return "LINUX-CPU";

                var cpuId = string.Empty;
                var mc = new ManagementClass("Win32_Processor");
                var moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    cpuId = mo.Properties["ProcessorId"].Value?.ToString() ?? "";
                    break;
                }

                return cpuId;
            }
            catch
            {
                return "CPU-UNKNOWN";
            }
        }

        private static string GetMotherboardId()
        {
            try
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return "LINUX-MB";

                var motherboardId = string.Empty;
                var mc = new ManagementClass("Win32_BaseBoard");
                var moc = mc.GetInstances();

                foreach (ManagementObject mo in moc)
                {
                    motherboardId = mo.Properties["SerialNumber"].Value?.ToString() ?? "";
                    break;
                }

                return motherboardId;
            }
            catch
            {
                return "MB-UNKNOWN";
            }
        }

        private static string GenerateHash(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash).Substring(0, 32);
            }
        }
    }
}
