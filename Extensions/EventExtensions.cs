using System;

namespace PerfectohubRu.Extensions
{
    public static class EventExtensions
    {
        public static void Raise<TArg>(this Action<TArg> eventDelegate, TArg arg) 
        { 
            var action = eventDelegate;

            if (action != null)
                action(arg);
        }
    }
}
