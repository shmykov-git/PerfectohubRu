using Calls.Model.Ats;
using Calls.Model.Ats.Beeline;
using Calls.Model.Enums;
using Mapster;
using Shared.Extensions;

namespace Calls.Model.Ats.Map
{
    public class AtsBeelineCallMap : IRegister
    {
        /// <summary>
        /// direction in ["INB", "OUT"]
        /// 
        /// </summary>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AtsBeelineView.AtsBeelineCall, AtsCall>()
                .Map(d => d.ClientPhone, s => s.direction == "INB" ? s.from.phone.ToSystemPhone() : s.to.phone.ToSystemPhone())
                .Map(d => d.CompanyPhone, s => s.direction == "INB" ? s.to.phone.ToSystemPhone() : s.from.phone.ToSystemPhone())
                .Map(d => d.ClientInboundPhone, s => !string.IsNullOrWhiteSpace(s.inboundPhone) ? (s.direction == "INB" ? null : s.inboundPhone.ToSystemPhone()) : null)
                .Map(d => d.CompanyInboundPhone, s => !string.IsNullOrWhiteSpace(s.inboundPhone) ? (s.direction == "INB" ? s.inboundPhone.ToSystemPhone() : null) : null)
                .Map(d => d.Time, s => s.startTime.ToDateTime())
                .Map(d => d.Direction, s => s.direction == "INB" ? AtsCallDirection.In : AtsCallDirection.Out)
                .Map(d => d.Duration, s => s.duration)
                .Map(d => d.Status, s => GetStatusFrom(s.status))
                ;

            config.NewConfig<AtsBeelineView.AtsBeelineCall, AtsPassportCall>()
                .Map(d => d.Name, s => GetPassportName(s))
                .Inherits<AtsBeelineView.AtsBeelineCall, AtsCall>()
            ;
        }

        private AtsCallStatus GetStatusFrom(string beelineStatus)
        {
            switch (beelineStatus)
            {
                case "RECEIVED_BY_ABONENT":
                case "RECEIVED":
                case "PLACED":
                    return AtsCallStatus.Received;

                case "INBMISSED_BY_ABONENT":
                case "INBMISSED":
                case "OUTMISSED":
                    return AtsCallStatus.Missed;

                default:
                    return AtsCallStatus.Unknown;
            }
        }

        private string GetPassportName(AtsBeelineView.AtsBeelineCall c)
        {
            if (c.direction == "INB")
            {
                return
                    c.from.abonent?.name ??
                    c.from.clientService?.name ??
                    c.from.abonent?.department ??
                    c.from.clientService?.name ??
                    c.from.phone;
            }
            else
            {
                return
                    c.to.abonent?.name ??
                    c.to.clientService?.name ??
                    c.to.abonent?.department ??
                    c.to.clientService?.name ??
                    c.to.phone;
            }
        }
    }
}
