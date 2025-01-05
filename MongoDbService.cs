using MongoDB.Bson;
using MongoDB.Driver;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public async Task ClearWorldsCollectionAsync(string collectionName)
    {
        var collection = _database.GetCollection<BsonDocument>(collectionName);
        await collection.DeleteManyAsync(Builders<BsonDocument>.Filter.Empty);
    }

    public async Task SaveWorldAsync(string collectionName, string planetName, string generationType,
        int width, int height, double coverage, bool[,] world)
    {
        var collection = _database.GetCollection<BsonDocument>(collectionName);

        var document = new BsonDocument
        {
            { "planetName", planetName },
            { "generationType", generationType },
            { "width", width },
            { "height", height },
            { "coverage", coverage },
            { "world", ConvertWorldToBsonArray(world) }
        };

        await collection.InsertOneAsync(document);
    }

    public async Task SaveStatsWithAverageAsync(string collectionName, string planetName, int simulationNumber, double totalCells, double[] percentConquered, Dictionary<string, double> averageConquered)
    {
        var stats = new BsonDocument
    {
        { "Planet", planetName },
        { "SimulationNumber", simulationNumber },
        { "TotalCells", totalCells },
        { "PercentConquered", new BsonArray(percentConquered) },
        { "AverageConquered", new BsonDocument(averageConquered.Select(kvp => new BsonElement(kvp.Key, kvp.Value))) }
    };

        var collection = _database.GetCollection<BsonDocument>(collectionName);
        await collection.InsertOneAsync(stats);
    }


    private BsonArray ConvertWorldToBsonArray(bool[,] world)
    {
        var array = new BsonArray();
        for (int i = 0; i < world.GetLength(0); i++)
        {
            var row = new BsonArray();
            for (int j = 0; j < world.GetLength(1); j++)
            {
                row.Add(world[i, j]);
            }
            array.Add(row);
        }
        return array;
    }

    public async Task SaveStatsAsync(string collectionName, string planetName, int simulationNumber, double totalCells, double[] percentConquered)
    {
        var stats = new BsonDocument
    {
        { "Planet", planetName },
        { "SimulationNumber", simulationNumber },
        { "TotalCells", totalCells },
        { "PercentConquered", new BsonArray(percentConquered)
            }
    };

        var collection = _database.GetCollection<BsonDocument>(collectionName);
        await collection.InsertOneAsync(stats);
    }



}