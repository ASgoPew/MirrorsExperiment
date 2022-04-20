﻿using System.Collections.Generic;
using System.Drawing;

namespace MirrorsExperiment
{
    public abstract class Wall
    {
        public Point P1 = new Point(0, 0);
        public Point P2 = new Point(0, 0);

        public PointF SegmentCenter => new PointF((P1.X + P2.X) / 2f, (P1.Y + P2.Y) / 2f);

        public Wall(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public abstract void Draw(Graphics g);

        public abstract bool Intersect(Point p1, Point p2, List<PointF> intersections);

        public abstract Point Reflect(Point p1, Point p2);
    }
}