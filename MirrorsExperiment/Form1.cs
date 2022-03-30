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
                if (segmentIndex >= 0)
                    e.Graphics.DrawLine(pen, source, old);
            }
        }

        private bool down = false;
        private int pointIndex = -1;
        private Point old;
        private int segmentIndex = -1;
        private Point source;
        // Обработка начала нажатия
        private void drawPanel_MouseDown(object sender, MouseEventArgs e)
        {
            const int R = 10;
            down = true;
            for (int i = 0; i < Experiment.Room.Walls.Count; i++)
            {
                Point p1 = e.Location;
                Point p2 = Experiment.Room.Walls[i].P1;
                if (MyExtensions.PointDistance(p1, p2) < R && e.Button.HasFlag(MouseButtons.Left))
                {
                    pointIndex = i;
                    old = p1;
                    return;
                }
            }
            for (int i = 0; i < Experiment.Room.Walls.Count; i++)
            {
                Point p1 = e.Location;
                Point p2 = Experiment.Room.Walls[i].P1;
                Point p3 = Experiment.Room.Walls[i].P2;
                Point p4 = MyExtensions.PointToSegmentProject(p2, p3, p1);
                if (MyExtensions.PointInRect(p4, p2, p3) && MyExtensions.PointDistance(p1, p4) < R)
                {
                    if (e.Button.HasFlag(MouseButtons.Left))
                    {
                        segmentIndex = i;
                        source = p4;
                        drawPanel.Invalidate();
                    }
                    else
                    {
                        
                    }
                    return;
                }
            }
        }

        // Обработка перемещающегося нажатия
        private void drawPanel_MouseMove(object sender, MouseEventArgs e)
        {
            const int R = 10;
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
                    if (Experiment.Room.Walls[segmentIndex] is SphericalMirror wall)
                    {
                        Point p1 = e.Location;
                        Point p2 = wall.P1;
                        Point p3 = wall.P2;
                        Point segCenter = new Point((p2.X + p3.X)/2, (p2.Y + p3.Y)/2);
                        //wall.Radius = ;
                        //Point p4 = project(p2, p3, p1);
                        drawPanel.Invalidate();
                    }
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
        }
    }
}
