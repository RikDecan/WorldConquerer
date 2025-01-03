using ConsoleAppSquareMaster.World;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ConsoleAppSquareMaster
{
    public class WorldModel
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string GenerationType { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double Coverage { get; set; }
        public bool[,] WorldData { get; set; }
    }

    public class ConquestResult
    {
        public ObjectId Id { get; set; }
        public ObjectId WorldId { get; set; }
        public Dictionary<int, EmpireData> Empires { get; set; }
        public DateTime SimulationDate { get; set; }
    }

    public class EmpireData
    {
        public int EmpireId { get; set; }
        public string ConquerStrategy { get; set; }
        public int CellCount { get; set; }
        public double CoveragePercentage { get; set; }
        public (int x, int y) StartPosition { get; set; }
    }

    public class WorldService
    {
        private readonly IMongoCollection<WorldModel> _worlds;
        private readonly IMongoCollection<ConquestResult> _conquestResults;

        public WorldService()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("WorldDatabase");
            _worlds = database.GetCollection<WorldModel>("Worlds");
            _conquestResults = database.GetCollection<ConquestResult>("ConquestResults");
            //doe maar verder


        }



        public async Task SaveWorldAsync(string name, string generationType, bool[,] worldData, int width, int height, double coverage)
        {
            var world = new WorldModel
            {
                Name = name,
                GenerationType = generationType,
                WorldData = worldData,
                Width = width,
                Height = height,
                Coverage = coverage
            };

            await _worlds.InsertOneAsync(world);
        }

        public async Task SaveConquestResultAsync(ObjectId worldId, Dictionary<int, EmpireData> empireData)
        {
            var result = new ConquestResult
            {
                WorldId = worldId,
                Empires = empireData,
                SimulationDate = DateTime.UtcNow
            };

            await _conquestResults.InsertOneAsync(result);
        }

        public async Task<IEnumerable<ConquestResult>> GetStatisticsAsync(string conquerStrategy)
        {
            var filter = Builders<ConquestResult>.Filter.ElemMatch(
                x => x.Empires.Values,
                empire => empire.ConquerStrategy == conquerStrategy
            );

            return await _conquestResults.Find(filter).ToListAsync();
        }
    }

    public static class WorldGeneratorExtensions
    {
        public static async Task GenerateAndSaveWorldsAsync(this RandomWorldGenerator generator, WorldService service, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var width = 100;
                var height = 100;
                var coverage = 0.60;

                var world = generator.GenerateWorld(width, height, coverage);
                var generationType = generator.GetLastUsedGenerationType();

                await service.SaveWorldAsync(
                    $"World_{DateTime.UtcNow.Ticks}_{i}",
                    generationType,
                    world,
                    width,
                    height,
                    coverage
                );
            }
        }
    }


}