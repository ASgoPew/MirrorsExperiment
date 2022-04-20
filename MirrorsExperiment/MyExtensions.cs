using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MirrorsExperiment
{
    public enum seg_cross_t { seg_crossing = 1, seg_collide, seg_nocross };
    public enum turn_t { left = 1, right = -1, collinear = 0 };
    public enum point_loc_t { inside, outside };

    public static class MyExtensions
    {
        public static void SetDoubleBuffered(this Panel panel)
        {
            typeof(Panel).InvokeMember(
               "DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
               null,
               panel,
               new object[] { true });
        }

        // Проекция точки на прямую
        public static bool PointToSegmentProject(Point line1, Point line2, Point toProject, ref Point project)
        {
            float a = line2.Y - line1.Y;
            float b = line1.X - line2.X;
            if (a == 0 && b == 0)
                return false;
            float c = line1.Y * line2.X - line1.X * line2.Y;
            float t = (-a * toProject.X - b * toProject.Y - c) / (a * a + b * b);
            project = new Point((int)(toProject.X + a * t), (int)(toProject.Y + b * t));
            return true;
        }

        // Расстояние между точками
        public static double PointDistance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
        public static double PointDistance(PointF p1, PointF p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static int LineSide(Point line1, Point line2, Point p)
        {
            float result = (line2.Y - line1.Y) * p.X + (line1.X - line2.X) * p.Y +
                line1.Y * line2.X - line1.X * line2.Y;
            return result >= 0 ? 1 : -1;
        }

        // Проверка на вложенность точки в прямоугольник
        public static bool PointInRect(Point p, Point p1, Point p2)
        {
            double xmax = Math.Max(p1.X, p2.X);
            double xmin = Math.Min(p1.X, p2.X);
            double ymax = Math.Max(p1.Y, p2.Y);
            double ymin = Math.Min(p1.Y, p2.Y);
            return p.X <= xmax && p.X >= xmin &&
                p.Y <= ymax && p.Y >= ymin;
        }

        const double eps_precision_compare_double = 1e-09;
        public static double machine_epsilon = 1.0d;

        public static bool _equal(double x, double y)
        {
            return (Math.Abs(x - y) <= eps_precision_compare_double);
        }
        public static bool _less(double x, double y)
        {
            return x <= y + eps_precision_compare_double;
        }
        public static bool _more(double x, double y)
        {
            return x >= y - eps_precision_compare_double;
        }

        public static turn_t turn(PointF a, PointF b, PointF c)
        {
            double l = (b.X - a.X) * (c.Y - a.Y);
            double r = (c.X - a.X) * (b.Y - a.Y);

            double e = (Math.Abs(l) + Math.Abs(r)) * machine_epsilon * 4;

            double det = l - r;

            // Левая система координат => точка справа, когда детерминант положительный
            if (det > e)
                return turn_t.right;
            if (det< -e)
                return turn_t.left;

            return turn_t.collinear;
        }

        public static (float X, float Y, float Z) calc_delta(PointF a, PointF b, PointF c, PointF d)
        {
            return ((b.X - a.X) * (c.Y - d.Y) - (c.X - d.X) * (b.Y - a.Y),
                          (c.X - a.X) * (c.Y - d.Y) - (c.X - d.X) * (c.Y - a.Y),
                          (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y));
        }

        public static PointF det(PointF a, PointF b)
        {
            float l = a.X * b.Y;
            float r = b.X * a.Y;

            float e = (float)((Math.Abs(l) + Math.Abs(r)) * machine_epsilon * 4);

            return new PointF(l - r, e);
        }

        public static (float X, float Y, float Z) equation(PointF a, PointF b)
        {
            return (a.Y - b.Y, b.X - a.X, a.X * (b.Y - a.Y) + a.Y * (a.X - b.X));
        }

        public static int rank((float X, float Y, float Z) a, (float X, float Y, float Z) b)
        {
            PointF det1 = det(new PointF(a.X, a.Y), new PointF(b.X, b.Y));
            PointF det2 = det(new PointF(a.X, a.Z), new PointF(b.X, b.Z));
            return _equal(det1.X, 0) && _equal(det2.X, 0) ? 1 : 2;
        }

        public static bool point_in_rect(PointF rect1, PointF rect2, PointF a)
        {
            double xmax = Math.Max(rect1.X, rect2.X), xmin = Math.Min(rect1.X, rect2.X), ymax = Math.Max(rect1.Y, rect2.Y), ymin = Math.Min(rect1.Y, rect2.Y);
            return (_less(a.X, xmax) && _more(a.X, xmin) &&
                    _less(a.Y, ymax) && _more(a.Y, ymin));
        }

        public static seg_cross_t seg_cross(PointF a, PointF b, PointF c, PointF d, ref PointF result)
        {
            if (a == b && c == d)
                return a == c ? seg_cross_t.seg_collide : seg_cross_t.seg_nocross;

            (float X, float Y, float Z) delta = calc_delta(a, b, c, d);

            if (a == b)
                return point_in_rect(c, d, a) && turn(c, d, a) == turn_t.collinear ? seg_cross_t.seg_crossing : seg_cross_t.seg_nocross;
            else if (c == d)
                return point_in_rect(a, b, c) && turn(a, b, c) == turn_t.collinear ? seg_cross_t.seg_crossing : seg_cross_t.seg_nocross;

            // На этом моменте ни один из отрезков не является точкой

            if (_equal(delta.X, 0))
            { // Коллинеарны <=> delta = 0
                if (rank(equation(a, b), equation(c, d)) == 1)
                { // На одной линии <=> rank == 1
                  // На одной линии пересекаются <=> одна из точек отрезка внутри прямоугольника другого отрезка
                    bool abc = point_in_rect(a, b, c), abd = point_in_rect(a, b, d), cda = point_in_rect(c, d, a), cdb = point_in_rect(c, d, b);
                    if (!cda && !abc && cdb && abd && b == d)
                    {
                        result = b;
                        return seg_cross_t.seg_crossing;
                    }
                    else if (!cda && !abd && cdb && abc && b == c)
                    {
                        result = b;
                        return seg_cross_t.seg_crossing;
                    }
                    else if (!cdb && !abc && cda && abd && a == d)
                    {
                        result = a;
                        return seg_cross_t.seg_crossing;
                    }
                    else if (!cdb && !abd && cda && abc && a == c)
                    {
                        result = a;
                        return seg_cross_t.seg_crossing;
                    }
                    else if (abc || abd || cda || cdb)
                        return seg_cross_t.seg_collide;
                    return seg_cross_t.seg_nocross;
                }
                else
                    return seg_cross_t.seg_nocross;
            }
            else
            {
                if (a == c || a == d)
                {
                    result.X = a.X;
                    result.Y = a.Y;
                    return seg_cross_t.seg_crossing;
                }
                else if (b == c || b == d)
                {
                    result.X = b.X;
                    result.Y = b.Y;
                    return seg_cross_t.seg_crossing;
                }
                double lambda = delta.Y / delta.X;
                double mhyu = delta.Z / delta.X;
                if (lambda > -machine_epsilon && lambda < 1 + machine_epsilon && mhyu > -machine_epsilon && mhyu < 1 + machine_epsilon)
                { // Пересечение прямых внутри отрезков
                    result.X = (float)((1 - lambda) * a.X + lambda * b.X);
                    result.Y = (float)((1 - lambda) * a.Y + lambda * b.Y);
                    return seg_cross_t.seg_crossing;
                }
                else
                    return seg_cross_t.seg_nocross;
            }
        }

        public static point_loc_t point_in_seg(PointF a, PointF b, PointF c)
        {
            PointF _ = new PointF();
            seg_cross_t type = seg_cross(a, b, c, c, ref _);
            return type == seg_cross_t.seg_collide || type == seg_cross_t.seg_crossing ? point_loc_t.inside : point_loc_t.outside;
        }
    }
}
