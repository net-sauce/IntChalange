using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace API.PROCESS.Entities
{
    [BsonNoId]
    [BsonIgnoreExtraElements]
    public record JobEnity
    {
        [BsonRepresentation(BsonType.String)]
        public Guid JobID { get; init; }
        public int ClientID { get; init; }
        public List<string>? FilesRequired { get; init; }

        public List<string>? FillesUploaded { get; init; }
        public JobStatus Status { get; init; }
    }

}