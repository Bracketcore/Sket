using System;
using MongoDB.Entities;

namespace Bracketcore.Sket.Model
{

    public abstract class SketPersistedModel : Entity
    {
        public One<SketUserModel> OwnerID { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}