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
        public Random Random = new Random();
        public Room Room = new Room();
        public LightBeam LightSource = null;

        // Инициализация комнаты начальными параметрами
        public Experiment(Form1 form, int wallsCount)
        {
            form.Experiment = this;
            Panel drawPanel = form.drawPanel;
            Initialize(wallsCount, drawPanel.Width, drawPanel.Height);
        }

        public void Initialize(int wallsCount, int width, int height)
        {
            Room.Walls.Clear();
            int R = width / 4;
            Point center = new Point(width / 2, height / 2);
            double _phi = (360 / (wallsCount )) * Math.PI / 180;
            double phi = _phi;
            Point first = new Point(center.X + (int)(Math.Cos(0) * R), center.Y + (int)(Math.Sin(0) * R));
            Point p1 = first;
            Point p2;
            for (int i = 0; i < wallsCount; i++)
            {
                int r = Random.Next(R / 4, (int)(R * 1.3));
                if (i < wallsCount - 1)
                    p2 = new Point(center.X + (int)(Math.Cos(phi) * r), center.Y + (int)(Math.Sin(phi) * r));
                else
                    p2 = first;
                Room.Walls.Add(new FlatMirror(p1, p2));
                p1 = p2;
                phi += _phi;
            }
        }
    }
}
