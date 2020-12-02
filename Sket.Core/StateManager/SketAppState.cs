using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Sket.Core.StateManager
{
    public abstract class SketAppState<T> : ComponentBase, IDisposable where T : ISketAppState
    {
        [Inject] public T AppState { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event Action<ComponentBase> StateChanged;

        protected void NotifyStateChanged(ComponentBase Source)
        {
            StateChanged?.Invoke(Source);
        }

        private async Task AppState_StateChanged(ComponentBase Source)
        {
            if (Source != this) await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            AppState.StateChanged += async Source => await AppState_StateChanged(Source);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }

    public interface ISketAppState
    {
        public event Action<ComponentBase> StateChanged;
    }
}