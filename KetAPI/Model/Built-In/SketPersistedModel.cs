using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace Bracketcore.KetAPI.Model
{

    public abstract class SketPersistedModel : Entity, ICreatedOn
    {
        public DateTime CreatedOn { get; set; }
        public One<SketUserModel> OwnerID { get; set; }

    }
}