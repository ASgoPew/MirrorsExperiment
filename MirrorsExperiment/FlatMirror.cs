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
    }
}
