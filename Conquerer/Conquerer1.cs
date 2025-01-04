using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster.Conquerer
{
    public class Conquerer1 : BaseConquerer
    {
        protected override void ExecuteConquerStrategy(int nEmpires, int turns)
        {
            for (int t = 0; t < turns; t++)
            {
                for (int e = 1; e <= nEmpires; e++)
                {
                    if (!empires.ContainsKey(e) || empires[e].Count == 0)
                        continue;

                    // Kies een willekeurige cel van het rijk
                    var index = random.Next(empires[e].Count);
                    var (x, y) = empires[e][index];

                    // Kies een willekeurige richting
                    int direction = random.Next(4);
                    int newX = x + (direction == 0 ? 1 : direction == 1 ? -1 : 0);
                    int newY = y + (direction == 2 ? 1 : direction == 3 ? -1 : 0);

                    // Controleer en breid uit
                    if (IsValidPosition(newX, newY) && worldempires[newX, newY] == 0)
                    {
                        worldempires[newX, newY] = e;
                        empires[e].Add((newX, newY));
                    }
                }
            }
        }
    }
}