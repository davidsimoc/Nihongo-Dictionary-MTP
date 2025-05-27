using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace Nihongo_Dictionary
{
    internal class EventBlockerBehavior
    {
        public static readonly DependencyProperty BlockClickProperty =
           DependencyProperty.RegisterAttached(
               "BlockClick",
               typeof(bool),
               typeof(EventBlockerBehavior),
               new PropertyMetadata(false, OnBlockClickChanged));

        public static bool GetBlockClick(DependencyObject obj)
        {
            return (bool)obj.GetValue(BlockClickProperty);
        }

        public static void SetBlockClick(DependencyObject obj, bool value)
        {
            obj.SetValue(BlockClickProperty, value);
        }

        private static void OnBlockClickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.PreviewMouseLeftButtonDown += Element_PreviewMouseLeftButtonDown;
                }
                else
                {
                    element.PreviewMouseLeftButtonDown -= Element_PreviewMouseLeftButtonDown;
                }
            }
        }

        private static void Element_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
