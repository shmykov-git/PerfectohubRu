using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PerfectohubRu.Extensions
{
    public static class UIElementCollectionExtensions
    {
        public static IEnumerable<T> Select<T>(this UIElementCollection elements, Func<UIElement, T> selectFn)
        {
            foreach (UIElement element in elements)
                yield return selectFn(element);
        }
    }
}
