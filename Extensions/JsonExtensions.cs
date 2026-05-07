using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Perfecto.Deploy.Extensions
{
    public static class JsonExtensions
    {
        private static JsonSerializerSettings __jsonSettings = null;
        private static JsonSerializerSettings __jsonLowerSettings = null;
        private static JsonSerializerSettings _jsonSettings => __jsonSettings ?? (__jsonSettings = GetJsonSettings(false));
        private static JsonSerializerSettings _jsonLowerSettings => __jsonLowerSettings ?? (__jsonLowerSettings = GetJsonSettings(true));

        private static JsonSerializerSettings GetJsonSettings(bool lowerCamelCase)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None,
            };

            settings.Converters.Add(new StringEnumConverter());

            if (lowerCamelCase)
                settings.ContractResolver = new DefaultContractResolver() { NamingStrategy = new CamelCaseNamingStrategy() };

            return settings;
        }

        public static string ToJsonQueryStr(this object queryArgs)
        {
            List<(string, string)> query = new List<(string, string)>();

            foreach (var p in queryArgs.GetType().GetProperties())
            {
                var value = p.GetValue(queryArgs);

                if (value == null)
                    continue;

                var attribute = (JsonPropertyAttribute)p.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(JsonPropertyAttribute));
                var type = p.PropertyType;
                var name = attribute?.PropertyName ?? p.Name;

                if (value is object[] valueArr)
                {
                    name += Uri.EscapeDataString("[]");

                    foreach (var valueItem in valueArr)
                        query.Add((name, valueItem.ToString()));
                }
                else if (type.IsValueType || type == typeof(string))
                {
                    query.Add((name, value.ToString()));
                }
                else
                {
                    query.Add((name, value.ToJsonInLine()));
                }
            }

            var queryStr = query.Select(v => $"{v.Item1}={Uri.EscapeDataString(v.Item2)}").SJoin("&");

            return queryStr;
        }

        public static string ToQueryStr<T>(this T value) => typeof(T)
            .GetProperties()
            .Select(p =>
            {
                var v = p.GetValue(value)?.ToJsonStr(Formatting.None);

                return v == null
                    ? null
                    : $"{p.Name.ToLower()}={v}";
            })
            .Where(v => v != null)
            .SJoin("&");

        public static string ToJsonInLine<T>(this T value) => value.ToJsonStr(Formatting.None);

        public static string ToJsonStr<T>(this T value, Formatting formatting = Formatting.Indented)
        {
            if (value == null)
                return null;

            return typeof(T) == typeof(object)
                ? JsonConvert.SerializeObject(value, value.GetType(), formatting, _jsonSettings)
                : JsonConvert.SerializeObject(value, formatting, _jsonSettings);
        }

        public static string ToJsonStrLower<T>(this T value, Formatting formatting = Formatting.Indented)
        {
            return typeof(T) == typeof(object)
                ? JsonConvert.SerializeObject(value, value.GetType(), formatting, _jsonLowerSettings)
                : JsonConvert.SerializeObject(value, formatting, _jsonLowerSettings);
        }

        public static bool IsJson(this string value) => value.StartsWith("{");

        public static T FromJsonStr<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value, _jsonSettings);
        }

        //public static async Task<T?> ReadFromJsonStrAsync<T>(this HttpContent content)
        //{
        //    var str = await content.ReadAsStringAsync();

        //    return str.FromJsonStr<T>();
        //}

        //public static async Task<T?> ReadFromJsonStreamAsync<T>(this Stream stream)
        //{
        //    using StreamReader reader = new StreamReader(stream);
        //    var str = await reader.ReadToEndAsync();

        //    return str.FromJsonStr<T>();
        //}

        public static object FromJsonStr(this string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, _jsonSettings);
        }

        /// <summary>
        /// Deserialize string using Microsoft json serializer
        /// </summary>
        public static T FromJsonStrMs<T>(this string value, JsonSerializerOptions options = null)
        {
            var defaultOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<T>(value, options ?? defaultOptions);
        }

        public static JObject AsJObject(this string value) => value.FromJsonStr<JObject>();

        public static JObject ToJObject<T>(this T value) => JObject.FromObject(value);
    }
}
