public class Database
{
    private IMongoDatabase _database;
    public Database(string connectionString, string databaseName)
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Region> Regions
        => _database.GetCollection<Region>("Regions");
}