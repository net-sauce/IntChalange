using MongoDB.Bson.Serialization.Attributes;

namespace API.USER.Entities
{
    [BsonNoId]
    [BsonIgnoreExtraElements]
    public record UserEntity: UserIDEntity
    {

        public IEnumerable<ClientEntity> Clients { get; set; }
    }
}
