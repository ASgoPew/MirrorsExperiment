using System;
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
            Point p2 = P1;
            p2.X += (int)(Math.Cos(Angle * Math.PI / 180) * VectorLen);
            p2.Y += (int)(Math.Sin(Angle * Math.PI / 180) * VectorLen);

            //PointF intersection = new PointF();
            PointF closest = p2;
            foreach (var wall in experiment.Room.Walls)
            {
                PointF intersection = new PointF();
                if (wall != CurrentWall && wall.Intersect(P1, p2, ref intersection)
                    && MyExtensions.PointDistance(P1, intersection) < MyExtensions.PointDistance(P1, closest))
                {
                    closest = intersection;
                    Colliding = wall;
                }
            }
            P2 = new Point((int)closest.X, (int)closest.Y);
        }

        public LightBeam GenerateNext(Experiment experiment)
        {
            if (Colliding == null)
                return null;
            Point projection = MyExtensions.PointToSegmentProject(Colliding.P1, Colliding.P2, P1);
            Point simmetrical = new Point(projection.X + (projection.X - P1.X), projection.Y + (projection.Y - P1.Y));
            Next = new LightBeam(P2, new Point(P2.X + (P2.X - simmetrical.X), P2.Y + (P2.Y - simmetrical.Y)));
            Next.CurrentWall = Colliding;
            Next.CalculateEnd(experiment);
            return Next;
        }
    }
}