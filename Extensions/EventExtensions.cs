using System;

namespace PerfectohubRu.Extensions
{
    public static class EventExtensions
    {
        public static void Raise(this Action eventDelegate)
        {
            var action = eventDelegate;

            if (action != null)
                action();
        }

        public static void Raise<TArg>(this Action<TArg> eventDelegate, TArg arg)
        {
            var action = eventDelegate;

            if (action != null)
                action(arg);
        }

        public static void Raise<TArg1, TArg2>(this Action<TArg1, TArg2> eventDelegate, TArg1 arg1, TArg2 arg2)
        {
            var action = eventDelegate;

            if (action != null)
                action(arg1, arg2);
        }

        public static void Raise<TArg1, TArg2, TArg3>(this Action<TArg1, TArg2, TArg3> eventDelegate, TArg1 arg1, TArg2 arg2, TArg3 arg3)
        {
            var action = eventDelegate;

            if (action != null)
                action(arg1, arg2, arg3);
        }
    }
}
