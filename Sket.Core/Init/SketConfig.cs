using System;
using System.Collections.Generic;
using UnoRoute.Sket.Core.Entity;

namespace UnoRoute.Sket.Core.Init
{
    public class SketConfig : IDisposable
    {
        public SketSettings Settings { get; set; }
        public List<Type> Context { get; set; } = new List<Type>();
        public IEnumerable<SketRoleModel> Roles { get; set; } = new List<SketRoleModel>();

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