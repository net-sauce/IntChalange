using MongoDB.Bson.Serialization.Attributes;

namespace API.USER.Entities
{
    [BsonNoId]
    [BsonIgnoreExtraElements]
    public record UserIDEntity
    {
        public int UserID { get; init; }
        public string UserName { get; init; }
    }
}
