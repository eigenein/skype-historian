using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace SkypeHistorian.Helpers
{
    internal static class NativeMethods
    {
        private const UInt32 FLASHW_TRAY = 0x00000002;
        private const UInt32 FLASHW_TIMERNOFG = 0x0000000C;
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        public static void MakeTheTaskbarBlinkMyApplication()
        {
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            fInfo.dwFlags = FLASHW_TRAY | FLASHW_TIMERNOFG;
            fInfo.uCount = 5;
            fInfo.dwTimeout = 0;

            FlashWindowEx(ref fInfo);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }
    }
}
