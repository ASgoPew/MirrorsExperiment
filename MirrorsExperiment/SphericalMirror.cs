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
            set => _Radius = (float)Math.Max(value, MyExtensions.PointDistance(P1, P2) / 2f + 0.2);
        }

        public SphericalMirror(Point p1, Point p2, int radius = 1000)
            : base(p1, p2)
        {
            Radius = radius;
        }

        public override void Draw(Graphics g)
        {
            Radius = Radius;
            //Radius = (float)(MyExtensions.PointDistance(P1, P2) / 2f + 0.1);

            Pen pen = new Pen(Color.Red, 3);
            float nx = P2.Y - P1.Y;
            float ny = P1.X - P2.X;
            PointF center = SegmentCenter;
            //g.DrawLine(new Pen(Color.Blue, 1), new Point((int)center.X, (int)center.Y), new Point((int)(center.X + nx), (int)(center.Y + ny)));
            float r = (float)MyExtensions.PointDistance(P1, P2) / 2f;
            float R = Radius;
            float t = (float)Math.Sqrt((R * R - r * r) / (nx * nx + ny * ny));
            PointF circleCenter = new PointF((float)(center.X + nx * t), (float)(center.Y + ny * t));

            float cos1 = (P1.X - circleCenter.X) / R;
            float sin1 = (P1.Y - circleCenter.Y) / R;
            float cos2 = (P2.X - circleCenter.X) / R;
            float sin2 = (P2.Y - circleCenter.Y) / R;
            float angle1 = (float)(Math.Acos(cos1) * 180 / Math.PI);
            if (sin1 < 0)
                angle1 = 360 - angle1;
            float angle2 = (float)(Math.Acos(cos2) * 180 / Math.PI);
            if (sin2 < 0)
                angle2 = 360 - angle2;
            double angleBegin, angleDiff;
            if (angle1 >= angle2)
            {
                if (angle1 - angle2 <= 180)
                {
                    angleBegin = angle2;
                    angleDiff = angle1 - angle2;
                }
                else
                {
                    angleBegin = angle1;
                    angleDiff = 360 - (angle1 - angle2);
                }
            }
            else
            {
                if (angle2 - angle1 <= 180)
                {
                    angleBegin = angle1;
                    angleDiff = angle2 - angle1;
                }
                else
                {
                    angleBegin = angle2;
                    angleDiff = 360 - (angle2 - angle1);
                }
            }

            g.DrawArc(pen, new Rectangle((int)(circleCenter.X - R), (int)(circleCenter.Y - R), (int)(2 * R), (int)(2 * R)),
                (float)angleBegin, (float)angleDiff);

            //g.DrawRectangle(new Pen(Color.Black, 1), new Rectangle((int)(circleCenter.X - R), (int)(circleCenter.Y - R), (int)(2 * R), (int)(2 * R)));
            //g.DrawLine(pen, P1, circleCenter);
            //g.DrawLine(pen, P2, circleCenter);
            //g.DrawLine(pen, P1, P2);
        }
    }
}
