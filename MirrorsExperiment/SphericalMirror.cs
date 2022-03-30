using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsExperiment
{
    public class SphericalMirror : Wall
    {
        private int _Radius = 10000;
        public int Radius
        {
            get => _Radius;
            set => _Radius = (int)Math.Max(value, MyExtensions.PointDistance(SegmentCenter, P1));
        }

        public SphericalMirror(Point p1, Point p2)
            : base(p1, p2)
        {

        }

        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(Color.Red, 3);
            double nx = P2.Y - P1.Y;
            double ny = P1.X - P2.X;
            Point center = SegmentCenter;
            double r = MyExtensions.PointDistance(center, P1) / 2.0;
            double t = Math.Sqrt((Radius * Radius - r * r) / (nx * nx + ny * ny));
            Point circleCenter = new Point((int)(center.X + nx * t), (int)(center.Y + ny * t));
            double vecX1 = (P1.X - circleCenter.X) / Radius;
            double vecY1 = (P1.Y - circleCenter.Y) / Radius;
            double vecX2 = (P2.X - circleCenter.X) / Radius;
            double vecY2 = (P2.Y - circleCenter.Y) / Radius;
            double startAngle = Math.Acos(vecX1);
            double endAngle = Math.Acos(vecX2);
            g.DrawArc(pen, new Rectangle(circleCenter.X - Radius, circleCenter.Y - Radius, 2 * Radius, 2 * Radius),
                (float)endAngle, (float)startAngle);

            //g.DrawLine(pen, P1, circleCenter);
            //g.DrawLine(pen, P2, circleCenter);
            //g.DrawLine(pen, P1, P2);
        }
    }
}
