using ConsoleAppSquareMaster.Conquerer;
using ConsoleAppSquareMaster.World;
using MongoDB.Driver;

namespace ConsoleAppSquareMaster
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("World Conquest Simulation\n");

            Console.Write("Press any key to start simulating");
            Console.ReadKey();

            string[] planets = { "Tatooine", "Scarif", "Endor", "Ilum", "Alderaan", "Coruscant", "Mandalore", "Naboo", "Mustafar", "Crait" };
            Random random = new Random();

            var mongoService = new MongoDbService("mongodb://localhost:27017", "WorldDatabase");
            await mongoService.ClearWorldsCollectionAsync("Worlds");

            // Process worlds concurrently
            var worldTasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                int currentIndex = i; // Capture the current index for async operation
                var worldTask = ProcessWorldAsync(planets[i], random, mongoService);
                worldTasks.Add(worldTask);
            }

            await Task.WhenAll(worldTasks);

            Console.WriteLine("\nAll world simulations completed!");
        }

        static async Task ProcessWorldAsync(string planetName, Random random, MongoDbService mongoService)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n-{planetName}-");
            Console.ForegroundColor = ConsoleColor.Gray;

            // Generate world
            int width = random.Next(30, 101);
            int height = random.Next(30, 101);
            double coverage = 0.15 + (random.NextDouble() * (0.95 - 0.15));
            var worldGenerator = new RandomWorldGenerator();
            bool[,] world = await Task.Run(() => worldGenerator.GenerateWorld(width, height, coverage));

            // Generate empires and strategies
            int numberOfEmpires = random.Next(8);
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

            // Execute conquests sequentially for now (to ensure correct empire order)

            //for (int empireId = 1; empireId <= numberOfEmpires; empireId++)
            //{
            //    Console.WriteLine($"Executing strategy for Empire {empireId}: {assignedStrategies[empireId - 1].GetType().Name}");
            //    result = await Task.Run(() => assignedStrategies[empireId - 1].Conquer(world, empireId, numberOfTurns));
            //}

            // Save visualization asynchronously
            var bmw = new BitmapWriter();
            await Task.Run(() => bmw.DrawWorld(result));


             await DisplayStatisticsAsync(result, numberOfEmpires);

            await mongoService.SaveWorldAsync("Worlds", planetName, "Random", width, height, coverage, world);
            Console.WriteLine($"World {planetName} saved to MongoDB!");
        }





        static async Task DisplayStatisticsAsync(int[,] world, int numberOfEmpires)
        {
            var stats = await Task.Run(() =>
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

                return (totalCells, empireCounts);
            });

            Console.WriteLine("Statistics:");
            Console.WriteLine($"Unconquered territory: {stats.empireCounts[0]} cells ({(stats.empireCounts[0] * 100.0 / stats.totalCells):F2}%)");
            for (int i = 1; i <= numberOfEmpires; i++)
            {
                Console.WriteLine($"Empire {i}: {stats.empireCounts[i]} cells ({(stats.empireCounts[i] * 100.0 / stats.totalCells):F2}%)");
            }
        }
    }
}