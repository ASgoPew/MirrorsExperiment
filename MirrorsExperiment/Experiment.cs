using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MirrorsExperiment
{
    public class Experiment
    {
        // Форма окна, которому соответствует эксперимент
        public Form1 Form = null;
        // Таймер, по которому эксперимент создает новые лучи (если не мгновенные вычисления)
        public System.Timers.Timer timer = new System.Timers.Timer();
        // Генератор случайных чисел
        public Random Random = new Random();
        // Комната эксперимента
        public Room Room;
        // Источник света, задаваемый пользователем
        public LightBeam LightSource = null;
        // Расстояние, в пределах которого засчитывается попадание луча в угол комнаты
        public int PointStopDistance = 2;
        // Мгновенно ли вычисляется эксперимент
        public bool Instant = false;
        // Количество шагов эксперимента (если мгновенные вычисления)
        public int InstantSteps = 10;

        public Experiment(Form1 form, int wallsCount)
        {
            Form = form;
            Form.Experiment = this;
            Room = new Room(this);
            Initialize(wallsCount, Form.drawPanel.Width, Form.drawPanel.Height);
            timer.Interval = 10;
            timer.Elapsed += Timer_Elapsed;
        }

        // Инициализация комнаты начальными параметрами
        public void Initialize(int wallsCount, int width, int height)
        {
            Room.Walls.Clear();
            int R = width / 4;
            Point center = new Point(width / 2, height / 2);
            double _phi = (360 / (wallsCount )) * Math.PI / 180;
            double phi = _phi;
            Point first = new Point(center.X + (int)(Math.Cos(0) * R), center.Y + (int)(Math.Sin(0) * R));
            Point p1 = first;
            Point p2;
            for (int i = 0; i < wallsCount; i++)
            {
                int r = Random.Next(R / 4, (int)(R * 1.3));
                if (i < wallsCount - 1)
                    p2 = new Point(center.X + (int)(Math.Cos(phi) * r), center.Y + (int)(Math.Sin(phi) * r));
                else
                    p2 = first;
                Room.AddWall(new FlatMirror(p1, p2));
                p1 = p2;
                phi += _phi;
            }

            //File.WriteAllText("test.txt", "");
        }

        // Расчет лучей по таймеру
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (Instant)
                {
                    timer.Stop();
                    for (int i = 0; i < InstantSteps; i++)
                        GenerateNextLightBeam();
                    return;
                }

                GenerateNextLightBeam();
            }
            catch (Exception exc)
            {
                timer.Stop();
                MessageBox.Show(exc.ToString());
                Environment.Exit(1);
            }
        }

        // Генерация следующего луча
        private void GenerateNextLightBeam()
        {
            LightBeam light = LightSource;
            int count = 1;
            while (light.Next != null)
            {
                light = light.Next;
                count++;
            }
            light.GenerateNext(this);
            //light = light.Next;
            Form.show($"light beams: {count + 1}");//, last: {light?.P1} -> {light?.P2}");
            Form.drawPanel.Invalidate();
        }
    }
}
