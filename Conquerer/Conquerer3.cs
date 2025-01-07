using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster.Conquerer
{
    public class Conquerer3 : BaseConquerer
    {
        protected override void ExecuteConquerStrategy(int nEmpires, int turns)
        {
            for (int t = 0; t < turns; t++)
            {
                for (int e = 1; e <= nEmpires; e++)
                {
                    if (!empires.ContainsKey(e) || empires[e].Count == 0)
                        continue;

                    var candidates = new List<(int, int)>();

                    foreach (var (x, y) in empires[e])
                    {
                        AddCandidate(x + 1, y, candidates);
                        AddCandidate(x - 1, y, candidates);
                        AddCandidate(x, y + 1, candidates);
                        AddCandidate(x, y - 1, candidates);
                    }

                    if (candidates.Count > 0)
                    {
                        var (newX, newY) = candidates[random.Next(candidates.Count)];
                        worldempires[newX, newY] = e;
                        empires[e].Add((newX, newY));
                    }
                }
            }
        }

        private void AddCandidate(int x, int y, List<(int, int)> candidates)
        {
            if (IsValidPosition(x, y) && worldempires[x, y] == 0)
            {
                candidates.Add((x, y));
            }
        }
    }

}
