using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MirrorsExperiment
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            do
            {
                MyExtensions.machine_epsilon /= 2.0d;
            }
            while ((double)(1.0 + MyExtensions.machine_epsilon) != 1.0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Form1 form1 = new Form1();
            form1.WindowState = FormWindowState.Maximized;
            Experiment experiment = new Experiment(form1, 4);

            Application.Run(form1);
        }
    }
}
