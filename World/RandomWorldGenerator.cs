using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster.World
{
    public class RandomWorldGenerator
    {
        private readonly Random random;
        private readonly List<IWorldGenerator> generators;
        private string lastUsedGenerationType;

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
            lastUsedGenerationType = generator.GetType().Name;
            return generator.Generate(width, height, coverage);
        }

        public string GetLastUsedGenerationType() => lastUsedGenerationType;
    }
}
