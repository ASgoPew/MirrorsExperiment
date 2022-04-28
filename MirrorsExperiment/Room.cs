using System.Collections.Generic;

namespace MirrorsExperiment
{
    public class Room
    {
        public Experiment Experiment;
        public List<Wall> Walls = new List<Wall>();

        public Room(Experiment experiment)
        {
            Experiment = experiment;
        }

        public void AddWall(Wall wall)
        {
            Walls.Add(wall);
            wall.Experiment = Experiment;
        }

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