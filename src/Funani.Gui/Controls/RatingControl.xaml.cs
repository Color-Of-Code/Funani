
namespace Funani.Gui.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Interaction logic for RatingControl.xaml
    /// </summary>
    public partial class RatingControl : UserControl
    {
        public static readonly DependencyProperty RatingValueProperty =
            DependencyProperty.Register("RatingValue", typeof(int?), typeof(RatingControl),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(RatingValueChanged)));


        private int _maxValue = 5;

        public int? RatingValue
        {
            get { return (int)GetValue(RatingValueProperty); }
            set
            {
                if (value.HasValue)
                {
                    if (value < 0)
                    {
                        SetValue(RatingValueProperty, 0);
                    }
                    else if (value > _maxValue)
                    {
                        SetValue(RatingValueProperty, _maxValue);
                    }
                    else
                    {
                        SetValue(RatingValueProperty, value);
                    }
                }
                else
                {
                    SetValue(RatingValueProperty, value);
                }
            }
        }

        private static void RatingValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            RatingControl parent = sender as RatingControl;
            int? ratingValue = (int?)e.NewValue;
            UIElementCollection children = ((Grid)(parent.Content)).Children;
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

        public RatingControl()
        {
            InitializeComponent();
        }

        private void RatingButtonClickEventHandler(Object sender, RoutedEventArgs e)
        {
            ToggleButton button = sender as ToggleButton;

            int newRating = int.Parse((String)button.Tag);

            if ((bool)button.IsChecked || newRating < RatingValue)
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
