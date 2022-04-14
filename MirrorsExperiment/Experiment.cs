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

            Initialize(drawPanel, wallsCount);
        }

        public void Initialize(Panel drawPanel, int wallsCount)
        {
            Room.Walls.Clear();
            Point first = new Point(Random.Next(0, drawPanel.Width), Random.Next(0, drawPanel.Height));
            Point p1 = first;
            Point p2;
            for (int i = 0; i < wallsCount; i++)
            {
                if (i < wallsCount - 1)
                    p2 = new Point(Random.Next(0, drawPanel.Width), Random.Next(0, drawPanel.Height));
                else
                    p2 = first;
                Room.Walls.Add(new FlatMirror(p1, p2));
                p1 = p2;
            }
        }
    }
}
