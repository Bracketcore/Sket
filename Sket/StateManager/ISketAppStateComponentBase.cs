using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Bracketcore.Sket.StateManager
{
    public interface ISketAppStateComponentBase
    {
        Task AppStateStateChanged(ComponentBase source, string property);
    }
}