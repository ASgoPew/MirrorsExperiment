using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MirrorsExperiment
{
    public partial class Form1 : Form
    {
        public System.Timers.Timer timer = new System.Timers.Timer(30);

        public Form1()
        {
            InitializeComponent();
            drawPanel.SetDoubleBuffered();
            timer.Elapsed += Timer_Elapsed;
        }

        // Расчет лучей по таймеру
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
        }

        // Кнопка запуска моделирования
        private void buttonStart_Click(object sender, EventArgs e)
        {
            timer.Start();
        }

        // Отрисовка моделируемой комнаты
        private void drawPanel_Paint(object sender, PaintEventArgs e)
        {
            const int R = 5;
            Pen pen = new Pen(Color.Black, 3);
            foreach (Wall wall in Experiment.Room.Walls)
            {
                e.Graphics.FillEllipse(Brushes.Red, wall.P1.X - R, wall.P1.Y - R, 2 * R, 2 * R);
                wall.Draw(e.Graphics);
            }
            if (source is Point p)
                e.Graphics.DrawLine(pen, p, old);
            if (tmp != null)
                e.Graphics.DrawString(tmp, SystemFonts.DefaultFont, Brushes.Black, 100, 30);
        }

        private string tmp = null;
        private void show(string text)
        {
            tmp = text;
            drawPanel.Invalidate();
        }

        private bool down = false;
        private int pointIndex = -1;
        private Point old;
        private int segmentIndex = -1;
        private Point? source = null;
        // Обработка начала нажатия
        private void drawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            const int R = 10;
            // Moving points
            for (int i = 0; i < Experiment.Room.Walls.Count; i++)
            {
                Point p1 = e.Location;
                Point p2 = Experiment.Room.Walls[i].P1;
                if (MyExtensions.PointDistance(p1, p2) < R && e.Button.HasFlag(MouseButtons.Left))
                {
                    down = true;
                    pointIndex = i;
                    old = p1;
                    return;
                }
            }

            // Changing spherical mirror radius or type
            for (int i = 0; i < Experiment.Room.Walls.Count; i++)
                if (Experiment.Room.Walls[i] is SphericalMirror mirror)
                {
                    Point p1 = e.Location;
                    Point p2 = mirror.P3;
                    if (MyExtensions.PointDistance(p1, p2) < R)
                    {
                        if (e.Button.HasFlag(MouseButtons.Left))
                        {
                            down = true;
                            segmentIndex = i;
                            old = p1;
                            return;
                        }
                        else if (e.Button.HasFlag(MouseButtons.Middle))
                        {
                            Experiment.Room.Walls[i] = new FlatMirror(mirror.P1, mirror.P2);
                            drawPanel.Invalidate();
                            return;
                        }
                    }
                }

            // Change flat wall type
            if (e.Button.HasFlag(MouseButtons.Middle))
                for (int i = 0; i < Experiment.Room.Walls.Count; i++)
                    if (Experiment.Room.Walls[i] is FlatMirror wall)
                    {
                        Point p1 = e.Location;
                        Point p2 = wall.P1;
                        Point p3 = wall.P2;
                        Point p4 = MyExtensions.PointToSegmentProject(p2, p3, p1);
                        if (MyExtensions.PointInRect(p4, p2, p3) && MyExtensions.PointDistance(p1, p4) < R)
                        {
                            Experiment.Room.Walls[i] = new SphericalMirror(p2, p3, 10000);
                            drawPanel.Invalidate();
                            return;
                        }
                    }

            // Light source
            if (e.Button.HasFlag(MouseButtons.Right))
            {
                down = true;
                source = e.Location;
                old = e.Location;
            }
        }

        // Обработка перемещающегося нажатия
        private void drawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (down)
            {
                if (pointIndex >= 0)
                {
                    Wall w1 = Experiment.Room.Walls[pointIndex];
                    Wall w2 = Experiment.Room.Walls[(pointIndex + Experiment.Room.Walls.Count - 1)
                        % Experiment.Room.Walls.Count];
                    w1.P1.X += e.X - old.X;
                    w1.P1.Y += e.Y - old.Y;
                    w2.P2.X += e.X - old.X;
                    w2.P2.Y += e.Y - old.Y;
                    drawPanel.Invalidate();
                }
                else if (segmentIndex >= 0)
                {
                    SphericalMirror mirror = (SphericalMirror)Experiment.Room.Walls[segmentIndex];
                    Point p1 = e.Location;
                    Point p2 = mirror.P1;
                    Point p3 = mirror.P2;
                    float r = (float)MyExtensions.PointDistance(p2, p3) / 2f;
                    Point C = MyExtensions.PointToSegmentProject(p2, p3, p1);
                    float d = (float)Math.Min(MyExtensions.PointDistance(p1, C), r);
                    mirror.Radius = (-MyExtensions.LineSide(p2, p3, p1)) * (r * r + d * d) / (2 * d);
                    drawPanel.Invalidate();
                }
                else if (source != null)
                {
                    drawPanel.Invalidate();
                }
                old = e.Location;
            }
        }

        // Обработка окончания нажатия
        private void drawPanel_MouseUp(object sender, MouseEventArgs e)
        {
            down = false;
            pointIndex = -1;
            segmentIndex = -1;
            if (source is Point p && MyExtensions.PointDistance(e.Location, p) < 10)
            {
                source = null;
                drawPanel.Invalidate();
            }
        }
    }
}
