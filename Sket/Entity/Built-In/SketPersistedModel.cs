using System;
using MongoDB.Entities;

namespace Bracketcore.Sket.Entity
{
    /// <summary>
    /// Abstract model for the Persisted model
    /// </summary>
    public abstract class SketPersistedModel : MongoDB.Entities.Entity
    {
        public One<SketUserModel> OwnerID { get; set; }

        public DateTime ModifiedOn { get; set; }
    }
}