using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MirrorsExperiment
{
    public abstract class Wall
    {
        public Experiment Experiment;
        public Point P1 = new Point(0, 0);
        public Point P2 = new Point(0, 0);

        public PointF SegmentCenter => new PointF((P1.X + P2.X) / 2f, (P1.Y + P2.Y) / 2f);

        public Wall() {}

        public Wall(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public abstract void Draw(Graphics g);

        public abstract bool Intersect(Point p1, Point p2, List<PointF> intersections);

        public abstract Point Reflect(Point p1, Point p2);

        public virtual void Read(BinaryReader br)
        {
            P1.X = br.ReadInt32();
            P1.Y = br.ReadInt32();
            P2.X = br.ReadInt32();
            P2.Y = br.ReadInt32();
        }

        public virtual void Write(BinaryWriter bw)
        {
            bw.Write((int)P1.X);
            bw.Write((int)P1.Y);
            bw.Write((int)P2.X);
            bw.Write((int)P2.Y);
        }
        
        public static Wall Read(Experiment experiment, BinaryReader br)
        {
            Wall result;
            switch (br.ReadByte())
            {
                case 0:
                    result = new FlatMirror();
                    break;
                case 1:
                    result = new SphericalMirror();
                    break;
                default:
                    throw new InvalidDataException();
            }
            result.Read(br);
            result.Experiment = experiment;
            return result;
        }
    }
}