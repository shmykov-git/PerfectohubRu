using Calls.Extensions;
using Shared.Model.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Shared.Model.Options
{

    public class ClientOptions
    {
        public string KnownMarks { get; set; }

        public int RecallClientLimit { get; set; }
        public AtsType AtsType { get; set; }

        public AuthOptions BotAuth { get; set; }
        public int BotPollingInterval { get; set; }
        public int MaxMessageSize { get; set; }

        private Dictionary<MarkType, string> __marks;

        [JsonIgnore] public Dictionary<MarkType, string> Marks => __marks ?? (__marks = ClientOptionsExtensions.GetMarks(KnownMarks));
        public DateTime GetClientTime(DateTime? utcTime = null) => (utcTime ?? DateTime.UtcNow).ToLocalTime();
    }
}
