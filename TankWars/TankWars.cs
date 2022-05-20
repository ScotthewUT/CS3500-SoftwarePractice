// AUTHORS: Scott Crowley (u1178178) & David Gillespie (u0720569)
// VERSION: 24 November 2019

using System;
using System.Windows.Forms;

namespace TankWars
{
    static class TankWars
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ClientController ctrl = new ClientController();
            Application.Run(new ClientViewer(ctrl));
        }
    }
}
