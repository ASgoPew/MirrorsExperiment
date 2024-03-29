﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MirrorsExperiment
{
    public partial class Form1 : Form
    {
        public Experiment Experiment;

        public Form1()
        {
            InitializeComponent();
            drawPanel.SetDoubleBuffered();
        }

        // Кнопка запуска моделирования
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (Experiment.timer.Enabled)
                Experiment.timer.Stop();
            else
            {
                if (Experiment.LightSource is LightBeam light)
                {
                    light.CalculateEnd(Experiment);
                    drawPanel.Invalidate();
                    Experiment.timer.Start();
                }
            }
        }

        // Отрисовка моделируемой комнаты
        private void drawPanel_Paint(object sender, PaintEventArgs e)
        {
            const int R = 5;
            foreach (Wall wall in Experiment.Room.Walls)
            {
                e.Graphics.FillEllipse(Brushes.Red, wall.P1.X - R, wall.P1.Y - R, 2 * R, 2 * R);
                wall.Draw(e.Graphics);
            }
            if (source is Point p)
                e.Graphics.DrawLine(new Pen(Color.Black, 5), p, sourceEnd.Value);
            if (tmp != null)
                e.Graphics.DrawString(tmp, SystemFonts.DefaultFont, Brushes.Black, 100, 30);
            if (Experiment.LightSource is LightBeam light)
            {
                while (light != null)
                {
                    if (light.Colliding != null)
                    {
                        /*Point project = new Point();
                        if (MyExtensions.PointToSegmentProject(light.Colliding.P1, light.Colliding.P2, light.P1, ref project))
                            e.Graphics.FillEllipse(Brushes.Gray, project.X - R, project.Y - R, 2 * R, 2 * R);*/
                    }
                    e.Graphics.DrawLine(light.Next == null ? new Pen(Color.Red, 6) : new Pen(Color.DeepSkyBlue, 2), light.P1, light.P2);
                    light = light.Next;
                }
            }
        }

        private string tmp = null;
        public void show(string text)
        {
            tmp = text;
            drawPanel.Invalidate();
        }

        private bool down = false;
        private int pointIndex = -1;
        private Point old;
        private int segmentIndex = -1;
        private Point? source = null;
        private Point? sourceEnd = null;
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
                    Reset();
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
                            Reset();
                            down = true;
                            segmentIndex = i;
                            old = p1;
                            return;
                        }
                        else if (e.Button.HasFlag(MouseButtons.Middle))
                        {
                            Reset();
                            Experiment.Room.ChangeWallType(i);
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
                        Point p4 = new Point();
                        if (MyExtensions.PointToSegmentProject(p2, p3, p1, ref p4)
                            && MyExtensions.PointInRect(p4, p2, p3)
                            && MyExtensions.PointDistance(p1, p4) < R)
                        {
                            Reset();
                            Experiment.Room.ChangeWallType(i);
                            drawPanel.Invalidate();
                            return;
                        }
                    }

            // Light source
            if (e.Button.HasFlag(MouseButtons.Right))
            {
                down = true;
                source = e.Location;
                sourceEnd = e.Location;
                //old = e.Location;
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
                    Point C = new Point();
                    if (MyExtensions.PointToSegmentProject(p2, p3, p1, ref C))
                    {
                        float d = (float)Math.Min(MyExtensions.PointDistance(p1, C), r);
                        mirror.Radius = (-MyExtensions.LineSide(p2, p3, p1)) * (r * r + d * d) / (2 * d);
                        drawPanel.Invalidate();
                    }
                }
                else if (source != null)
                {
                    sourceEnd = e.Location;
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
            if (e.Button.HasFlag(MouseButtons.Right) && source is Point lightSource)
            {
                if (MyExtensions.PointDistance(e.Location, lightSource) < 10)
                {
                    source = null;
                    Reset();
                }
                else
                {
                    sourceEnd = e.Location;
                    Experiment.LightSource = new LightBeam(lightSource, e.Location);
                    Experiment.LightSource.CalculateEnd(Experiment);
                    drawPanel.Invalidate();
                }
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            source = null;
            Experiment.timer.Stop();
            Experiment.Initialize(trackBar1.Value, drawPanel.Width, drawPanel.Height);
            Reset();
        }

        private void Reset()
        {
            Experiment.timer.Stop();
            Experiment.LightSource = null;
            if (source is Point p1 && sourceEnd is Point p2)
            {
                Experiment.LightSource = new LightBeam(p1, p2);
                Experiment.LightSource.CalculateEnd(Experiment);
            }
            drawPanel.Invalidate();
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            Experiment.PointStopDistance = trackBar2.Value;
        }

        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
            if (trackBar3.Value == 0)
            {
                Experiment.Instant = true;
                numericUpDown1.Enabled = true;
            }
            else
            {
                Experiment.Instant = false;
                numericUpDown1.Enabled = false;
                Experiment.timer.Interval = trackBar3.Value;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Experiment.InstantSteps = (int)numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (Stream s = File.Open(dialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                using (BinaryWriter bw = new BinaryWriter(s))
                {
                    bw.Write((int)Experiment.Room.Walls.Count);
                    foreach (var wall in Experiment.Room.Walls)
                        wall.Write(bw);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Experiment.Room.Walls.Clear();
                using (Stream s = dialog.OpenFile())
                using (BinaryReader br = new BinaryReader(s))
                {
                    int count = br.ReadInt32();
                    for (int i = 0; i < count; i++)
                        Experiment.Room.Walls.Add(Wall.Read(Experiment, br));
                }
                drawPanel.Invalidate();
            }
        }
    }
}
