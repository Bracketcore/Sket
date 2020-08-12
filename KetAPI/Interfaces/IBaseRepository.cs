using System.Collections.Generic;
using System.Threading.Tasks;
using Bracketcore.KetAPI.Model;

namespace Bracketcore.KetAPI.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<ContextModel<T>> BeforeCreate(T Doc);
        Task<T> Create(T Doc);
        Task<T> AfterCreate(T Doc);
        Task<string> CreateBulk(IEnumerable<T> fix);
        Task<int> Count();
        Task<List<T>> FindAll();
        Task<T> FindById(string Id);
        Task<string> Update(string Id, T Doc);
        Task<string> BulkUpdate(IEnumerable<T> Doc);
        Task<string> DestroyAll(IEnumerable<T> Doc);
        Task<string> BeforeDestroyById(string Id);
        Task<string> DestroyById(string Id);
        Task<string> AfterDestroyById(string Id);
        Task<bool> Exist(string Id);
    }
}