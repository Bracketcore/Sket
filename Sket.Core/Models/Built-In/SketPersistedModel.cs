using System;
using System.Text.Json.Serialization;
using MongoDB.Entities;

namespace Sket.Core.Models
{
    /// <summary>
    ///     Abstract model for the Persisted model
    /// </summary>
    public class SketPersistedModel : Entity, IDisposable, ICreatedOn, IModifiedOn
    {
        [JsonIgnore] public One<SketUserModel> OwnerId { get; set; }

        [JsonIgnore] public DateTime CreatedOn { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [JsonIgnore] public DateTime ModifiedOn { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}