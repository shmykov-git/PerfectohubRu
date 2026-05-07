using Calls.Model.Ats.Tele2;
using Calls.Model.Enums;
using Mapster;
using Shared.Extensions;

namespace Calls.Model.Ats.Map
{
    public class AtsTele2CallMap : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AtsTele2Call, AtsCall>()
                .Map(d => d.ClientPhone, s => GetDirection(s) == AtsCallDirection.In ? s.CallerNumber.ToSystemPhone() : (s.CalleeNumber ?? s.DestinationNumber).ToSystemPhone())
                .Map(d => d.ClientInboundPhone, s => GetDirection(s) == AtsCallDirection.Out && 
                                                     s.DestinationNumber != null && 
                                                     s.CalleeNumber != null &&
                                                     s.DestinationNumber != s.CalleeNumber 
                                                        ? s.DestinationNumber.ToSystemPhone()
                                                        : null)
                .Map(d => d.CompanyPhone, s => GetDirection(s) == AtsCallDirection.In ? s.CalleeNumber.ToSystemPhone() : s.CallerNumber.ToSystemPhone())
                .Map(d => d.CompanyInboundPhone, s => GetDirection(s) == AtsCallDirection.In &&
                                                     s.DestinationNumber != null &&
                                                     s.CalleeNumber != null &&
                                                     s.DestinationNumber != s.CalleeNumber
                                                        ? s.DestinationNumber.ToSystemPhone()
                                                        : null)
                .Map(d => d.Time, s => s.Date.ToUniversalTime())
                .Map(d => d.Direction, s => GetDirection(s))
                .Map(d => d.Duration, s => s.ConversationDuration)
                .Map(d => d.Status, s => GetStatusFrom(s.CallStatus))
                ;

            config.NewConfig<AtsTele2Call, AtsPassportCall>()
                .Map(d => d.Name, s => GetPassportName(s))
                .Inherits<AtsTele2Call, AtsCall>()
                ;
        }

        AtsCallDirection GetDirection(AtsTele2Call c)
        {
            switch (c.CallType)
            {
                case "SINGLE_CHANNEL":
                case "MULTI_CHANNEL":
                    if (c.CallerName == null && c.CalleeName == null)
                        return AtsCallDirection.None;

                    if (c.CallerName != null && c.CalleeName != null)
                        return AtsCallDirection.InOut;

                    return c.CallerName != null ? AtsCallDirection.Out : AtsCallDirection.In;

                case "UNKNOWN_CALL":
                    return AtsCallDirection.None;

                case "public":
                    return AtsCallDirection.InOut;

                case "CALLBACK":
                case "OUTGOING":
                    return AtsCallDirection.Out;

                default:
                    return AtsCallDirection.None;
            }
        }

        private AtsCallStatus GetStatusFrom(string tele2Status)
        {
            switch (tele2Status)
            {
                case "ANSWERED_COMMON":
                case "ANSWERED_BY_ORIGINAL_CLIENT":
                case "ANSWERED_BY_BUSY_FORWARD_CLIENT":
                case "ANSWERED_BY_NO_ANSWER_FORWARD_CLIENT":
                case "CANCELLED_BY_PICKUP":
                    return AtsCallStatus.Received;

                case "NOT_ANSWERED_COMMON":
                case "CANCELLED_BY_CALLER":
                case "DENIED_DUE_TO_MAX_SESSION":
                case "DENIED_DUE_TO_INCOMING_CALLS_BLOCKED":
                case "DENIED_DUE_TO_OUTGOING_CALLS_BLOCKED":
                case "DENIED_DUE_TO_ONLY_public_CALLS_ENABLED":
                case "DENIED_DUE_TO_BLACK_LISTED":
                case "DENIED_NOT_IN_WHITE_LIST":
                case "DENIED_DUE_TO_NOT_WORK_TIME":
                case "DENIED_DUE_TO_UNKNOWN_NUMBER":
                case "DESTINATION_BUSY":
                case "CANCELLED_BY_MCN":
                    return AtsCallStatus.Missed;

                default:
                    return AtsCallStatus.Unknown;
            }
        }

        private string GetPassportName(AtsTele2Call c)
        {
            return GetDirection(c) == AtsCallDirection.In
                ? c.CalleeName
                : c.CallerName;
        }
    }
}
