using System.Collections.Generic;

namespace MirrorsExperiment
{
    public class Room
    {
        // Эксперимент, соответствующий комнате
        public Experiment Experiment;
        // Список стен комнаты
        public List<Wall> Walls = new List<Wall>();

        public Room(Experiment experiment)
        {
            Experiment = experiment;
        }

        // Добавление стены
        public void AddWall(Wall wall)
        {
            Walls.Add(wall);
            wall.Experiment = Experiment;
        }

        // Изменение типа стены
        public void ChangeWallType(int index)
        {
            if (Walls[index] is SphericalMirror)
            {
                Walls[index] = new FlatMirror(Walls[index].P1, Walls[index].P2);
                Walls[index].Experiment = Experiment;
            }
            else
            {
                Walls[index] = new SphericalMirror(Walls[index].P1, Walls[index].P2, 10000);
                Walls[index].Experiment = Experiment;
            }
        }
    }
}