using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Funani.Gui.Controls
{
    public static class PixelBasedScrollingBehavior
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool),
                                                typeof(PixelBasedScrollingBehavior),
                                                new UIPropertyMetadata(false, HandleIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsEnabledProperty, value);
        }

        private static void HandleIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vsp = d as VirtualizingStackPanel;
            if (vsp == null)
            {
                return;
            }

            PropertyInfo property = typeof(VirtualizingStackPanel).
                GetProperty("IsPixelBased", BindingFlags.NonPublic | BindingFlags.Instance);

            if (property == null)
            {
                throw new InvalidOperationException("Pixel-based scrolling behaviour hack no longer works!");
            }

            property.SetValue(vsp, (bool)e.NewValue, new object[0]);
        }
    }
}