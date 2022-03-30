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
        public static Point PointToSegmentProject(Point line1, Point line2, Point toProject)
        {
            double m = (double)(line2.Y - line1.Y) / (line2.X - line1.X);
            double b = (double)line1.Y - (m * line1.X);

            double x = (m * toProject.Y + toProject.X - m * b) / (m * m + 1);
            double y = (m * m * toProject.Y + m * toProject.X + b) / (m * m + 1);

            return new Point((int)x, (int)y);
        }

        // Расстояние между точками
        public static double PointDistance(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
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
    }
}
