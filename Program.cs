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

            // Process each world 3 times
            var worldTasks = new List<Task>();

<<<<<<< Updated upstream
            for (int i = 0; i < planets.Length; i++)
            {
                for (int simulationNumber = 1; simulationNumber <= 3; simulationNumber++)
                {
                    int currentIndex = i; // Capture current index for async operation
                    int currentSimulation = simulationNumber; // Capture simulation number
                    var worldTask = ProcessWorldAsync(planets[currentIndex], currentSimulation, random, mongoService);
=======
            // Loop over the planets and simulate each world three times
            for (int i = 0; i < planets.Length; i++)
            {
                for (int j = 0; j < 3; j++) // Each world is simulated 3 times
                {
                    int currentIndex = i; // Capture the current index for async operation
                    var worldTask = ProcessWorldAsync(planets[i], random, mongoService, j + 1);
>>>>>>> Stashed changes
                    worldTasks.Add(worldTask);
                }
            }

            await Task.WhenAll(worldTasks);

            Console.WriteLine("\nAll world simulations completed!");
        }
<<<<<<< Updated upstream
        static async Task ProcessWorldAsync(string planetName, int simulationNumber, Random random, MongoDbService mongoService)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n-{planetName} (Simulation {simulationNumber})-");
=======

        static async Task ProcessWorldAsync(string planetName, Random random, MongoDbService mongoService, int simulationNumber)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n-{planetName} Simulation {simulationNumber}-");
>>>>>>> Stashed changes
            Console.ForegroundColor = ConsoleColor.Gray;

            // Generate world
            int width = random.Next(30, 101);
            int height = random.Next(30, 101);
            double coverage = 0.15 + (random.NextDouble() * (0.95 - 0.15));
            var worldGenerator = new RandomWorldGenerator();
            bool[,] world = await Task.Run(() => worldGenerator.GenerateWorld(width, height, coverage));

            // Generate empires and strategies
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

            // Execute conquests sequentially for now (to ensure correct empire order)
<<<<<<< Updated upstream
            for (int empireId = 1; empireId <= numberOfEmpires; empireId++)
            {
                Console.WriteLine($"Executing strategy for Empire {empireId}: {assignedStrategies[empireId - 1].GetType().Name}");
                result = await Task.Run(() => assignedStrategies[empireId - 1].Conquer(world, empireId, numberOfTurns));
            }
=======
            //for (int empireId = 1; empireId <= numberOfEmpires; empireId++)
            //{
            //    Console.WriteLine($"Executing strategy for Empire {empireId}: {assignedStrategies[empireId - 1].GetType().Name}");
            //    result = await Task.Run(() => assignedStrategies[empireId - 1].Conquer(world, empireId, numberOfTurns));
            //}
>>>>>>> Stashed changes

            // Save visualization asynchronously
            var bmw = new BitmapWriter();
            await Task.Run(() => bmw.DrawWorld(result));

<<<<<<< Updated upstream
            await DisplayStatisticsAsync(result, numberOfEmpires, planetName, simulationNumber, mongoService);

            // Save to MongoDB with simulation number
            await mongoService.SaveWorldAsync("Worlds", $"{planetName}_Simulation_{simulationNumber}", "Random", width, height, coverage, world);
            Console.WriteLine($"World {planetName} (Simulation {simulationNumber}) saved to MongoDB!");
        }


        //static async Task ProcessWorldAsync(string planetName, Random random, MongoDbService mongoService)
        //{
        //    Console.ForegroundColor = ConsoleColor.Cyan;
        //    Console.WriteLine($"\n-{planetName}-");
        //    Console.ForegroundColor = ConsoleColor.Gray;

        //    // Generate world
        //    int width = random.Next(30, 101);
        //    int height = random.Next(30, 101);
        //    double coverage = 0.15 + (random.NextDouble() * (0.95 - 0.15));
        //    var worldGenerator = new RandomWorldGenerator();
        //    bool[,] world = await Task.Run(() => worldGenerator.GenerateWorld(width, height, coverage));

        //    // Generate empires and strategies
        //    int numberOfEmpires = random.Next(7, 9);
        //    var conquerStrategies = new List<IConquerer>
        //    {
        //        new Conquerer1(),
        //        new Conquerer2(),
        //        new Conquerer3()
        //    };

        //    var assignedStrategies = new List<IConquerer>();
        //    for (int e = 0; e < numberOfEmpires; e++)
        //    {
        //        assignedStrategies.Add(conquerStrategies[random.Next(conquerStrategies.Count)]);
        //    }

        //    const int numberOfTurns = 12500;
        //    var result = new int[width, height];

        //    // Execute conquests sequentially for now (to ensure correct empire order)

        //    for (int empireId = 1; empireId <= numberOfEmpires; empireId++)
        //    {
        //        Console.WriteLine($"Executing strategy for Empire {empireId}: {assignedStrategies[empireId - 1].GetType().Name}");
        //        result = await Task.Run(() => assignedStrategies[empireId - 1].Conquer(world, empireId, numberOfTurns));
        //    }

        //    // Save visualization asynchronously
        //    var bmw = new BitmapWriter();
        //    await Task.Run(() => bmw.DrawWorld(result));


        //     await DisplayStatisticsAsync(result, numberOfEmpires);

        //    await mongoService.SaveWorldAsync("Worlds", planetName, "Random", width, height, coverage, world);
        //    Console.WriteLine($"World {planetName} saved to MongoDB!");
        //}



        static async Task DisplayStatisticsAsync(int[,] world, int numberOfEmpires, string planetName, int simulationNumber, MongoDbService mongoService)
        {


            //await mongoService.ClearWorldsCollectionAsync("Stats");


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

                // Bereken de percentages
                var percentConquered = new double[numberOfEmpires + 1];
                for (int i = 0; i <= numberOfEmpires; i++)
                {
                    percentConquered[i] = (empireCounts[i] * 100.0) / totalCells;
                }

                return (totalCells, percentConquered);
            });

            // Print de statistieken
            Console.WriteLine("Statistics:");
            Console.WriteLine($"Unconquered territory: {stats.percentConquered[0]:F2}%");
            for (int i = 1; i <= numberOfEmpires; i++)
            {
                Console.WriteLine($"Empire {i}: {stats.percentConquered[i]:F2}%");
            }

            // Sla de statistieken op in MongoDB
            await mongoService.SaveStatsAsync("Stats", planetName, simulationNumber, stats.totalCells, stats.percentConquered);
        }
=======
            // Display statistics for the simulation
            await DisplayStatisticsAsync(result, numberOfEmpires);

            // Save the world to MongoDB with the simulation number included in the name
            string simulationName = $"{planetName}_Sim{simulationNumber}";
            await mongoService.SaveWorldAsync("Worlds", simulationName, "Random", width, height, coverage, world);
            Console.WriteLine($"World {simulationName} saved to MongoDB!");
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


>>>>>>> Stashed changes
    }
}

