using System;
using System.Threading.Tasks;

namespace PerfectohubRu.Tools
{
    public static class DispatcherHelper
    {
        public static void Dispatch(Func<Task> action)
        {
            var app = App.Current;

            if (app == null) return;

            _ = app.Dispatcher.BeginInvoke(action);
        }

        public static void Dispatch(Action action)
        {
            var app = App.Current;

            if (app == null) return;

            _ = app.Dispatcher.BeginInvoke(action);
        }
    }
}
