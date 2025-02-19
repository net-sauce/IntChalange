using MongoDB.Bson.Serialization.Attributes;

namespace API.USER.Entities
{
    [BsonNoId]
    [BsonIgnoreExtraElements]
    public record ClientEntity
    {
        public int ID { get; init; }
        public string Name { get; init; }
    }
}
