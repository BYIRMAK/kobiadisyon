using KobiPOS.Models;
using System.Security.Cryptography;
using System.Text;

namespace KobiPOS.Services
{
    public class LicenseService
    {
        private readonly DatabaseService _db;
        private const int TRIAL_DAYS = 7;

        public LicenseService()
        {
            _db = DatabaseService.Instance;
        }

        public LicenseStatus GetLicenseStatus()
        {
            var status = new LicenseStatus();

            // Check for active license
            using var connection = _db.GetConnection();
            connection.Open();

            var command = new Microsoft.Data.Sqlite.SqliteCommand(@"
                SELECT * FROM Licenses 
                WHERE HardwareID = @hardwareId AND IsActive = 1
                ORDER BY ExpiryDate DESC
                LIMIT 1
            ", connection);

            var hardwareId = HardwareService.GetHardwareID();
            command.Parameters.AddWithValue("@hardwareId", hardwareId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var expiryDate = DateTime.Parse(reader.GetString(5));
                if (expiryDate > DateTime.Now)
                {
                    status.IsLicensed = true;
                    status.LicenseType = "Tam Lisans";
                    status.ExpiryDate = expiryDate;
                    status.CustomerName = reader.GetString(2);
                    status.DaysRemaining = (expiryDate - DateTime.Now).Days;
                    status.IsReadOnly = false;
                    return status;
                }
            }

            // Check trial period
            var trialStartStr = _db.GetSetting("TrialStartDate");
            if (!string.IsNullOrEmpty(trialStartStr))
            {
                var trialStart = DateTime.Parse(trialStartStr);
                var trialEnd = trialStart.AddDays(TRIAL_DAYS);
                var daysRemaining = (trialEnd - DateTime.Now).Days;

                if (daysRemaining > 0)
                {
                    status.IsLicensed = false;
                    status.LicenseType = "Deneme Sürümü";
                    status.ExpiryDate = trialEnd;
                    status.DaysRemaining = daysRemaining;
                    status.IsReadOnly = false;
                    return status;
                }
                else
                {
                    status.IsLicensed = false;
                    status.LicenseType = "Deneme Süresi Doldu";
                    status.ExpiryDate = trialEnd;
                    status.DaysRemaining = 0;
                    status.IsReadOnly = true;
                    return status;
                }
            }

            // No trial start date, this shouldn't happen
            status.IsLicensed = false;
            status.LicenseType = "Lisanssız";
            status.IsReadOnly = true;
            return status;
        }

        public bool ActivateLicense(string licenseKey, string customerName)
        {
            try
            {
                var hardwareId = HardwareService.GetHardwareID();

                // Validate license key format
                if (!ValidateLicenseKeyFormat(licenseKey))
                    return false;

                using var connection = _db.GetConnection();
                connection.Open();

                // Check if license already exists for this hardware
                var checkCommand = new Microsoft.Data.Sqlite.SqliteCommand(@"
                    SELECT COUNT(*) FROM Licenses WHERE HardwareID = @hardwareId AND IsActive = 1
                ", connection);
                checkCommand.Parameters.AddWithValue("@hardwareId", hardwareId);
                var existing = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (existing > 0)
                {
                    // Deactivate old license
                    var deactivateCommand = new Microsoft.Data.Sqlite.SqliteCommand(@"
                        UPDATE Licenses SET IsActive = 0 WHERE HardwareID = @hardwareId
                    ", connection);
                    deactivateCommand.Parameters.AddWithValue("@hardwareId", hardwareId);
                    deactivateCommand.ExecuteNonQuery();
                }

                // Insert new license
                var activationDate = DateTime.Now;
                var expiryDate = activationDate.AddYears(1);

                var insertCommand = new Microsoft.Data.Sqlite.SqliteCommand(@"
                    INSERT INTO Licenses (LicenseKey, CustomerName, HardwareID, ActivationDate, ExpiryDate, IsActive)
                    VALUES (@licenseKey, @customerName, @hardwareId, @activationDate, @expiryDate, 1)
                ", connection);

                insertCommand.Parameters.AddWithValue("@licenseKey", licenseKey);
                insertCommand.Parameters.AddWithValue("@customerName", customerName);
                insertCommand.Parameters.AddWithValue("@hardwareId", hardwareId);
                insertCommand.Parameters.AddWithValue("@activationDate", activationDate.ToString("yyyy-MM-dd HH:mm:ss"));
                insertCommand.Parameters.AddWithValue("@expiryDate", expiryDate.ToString("yyyy-MM-dd HH:mm:ss"));

                insertCommand.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateLicenseKeyFormat(string licenseKey)
        {
            // Expected format: XXXXX-XXXXX-XXXXX-XXXXX (20 chars + 3 dashes = 23)
            if (string.IsNullOrWhiteSpace(licenseKey))
                return false;

            var parts = licenseKey.Split('-');
            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (part.Length != 5)
                    return false;
            }

            return true;
        }

        public static string GenerateLicenseKey(string hardwareId)
        {
            // Generate a license key based on hardware ID
            using (var sha256 = SHA256.Create())
            {
                var input = $"{hardwareId}-{DateTime.Now.Ticks}";
                var bytes = Encoding.UTF8.GetBytes(input);
                var hash = sha256.ComputeHash(bytes);
                var hex = BitConverter.ToString(hash).Replace("-", "");

                // Format as XXXXX-XXXXX-XXXXX-XXXXX
                return $"{hex.Substring(0, 5)}-{hex.Substring(5, 5)}-{hex.Substring(10, 5)}-{hex.Substring(15, 5)}";
            }
        }
    }

    public class LicenseStatus
    {
        public bool IsLicensed { get; set; }
        public string LicenseType { get; set; } = string.Empty;
        public DateTime? ExpiryDate { get; set; }
        public int DaysRemaining { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public bool IsReadOnly { get; set; }
    }
}
