using Shared.Libraries;

namespace Shared.Extensions
{
    public static class PhoneExtensions
    {
        // todo: 7 to options
        public static string ToSystemPhone(this string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            if (phone.Length == 12)
                return phone;

            if (phone.Length == 11)
                return $"+{phone}";

            if (phone.Length == 10)
                return $"+7{phone}";

            var digits = Values.Regexes.OnlyDigits.Replace(phone, "");

            if (digits.Length == 11)
                return $"+{digits}";

            if (digits.Length == 10)
                return $"+7{digits}";

            return digits;
        }
    }
}
