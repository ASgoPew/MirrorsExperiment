using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MirrorsExperiment
{
    public class FlatMirror : Wall
    {
        public FlatMirror(Point p1, Point p2)
            : base(p1, p2)
        {

        }

        public override void Draw(Graphics g)
        {
            Pen pen = new Pen(Color.Black, 3);
            g.DrawLine(pen, P1, P2);
        }

        public override bool Intersect(Point p1, Point p2, List<PointF> intersections)
        {
            PointF intersection = new PointF();
            if (MyExtensions.seg_cross(P1, P2, p1, p2, ref intersection) == seg_cross_t.seg_crossing)
            {
                intersections.Add(intersection);
                return true;
            }
            return false;
        }

        public override Point Reflect(Point p1, Point p2)
        {
            Point projection = new Point();
            if (!MyExtensions.PointToSegmentProject(P1, P2, p1, ref projection))
                throw new Exception();
            Point simmetrical = new Point(projection.X + (projection.X - p1.X), projection.Y + (projection.Y - p1.Y));
            return new Point(p2.X + (p2.X - simmetrical.X), p2.Y + (p2.Y - simmetrical.Y));
        }
    }
}
