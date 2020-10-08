using MongoDB.Entities;
using System;
using System.Text.Json.Serialization;

namespace Bracketcore.Sket.Entity
{
    /// <summary>
    /// Abstract model for the Persisted model
    /// </summary>
    public abstract class SketPersistedModel : MongoDB.Entities.Entity, IDisposable, IModifiedOn, ICreatedOn
    {
    [JsonIgnore]
        public One<SketUserModel> OwnerID { get; set; }
        [JsonIgnore]
        public DateTime ModifiedOn { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [JsonIgnore]
        public DateTime CreatedOn { get; set; }
    }
}