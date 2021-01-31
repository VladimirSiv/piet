using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Piet.Core;

namespace Piet
{
    public partial class MainPage : Page
    {
        private String folderPath;
        private int columnsRowsNum;
        private int refreshTime;
        private ArrangementMode arrangementMode;
        private Dispatcher dispatcher = Piet.Dispatcher.GetInstance();

        public MainPage()
        {
            InitializeComponent();
            if (App.isOnStartup) RunApplicationOnStartup(); else SetSettingsValues();
        }

        private void SetSettingsValues()
        {
            columnsRowsNum = Properties.Settings.Default.columnsRowsNum;
            columnsRowsNumBox.SetValue(columnsRowsNum);

            var arrangementModeSettings = Properties.Settings.Default.arrangementMode;
            arrangementMode = Enum.IsDefined(typeof(ArrangementMode), arrangementModeSettings)
                ? (ArrangementMode)arrangementModeSettings
                : ArrangementMode.Column;
            if (arrangementMode == ArrangementMode.Column) SetColumsArrangementMode(); else SetRowArrangementMode();

            folderPath = Properties.Settings.Default.folderPath;
            folderBox.Text = folderPath;

            if (App.isRunning) SetStartWorkingBtn(); else SetStartInitialBtn();

            if (App.isRunning) DisableFolderBtnBox();

            refreshTime = Properties.Settings.Default.refreshTime;
            refreshTimeBox.SetValue(refreshTime);
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.columnsRowsNum = columnsRowsNum;
            Properties.Settings.Default.folderPath = folderPath;
            Properties.Settings.Default.refreshTime = refreshTime;
            Properties.Settings.Default.arrangementMode = Convert.ToInt32(arrangementMode);
            Properties.Settings.Default.Save();
        }

        private void FolderBtnClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            DialogResult status = folderDlg.ShowDialog();
            if (status == System.Windows.Forms.DialogResult.OK)
            {
                folderBox.Text = folderDlg.SelectedPath;
                folderPath = folderDlg.SelectedPath;
            }
        }

        private void StartBtnLeftButtonDown(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            if (!App.isRunning) RunApplication(); else StopApplication();
        }

        private void RunApplicationOnStartup()
        {
            App.isRunning = true;
            App.isOnStartup = false;
            SetSettingsValues();
            RunApplication();
        }

        private void RunApplication()
        {
            if (!Directory.Exists(folderPath))
            {
                System.Windows.MessageBox.Show("Invalid folder path", "Alert", MessageBoxButton.OK, MessageBoxImage.Error);
                SetStartInitialBtn();
                EnableFolderBtnBox();
            }
            else
            {
                App.isRunning = true;
                DisableFolderBtnBox();
                dispatcher.Start();
            }
        }

        private void StopApplication()
        {
            dispatcher.Stop();
            App.isRunning = false;
            EnableFolderBtnBox();
            startBtnLabel.Content = "START";
            StartBtnBorder.Background = System.Windows.Application.Current.FindResource("StartButtonTileBackgroundColor") as SolidColorBrush;
            startBtnLabel.Foreground = System.Windows.Application.Current.FindResource("StartButtonTileFontColor") as SolidColorBrush;
        }

        private void SettingsBtnLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Page settingsPage = new SettingsPage();
            NavigationService.Navigate(settingsPage);
            SaveSettings();
        }

        private void DisableFolderBtnBox()
        {
            folderBox.IsEnabled = false;
            folderBtn.Background = System.Windows.Application.Current.FindResource("TileBackgroundColor") as SolidColorBrush;
            folderBtnLabel.Foreground = System.Windows.Application.Current.FindResource("OptionTileTextColor") as SolidColorBrush;
            folderBtn.IsHitTestVisible = false;
        }

        private void EnableFolderBtnBox()
        {
            folderBox.IsEnabled = true;
            folderBtn.Background = System.Windows.Application.Current.FindResource("SelectFolderTileBackgroundColor") as SolidColorBrush;
            folderBtnLabel.Foreground = System.Windows.Application.Current.FindResource("StartButtonTileFontColor") as SolidColorBrush;
            folderBtn.IsHitTestVisible = true;
        }

        private void StartBtnEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!App.isRunning)
            {
                StartBtnBorder.Background = System.Windows.Application.Current.FindResource("StartButtonTileFontColor") as SolidColorBrush;
                startBtnLabel.Foreground = System.Windows.Application.Current.FindResource("StartButtonTileFontColorHover") as SolidColorBrush;
            }
            else
            {
                StartBtnBorder.Background = System.Windows.Application.Current.FindResource("SelectFolderTileBackgroundColor") as SolidColorBrush;
                startBtnLabel.Foreground = System.Windows.Application.Current.FindResource("StartButtonTileFontColor") as SolidColorBrush;
                startBtnLabel.Content = "STOP";
            }
        }

        private void StartBtnLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!App.isRunning)
            {
                SetStartInitialBtn();
            }
            else
            {
                SetStartWorkingBtn();
            }
        }

        private void SetStartInitialBtn()
        {
            StartBtnBorder.Background = System.Windows.Application.Current.FindResource("StartButtonTileBackgroundColor") as SolidColorBrush;
            startBtnLabel.Foreground = System.Windows.Application.Current.FindResource("StartButtonTileFontColor") as SolidColorBrush;
            startBtnLabel.Content = "START";
        }

        private void SetStartWorkingBtn()
        {
            StartBtnBorder.Background = System.Windows.Application.Current.FindResource("StartButtonTileBackgroundColor") as SolidColorBrush;
            startBtnLabel.Foreground = System.Windows.Application.Current.FindResource("StartButtonTileFontColor") as SolidColorBrush;
            startBtnLabel.Content = "ACTIVE";
        }

        private void FolderBtnEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            folderBtn.Background = System.Windows.Application.Current.FindResource("HomeSettingsTileBackgroundColor") as SolidColorBrush;
            folderBtnLabel.Foreground = Brushes.Black;
        }

        private void FolderBtnLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            folderBtn.Background = System.Windows.Application.Current.FindResource("SelectFolderTileBackgroundColor") as SolidColorBrush;
            folderBtnLabel.Foreground = Brushes.White;
        }

        private void SettingsBtnEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            settingsBtn.Background = System.Windows.Application.Current.FindResource("StartButtonTileBackgroundColor") as SolidColorBrush;
        }

        private void SettingsBtnLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            settingsBtn.Background = System.Windows.Application.Current.FindResource("HomeSettingsTileBackgroundColor") as SolidColorBrush;
        }

        private void RowsImageEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (arrangementMode == ArrangementMode.Column)
                rowsImage.Source = new BitmapImage(new Uri("/resources/Rows_Selected.png", UriKind.Relative));
        }

        private void RowsImageLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (arrangementMode == ArrangementMode.Column)
                rowsImage.Source = new BitmapImage(new Uri("/resources/Rows_Deselected.png", UriKind.Relative));
        }

        private void ColumnsImageEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (arrangementMode == ArrangementMode.Row)
                columnsImage.Source = new BitmapImage(new Uri("/resources/Columns_Selected.png", UriKind.Relative));
        }

        private void ColumnsImageLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (arrangementMode == ArrangementMode.Row)
                columnsImage.Source = new BitmapImage(new Uri("/resources/Columns_Deselected.png", UriKind.Relative));
        }

        private void ColumnsRowsNumChanged(int value)
        {
            columnsRowsNum = value;
            Piet.Dispatcher.columnsRowsNum = columnsRowsNumBox.Value;
        }

        private void RowsImageMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (arrangementMode == ArrangementMode.Column)
            {
                SetRowArrangementMode();
            }
        }

        private void ColumnsImageMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (arrangementMode == ArrangementMode.Row)
            {
                SetColumsArrangementMode();
            }
        }

        private void SetColumsArrangementMode()
        {
            arrangementMode = ArrangementMode.Column;
            Piet.Dispatcher.arrangementMode = arrangementMode;
            rowsImage.Source = new BitmapImage(new Uri("/resources/Rows_Deselected.png", UriKind.Relative));
            rowsImageLabel.Foreground = System.Windows.Application.Current.FindResource("ColumnsRowsDeselectedTextColor") as SolidColorBrush;
            columnsImage.Source = new BitmapImage(new Uri("/resources/Columns_Selected.png", UriKind.Relative));
            columnsImageLabel.Foreground = System.Windows.Application.Current.FindResource("ColumnsRowsSelectedTextColor") as SolidColorBrush;
        }

        private void SetRowArrangementMode()
        {
            arrangementMode = ArrangementMode.Row;
            Piet.Dispatcher.arrangementMode = arrangementMode;
            rowsImage.Source = new BitmapImage(new Uri("/resources/Rows_Selected.png", UriKind.Relative));
            rowsImageLabel.Foreground = System.Windows.Application.Current.FindResource("ColumnsRowsSelectedTextColor") as SolidColorBrush;
            columnsImage.Source = new BitmapImage(new Uri("/resources/Columns_Deselected.png", UriKind.Relative));
            columnsImageLabel.Foreground = System.Windows.Application.Current.FindResource("ColumnsRowsDeselectedTextColor") as SolidColorBrush;
        }

        private void RefreshTimeChanged(int value)
        {
            refreshTime = value;
            Piet.Dispatcher.refreshTime = refreshTime;
        }

        private void FolderBoxChanged(object sender, TextChangedEventArgs e)
        {
            folderPath = folderBox.Text;
            Piet.Dispatcher.folderPath = folderPath;
        }
    }
}
