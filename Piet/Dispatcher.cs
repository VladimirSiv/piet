using System;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Piet.Core;
using System.Collections.Generic;
using System.Windows.Media;

namespace Piet
{
    public sealed class Dispatcher
    {
        private static Dispatcher instance = null;
        public static ArrangementMode arrangementMode;
        public static HandlingType handlingType;
        public static int refreshTime;
        public static int columnsRowsNum;
        public static int offsetSize;
        public static int cornerRadius;
        public static String folderPath;
        public static int[] offsetColor;
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        private Dispatcher()
        {
            dispatcherTimer.Tick += new EventHandler(TickEventHandlerAsync);
            SetInitialValuesFromSettingPage();
        }

        public void Start()
        {
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0);
            dispatcherTimer.Start();
        }

        public void Stop()
        {
            dispatcherTimer.Stop();
        }

        private async void TickEventHandlerAsync(object sender, EventArgs e)
        {
            // Initially we set Interval to 0 to get the first tick right away
            // But at every other tick we set interval to desire state
            // I guess there's a better way to achieve this...
            // But for now it's ok.
            dispatcherTimer.Interval = new TimeSpan(0, refreshTime, 0);
            await Task.Run(() =>
            {
                ChangeBackground();
            });
        }

        private void ChangeBackground()
        {
            List<String> filePaths = new List<String>(GetFiles(folderPath, "*.jpg|*.jpeg|*.png", SearchOption.AllDirectories));
            Grid grid = new Grid(columnsRowsNum, arrangementMode, handlingType, filePaths, offsetSize, offsetColor, cornerRadius);
            grid.SetBackground();
        }

        private String[] GetFiles(string sourceFolder, string filters, SearchOption searchOption)
        {
            return filters.Split('|').SelectMany(filter =>
            {
                return Directory.GetFiles(sourceFolder, filter, searchOption);
            }).ToArray();
        }

        public static Dispatcher GetInstance()
        {
            if (instance == null)
            {
                instance = new Dispatcher();
            }
            return instance;
        }

        private void SetInitialValuesFromSettingPage()
        {
            offsetSize = Properties.Settings.Default.offsetSize;

            var handlingTypeSettings = Properties.Settings.Default.handlingType;
            handlingType = Enum.IsDefined(typeof(HandlingType), handlingTypeSettings)
                ? (HandlingType)handlingTypeSettings
                : HandlingType.Omit;

            Color color = ((SolidColorBrush)new BrushConverter().ConvertFromString(Properties.Settings.Default.offsetBackgroundColor)).Color;
            offsetColor = new int[] { color.R, color.G, color.B };

            cornerRadius = Properties.Settings.Default.imageCornerRadius;
        }
    }
}
