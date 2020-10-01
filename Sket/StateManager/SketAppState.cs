using Microsoft.AspNetCore.Components;
using System;

namespace Bracketcore.Sket.StateManager
{
    public abstract class SketAppState : IDisposable, ISketAppState
    {
        public event Action<ComponentBase, string> StateChanged;

        public void NotifyStateChanged(ComponentBase source, string property) => StateChanged?.Invoke(source, property);

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