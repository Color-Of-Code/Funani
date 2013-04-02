using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Funani.Gui.Controls
{
    /// <summary>
    ///     Interaction logic for RatingControl.xaml
    /// </summary>
    public partial class RatingControl : UserControl
    {
        public static readonly DependencyProperty RatingValueProperty =
            DependencyProperty.Register("RatingValue", typeof (int?), typeof (RatingControl),
                                        new FrameworkPropertyMetadata(null,
                                                                      FrameworkPropertyMetadataOptions
                                                                          .BindsTwoWayByDefault, RatingValueChanged));


        private const int MaxValue = 5;

        public RatingControl()
        {
            InitializeComponent();
        }

        public int? RatingValue
        {
            get { return (int) GetValue(RatingValueProperty); }
            set
            {
                if (value.HasValue)
                {
                    if (value < 0)
                    {
                        SetValue(RatingValueProperty, 0);
                    }
                    else if (value > MaxValue)
                    {
                        SetValue(RatingValueProperty, MaxValue);
                    }
                    else
                    {
                        SetValue(RatingValueProperty, value);
                    }
                }
                else
                {
                    SetValue(RatingValueProperty, null);
                }
            }
        }

        private static void RatingValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var parent = sender as RatingControl;
            var ratingValue = (int?) e.NewValue;
            Debug.Assert(parent != null, "parent != null");
            UIElementCollection children = ((Grid) (parent.Content)).Children;
            ToggleButton button = null;

            if (ratingValue.HasValue)
            {
                for (int i = 0; i < ratingValue.Value; i++)
                {
                    button = children[i] as ToggleButton;
                    if (button != null)
                        button.IsChecked = true;
                }

                for (int i = ratingValue.Value; i < children.Count; i++)
                {
                    button = children[i] as ToggleButton;
                    if (button != null)
                        button.IsChecked = false;
                }
            }
            else
            {
                for (int i = 0; i < children.Count; i++)
                {
                    button = children[i] as ToggleButton;
                    if (button != null)
                        button.IsChecked = false;
                }
            }
        }

        private void RatingButtonClickEventHandler(Object sender, RoutedEventArgs e)
        {
            var button = sender as ToggleButton;

            Debug.Assert(button != null, "button is null");
            int newRating = int.Parse((String) button.Tag);
            bool isChecked = button.IsChecked ?? false;
            if (isChecked || newRating < RatingValue)
            {
                RatingValue = newRating;
            }
            else
            {
                RatingValue = newRating - 1;
            }

            e.Handled = true;
        }
    }
}