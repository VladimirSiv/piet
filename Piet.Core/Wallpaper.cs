using System;
using System.Runtime.InteropServices;

namespace Piet.Core
{
    public class Wallpaper
    {
        Wallpaper() { }

        internal sealed class Win32
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern int SystemParametersInfo(
                int uAction,
                int uParam,
                String lpvParam,
                int fuWinIni);
        }

        public static void Set(String filepath)
        {
            const int SET_DESKTOP_BACKGROUND = 20;
            const int UPDATE_INI_FILE = 1;
            const int SEND_WINDOWS_INI_CHANGE = 2;
            Win32.SystemParametersInfo(SET_DESKTOP_BACKGROUND, 0, filepath, UPDATE_INI_FILE | SEND_WINDOWS_INI_CHANGE);
        }
    }
}
