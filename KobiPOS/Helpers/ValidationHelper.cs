namespace KobiPOS.Helpers
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return phone.Length >= 10 && phone.All(c => char.IsDigit(c) || c == ' ' || c == '-' || c == '(' || c == ')');
        }

        public static bool IsValidPrice(decimal price)
        {
            return price >= 0;
        }
    }
}
