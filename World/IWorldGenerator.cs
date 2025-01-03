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

            // Place initial seeds
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

            // Grow from seeds
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

    public class RandomWorldGenerator
    {
        private readonly Random random;
        private readonly List<IWorldGenerator> generators;

        public RandomWorldGenerator(int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
            generators = new List<IWorldGenerator>
            {
                new ColumnBasedGenerator(random),
                new SeedBasedGenerator(random)
            };
        }

        public bool[,] GenerateWorld(int width, int height, double coverage)
        {
            var generator = generators[random.Next(generators.Count)];
            return generator.Generate(width, height, coverage);
        }
    }
}
