using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PerfectohubRu.Tools
{
    public static class ReactHelper
    {
        private static readonly ConcurrentDictionary<string, ActionItem> actions = new ConcurrentDictionary<string, ActionItem>();

        private static ActionItem CreateActionItem(string actionKey, TimeSpan interval)
        {
            var item = new ActionItem();
            item.time = DateTime.UtcNow + interval;

            item.task = Task.Run(async () =>{
                var utcNow = DateTime.UtcNow;

                while (utcNow < item.time)
                {
                    await Task.Delay(item.time - utcNow);
                    utcNow = DateTime.UtcNow;
                }
            });

            item.task.ContinueWith(t => actions.TryRemove(actionKey, out var _));

            return item;
        }

        public static Task LastActionIn(string actionKey, TimeSpan interval)
        {
            var item = actions.GetOrAdd(actionKey, _ => CreateActionItem(actionKey, interval));
            item.time = DateTime.UtcNow + interval;

            return item.task;
        }

        private class ActionItem
        {
            public Task task;
            public DateTime time;
        }
    }
}
