﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MirrorsExperiment
{
    public class LightBeam
    {
        // Начальная точка сегмента луча
        public Point P1;
        // Конечная точка сегмента луча
        public Point P2;
        // Стена, от которой сегмент луча оттолкнулся
        public Wall CurrentWall = null;
        // Стена, в который сегмент луча уперся
        public Wall Colliding = null;
        // Угол сегмента луча в радианах
        public double Angle;
        // Ссылка на следующий сегмент луча
        public LightBeam Next = null;

        public LightBeam(Point p, double angle)
        {
            P1 = p;
            Angle = angle;
        }

        public LightBeam(Point p1, Point p2)
        {
            P1 = p1;
            double l = MyExtensions.PointDistance(p1, p2);
            double sin = (p2.Y - p1.Y) / l;
            double cos = (p2.X - p1.X) / l;
            Angle = Math.Acos(cos) * 180 / Math.PI;
            if (sin < 0)
                Angle = 360 - Angle;
        }

        // Рассчитать конечную точку сегмента луча
        public void CalculateEnd(Experiment experiment)
        {
            const int VectorLen = 10000;
            if (double.IsNaN(Angle))
            {
                P2 = new Point(-VectorLen, -VectorLen);
                return;
            }

            Point p2 = P1;
            p2.X += (int)(Math.Cos(Angle * Math.PI / 180) * VectorLen);
            p2.Y += (int)(Math.Sin(Angle * Math.PI / 180) * VectorLen);

            //PointF intersection = new PointF();
            PointF closest = p2;
            foreach (var wall in experiment.Room.Walls)
            {
                List<PointF> intersections = new List<PointF>();
                if ((wall != CurrentWall || wall is SphericalMirror)
                        && wall.P1 != wall.P2
                        && wall.Intersect(P1, p2, intersections))
                    foreach (PointF intersection in intersections)
                    {
                        //File.AppendAllText("test.txt", String.Join(", ", intersections) + "\n");
                        double dist = MyExtensions.PointDistance(P1, intersection);
                        if (dist >= 1 && dist < MyExtensions.PointDistance(P1, closest))
                        {
                            closest = intersection;
                            Colliding = wall;
                        }
                    }
            }
            P2 = new Point((int)closest.X, (int)closest.Y);
            foreach (var wall in experiment.Room.Walls)
                if (MyExtensions.PointDistance(wall.P1, closest) <= experiment.PointStopDistance)
                {
                    Colliding = null;
                    break;
                }
        }

        // Сгенерировать следующий луч
        public LightBeam GenerateNext(Experiment experiment)
        {
            if (Colliding == null)
                return null;
            Next = new LightBeam(P2, Colliding.Reflect(P1, P2));
            Next.CurrentWall = Colliding;
            Next.CalculateEnd(experiment);
            return Next;
        }
    }
}