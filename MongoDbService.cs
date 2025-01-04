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
}