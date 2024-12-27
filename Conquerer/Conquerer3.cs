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
