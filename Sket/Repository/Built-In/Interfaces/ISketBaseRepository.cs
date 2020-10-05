using System.Collections.Generic;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using MongoDB.Driver;

namespace Bracketcore.Sket.Repository.Interfaces
{
    public interface ISketBaseRepository<T>
    {
        public SketContextModel<T> SketContextModel { get; set; }
        Task<SketContextModel<T>> BeforeCreate(T doc);
        Task<T> Create(T doc);
        Task<T> AfterCreate(T doc);
        Task<string> CreateBulk(IEnumerable<T> fix);
        Task<int> Count();
        Task<List<T>> FindAll();
        Task<T> FindById(string id);
        Task<IEnumerable<T>> FindByFilter(FilterDefinition<T> filter);
        Task<IEnumerable<T>> FindByFilter(FilterDefinition<T> filter, SortDefinition<T> sort);
        Task<IEnumerable<T>> FindByFilter(FilterDefinition<T> filter, SortDefinition<T> sort, int skip);
        Task<IEnumerable<T>> FindByFilter(FilterDefinition<T> filter, SortDefinition<T> sort, int skip, int limit);
        Task<string> Update(string id, T doc);
        Task<string> BulkUpdate(IEnumerable<T> doc);
        Task<string> DestroyAll(IEnumerable<T> doc);
        Task<string> BeforeDestroyById(string id);
        Task<string> DestroyById(string id);
        Task<string> AfterDestroyById(string id);
        Task<bool> Exist(string id);
    }
}