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
            {
                if (phone.StartsWith("8"))
                    return $"+7{phone.Substring(1)}";

                return $"+{phone}";
            }

            if (phone.Length == 10)
                return $"+7{phone}";

            return phone.ToCorrectSystemPhone();
        }

        public static string ToCorrectSystemPhone(this string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            var cleanedPhone = Values.Regexes.OnlyDigits.Replace(phone, "");

            if (cleanedPhone.Length == 11)
            {
                if (cleanedPhone.StartsWith("8"))
                    return $"+7{cleanedPhone.Substring(1)}";

                return $"+{cleanedPhone}";
            }

            if (cleanedPhone.Length == 10)
                return $"+7{cleanedPhone}";

            return cleanedPhone;
        }
    }
}
