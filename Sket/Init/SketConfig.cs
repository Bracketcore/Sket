using Bracketcore.Sket.Entity;
using System;
using System.Collections.Generic;

namespace Bracketcore.Sket
{
    public class SketConfig : IDisposable
    {
        public SketSettings Settings { get; set; }
        public List<Type> Context { get; set; } = new List<Type>();
        public IEnumerable<SketRoleModel> Roles { get; set; } = new List<SketRoleModel>();

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