using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MirrorsExperiment
{
    public class SphericalMirror : Wall
    {
        private float _Radius = 1000;
        public float Radius
        {
            get => _Radius;
            set => _Radius = value >= 0
                ? (float)Math.Min(Math.Max(value,  (MyExtensions.PointDistance(P1, P2) / 2f + 0.2)), 10000)
                : (float)Math.Max(Math.Min(value, -(MyExtensions.PointDistance(P1, P2) / 2f + 0.2)), -10000);
        }
        public Point P3;
        public const int P3delta = 10;
        public PointF CircleCenter;

        public SphericalMirror(Point p1, Point p2, int radius = 1000)
            : base(p1, p2)
        {
            Radius = radius;
        }

        public override void Draw(Graphics g)
        {
            //Radius = (float)(MyExtensions.PointDistance(P1, P2) / 2f + 0.1);
            Radius = Radius;
            Point p1 = Radius > 0 ? P1 : P2;
            Point p2 = Radius > 0 ? P2 : P1;
            Pen pen = new Pen(Color.Black, 3);
            float nx = p2.Y - p1.Y;
            float ny = p1.X - p2.X;
            float nLength = (float)Math.Sqrt(nx * nx + ny * ny);
            PointF center = SegmentCenter;
            float r = (float)MyExtensions.PointDistance(p1, p2) / 2f;
            float R = Math.Abs(Radius);
            float t = (float)Math.Sqrt(R * R - r * r) / nLength;
            CircleCenter = new PointF((float)(center.X + nx * t), (float)(center.Y + ny * t));

            float cos1 = (p1.X - CircleCenter.X) / R;
            float sin1 = (p1.Y - CircleCenter.Y) / R;
            float cos2 = (p2.X - CircleCenter.X) / R;
            float sin2 = (p2.Y - CircleCenter.Y) / R;
            float angle1 = (float)(Math.Acos(cos1) * 180 / Math.PI);
            if (sin1 < 0)
                angle1 = 360 - angle1;
            float angle2 = (float)(Math.Acos(cos2) * 180 / Math.PI);
            if (sin2 < 0)
                angle2 = 360 - angle2;
            float angleBegin = angle1 >= angle2 && angle1 - angle2 <= 180 || angle1 < angle2 && angle2 - angle1 > 180
                ? angle2
                : angle1;
            float angleDiff = Math.Min(Math.Abs(angle1 - angle2), 360 - Math.Abs(angle1 - angle2));

            g.DrawArc(pen, (int)(CircleCenter.X - R), (int)(CircleCenter.Y - R), (int)(2 * R), (int)(2 * R),
                angleBegin, angleDiff);

            float t2 = -(float)R / nLength;
            P3 = new Point((int)(CircleCenter.X + nx * t2), (int)(CircleCenter.Y + ny * t2));
            g.DrawEllipse(new Pen(Color.Red, 0.5f), P3.X - P3delta, P3.Y - P3delta, 2 * P3delta, 2 * P3delta);
        }
    }
}
