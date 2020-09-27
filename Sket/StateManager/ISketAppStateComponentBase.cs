using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Bracketcore.Sket.StateManager
{
    public interface ISketAppStateComponentBase
    {
        Task AppStateStateChanged(ComponentBase source, string property);
    }
}