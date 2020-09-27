using System;
using Microsoft.AspNetCore.Components;

namespace Bracketcore.Sket.StateManager
{
    public interface ISketAppState
    {
        public event Action<ComponentBase, string> StateChanged;

        public void NotifyStateChanged(ComponentBase source, string property);
    }
}