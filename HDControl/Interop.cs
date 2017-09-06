using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace HDControl
{
    public static class Interop
    {
        private const int SB_BOTTOM = 7;
        private const int WM_VSCROLL = 0x115;

        public static void ScrollToBottom(this Control tb)
        {
            try
            {
                SendMessage(tb.Handle, 0x115, (IntPtr)7, IntPtr.Zero);
            }
            catch { }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);
    }
}
