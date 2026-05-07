using Calls.Extensions;
using Shared.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shared.Model.Options
{
    public class ClientOptions
    {
        public string TelegramBotToken { get; set; }
        public string MaxBotToken { get; set; }
        public string KnownMarks { get; set; }

        public int RecallClientLimit { get; set; }

        public AtsType AtsType { get; set; }


        private Dictionary<MarkType, string> __marks;

        [JsonIgnore] public Dictionary<MarkType, string> Marks => __marks ?? (__marks = ClientOptionsExtensions.GetMarks(KnownMarks));
        public DateTime GetClientTime(DateTime? utcTime = null) => (utcTime ?? DateTime.UtcNow).ToLocalTime();
    }
}
