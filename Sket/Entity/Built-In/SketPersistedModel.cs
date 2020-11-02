using System;
using System.Text.Json.Serialization;
using MongoDB.Entities;

namespace Bracketcore.Sket.Entity
{
    /// <summary>
    ///     Abstract model for the Persisted model
    /// </summary>
    public abstract class SketPersistedModel : MongoDB.Entities.Entity, IDisposable, ICreatedOn
    {
        [JsonIgnore] public One<SketUserModel> OwnerID { get; set; }

        [JsonIgnore] public DateTime ModifiedOn { get; set; }

        [JsonIgnore] public DateTime CreatedOn { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}