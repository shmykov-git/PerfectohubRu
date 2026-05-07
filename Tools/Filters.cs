using Calls.Model.Ats;
using Calls.Model.Enums;
using System;
using System.Collections.Generic;

namespace Calls.Model.Libraries
{
    public static class Filters
    {
        public static Func<AtsCall, bool> ExternalIn(HashSet<string> knownPhones, HashSet<string> activePhones) =>
            c => 
            {
                if (c.Direction != AtsCallDirection.In)
                    return false;

                var isActive = activePhones.Contains(c.CompanyPhone);
                var isInternal = knownPhones.Contains(c.CompanyPhone) && knownPhones.Contains(c.ClientPhone);

                return isActive && !isInternal;
            };

        public static bool Out(AtsCall c) => c.Direction == AtsCallDirection.Out;
        public static bool In(AtsCall c) => c.Direction == AtsCallDirection.In;

        public static bool Received(AtsCall c) => c.Status == AtsCallStatus.Received && c.Duration > 0; // Received must have duration > 0
        public static bool Missed(AtsCall c) => c.Status == AtsCallStatus.Missed; // Missed can have duration > 0

        public static bool OutReceived(AtsCall c) => Out(c) && Received(c);
        public static bool OutMissed(AtsCall c) => Out(c) && Missed(c);
    }
}
