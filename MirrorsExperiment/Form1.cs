using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
                e.Graphics.DrawLine(pen, wall.P1, wall.P2);
                if (segmentIndex >= 0)
                    e.Graphics.DrawLine(pen, source, old);
            }
        }

        // Проекция точки на прямую
        private Point project(Point line1, Point line2, Point toProject)
        {
            double m = (double)(line2.Y - line1.Y) / (line2.X - line1.X);
            double b = (double)line1.Y - (m * line1.X);

            double x = (m * toProject.Y + toProject.X - m * b) / (m * m + 1);
            double y = (m * m * toProject.Y + m * toProject.X + b) / (m * m + 1);

            return new Point((int)x, (int)y);
        }

        // Расстояние между точками
        private double distance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        // Проверка на вложенность точки в прямоугольник
        private bool inside(Point p, Point p1, Point p2)
        {
            double xmax = Math.Max(p1.X, p2.X);
            double xmin = Math.Min(p1.X, p2.X);
            double ymax = Math.Max(p1.Y, p2.Y);
            double ymin = Math.Min(p1.Y, p2.Y);
            return p.X <= xmax && p.X >= xmin &&
                p.Y <= ymax && p.Y >= ymin;
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
                if (distance(p1, p2) < R && e.Button.HasFlag(MouseButtons.Left))
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
                Point p4 = project(p2, p3, p1);
                if (inside(p4, p2, p3) && distance(p1, p4) < R)
                {
                    if (e.Button.HasFlag(MouseButtons.Right))
                    {
                        segmentIndex = i;
                        source = p4;
                        drawPanel.Invalidate();
                    }
                    else
                    {
                        Form2 f = new Form2();
                        if (f.ShowDialog() == DialogResult.OK)
                        {

                        }
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
                    Point p1 = e.Location;
                    Point p2 = Experiment.Room.Walls[segmentIndex].P1;
                    Point p3 = Experiment.Room.Walls[segmentIndex].P2;
                    Point p4 = project(p2, p3, p1);
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
        }
    }
}
