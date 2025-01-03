using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster.World
{
    public interface IWorldGenerator
    {
        bool[,] Generate(int width, int height, double coverage);
    }
}
