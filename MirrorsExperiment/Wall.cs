using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MirrorsExperiment
{
    public abstract class Wall
    {
        // Эксперимент, соответствующий стене
        public Experiment Experiment;
        // Начальная точка стены
        public Point P1 = new Point(0, 0);
        // Конечная точка стены
        public Point P2 = new Point(0, 0);

        // Центр отрезка от начальной точки до конечной
        public PointF SegmentCenter => new PointF((P1.X + P2.X) / 2f, (P1.Y + P2.Y) / 2f);

        public Wall() {}

        public Wall(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        // Отрисовка стены
        public abstract void Draw(Graphics g);

        // Пересечение стены лучём
        public abstract bool Intersect(Point p1, Point p2, List<PointF> intersections);

        // Отражение вектора относительно стены
        public abstract Point Reflect(Point p1, Point p2);

        // Чтение стены из потока данных
        public virtual void Read(BinaryReader br)
        {
            P1.X = br.ReadInt32();
            P1.Y = br.ReadInt32();
            P2.X = br.ReadInt32();
            P2.Y = br.ReadInt32();
        }

        // Запись стены в поток данных
        public virtual void Write(BinaryWriter bw)
        {
            bw.Write((int)P1.X);
            bw.Write((int)P1.Y);
            bw.Write((int)P2.X);
            bw.Write((int)P2.Y);
        }
        
        // Чтение стены любого типа из потока данных
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