using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Sket.Core.StateManager
{
    public abstract class SketAppState<T> : ComponentBase where T : ISketAppState
    {
        [Inject] public T AppState { get; set; }

    
        public event Action<ComponentBase> StateChanged;

        protected void NotifyStateChanged(ComponentBase source)
        {
            StateChanged?.Invoke(source);
        }

        private async Task AppState_StateChanged(ComponentBase source)
        {
            if (source != this) await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            AppState.StateChanged += async source => await AppState_StateChanged(source);
        }

       
    }

    public interface ISketAppState
    {
        public event Action<ComponentBase> StateChanged;
    }
}