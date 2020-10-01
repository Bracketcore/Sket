using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Bracketcore.Sket.Controllers
{
    public interface ISketBaseController<T>
    {
        Task<IActionResult> Create(T doc);
        Task<IActionResult> GetAll();
        Task<IActionResult> GetById(string id);
        Task<IActionResult> Update(string id, T updateDoc);
        Task<IActionResult> Remove(string id);
        IActionResult Exist(string id);
    }
}