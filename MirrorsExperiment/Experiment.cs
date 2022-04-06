using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MirrorsExperiment
{
    public class Experiment
    {
        public static Dictionary<object, Experiment> Instances = new Dictionary<object, Experiment>();

        public Random Random = new Random();
        public Room Room = new Room();
        public LightBeam LightSource = null;

        // Инициализация комнаты начальными параметрами
        public Experiment(Panel drawPanel, int wallsCount)
        {
            Instances[drawPanel] = this;

            Room.Walls.Clear();
            Point p1 = new Point(Random.Next(0, drawPanel.Width / 2), Random.Next(0, drawPanel.Height / 2));
            Point p2 = new Point(Random.Next(0, drawPanel.Width / 2), Random.Next(drawPanel.Height / 2, drawPanel.Height));
            Point p3 = new Point(Random.Next(drawPanel.Width / 2, drawPanel.Width), Random.Next(drawPanel.Height / 2, drawPanel.Height));
            Point p4 = new Point(Random.Next(drawPanel.Width / 2, drawPanel.Width), Random.Next(0, drawPanel.Height / 2));
            Point p5 = new Point(Random.Next(drawPanel.Width / 2, drawPanel.Width), Random.Next(0, drawPanel.Height / 2));
            Room.Walls.Add(new FlatMirror(p1, p2));
            Room.Walls.Add(new FlatMirror(p2, p3));
            //Room.Walls.Add(new SphericalMirror(p3, p4, 0));
            Room.Walls.Add(new FlatMirror(p3, p4));
            Room.Walls.Add(new FlatMirror(p4, p5));
            //Room.Walls.Add(new SphericalMirror(p5, p1, 0));
            Room.Walls.Add(new FlatMirror(p5, p1));
        }
    }
}
