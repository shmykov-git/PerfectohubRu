namespace Calls.Model.Enums
{
    public enum AtsCallStatus
    {
        Unknown,
    
        /// <summary>
        /// Вызов получен
        /// </summary>
        Received,

        /// <summary>
        /// Вызов пропущен
        /// </summary>
        Missed,

        /// <summary>
        /// Вызов сброшен
        /// </summary>
        Cancelled
    }
}
