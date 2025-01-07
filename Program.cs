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


            var worldTasks = new List<Task>();

            for (int i = 0; i < planets.Length; i++)
            {
                for (int simulationNumber = 1; simulationNumber <= 3; simulationNumber++)
                {
                    int currentIndex = i;
                    int currentSimulation = simulationNumber; 
                    var worldTask = ProcessWorldAsync(planets[currentIndex], currentSimulation, random, mongoService);
                    worldTasks.Add(worldTask);
                }
            }

            await Task.WhenAll(worldTasks);

            Console.WriteLine("\nAll world simulations completed!");
        }
        static async Task ProcessWorldAsync(string planetName, int simulationNumber, Random random, MongoDbService mongoService)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n-{planetName} (Simulation {simulationNumber})-");
            Console.ForegroundColor = ConsoleColor.Gray;

            int width = random.Next(30, 101);
            int height = random.Next(30, 101);
            double coverage = 0.15 + (random.NextDouble() * (0.95 - 0.15));
            var worldGenerator = new RandomWorldGenerator();
            bool[,] world = await Task.Run(() => worldGenerator.GenerateWorld(width, height, coverage));

            int numberOfEmpires = random.Next(7, 9);
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
                result = await Task.Run(() => assignedStrategies[empireId - 1].Conquer(world, empireId, numberOfTurns));
            }

            var bmw = new BitmapWriter();
            await Task.Run(() => bmw.DrawWorld(result));

            await DisplayStatisticsAsync(world, result, numberOfEmpires, planetName, simulationNumber, mongoService, assignedStrategies);

            await mongoService.SaveWorldAsync("Worlds", $"{planetName}_Simulation_{simulationNumber}", "Random", width, height, coverage, world);
            Console.WriteLine($"World {planetName} (Simulation {simulationNumber}) saved to MongoDB!");
        }


        static async Task DisplayStatisticsAsync(bool[,] blancWorld, int[,] world, int numberOfEmpires, string planetName, int simulationNumber, MongoDbService mongoService, List<IConquerer> assignedStrategies)
        {
            var allStatistics = new List<double[]>();

            var stats = await Task.Run(() =>
            {

                double totalCells = 0;

                for (int y = 0; y < blancWorld.GetLength(0); y++)
                {
                    for (int x = 0; x < blancWorld.GetLength(1); x++)
                    {
                        if (blancWorld[y, x])
                        {
                            totalCells++;
                        }
                    }
                }

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

                var percentConquered = new double[numberOfEmpires + 1];
                for (int i = 0; i <= numberOfEmpires; i++)
                {
                    percentConquered[i] = (empireCounts[i] * 100.0) / totalCells;
                }

                return (totalCells, percentConquered);
            });

            allStatistics.Add(stats.percentConquered);


            if (simulationNumber == 3)
            {
                var averagePercentConquered = new double[numberOfEmpires + 1];

                for (int i = 0; i <= numberOfEmpires; i++)
                {
                    averagePercentConquered[i] = allStatistics.Average(s => s[i]);
                }

                var averageConquered = new Dictionary<string, double>();
                for (int i = 1; i <= numberOfEmpires; i++)
                {
                    string conquererName = assignedStrategies[i - 1].GetType().Name;
                    averageConquered[conquererName] = averagePercentConquered[i];
                }

                Console.WriteLine("\nAverage Statistics (across simulations):");
                Console.WriteLine($"Unconquered territory: {100.0 - averagePercentConquered[0]:F2}%");
                foreach (var kvp in averageConquered)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value:F2}%");
                }

                await mongoService.SaveStatsWithAverageAsync("Stats", planetName, simulationNumber, stats.totalCells, stats.percentConquered, averageConquered);
                Console.WriteLine("Statistics with averages saved to MongoDB!");
            }

        }
    }
}