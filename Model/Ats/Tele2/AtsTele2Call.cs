using System;

namespace Calls.Model.Ats.Tele2
{
    public class AtsTele2Call
    {
        /// <summary>
        /// Уникальный идентификатор звонка
        /// </summary>
        public Guid Uuid { get; set; }

        /// <summary>
        /// Дата и время звонка
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Тип канала звонка
        /// </summary>
        public string CallType { get; set; }

        /// <summary>
        /// Номер назначения
        /// </summary>
        public string DestinationNumber { get; set; }

        /// <summary>
        /// Номер звонящего
        /// </summary>
        public string CallerNumber { get; set; }

        /// <summary>
        /// Имя звонящего
        /// </summary>
        public string CallerName { get; set; }

        /// <summary>
        /// Номер принимающего звонок
        /// </summary>
        public string CalleeNumber { get; set; }

        /// <summary>
        /// Имя принимающего звонок
        /// </summary>
        public string CalleeName { get; set; }

        /// <summary>
        /// Общая длительность звонка (секунды)
        /// </summary>
        public int CallDuration { get; set; }

        /// <summary>
        /// Длительность разговора (секунды)
        /// </summary>
        public int ConversationDuration { get; set; }

        /// <summary>
        /// Статус звонка
        /// </summary>
        public string CallStatus { get; set; }

        /// <summary>
        /// Имя файла записи разговора
        /// </summary>
        public string RecordFileName { get; set; }

        /// <summary>
        /// Переопределение метода ToString для удобного вывода информации
        /// </summary>
        public override string ToString()
        {
            return $"Звонок от {CallerName} ({CallerNumber}) к {CalleeName} ({CalleeNumber}) - " +
                   $"Длительность: {ConversationDuration} сек. из {CallDuration} сек., Статус: {CallStatus}";
        }

        /// <summary>
        /// Проверить, был ли звонок отвечен
        /// </summary>
        public bool IsAnswered()
        {
            return CallStatus == "ANSWERED_BY_ORIGINAL_CLIENT" ||
                   CallStatus == "ANSWERED" ||
                   CallStatus == "ANSWERED_BY_FORWARDING";
        }
    }
}