using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Bracketcore.Sket.StateManager
{
    /// <summary>
    ///  Use to handle App state in blazor app
    /// </summary>
    /// <typeparam name="T">Should be a typeof SketAppState </typeparam>
    public abstract class SketAppStateComponentBase<T> : ComponentBase, ISketAppStateComponentBase, IDisposable
        where T : SketAppState
    {
        [Inject] public T AppState { get; set; }

        public async Task AppStateStateChanged(ComponentBase source, string property)
        {
            if (source != this)
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        protected override void OnInitialized()
        {
            AppState.StateChanged += async (source, property) => await AppStateStateChanged(source, property);
        }

        public void Dispose()
        {
            AppState.StateChanged -= async (source, property) => await AppStateStateChanged(source, property);
            GC.SuppressFinalize(this);
        }

    }
}