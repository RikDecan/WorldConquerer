using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster.Conquerer
{
    public interface IConquerer
    {
        int[,] Conquer(bool[,] world, int nEmpires, int turns);
    }

    public abstract class BaseConquerer : IConquerer
    {
        protected Random random = new Random();
        protected int[,] worldempires;
        protected int maxx, maxy;
        protected Dictionary<int, List<(int, int)>> empires;

        public int[,] Conquer(bool[,] world, int nEmpires, int turns)
        {
            InitializeWorld(world);
            InitializeEmpires(nEmpires);
            ExecuteConquerStrategy(nEmpires, turns);
            return worldempires;
        }

        
        //dit is de Interface voor 1 instantie , kijk maar gerust verder
        //ksnap het ni kga nog ff kijken oke wacht he

        protected void InitializeWorld(bool[,] world)
        {
            maxx = world.GetLength(0);
            maxy = world.GetLength(1);
            worldempires = new int[maxx, maxy];
            for (int i = 0; i < maxx; i++)
                for (int j = 0; j < maxy; j++)
                    worldempires[i, j] = world[i, j] ? 0 : -1;
        }

        protected void InitializeEmpires(int nEmpires)
        {
            empires = new Dictionary<int, List<(int, int)>>();
            for (int i = 0; i < nEmpires; i++)
            {
                int x, y;
                bool ok = false;
                while (!ok)
                {
                    x = random.Next(maxx);
                    y = random.Next(maxy);
                    if (worldempires[x, y] == 0)
                    {
                        ok = true;
                        worldempires[x, y] = i + 1;
                        empires.Add(i + 1, new List<(int, int)> { (x, y) });
                    }
                }
            }
        }

        protected abstract void ExecuteConquerStrategy(int nEmpires, int turns);

        protected bool IsValidPosition(int x, int y)
        {
            return x >= 0 && x < maxx && y >= 0 && y < maxy;
        }
    }

}
