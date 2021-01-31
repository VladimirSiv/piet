using System;
using System.Drawing;
using System.Windows;
using Forms = System.Windows.Forms;

namespace Piet
{
    public partial class App : Application
    {
        public static bool isRunning { get; set; } = false;
        public static bool isOnStartup = false;
        private static Forms.NotifyIcon notifyIcon;

        public static Forms.NotifyIcon Notify
        {
            get { return notifyIcon; }
        }

        public App()
        {
            notifyIcon = new Forms.NotifyIcon();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            notifyIcon.Icon = new Icon(Application.GetResourceStream(new Uri("pack://application:,,,/resources/logo.ico")).Stream);
            notifyIcon.MouseClick += NotifyIconClick;
            notifyIcon.Text = "Piet";
            notifyIcon.Visible = true;
            notifyIcon.ContextMenuStrip = new Forms.ContextMenuStrip();
            notifyIcon.ContextMenuStrip.Items.Add("Exit", null, NotifyMenuExitClick);

            isOnStartup = false;
            if (e.Args.Length == 1 && e.Args[0].Equals("startup"))
            {
                isOnStartup = true;
            }

            MainWindow mainWindow = new MainWindow();
            base.OnStartup(e);
        }

        private void NotifyIconClick(object sender, Forms.MouseEventArgs e)
        {
            if (e.Button == Forms.MouseButtons.Left)
            {
                MainWindow.WindowState = WindowState.Normal;
                MainWindow.Show();
            }
        }

        private void NotifyMenuExitClick(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void NotifyBallon()
        {
            notifyIcon.ShowBalloonTip(2000, "Piet", "Continues working in the background", Forms.ToolTipIcon.Info);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose();
            base.OnExit(e);
        }
    }
}
