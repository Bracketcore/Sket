using System;

namespace Sket.Core.Models
{
    public class SketContextModel<T> : IDisposable
    {
        public T Model { get; set; }

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