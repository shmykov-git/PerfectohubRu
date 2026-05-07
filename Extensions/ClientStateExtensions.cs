using PerfectohubRu.Model;
using Shared.Model.Enums;

namespace PerfectohubRu.Extensions
{
    public static class ClientStateExtensions 
    {
        public static string GetAtsToken(this ClientData clientState)
        {
            return clientState.BeelineAtsToken ?? clientState.Tele2AtsToken;
        }

        public static bool HasAtsToken(this ClientData clientState)
        {
            return clientState.GetAtsToken() != null;
        }

        public static AtsType GetAtsType(this ClientData clientState)
        {
            if (clientState.BeelineAtsToken != null)
                return AtsType.Beeline;

            if (clientState.Tele2AtsToken != null)
                return AtsType.Tele2;

            return AtsType.None;
        }

        public static string GetAtsName(this ClientData clientState)
        {
            if (clientState.BeelineAtsToken != null)
                return "Билайн АТС";

            if (clientState.Tele2AtsToken != null)
                return "Tele2 АТС";

            return "Unknown АТС";
        }
    }
}
