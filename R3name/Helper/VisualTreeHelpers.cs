using System.Windows;
using System.Windows.Media;

namespace R3name.Helper;

public static class VisualTreeHelpers                                      
{

    public static T FindVisualParent<T>(this DependencyObject obj) where T : DependencyObject
    {
        var parent = obj;
        while (parent != null && parent is not T)
        {
            if (parent is FrameworkContentElement contentElement)
                parent = contentElement.Parent;
            else                                                                                
                parent = VisualTreeHelper.GetParent(parent);
        }
        return (T)parent;
    }
}