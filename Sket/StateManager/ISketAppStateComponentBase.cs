using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Bracketcore.Sket.StateManager
{
    public interface ISketAppStateComponentBase
    {
        private Task AppState_StateChanged(ComponentBase source, string property)
        {
            throw new System.NotImplementedException();
        }
    }
}