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
        public int Radius;

        public SphericalMirror(Point p1, Point p2)
            : base(p1, p2)
        {

        }
    }
}
