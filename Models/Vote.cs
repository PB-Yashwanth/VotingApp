using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace VotingApp.Models
{
    public class Vote
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public string VoterName { get; set; }
        public string Candidate { get; set; }
        public string IP { get; set; }
        public DateTime VoteTime { get; set; }
    }
}
