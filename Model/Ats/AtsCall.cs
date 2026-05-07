using Calls.Model.Enums;
using System;

namespace Calls.Model.Ats
{
    /// <summary>
    /// Inbound важен в логике отображения, т.к. важно кому именно позвонили
    /// В логике фильтрации это не важно, т.к. мы знаем все телефоны компании
    /// </summary>
    public class AtsCall
    {
        public string ClientPhone { get; set; }
        public string ClientInboundPhone { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyInboundPhone { get; set; }
        public DateTime Time { get; set; }
        public AtsCallDirection Direction { get; set; }
        public int Duration { get; set; }
        public AtsCallStatus Status { get; set; }
    }
}
