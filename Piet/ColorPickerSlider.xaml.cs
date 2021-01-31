using System.Windows;
using System.Windows.Controls;

namespace Piet
{
    public partial class ColorPickerSlider : UserControl
    {
        public delegate void ColorPickerSliderValueChangedHandler(double value);
        public event ColorPickerSliderValueChangedHandler OnValueChanged;
        protected bool UpdatingValues = false;
        public string FormatString { get; set; }

        public ColorPickerSlider()
        {
            FormatString = "F2";
            InitializeComponent();
        }

        private void SliderOnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var value = Slider.Value;
            if (!UpdatingValues)
            {
                UpdatingValues = true;
                TextBox.Text = value.ToString(FormatString);
                OnValueChanged?.Invoke(value);
                UpdatingValues = false;
            }
        }

        private void TextBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!UpdatingValues)
            {
                var text = TextBox.Text;
                bool OK = false;
                double parsedValue = 0;
                OK = double.TryParse(text, out parsedValue);
                if (OK)
                {
                    UpdatingValues = true;
                    Slider.Value = parsedValue;
                    OnValueChanged?.Invoke(parsedValue);
                    UpdatingValues = false;
                }
            }
        }
    }
}
