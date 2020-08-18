using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bracketcore.KetAPI.Interfaces
{
    public interface ICrudRepository<T>
    {
        Task<object> Create(T doc);
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(string id);
        Task<string> Update(string id, T updateDoc);
        Task<string> DeleteById(string id);
        Task<bool> Exist(string id);
        void ChangeEvent();
    }
}