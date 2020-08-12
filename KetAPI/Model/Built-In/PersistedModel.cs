using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{

    public abstract class PersistedModel : Entity, IModifiedOn, ICreatedOn
    {
        public DateTime ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string OwnerID { get; set; }

    }
}