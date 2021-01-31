using Microsoft.Win32;
using Piet.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Piet
{
    public partial class SettingsPage : Page
    {
        private String offsetBackgroundColor;
        private int offsetSize;
        private int cornerRadius;
        private HandlingType handlingType;
        private bool runOnStartup;

        public SettingsPage()
        {
            InitializeComponent();
            SetSettingsValues();
        }

        private void SetSettingsValues()
        {
            offsetSize = Properties.Settings.Default.offsetSize;
            offsetSizeBox.SetValue(offsetSize);
            Piet.Dispatcher.offsetSize = offsetSize;

            var handlingTypeSettings = Properties.Settings.Default.handlingType;
            handlingType = Enum.IsDefined(typeof(HandlingType), handlingTypeSettings)
                ? (HandlingType)handlingTypeSettings
                : HandlingType.Omit;
            if (handlingType == HandlingType.Omit) SetOmitHandlingType(); else SetCutOffHandlingType();

            offsetBackgroundColor = Properties.Settings.Default.offsetBackgroundColor;
            colorToggleButton.Background = (SolidColorBrush)new BrushConverter().ConvertFromString(offsetBackgroundColor);

            cornerRadius = Properties.Settings.Default.imageCornerRadius;
            imageCornerRadiusBox.SetValue(cornerRadius);
            Piet.Dispatcher.cornerRadius = cornerRadius;

            runOnStartup = Properties.Settings.Default.runOnStartup;
            if (runOnStartup) SetRunOnStartupEnabled(); else SetRunOnStartupDisabled();
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.offsetSize = offsetSize;
            Properties.Settings.Default.offsetBackgroundColor = offsetBackgroundColor;
            Properties.Settings.Default.imageCornerRadius = cornerRadius;
            Properties.Settings.Default.handlingType = Convert.ToInt32(handlingType);
            Properties.Settings.Default.runOnStartup = runOnStartup;
            Properties.Settings.Default.Save();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            Page mainPage = new MainPage();
            NavigationService.Navigate(mainPage);
            SaveSettings();
        }

        private void ColorPickerChanged(Color e)
        {
            if (colorPicker.mainColor != null)
            {
                System.Windows.Media.Color color = colorPicker.mainColor;
                Piet.Dispatcher.offsetColor = new int[] { color.R, color.G, color.B };
                colorToggleButton.Background = new SolidColorBrush(color);
                offsetBackgroundColor = new BrushConverter().ConvertToString(color);
            }
        }

        private void ImageCornerRadiusChanged(int value)
        {
            cornerRadius = value;
            Piet.Dispatcher.cornerRadius = value;
        }

        private void OffsetSizeChanged(int value)
        {
            offsetSize = value;
            Piet.Dispatcher.offsetSize = value;
        }

        private void CutOffImageMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (handlingType == HandlingType.Omit)
                cutOffImage.Source = new BitmapImage(new Uri("/resources/Columns_Selected.png", UriKind.Relative));
        }

        private void CutOffImageMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (handlingType == HandlingType.Omit)
                cutOffImage.Source = new BitmapImage(new Uri("/resources/Columns_Deselected.png", UriKind.Relative));
        }

        private void CutOffImageLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (handlingType == HandlingType.Omit)
            {
                SetCutOffHandlingType();
            }
        }

        private void SetCutOffHandlingType()
        {
            handlingType = HandlingType.CutOff;
            Piet.Dispatcher.handlingType = HandlingType.CutOff;
            cutOffImage.Source = new BitmapImage(new Uri("/resources/Columns_Selected.png", UriKind.Relative));
            cutOffImageLabel.Foreground = System.Windows.Application.Current.FindResource("ColumnsRowsSelectedTextColor") as SolidColorBrush;
            omitImage.Source = new BitmapImage(new Uri("/resources/Omit_Deselected.png", UriKind.Relative));
            omitImageLabel.Foreground = System.Windows.Application.Current.FindResource("ColumnsRowsDeselectedTextColor") as SolidColorBrush;
        }

        private void OmitImageEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (handlingType == HandlingType.CutOff)
                omitImage.Source = new BitmapImage(new Uri("/resources/Omit_Selected.png", UriKind.Relative));
        }

        private void OmitImageMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (handlingType == HandlingType.CutOff)
                omitImage.Source = new BitmapImage(new Uri("/resources/Omit_Deselected.png", UriKind.Relative));
        }

        private void OmitImageLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (handlingType == HandlingType.CutOff)
            {
                SetOmitHandlingType();
            }
        }

        private void SetOmitHandlingType()
        {
            handlingType = HandlingType.Omit;
            Piet.Dispatcher.handlingType = HandlingType.Omit;
            cutOffImage.Source = new BitmapImage(new Uri("/resources/Columns_Deselected.png", UriKind.Relative));
            cutOffImageLabel.Foreground = System.Windows.Application.Current.FindResource("ColumnsRowsDeselectedTextColor") as SolidColorBrush;
            omitImage.Source = new BitmapImage(new Uri("/resources/Omit_Selected.png", UriKind.Relative));
            omitImageLabel.Foreground = System.Windows.Application.Current.FindResource("ColumnsRowsSelectedTextColor") as SolidColorBrush;
        }

        private void SaveButtonMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            saveButton.Background = System.Windows.Application.Current.FindResource("HomeSettingsTileHoverBackgroundColor") as SolidColorBrush;
        }

        private void SaveButtonMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            saveButton.Background = System.Windows.Application.Current.FindResource("HomeSettingsTileBackgroundColor") as SolidColorBrush;
        }

        private void RunOnStartupBoxMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!runOnStartup)
            {
                runOnStartupBox.Background = Brushes.Black;
            }
        }

        private void RunOnStartupBoxMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!runOnStartup)
            {
                runOnStartupBox.Background = Brushes.White;
            }
        }

        private void RunOnStartupLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (!runOnStartup)
            {
                try
                {
                    regKey.SetValue("Piet", System.Windows.Forms.Application.ExecutablePath.ToString() + " startup");
                    App.Notify.ShowBalloonTip(2000, "Piet", "Will run on Windows startup", ToolTipIcon.Info);
                    runOnStartup = true;
                    SetRunOnStartupEnabled();
                }
                catch (Exception)
                {
                    App.Notify.ShowBalloonTip(2000, "Piet", "Failed to set startup option", ToolTipIcon.Info);
                }
            }
            else
            {
                if (regKey.GetValue("Piet") != null)
                {
                    regKey.DeleteValue("Piet");
                    runOnStartup = false;
                    SetRunOnStartupDisabled();
                    App.Notify.ShowBalloonTip(2000, "Piet", "Startup option disabled", ToolTipIcon.Info);
                }
                else
                {
                    App.Notify.ShowBalloonTip(2000, "Piet", "Registry key not present. Re-enable startup option to fix the problem.", ToolTipIcon.Info);
                    runOnStartup = false;
                    SetRunOnStartupDisabled();
                }
            }
            SaveSettings();
        }

        private void SetRunOnStartupEnabled()
        {
            runOnStartupBox.Background = Brushes.Black;
        }

        private void SetRunOnStartupDisabled()
        {
            runOnStartupBox.Background = Brushes.White;
        }

        private void colorToggleButtonLeftMouseButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            colorPopUp.IsOpen = true;
        }
    }
}
