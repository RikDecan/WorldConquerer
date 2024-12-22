using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleAppSquareMaster;

namespace ConsoleAppSquareMaster.Conquerer
{
    public class Conquer1
    {
        private bool[,] world;
        private int[,] worldempires;
        private int maxx, maxy;
        private Random random = new Random(1);

        public int[,] SimulateConquest(int nEmpires, int turns) // Methode hernoemd
        {
            Dictionary<int, List<(int, int)>> empires = new(); // Key is het rijk-id, waarde is de lijst van cellen (x, y) die het rijk controleert
            int x, y;
            for (int i = 0; i < nEmpires; i++)
            {
                bool ok = false;
                while (!ok)
                {
                    x = random.Next(maxx);
                    y = random.Next(maxy);
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
    }
}

