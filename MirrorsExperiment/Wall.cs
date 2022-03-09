using System.Drawing;

namespace MirrorsExperiment
{
    public abstract class Wall
    {
        public Point P1;
        public Point P2;

        public Wall(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }
    }
}