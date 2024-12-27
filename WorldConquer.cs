using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster
{
    public class WorldConquer
    {
        private bool[,] world;

        private int[,] worldempires;
        private int maxx, maxy;
        private Random random = new Random(1);

        public WorldConquer(bool[,] world)
        {
            this.world = world;
            maxx = world.GetLength(0);
            maxy = world.GetLength(1);
            worldempires = new int[maxx,maxy];
            for (int i = 0; i < world.GetLength(0); i++) for (int j = 0; j < world.GetLength(1); j++) if (world[i, j]) worldempires[i, j] = 0; else worldempires[i, j] = -1;
        }
        public int[,] Conquer1(int nEmpires, int turns)
        {
            Dictionary<int, List<(int, int)>> empires = new(); 
            int x, y;
            for (int i = 0; i < nEmpires; i++)
            {
                bool ok = false;
                while (!ok)
                {
                    x = random.Next(maxx); y = random.Next(maxy);
                    if (world[x, y])
                    {
                        ok = true;
                        worldempires[x, y] = i + 1;
                        empires.Add(i + 1, new List<(int, int)>() { (x, y) });
                    }
                }
            }
            int index;
            int direction;
            for (int i = 0; i < turns; i++)
            {
                for (int e = 1; e <= nEmpires; e++)
                {
                    index = random.Next(empires[e].Count);
                    direction = random.Next(4);
                    x = empires[e][index].Item1;
                    y = empires[e][index].Item2;
                    switch (direction)
                    {
                        case 0:
                            if (x < maxx - 1 && worldempires[x + 1, y] == 0)
                            {
                                worldempires[x + 1, y] = e;
                                empires[e].Add((x + 1, y));
                            }
                            break;
                        case 1:
                            if (x > 0 && worldempires[x - 1, y] == 0)
                            {
                                worldempires[x - 1, y] = e;
                                empires[e].Add((x - 1, y));
                            }
                            break;
                        case 2:
                            if (y < maxy - 1 && worldempires[x, y + 1] == 0)
                            {
                                worldempires[x, y + 1] = e;
                                empires[e].Add((x, y + 1));
                            }
                            break;
                        case 3:
                            if (y > 0 && worldempires[x, y - 1] == 0)
                            {
                                worldempires[x, y - 1] = e;
                                empires[e].Add((x, y - 1));
                            }
                            break;
                    }
                }
            }
            return worldempires;
        }
 
        public int[,] Conquer3(int nEmpires, int turns)
        {
            Dictionary<int, List<(int, int)>> empires = new();
            int x, y;
            for (int i = 0; i < nEmpires; i++)
            {
                bool ok = false;
                while (!ok)
                {
                    x = random.Next(maxx); y = random.Next(maxy);
                    if (world[x, y])
                    {
                        ok = true;
                        worldempires[x, y] = i + 1;
                        empires.Add(i + 1, new List<(int, int)>() { (x, y) });
                    }
                }
            }
            int index;
            for (int i = 0; i < turns; i++)
            {
                for (int e = 1; e <= nEmpires; e++)
                {
                    index = random.Next(empires[e].Count);
                    pickEmpty(empires[e], index, e);
                }
            }
            return worldempires;
        }

        private void pickEmpty(List<(int,int)> empire,int index,int e)
        {
            List<(int, int)> n = new List<(int, int)>();
            if (IsValidPosition(empire[index].Item1-1, empire[index].Item2)
                && (worldempires[empire[index].Item1 - 1, empire[index].Item2]==0)) n.Add((empire[index].Item1-1, empire[index].Item2));
            if (IsValidPosition(empire[index].Item1+1, empire[index].Item2)
                && (worldempires[empire[index].Item1 + 1, empire[index].Item2] == 0)) n.Add((empire[index].Item1+1, empire[index].Item2));
            if (IsValidPosition(empire[index].Item1, empire[index].Item2-1)
                && (worldempires[empire[index].Item1, empire[index].Item2-1] == 0)) n.Add((empire[index].Item1, empire[index].Item2-1));
            if (IsValidPosition(empire[index].Item1, empire[index].Item2+1)
                && (worldempires[empire[index].Item1, empire[index].Item2+1] == 0)) n.Add((empire[index].Item1, empire[index].Item2+1));
            int x = random.Next(n.Count);
            if (n.Count > 0)
            {
                empire.Add(n[x]);
                worldempires[n[x].Item1, n[x].Item2] = e;
            }
        }

        public int[,] Conquer2(int nEmpires,int turns)
        {
            Dictionary<int, List<(int, int)>> empires = new();
            int x, y;
            for (int i = 0; i < nEmpires; i++)
            {
                bool ok = false;
                while (!ok)
                {
                    x = random.Next(maxx); y = random.Next(maxy);
                    if (world[x, y])
                    {
                        ok = true;
                        worldempires[x, y] = i + 1;
                        empires.Add(i + 1, new List<(int, int)>() { (x, y) });
                    }
                }
            }
            int index;
            int direction;
            for (int i = 0; i < turns; i++)
            {
                for(int e = 1; e <= nEmpires; e++)
                {
                    index =FindWithMostEmptyNeighbours(e, empires[e]);
                    direction = random.Next(4);
                    x=empires[e][index].Item1;
                    y=empires[e][index].Item2;
                    switch (direction)
                    {
                        case 0:
                            if (x<maxx-1 && worldempires[x+1,y]==0)
                            {
                                worldempires[x + 1, y] = e;
                                empires[e].Add((x+1, y));
                            }
                            break;
                        case 1:
                            if (x>0 && worldempires[x - 1, y] == 0)
                            {
                                worldempires[x - 1, y] = e;
                                empires[e].Add((x - 1, y));
                            }
                            break;
                        case 2:
                            if (y < maxy - 1 && worldempires[x, y+1] == 0)
                            {
                                worldempires[x, y+1] = e;
                                empires[e].Add((x, y+1));
                            }
                            break;
                        case 3:
                            if (y >0 && worldempires[x, y-1] == 0)
                            {
                                worldempires[x, y-1] = e;
                                empires[e].Add((x, y-1));
                            }
                            break;
                    }
                }
            }
            return worldempires;
        }

        private int FindWithMostEmptyNeighbours(int e, List<(int, int)> empire)
        {            
            List<int> indexes= new List<int>();
            int n = 0;
            int calcN;
            for (int i = 0; i < empire.Count; i++)
            {
                calcN = EmptyNeighbours(e, empire[i].Item1, empire[i].Item2);
                if (calcN >= n)
                {
                    indexes.Clear();
                    n= calcN;
                    indexes.Add(i);
                }
            }
            return indexes[random.Next(indexes.Count)];
        }
        private int EmptyNeighbours(int empire,int x,int y)
        {
            int n = 0;
            if (IsValidPosition(x-1,y) && worldempires[x-1, y] == 0) n++; 
            if (IsValidPosition(x+1, y) && worldempires[x + 1, y] == 0) n++;
            if (IsValidPosition(x, y-1) && worldempires[x, y-1] == 0) n++;
            if (IsValidPosition(x, y+1) && worldempires[x, y+1] == 0) n++;
            return n;

        }
        private bool IsValidPosition(int x, int y)
        {
            if (x<0) return false;
            if (x >= world.GetLength(0)) return false;
            if (y<0) return false;
            if (y>= world.GetLength(1)) return false;
            return true;
        }
    }
}
