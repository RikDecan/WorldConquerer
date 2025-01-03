using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster.World
{
    public class ColumnBasedGenerator : IWorldGenerator
    {
        private readonly Random random;
        private readonly int maxRandom = 10;
        private readonly int chanceExtra;
        private readonly int chanceLess;

        public ColumnBasedGenerator(Random random, int chanceExtra = 6, int chanceLess = 3)
        {
            this.random = random;
            this.chanceExtra = chanceExtra;
            this.chanceLess = chanceLess;
        }

        public bool[,] Generate(int width, int height, double coverage)
        {
            bool[,] squares = new bool[width, height];
            int y1 = random.Next(height);
            int y2 = random.Next(height);
            int yb = Math.Min(y1, y2);
            int ye = Math.Max(y1, y2);

            for (int i = 0; i < width; i++)
            {
                for (int j = yb; j < ye; j++) squares[i, j] = true;
                switch (Build())
                {
                    case 1: if (yb > 0) yb--; break;
                    case -1: if (yb < height) yb++; break;
                }
                switch (Build())
                {
                    case 1: if (ye < height) ye++; break;
                    case -1: if (ye > 0) ye--; break;
                }
                if (ye < yb) break;
            }
            return squares;
        }

        private int Build()
        {
            int x = random.Next(maxRandom);
            if (x > chanceExtra) return 1;
            if (x < chanceLess) return -1;
            return 0;
        }
    }
}
