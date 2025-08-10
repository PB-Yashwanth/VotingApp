using MongoDB.Driver;
using VotingApp.Models;

namespace VotingApp.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Vote> _votesCollection;

        public MongoDbService(IConfiguration config)
        {
            var connectionString = config.GetConnectionString("MongoDb");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("VotingAppDB");
            _votesCollection = database.GetCollection<Vote>("Votes");
        }

        public bool VoterExists(string voterName)
        {
            return _votesCollection.Find(v => v.VoterName.ToLower() == voterName.ToLower()).Any();
        }

        public bool IPExists(string ip)
        {
            return _votesCollection.Find(v => v.IP == ip).Any();
        }

        public void AddVote(Vote vote)
        {
            _votesCollection.InsertOne(vote);
        }

        public List<Vote> GetVotes()
        {
            return _votesCollection.Find(_ => true).ToList();
        }
    }
}
