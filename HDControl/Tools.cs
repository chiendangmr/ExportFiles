using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HDControl
{
    public static class Tools
    {
        public static string ColorToHexa(Color color)
        {
            string Rhex = color.R.ToString("X");
            string Ghex = color.G.ToString("X");
            string Bhex = color.B.ToString("X");
            if (Rhex.Length < 2) Rhex = "0" + Rhex;
            if (Ghex.Length < 2) Ghex = "0" + Ghex;
            if (Bhex.Length < 2) Bhex = "0" + Bhex;
            return "0x" + Rhex + Ghex + Bhex; ;
        }

        public static Color HexaToColor(string hexa)
        {
            if (hexa.StartsWith("0x"))
                hexa = hexa.Substring(2);
            int colorInt = int.Parse(hexa, System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(0xff, (colorInt & 0xff0000) >> 16, (colorInt & 0x00ff00) >> 8, colorInt & 0x0000ff);
        }

        public static Color MixColor(Color from, Color to, float percent)
        {
            float amountFrom = 1.0f - percent;

            return Color.FromArgb(
            (int)(from.A * amountFrom + to.A * percent),
            (int)(from.R * amountFrom + to.R * percent),
            (int)(from.G * amountFrom + to.G * percent),
            (int)(from.B * amountFrom + to.B * percent));
        }

        public static bool RunOnly(bool otherPath = false)
        {
            #region Only Run One
            Process currentProcess = Process.GetCurrentProcess();
            string thisProcessName = currentProcess.ProcessName;
            var lstProcess = Process.GetProcesses().Where(o => o.ProcessName == thisProcessName && o.Id != currentProcess.Id && o.Modules.Count > 0 &&
                (otherPath || Path.GetDirectoryName(o.MainModule.FileName).ToLower() == Application.StartupPath.ToLower())).ToList();

            if (lstProcess.Count > 0)
                return false;
            #endregion

            return true;
        }
    }
}
