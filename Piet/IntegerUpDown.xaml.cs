using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Piet
{
    public partial class IntegerUpDown : UserControl
    {
        public static readonly DependencyProperty MinValueProperty =
        DependencyProperty.Register("MinValue", typeof(int), typeof(IntegerUpDown), new UIPropertyMetadata(null));
        public static readonly DependencyProperty MaxValueProperty =
        DependencyProperty.Register("MaxValue", typeof(int), typeof(IntegerUpDown), new UIPropertyMetadata(null));
        public static readonly DependencyProperty IncrementValueProperty =
        DependencyProperty.Register("IncrementValue", typeof(int), typeof(IntegerUpDown), new UIPropertyMetadata(null));
        public static readonly DependencyProperty FormatStringProperty =
        DependencyProperty.Register("FormatString", typeof(String), typeof(IntegerUpDown), new UIPropertyMetadata(null));

        public delegate void IntegerUpDownChangeHandler(int value);
        public event IntegerUpDownChangeHandler OnValueChange;

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public int IncrementValue
        {
            get { return (int)GetValue(IncrementValueProperty); }
            set { SetValue(IncrementValueProperty, value); }
        }

        public String FormatString
        {
            get { return (String)GetValue(FormatStringProperty); }
            set { SetValue(FormatStringProperty, value); }
        }

        public int Value { get; set; }


        public IntegerUpDown()
        {
            InitializeComponent();
        }

        public void SetValue(int value)
        {
            Value = value;
            textBox.Text = String.Format(FormatString, value);
            if (value >= MaxValue) DisableButtonUp();
            if (value <= MinValue) DisableButtonDown();
        }

        private void ButtonUpClick(object sender, RoutedEventArgs e)
        {
            if (buttonDown.IsHitTestVisible == false)
                EnableButtonDown();
            if (Value < MaxValue)
            {
                Value += IncrementValue;
                textBox.Text = String.Format(FormatString, Value);
            }
        }

        private void ButtonDownClick(object sender, RoutedEventArgs e)
        {
            if (buttonUp.IsHitTestVisible == false)
                EnableButtonUp();
            if (Value > MinValue)
            {
                Value -= IncrementValue;
                textBox.Text = String.Format(FormatString, Value);
            }
        }

        private void TextBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (buttonUp != null && buttonDown != null)
            {
                if (Value > MaxValue) textBox.Text = MaxValue.ToString();
                if (Value == MaxValue) DisableButtonUp();
                if (Value < MinValue) textBox.Text = MinValue.ToString();
                if (Value == MinValue) DisableButtonDown();
                OnValueChange?.Invoke(Value);
            }
        }

        private void EnableButtonDown()
        {
            buttonDown.IsHitTestVisible = true;
            buttonDownArrow.Fill = Brushes.Black;
        }

        private void DisableButtonDown()
        {
            buttonDown.IsHitTestVisible = false;
            buttonDownArrow.Fill = System.Windows.Application.Current.FindResource("OptionTileTextColor") as SolidColorBrush;
        }

        private void EnableButtonUp()
        {
            buttonUp.IsHitTestVisible = true;
            buttonUpArrow.Fill = Brushes.Black;
        }

        private void DisableButtonUp()
        {
            buttonUp.IsHitTestVisible = false;
            buttonUpArrow.Fill = System.Windows.Application.Current.FindResource("OptionTileTextColor") as SolidColorBrush;
        }
    }
}
