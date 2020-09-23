using System;
using MongoDB.Entities;

namespace Bracketcore.Sket.Entity
{
    /// <summary>
    /// Abstract model for the Persisted model
    /// </summary>
    public abstract class SketPersistedModel : MongoDB.Entities.Entity, IDisposable
    {
        public One<SketUserModel> OwnerID { get; set; }

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
    }
}