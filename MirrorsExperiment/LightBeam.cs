using System;
using System.Collections.Generic;
using System.Drawing;

namespace MirrorsExperiment
{
    public class LightBeam
    {
        public Point P1;
        public Point P2;
        public Wall CurrentWall = null;
        public Wall Colliding = null;
        public double Angle;
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
                if (MyExtensions.PointDistance(wall.P1, closest) <= 1)
                {
                    Colliding = null;
                    break;
                }
        }

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