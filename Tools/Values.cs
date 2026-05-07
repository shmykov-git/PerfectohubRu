using System.Text.RegularExpressions;

namespace Shared.Libraries
{
    public static class Values
    {
        public static class ConfigurationSection
        {
            public const string HttpClients = "HttpClients";
            public const string Profiles = "Profiles";
        }

        public static class MimeType
        {
            public const string ApplicationJson = "application/json";
            public const string MultipartFormData = "multipart/form-data";
        }

        public static class Regexes
        {
            public static Regex Brackets = new Regex(@"\{([^}]*)\}", RegexOptions.Compiled);
            public static Regex UniqueConstraint = new Regex(@"unique constraint\s+""([^""]+)""", RegexOptions.Compiled);
            public static Regex Phone10 = new Regex(@"^\d{10}$", RegexOptions.Compiled);
            public static Regex RussianPhoneRegex = new Regex(@"^(7|8|\+7)?(\d{10})$", RegexOptions.Compiled);
            public static readonly Regex OnlyDigits = new Regex(@"[^\d+]", RegexOptions.Compiled);
            public static readonly Regex ClientId = new Regex(@"^[a-z][a-z0-9]*$", RegexOptions.Compiled);
        }

        public static class HeaderName
        {
            public const string Authorization = "Authorization";
            public const string BeelineAtsToken = "X-MPBX-API-AUTH-TOKEN";
            public const string BeelineAtsAccountId = "X-MPBX-API-ACCOUNT-ID";
            public const string ClientId = "X-Forwarded-Prefix";
        }

        public static class AuthSchema
        {
            public const string Basic = "Basic";
            public const string Bearer = "Bearer";
        }
    }
}
