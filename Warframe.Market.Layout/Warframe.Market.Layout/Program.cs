using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Warframe.Market.Layout
{
    static class Program
    {
        public static bool scanRunning = false;
        public static bool paused = false;
        public static bool settingsOpen = false;
        public static bool hotdealsPaused = false;
        public static bool buyorderdealsPaused = false;
        public static bool setprofitsPaused = false;
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainFrame());
        }
    }
}
