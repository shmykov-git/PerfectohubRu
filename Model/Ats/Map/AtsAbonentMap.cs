using Calls.Model.Ats;
using Calls.Model.Ats.Beeline;
using Calls.Model.Ats.Tele2;
using Mapster;
using Shared.Extensions;

namespace Calls.Entities.Mapster
{
    public class AtsAbonentMap : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<AtsBeelineAbonent, ActivePhone>()
                .Map(d => d.Phone, s => s.phone.ToSystemPhone())
                .Map(d => d.Name, s => $"{s.firstName} {s.lastName}")
                ;

            config.NewConfig<AtsTele2Abonent, ActivePhone>()
                .Map(d => d.Phone, s => s.FullNumber.ToSystemPhone())
                .Map(d => d.Name, s => s.Name)
                ;
        }
    }
}
