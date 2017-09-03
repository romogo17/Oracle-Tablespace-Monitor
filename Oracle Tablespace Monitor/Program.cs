using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oracle_Tablespace_Monitor
{
    using LiveCharts; //Core of the library
    using LiveCharts.Wpf; //The WPF controls
    using LiveCharts.WinForms; //the WinForm wrappers
    using System.Threading;

    static class Program
    {
        public static SplashForm splashForm = null;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            Thread splashThread = new Thread(new ThreadStart(
            delegate
            {
                splashForm = new SplashForm();
                System.Windows.Forms.Application.Run(splashForm);
            }
            ));

            splashThread.SetApartmentState(ApartmentState.STA);
            splashThread.Start();

            //run form - time taking operation
            Application mainForm = new Application();
            mainForm.Load += new EventHandler(mainForm_Load);
            System.Windows.Forms.Application.Run(mainForm);


            //System.Windows.Forms.Application.Run(new Application());
        }

        static void mainForm_Load(object sender, EventArgs e)
        {
            //close splash
            if (splashForm == null)
            {
                return;
            }

            splashForm.Invoke(new Action(splashForm.Close));
            //splashForm.Dispose();
            splashForm = null;
        }
    }
}
