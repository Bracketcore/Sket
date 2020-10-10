using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Bracketcore.Sket.StateManager
{
    public abstract class ISketAppState<T> : ComponentBase
    {
        [Inject] public T AppState { get; set; }
        
        public event Action<ComponentBase, string> StateChanged;

        public void NotifyStateChanged(ComponentBase source, string property)
        {
            StateChanged?.Invoke(source, property);
        }

        private async Task AppState_StateChanged(ComponentBase Source, string Property)
        {
            if (Source != this) await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            StateChanged += async (Source, Property) => await AppState_StateChanged(Source, Property);

            base.OnInitialized();
        }

      
    }
}