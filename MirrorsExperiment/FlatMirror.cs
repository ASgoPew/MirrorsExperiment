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

        public override bool Intersect(Point p1, Point p2, ref PointF intersection)
        {
            return MyExtensions.seg_cross(P1, P2, p1, p2, ref intersection) == seg_cross_t.seg_crossing;
        }
    }
}
