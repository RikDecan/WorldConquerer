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
            for (int t = 0; t < turns; t++)
            {
                for (int e = 1; e <= nEmpires; e++)
                {
                    if (!empires.ContainsKey(e) || empires[e].Count == 0)
                        continue;

                    var maxEmpty = -1;
                    var candidates = new List<(int, int)>();

                    foreach (var (x, y) in empires[e])
                    {
                        int emptyCount = CountEmptyNeighbours(x, y);

                        if (emptyCount > maxEmpty)
                        {
                            maxEmpty = emptyCount;
                            candidates.Clear();
                            candidates.Add((x, y));
                        }
                        else if (emptyCount == maxEmpty)
                        {
                            candidates.Add((x, y));
                        }
                    }

                    if (candidates.Count > 0)
                    {
                        // Kies een willekeurige kandidaat
                        var (x, y) = candidates[random.Next(candidates.Count)];
                        TryExpandEmpire(e, x, y);
                    }
                }
            }
        }

        private int CountEmptyNeighbours(int x, int y)
        {
            int count = 0;
            if (IsValidPosition(x + 1, y) && worldempires[x + 1, y] == 0) count++;
            if (IsValidPosition(x - 1, y) && worldempires[x - 1, y] == 0) count++;
            if (IsValidPosition(x, y + 1) && worldempires[x, y + 1] == 0) count++;
            if (IsValidPosition(x, y - 1) && worldempires[x, y - 1] == 0) count++;
            return count;
        }

        private void TryExpandEmpire(int empireId, int x, int y)
        {
            var directions = new[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
            foreach (var (dx, dy) in directions.OrderBy(_ => random.Next()))
            {
                int newX = x + dx, newY = y + dy;
                if (IsValidPosition(newX, newY) && worldempires[newX, newY] == 0)
                {
                    worldempires[newX, newY] = empireId;
                    empires[empireId].Add((newX, newY));
                    break;
                }
            }
        }
    }
}
