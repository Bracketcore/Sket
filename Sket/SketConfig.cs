using System;
using System.Collections.Generic;
using Bracketcore.Sket.Entity;

namespace Bracketcore.Sket
{
    public class SketConfig : IDisposable
    {
        public SketSettings Settings { get; set; }
        public List<Type> Context { get; set; }
        public IEnumerable<SketRoleModel> Roles { get; set; }

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