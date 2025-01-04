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

            for (int i = 0; i < 10; i++) // 10 werelden simuleren
            {
                // Genereer een wereld
                int width = random.Next(30, 101);
                int height = random.Next(30, 101);
                double coverage = 0.15 + (random.NextDouble() * (0.95 - 0.15));
                var worldGenerator = new RandomWorldGenerator();
                bool[,] world = worldGenerator.GenerateWorld(width, height, coverage);

                // Genereer 4-5 rijken
                int numberOfEmpires = random.Next(4, 6);
                var conquerStrategies = new List<IConquerer>
                {
                    new Conquerer1(),
                    new Conquerer2(),
                    new Conquerer3()
                };

                // Koppel een willekeurige strategie aan elk rijk
                var assignedStrategies = new List<IConquerer>();
                for (int e = 0; e < numberOfEmpires; e++)
                {
                    assignedStrategies.Add(conquerStrategies[random.Next(conquerStrategies.Count)]);
                }

                mongoService.SaveWorld("Worlds", planets[i], "Random", width, height, coverage, world);
                Console.WriteLine($"World {planets[i]} saved to MongoDB!");

                Console.WriteLine("Initial World:");
                DisplayWorld(world);

                const int numberOfTurns = 25000;
                var result = new int[width, height];

                // Voer veroveringen uit
                for (int empireId = 1; empireId <= numberOfEmpires; empireId++)
                {
                    Console.WriteLine($"\nExecuting strategy for Empire {empireId}: {assignedStrategies[empireId - 1].GetType().Name}");

                    // Voer de strategie uit voor dit rijk
                    result = assignedStrategies[empireId - 1].Conquer(world, empireId, numberOfTurns);

                    // Toon tussenresultaat
                    Console.WriteLine($"\nResult for Empire {empireId}:");
                    DisplayConqueredWorld(result);
                }

                // Save visualisatie
                var bmw = new BitmapWriter();
                bmw.DrawWorld(result);

                // Bereken statistieken
                DisplayStatistics(result, numberOfEmpires);
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
            var empireCounts = new int[numberOfEmpires + 1]; // +1 voor onbezet gebied (0)

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

            Console.WriteLine("\nStatistics:");
            Console.WriteLine($"Unconquered territory: {empireCounts[0]} cells ({(empireCounts[0] * 100.0 / totalCells):F2}%)");
            for (int i = 1; i <= numberOfEmpires; i++)
            {
                Console.WriteLine($"Empire {i}: {empireCounts[i]} cells ({(empireCounts[i] * 100.0 / totalCells):F2}%)");
            }
        }
    }
}
