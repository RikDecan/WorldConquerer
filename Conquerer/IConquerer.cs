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


    //public class Conquerer2 : BaseConquerer
    //{
    //    protected override void ExecuteConquerStrategy(int nEmpires, int turns)
    //    {
    //        for (int i = 0; i < turns; i++)
    //        {
    //            for (int e = 1; e <= nEmpires; e++)
    //            {
    //                var index = FindWithMostEmptyNeighbours(e, empires[e]);
    //                var direction = random.Next(4);
    //                var x = empires[e][index].Item1;
    //                var y = empires[e][index].Item2;

    //                switch (direction)
    //                {
    //                    case 0:
    //                        if (x < maxx - 1 && worldempires[x + 1, y] == 0)
    //                        {
    //                            worldempires[x + 1, y] = e;
    //                            empires[e].Add((x + 1, y));
    //                        }
    //                        break;
    //                    case 1:
    //                        if (x > 0 && worldempires[x - 1, y] == 0)
    //                        {
    //                            worldempires[x - 1, y] = e;
    //                            empires[e].Add((x - 1, y));
    //                        }
    //                        break;
    //                    case 2:
    //                        if (y < maxy - 1 && worldempires[x, y + 1] == 0)
    //                        {
    //                            worldempires[x, y + 1] = e;
    //                            empires[e].Add((x, y + 1));
    //                        }
    //                        break;
    //                    case 3:
    //                        if (y > 0 && worldempires[x, y - 1] == 0)
    //                        {
    //                            worldempires[x, y - 1] = e;
    //                            empires[e].Add((x, y - 1));
    //                        }
    //                        break;
    //                }
    //            }
    //        }
    //    }

    //    private int FindWithMostEmptyNeighbours(int e, List<(int, int)> empire)
    //    {
    //        List<int> indexes = new List<int>();
    //        int n = 0;
    //        for (int i = 0; i < empire.Count; i++)
    //        {
    //            var calcN = EmptyNeighbours(e, empire[i].Item1, empire[i].Item2);
    //            if (calcN >= n)
    //            {
    //                indexes.Clear();
    //                n = calcN;
    //                indexes.Add(i);
    //            }
    //        }
    //        return indexes[random.Next(indexes.Count)];
    //    }

    //    private int EmptyNeighbours(int empire, int x, int y)
    //    {
    //        int n = 0;
    //        if (IsValidPosition(x - 1, y) && worldempires[x - 1, y] == 0) n++;
    //        if (IsValidPosition(x + 1, y) && worldempires[x + 1, y] == 0) n++;
    //        if (IsValidPosition(x, y - 1) && worldempires[x, y - 1] == 0) n++;
    //        if (IsValidPosition(x, y + 1) && worldempires[x, y + 1] == 0) n++;
    //        return n;
    //    }
    //}

    public class Conquerer3 : BaseConquerer
    {
        protected override void ExecuteConquerStrategy(int nEmpires, int turns)
        {
            for (int i = 0; i < turns; i++)
            {
                for (int e = 1; e <= nEmpires; e++)
                {
                    var index = random.Next(empires[e].Count);
                    PickEmpty(empires[e], index, e);
                }
            }
        }

        private void PickEmpty(List<(int, int)> empire, int index, int e)
        {
            List<(int, int)> n = new List<(int, int)>();

            if (IsValidPosition(empire[index].Item1 - 1, empire[index].Item2)
                && worldempires[empire[index].Item1 - 1, empire[index].Item2] == 0)
                n.Add((empire[index].Item1 - 1, empire[index].Item2));

            if (IsValidPosition(empire[index].Item1 + 1, empire[index].Item2)
                && worldempires[empire[index].Item1 + 1, empire[index].Item2] == 0)
                n.Add((empire[index].Item1 + 1, empire[index].Item2));

            if (IsValidPosition(empire[index].Item1, empire[index].Item2 - 1)
                && worldempires[empire[index].Item1, empire[index].Item2 - 1] == 0)
                n.Add((empire[index].Item1, empire[index].Item2 - 1));

            if (IsValidPosition(empire[index].Item1, empire[index].Item2 + 1)
                && worldempires[empire[index].Item1, empire[index].Item2 + 1] == 0)
                n.Add((empire[index].Item1, empire[index].Item2 + 1));

            if (n.Count > 0)
            {
                var pos = n[random.Next(n.Count)];
                empire.Add(pos);
                worldempires[pos.Item1, pos.Item2] = e;
            }
        }
    }
}
