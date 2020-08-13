using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bracketcore.KetAPI.Controllers
{
    public interface ICrudRepository<T>
    {
        Task<object> Create(T Doc);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(string id);
        Task<string> Update(string id, T updateDoc);
        Task<string> DeleteById(string Id);
        Task<bool> Exist(string id);
        void ChangeEvent();
    }
}