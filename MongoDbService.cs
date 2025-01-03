using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace ConsoleAppSquareMaster
{
    internal class MongoDbService
    {
        private readonly IMongoDatabase _database;

        public MongoDbService(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            return _database.GetCollection<BsonDocument>(collectionName);
        }

        public void SaveWorld(string collectionName, string name, string algorithm, int maxX, int maxY, double coverage, bool[,] worldData)
        {
            var collection = GetCollection(collectionName);

            var worldDocument = new BsonDocument
            {
                { "name", name },
                { "algorithm", algorithm },
                { "dimensions", new BsonDocument { { "x", maxX }, { "y", maxY } } },
                { "coverage", coverage },
                { "worldData", ConvertWorldToBsonArray(worldData) }
            };

            collection.InsertOne(worldDocument);
        }

        private BsonArray ConvertWorldToBsonArray(bool[,] worldData)
        {
            var array = new BsonArray();
            for (int i = 0; i < worldData.GetLength(0); i++)
            {
                var row = new BsonArray();
                for (int j = 0; j < worldData.GetLength(1); j++)
                {
                    row.Add(worldData[i, j]);
                }
                array.Add(row);
            }
            return array;
        }




    }
}
