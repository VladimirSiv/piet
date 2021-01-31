using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Piet
{
    public partial class ColorPicker : UserControl
    {
        public Color mainColor = Colors.White;
        public delegate void ColorPickerChangeHandler(Color color);
        public event ColorPickerChangeHandler OnPickColor;

        public ColorPicker()
        {
            InitializeComponent();
            RSlider.Slider.Maximum = 255;
            GSlider.Slider.Maximum = 255;
            BSlider.Slider.Maximum = 255;
            RSlider.Label.Content = "R";
            RSlider.Slider.TickFrequency = 1;
            RSlider.Slider.IsSnapToTickEnabled = true;
            GSlider.Label.Content = "G";
            GSlider.Slider.TickFrequency = 1;
            GSlider.Slider.IsSnapToTickEnabled = true;
            BSlider.Label.Content = "B";
            BSlider.Slider.TickFrequency = 1;
            BSlider.Slider.IsSnapToTickEnabled = true;
        }

        public void SetColor(Color color)
        {
            mainColor = color;
            RSlider.Slider.Value = mainColor.R;
            GSlider.Slider.Value = mainColor.G;
            BSlider.Slider.Value = mainColor.B;
            TestBorder.Background = new SolidColorBrush(mainColor);
            OnPickColor?.Invoke(color);
        }

        private void SampleImageOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            {
                Mouse.Capture(this);
                this.MouseMove += ColorPickerControlMouseMove;
                this.MouseUp += ColorPickerControlMouseUp;
            }
        }

        private void ColorPickerControlMouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(sampleImage);
            var img = sampleImage.Source as BitmapSource;

            if (pos.X > 0 && pos.Y > 0 && pos.X < img.PixelWidth && pos.Y < img.PixelHeight)
                SampleImageClick(img, pos);
        }

        private void ColorPickerControlMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            this.MouseMove -= ColorPickerControlMouseMove;
            this.MouseUp -= ColorPickerControlMouseUp;
        }

        protected void SampleImageClick(BitmapSource img, Point pos)
        {
            int stride = (int)img.Width * 4;
            int size = (int)img.Height * stride;
            byte[] pixels = new byte[(int)size];

            img.CopyPixels(pixels, stride, 0);

            var x = (int)pos.X;
            var y = (int)pos.Y;

            int index = y * stride + 4 * x;

            byte red = pixels[index];
            byte green = pixels[index + 1];
            byte blue = pixels[index + 2];
            byte alpha = pixels[index + 3];

            var color = Color.FromArgb(alpha, blue, green, red);
            SetColor(color);
        }

        private void RSliderOnOnValueChanged(double value)
        {
            var val = (byte)value;
            mainColor.R = val;
            SetColor(mainColor);
        }

        private void GSliderOnOnValueChanged(double value)
        {
            var val = (byte)value;
            mainColor.G = val;
            SetColor(mainColor);
        }

        private void BSliderOnOnValueChanged(double value)
        {
            var val = (byte)value;
            mainColor.B = val;
            SetColor(mainColor);
        }

        private void HexColorBoxOnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(hexColorBox.Text);
                SetColor(color);
            }
            catch (System.FormatException)
            {
                // Wrong format, but that's ok.
            }
        }
    }
}
