using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sket.Core.Controllers
{
    public interface ISketBaseController<T>
    {
        Task<ActionResult<T>> Create(T doc);
        Task<ActionResult<T>> GetAll();
        Task<ActionResult<T>> GetById(string id);
        Task<ActionResult<T>> Update(string id, T updateDoc);
        Task<ActionResult<T>> Remove(string id);
        Task<ActionResult> Exist(string id);
    }
}