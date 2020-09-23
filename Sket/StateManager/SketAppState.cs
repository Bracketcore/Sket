using Microsoft.AspNetCore.Components;
using System;

namespace Bracketcore.Sket.StateManager
{
    public abstract class SketAppState
    {
        public event Action<ComponentBase, string> StateChanged;

        protected void NotifyStateChanged(ComponentBase source, string property) =>
            StateChanged?.Invoke(source, property);
    }
}