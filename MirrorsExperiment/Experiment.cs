using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MirrorsExperiment
{
    public static class Experiment
    {
        public static Random Random = new Random();
        public static Room Room = new Room();
        public static LightSource LightSource = new LightSource();

        // Инициализация комнаты начальными параметрами
        public static void InitializeRoom(Form1 form1, int wallsCount)
        {
            Room.Walls.Clear();
            Panel panel = form1.drawPanel;
            Point p1 = new Point(Random.Next(0, panel.Width / 2), Random.Next(0, panel.Height / 2));
            Point p2 = new Point(Random.Next(0, panel.Width / 2), Random.Next(panel.Height / 2, panel.Height));
            Point p3 = new Point(Random.Next(panel.Width / 2, panel.Width), Random.Next(panel.Height / 2, panel.Height));
            Point p4 = new Point(Random.Next(panel.Width / 2, panel.Width), Random.Next(0, panel.Height / 2));
            Point p5 = new Point(Random.Next(panel.Width / 2, panel.Width), Random.Next(0, panel.Height / 2));
            Room.Walls.Add(new FlatMirror(p1, p2));
            Room.Walls.Add(new FlatMirror(p2, p3));
            Room.Walls.Add(new SphericalMirror(p3, p4, 0));
            Room.Walls.Add(new FlatMirror(p4, p5));
            Room.Walls.Add(new SphericalMirror(p5, p1, 0));
        }
    }
}
