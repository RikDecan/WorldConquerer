using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster.Conquerer
{
    public class Conquerer2 : BaseConquerer
    {
        protected override void ExecuteConquerStrategy(int nEmpires, int turns)
        {
            for (int i = 0; i < turns; i++)
            {
                for (int e = 1; e <= nEmpires; e++)
                {
                    var index = FindWithMostEmptyNeighbours(e, empires[e]);
                    var direction = random.Next(4);
                    var x = empires[e][index].Item1;
                    var y = empires[e][index].Item2;

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
        }

        private int FindWithMostEmptyNeighbours(int e, List<(int, int)> empire)
        {
            List<int> indexes = new List<int>();
            int n = 0;
            for (int i = 0; i < empire.Count; i++)
            {
                var calcN = EmptyNeighbours(e, empire[i].Item1, empire[i].Item2);
                if (calcN >= n)
                {
                    indexes.Clear();
                    n = calcN;
                    indexes.Add(i);
                }
            }
            return indexes[random.Next(indexes.Count)];
        }




        private int EmptyNeighbours(int empire, int x, int y)
        {
            int n = 0;
            if (IsValidPosition(x - 1, y) && worldempires[x - 1, y] == 0) n++;
            if (IsValidPosition(x + 1, y) && worldempires[x + 1, y] == 0) n++;
            if (IsValidPosition(x, y - 1) && worldempires[x, y - 1] == 0) n++;
            if (IsValidPosition(x, y + 1) && worldempires[x, y + 1] == 0) n++;
            return n;
        }
    }
}
