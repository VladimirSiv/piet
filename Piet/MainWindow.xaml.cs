using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;

namespace Piet
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            if (App.isOnStartup) this.Hide(); else this.Show();
            MainFrame.Content = new MainPage();
        }
        private void HeaderMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void CloseIconClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            App.Notify.ShowBalloonTip(2000, "Piet", "Continues working in the background", ToolTipIcon.Info);
            this.Hide();
        }

        private void MinimizeIconClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.WindowState != WindowState.Minimized)
            {
                this.WindowState = WindowState.Minimized;
            }
        }

        private void MinimizeIconMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            minimizeBtn.Background = System.Windows.Application.Current.FindResource("OptionTileTextColor") as SolidColorBrush;
        }

        private void MinimizeIconMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            minimizeBtn.Background = System.Windows.Application.Current.FindResource("MinimizeCloseTileBackgroundColor") as SolidColorBrush;
        }

        private void CloseIconMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            closeBtn.Background = System.Windows.Application.Current.FindResource("OptionTileTextColor") as SolidColorBrush;
        }

        private void CloseIconMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            closeBtn.Background = System.Windows.Application.Current.FindResource("MinimizeCloseTileBackgroundColor") as SolidColorBrush;

        }
    }
}
