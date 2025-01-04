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


            string[] planets = { "Tatooine", "Scarif", "Endor", "Ilum", "Alderaan", "Coruscant", "Mandalore", "Naboo", "Mustafar", "Crait" };

            Random random = new Random();

            var mongoService = new MongoDbService("mongodb://localhost:27017", "WorldDatabase");

            mongoService.ClearWorldsCollection("Worlds");

            bool[,] w = null;


            for (int i = 0; i < 10; i++)
            {

                var strategies = new IConquerer[]
                {
                new Conquerer1(),
                new Conquerer2(),
                new Conquerer3()
                };



                int width = random.Next(30, 101);
                int height = random.Next(30, 101);
                double coverage = 0.15 + (random.NextDouble() * (0.95 - 0.15));

                var worldGenerator = new RandomWorldGenerator();
                w = worldGenerator.GenerateWorld(width, height, coverage);

                mongoService.SaveWorld("Worlds", planets[i], "Random", width, height, coverage, w);
                Console.WriteLine("World saved to MongoDB!");

                Console.WriteLine("Initial World:");
                DisplayWorld(w);


            }




            var strategyNames = new string[]
            {
                "Random Conquest",
                "Most Empty Neighbours",
                "Systematic Conquest"
            };

            // Parameters for conquest
            const int numberOfEmpires = 1;
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

    }

}
