using System;

namespace Calls.Model.Ats.Beeline
{
    public class AtsBeelineView
    {
        public long ts { get; set; }
        public string type { get; set; }
        public Payload payload { get; set; }
        public object warnings { get; set; } // null в JSON
        public object errors { get; set; }   // null в JSON


        public class Payload
        {
            public int totalCount { get; set; }
            public string type { get; set; }
            public AtsBeelineCall[] calls { get; set; }
            public object chart { get; set; } // null в JSON
        }

        public class AtsBeelineCall
        {
            public string keyId { get; set; }
            public long startTime { get; set; }
            public string direction { get; set; }
            public string status { get; set; }
            public From from { get; set; }
            public To to { get; set; }
            public string inboundPhone { get; set; }
            public int duration { get; set; }
            public bool hasCallRecording { get; set; }
            public string userTimezone { get; set; }
            public bool ispublicCall { get; set; }
            public Guid externalTrackingId { get; set; }
        }

        // Информация об отправителе
        public class From
        {
            public string phone { get; set; }
            public Abonent abonent { get; set; }      // может быть null или объект
            public ClientService clientService { get; set; } // может быть null или объект
        }

        // Информация о получателе
        public class To
        {
            public string phone { get; set; }
            public Abonent abonent { get; set; }      // может быть null или объект
            public ClientService clientService { get; set; } // может быть null или объект
        }

        // Абонент
        public class Abonent
        {
            public int ext { get; set; }
            public string department { get; set; }
            public string name { get; set; }
            public string userId { get; set; }
        }

        // Сервис клиента (может быть Abonent или HuntGroup)
        public class ClientService
        {
            public int ext { get; set; }
            public string name { get; set; } // может быть null или строка
            public string dtype { get; set; } // "Abonent" или "HuntGroup"
        }

    }
}
