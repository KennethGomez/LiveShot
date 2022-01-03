using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace LiveShot.API.Utils
{
    public static class WindowUtils
    {
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject dependencyObject)
            where T : DependencyObject
        {
            int count = VisualTreeHelper.GetChildrenCount(dependencyObject);
            
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);

                if (child is T c)
                {
                    yield return c;
                }

                foreach (var childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
        
        public static bool IsOpen(Type window)
        {
            return Application.Current.Windows.Cast<Window>().Any(x => x.GetType() == window && x.IsActive);
        }
    }
}