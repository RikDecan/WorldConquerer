using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster.World
{
    public class SeedBasedGenerator : IWorldGenerator
    {
        private readonly Random random;
        private readonly int seedCount;

        public SeedBasedGenerator(Random random, int seedCount = 5)
        {
            this.random = random;
            this.seedCount = seedCount;
        }

        public bool[,] Generate(int width, int height, double coverage)
        {
            bool[,] squares = new bool[width, height];
            int coverageRequired = (int)(coverage * width * height);
            int currentCoverage = 0;
            var list = new List<(int, int)>();

            for (int i = 0; i < seedCount; i++)
            {
                int x = random.Next(width), y = random.Next(height);
                if (!list.Contains((x, y)))
                {
                    list.Add((x, y));
                    currentCoverage++;
                    squares[x, y] = true;
                }
            }

   
            while (currentCoverage < coverageRequired)
            {
                int index = random.Next(list.Count);
                int direction = random.Next(4);
                var (x, y) = list[index];

                (int dx, int dy) = direction switch
                {
                    0 => (1, 0),
                    1 => (-1, 0),
                    2 => (0, 1),
                    _ => (0, -1)
                };

                int newX = x + dx, newY = y + dy;
                if (newX >= 0 && newX < width && newY >= 0 && newY < height && !squares[newX, newY])
                {
                    squares[newX, newY] = true;
                    list.Add((newX, newY));
                    currentCoverage++;
                }
            }
            return squares;
        }
    }
}
