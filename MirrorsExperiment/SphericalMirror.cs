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
        public float AngleBegin;
        public float AngleDiff;

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
            AngleBegin = angle1 >= angle2 && angle1 - angle2 <= 180 || angle1 < angle2 && angle2 - angle1 > 180
                ? angle2
                : angle1;
            AngleDiff = Math.Min(Math.Abs(angle1 - angle2), 360 - Math.Abs(angle1 - angle2));

            g.DrawArc(pen, (int)(CircleCenter.X - R), (int)(CircleCenter.Y - R), (int)(2 * R), (int)(2 * R),
                AngleBegin, AngleDiff);
            g.DrawEllipse(new Pen(Color.Black, 0.4f), (int)(CircleCenter.X - R), (int)(CircleCenter.Y - R), (int)(2 * R), (int)(2 * R));

            float t2 = -(float)R / nLength;
            P3 = new Point((int)(CircleCenter.X + nx * t2), (int)(CircleCenter.Y + ny * t2));
            g.DrawEllipse(new Pen(Color.Red, 0.5f), P3.X - P3delta, P3.Y - P3delta, 2 * P3delta, 2 * P3delta);
        }

        public override bool Intersect(Point p1, Point p2, List<PointF> intersections)
        {
            float R = Math.Abs(Radius);
            float x0 = CircleCenter.X;
            float y0 = CircleCenter.Y;
            float a = p2.Y - p1.Y;
            float b = p1.X - p2.X;
            float c = p1.Y * p2.X - p1.X * p2.Y;
            float alpha = -a / b;
            float beta = -c / b - y0;
            float a2 = 1 + alpha * alpha;
            float a1 = 2 * (-x0 + alpha * beta);
            float a0 = x0 * x0 + beta * beta - R * R;
            float D = a1 * a1 - 4 * a2 * a0;
            if (MyExtensions._less(D, 0))
                return false;
            List<PointF> result = new List<PointF>();
            float x1 = (-a1 + (float)Math.Sqrt(D)) / (2 * a2);
            float y1 = (-a * x1 - c) / b;
            result.Add(new PointF(x1, y1));
            float x2 = (-a1 - (float)Math.Sqrt(D)) / (2 * a2);
            float y2 = (-a * x2 - c) / b;
            result.Add(new PointF(x2, y2));

            foreach (var p in result)
            {
                float cos = (p.X - CircleCenter.X) / R;
                float sin = (p.Y - CircleCenter.Y) / R;
                float angle = (float)(Math.Acos(cos) * 180 / Math.PI);
                if (sin < 0)
                    angle = 360 - angle;

                if (MyExtensions.point_in_rect(p1, p2, p)
                        && ((AngleBegin + AngleDiff < 360 && angle >= AngleBegin && angle <= AngleBegin + AngleDiff)
                        || (AngleBegin + AngleDiff >= 360 && (angle >= AngleBegin || angle <= AngleBegin + AngleDiff - 360))))
                    intersections.Add(p);
            }
            return true;
        }

        public override Point Reflect(Point p1, Point p2)
        {
            float nx = p2.X - CircleCenter.X;
            float ny = p2.Y - CircleCenter.Y;
            Point tangent1 = p2;
            Point tangent2 = new Point(p2.X + (int)(ny * 10000), p2.Y - (int)(nx * 10000));

            Point projection = new Point();
            if (!MyExtensions.PointToSegmentProject(tangent1, tangent2, p1, ref projection))
                throw new Exception();
            Point simmetrical = new Point(projection.X + (projection.X - p1.X), projection.Y + (projection.Y - p1.Y));
            return new Point(p2.X + (p2.X - simmetrical.X), p2.Y + (p2.Y - simmetrical.Y));
        }
    }
}
