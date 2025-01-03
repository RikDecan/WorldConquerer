using ConsoleAppSquareMaster.Conquerer;
using  ConsoleAppSquareMaster.World;
using MongoDB.Driver;


namespace ConsoleAppSquareMaster
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("World Conquest Simulation\n");

            // Create world

            //World world = new World();
            //var w = world.BuildWorld2(100, 100, 0.60);

            var worldGenerator = new RandomWorldGenerator();
            var w = worldGenerator.GenerateWorld(100, 100, 0.60);

            // Display the initial world
            Console.WriteLine("Initial World:");
            DisplayWorld(w);
            Console.WriteLine("\nPress any key to start conquering...");
            Console.ReadKey();

            // Test all three strategies on the same world
            var strategies = new IConquerer[]
            {
                new Conquerer1(),
                new Conquerer2(),
                new Conquerer3()
            };



            var strategyNames = new string[]
            {
                "Random Conquest",
                "Most Empty Neighbours",
                "Systematic Conquest"
            };

            // Parameters for conquest
            const int numberOfEmpires = 5;
            const int numberOfTurns = 25000;

            // Execute each strategy and show results
            for (int i = 0; i < strategies.Length; i++)
            {
                Console.Clear();
                Console.WriteLine($"\nExecuting {strategyNames[i]} Strategy:");

                var result = strategies[i].Conquer(w, numberOfEmpires, numberOfTurns);

                // Display the result
                Console.WriteLine($"\nResult of {strategyNames[i]}:");
                DisplayConqueredWorld(result);

                // Save visualization
                var bmw = new BitmapWriter();
                bmw.DrawWorld(result);

                // Calculate and display statistics
                DisplayStatistics(result, numberOfEmpires);

                //Console.WriteLine("\nPress any key to continue to next strategy...");
                //Console.ReadKey();
            }


            var mongoService = new MongoDbService("mongodb://localhost:27017", "WorldDatabase");

            // Generate World
            RandomWorldGenerator world = new RandomWorldGenerator();
            bool[,] generatedWorld = world.GenerateWorld(100, 100, 0.6);

            // Save World to MongoDB
            mongoService.SaveWorld("Worlds", "TestWorld1", "CoverageAlgorithm", 100, 100, 0.6, generatedWorld);

            Console.WriteLine("World saved to MongoDB!");
        }

        static void DisplayWorld(bool[,] world)
        {
            for (int i = 0; i < world.GetLength(1); i++)
            {
                for (int j = 0; j < world.GetLength(0); j++)
                {
                    Console.Write(world[j, i] ? "*" : " ");
                }
                Console.WriteLine();
            }
        }

        static void DisplayConqueredWorld(int[,] world)
        {
            for (int i = 0; i < world.GetLength(1); i++)
            {
                for (int j = 0; j < world.GetLength(0); j++)
                {
                    switch (world[j, i])
                    {
                        case -1: Console.Write(" "); break;
                        case 0: Console.Write("."); break;
                        default: Console.Write(world[j, i]); break;
                    }
                }
                Console.WriteLine();
            }
        }

        static void DisplayStatistics(int[,] world, int numberOfEmpires)
        {
            var totalCells = world.GetLength(0) * world.GetLength(1);
            var empireCounts = new int[numberOfEmpires + 1]; // +1 for unconquered territory (0)

            // Count cells for each empire
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


            // Display statistics
            Console.WriteLine("\nStatistics:");
            Console.WriteLine($"Unconquered territory: {empireCounts[0]} cells ({(empireCounts[0] * 100.0 / totalCells):F2}%)");
            for (int i = 1; i <= numberOfEmpires; i++)
            {
                Console.WriteLine($"Empire {i}: {empireCounts[i]} cells ({(empireCounts[i] * 100.0 / totalCells):F2}%)");
            }
        }

        static async Task AddToMongo(string[] args) // Make Main async
        {
            Console.WriteLine("World Conquest Simulation\n");

            try
            {
                // Test MongoDB connection first
                var worldService = new WorldService();

                // Generate and save worlds
                var worldGenerator = new RandomWorldGenerator();
                await worldGenerator.GenerateAndSaveWorldsAsync(worldService, 3); // Save 3 test worlds
                Console.WriteLine("Successfully saved worlds to MongoDB!");

                // Continue with your existing simulation...
                var w = worldGenerator.GenerateWorld(100, 100, 0.60);

                // Rest of your existing code...

            }
            catch (MongoDB.Driver.MongoConnectionException ex)
            {
                Console.WriteLine($"Failed to connect to MongoDB: {ex.Message}");
                Console.WriteLine("Please ensure MongoDB is running on localhost:27017");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }




    }

}
