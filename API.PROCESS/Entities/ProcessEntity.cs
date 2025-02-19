using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API.PROCESS.Entities
{
    [BsonNoId]
    [BsonIgnoreExtraElements]
    public record ProcessEntity
    {
        [BsonRepresentation(BsonType.String)] 
        public Guid PorcessID { get; init; }
        public string ProcessName { get; init; }
        public int ClientID { get; init; }
        public required List<string> FilesRequired { get; set; }
    }
}
