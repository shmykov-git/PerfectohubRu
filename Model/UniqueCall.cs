using System;

namespace PerfectohubRu.Model
{
    public class UniqueCall
    {
        /// <summary>
        /// Уникальный номер звонившего абонента.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Количество попыток дозвониться.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Общая продолжительность разговора
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Звонок будет отображен в боте
        /// </summary>
        public bool IsBotMessage { get; set; }

        /// <summary>
        /// Сколько раз перезвонили абоненту
        /// </summary>
        public int RecallTryCount { get; set; }

        /// <summary>
        /// Звонок прошел через общий номер Атс
        /// </summary>
        public bool HasInbound {  get; set; }

        /// <summary>
        /// Куда звонил абонент
        /// </summary>
        public string[] Whom { get; set; }

        /// <summary>
        /// Время последней попытки дозвониться
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// Время последнего успешного звонка
        /// </summary>
        public DateTime? SuccessLastTime { get; set; }
    }
}
