using ConsoleAppSquareMaster.Conquerer;
using ConsoleAppSquareMaster.World;
using MongoDB.Driver;

namespace ConsoleAppSquareMaster
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("World Conquest Simulation\n");

            string[] planets = { "Tatooine", "Scarif", "Endor", "Ilum", "Alderaan", "Coruscant", "Mandalore", "Naboo", "Mustafar", "Crait" };
            Random random = new Random();

            var mongoService = new MongoDbService("mongodb://localhost:27017", "WorldDatabase");
            mongoService.ClearWorldsCollection("Worlds");

            for (int worldIndex = 0; worldIndex < 10; worldIndex++)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n-{planets[worldIndex]}-");
                Console.ForegroundColor = ConsoleColor.Gray;  // Reset to default color

                int width = random.Next(30, 101);
                int height = random.Next(30, 101);
                double coverage = 0.15 + (random.NextDouble() * (0.95 - 0.15));
                var worldGenerator = new RandomWorldGenerator();
                bool[,] world = worldGenerator.GenerateWorld(width, height, coverage);

                int numberOfEmpires = 8;    //random.Next(4, 6);
                var conquerStrategies = new List<IConquerer>
                {
                    new Conquerer1(),
                    new Conquerer2(),
                    new Conquerer3()
                };

                var assignedStrategies = new List<IConquerer>();
                for (int e = 0; e < numberOfEmpires; e++)
                {
                    assignedStrategies.Add(conquerStrategies[random.Next(conquerStrategies.Count)]);
                }

                const int numberOfTurns = 12500;
                var result = new int[width, height];

                for (int empireId = 1; empireId <= numberOfEmpires; empireId++)
                {
                    Console.WriteLine($"Executing strategy for Empire {empireId}: {assignedStrategies[empireId - 1].GetType().Name}");
                    result = assignedStrategies[empireId - 1].Conquer(world, empireId, numberOfTurns);
                }

                var bmw = new BitmapWriter();
                bmw.DrawWorld(result);

                DisplayStatistics(result, numberOfEmpires);

                mongoService.SaveWorld("Worlds", planets[worldIndex], "Random", width, height, coverage, world);
                Console.WriteLine($"World {planets[worldIndex]} saved to MongoDB!");
            }
        }

        static void DisplayStatistics(int[,] world, int numberOfEmpires)
        {
            var totalCells = world.GetLength(0) * world.GetLength(1);
            var empireCounts = new int[numberOfEmpires + 1];

            for (int i = 0; i < world.GetLength(0); i++)
            {
                for (int j = 0; j < world.GetLength(1); j++)
                {
                    if (world[i, j] >= 0)
                    {
                        empireCounts[world[i, j]]++;
                    }
                }
            }

            Console.WriteLine("Statistics:");
            Console.WriteLine($"Unconquered territory: {empireCounts[0]} cells ({(empireCounts[0] * 100.0 / totalCells):F2}%)");
            for (int i = 1; i <= numberOfEmpires; i++)
            {
                Console.WriteLine($"Empire {i}: {empireCounts[i]} cells ({(empireCounts[i] * 100.0 / totalCells):F2}%)");
            }
        }
    }
}