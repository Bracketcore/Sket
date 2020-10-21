using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace Bracketcore.Sket.Repository
{
    /// <summary>
    ///     Based Repository
    /// </summary>
    /// <typeparam name="T">Repository Model</typeparam>
    public class SketBaseRepository<T> : ISketBaseRepository<T>, IDisposable where T : SketPersistedModel
    {
        protected Transaction _Tn = DB.Transaction();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        ///     Set model modification before its been created.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public virtual Task<T> BeforeCreate(T doc)
        {
            return Task.FromResult(doc);
        }

        /// <summary>
        ///     Create model
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public virtual async Task<T> Create(T doc)
        {
            try
            {
                var before = await BeforeCreate(doc);
                var n = await _Tn.SaveAsync(before);
                var after = await AfterCreate(doc);

                return after;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        ///     Set model modification after its been created.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public virtual async Task<T> AfterCreate(T doc)
        {
            await _Tn.CommitAsync();
            return await Task.FromResult(doc);
        }

        /// <summary>
        ///     Create multiple
        /// </summary>
        /// <param name="fix"></param>
        /// <returns></returns>
        public virtual async Task<string> CreateBulk(IEnumerable<T> fix)
        {
            var ls = new List<string>();

            foreach (var fi in fix)
            {
                var d = await Create(fi);
                ls.Add(d.ID);
            }

            return $"{string.Join(",", ls.ToArray())} created";
        }

        /// <summary>
        ///     Get count on the model
        /// </summary>
        /// <returns></returns>
        public virtual async Task<int> Count()
        {
            var c = await DB.Queryable<T>().ToListAsync();
            return c.Count;
        }

        /// <summary>
        ///     Get all model data
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<T>> FindAll()
        {
            var filter = Builders<T>.Filter.Empty;

            return await DB.Queryable<T>().ToListAsync();
        }

        /// <summary>
        ///     Get model data by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> FindById(string id)
        {
            return await DB.Find<T>().OneAsync(id);
        }

        /// <summary>
        ///     Update models by the id and the model structure
        /// </summary>
        /// <param name="id"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public virtual async Task<string> Update(string id, T doc)
        {
            try
            {
                doc.ID = id;
                await _Tn.SaveAsync(doc);
                await _Tn.CommitAsync();
                return $"{id} updated";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        ///     Set a list bulk model to be updated
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public virtual async Task<string> BulkUpdate(IEnumerable<T> doc)
        {
            var ls = new List<string>();
            foreach (var document in doc)
            {
                await Update(document.ID, document);
                ls.Add(document.ID);
            }

            return $"Updated {string.Join(",", ls.ToArray())}";
        }

        /// <summary>
        ///     Destroy bulk model list
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public virtual async Task<string> DestroyAll(IEnumerable<T> doc)
        {
            var ls = new List<string>();

            foreach (var docs in doc)
            {
                await DestroyById(docs.ID);
                ls.Add(docs.ID);
            }

            return $"{string.Join(",", ls.ToArray())} Deleted";
        }

        /// <summary>
        ///     modify model to destroy by id before been destroyed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<string> BeforeDestroyById(string id)
        {
            return Task.FromResult(id);
        }

        /// <summary>
        ///     destroy model by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<string> DestroyById(string id)
        {
            try
            {
                var before = await BeforeDestroyById(id);
                var del = await _Tn.DeleteAsync<T>(before);
                var after = await AfterDestroyById(id);
                return $"{id} Deleted";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        ///     modify model to destroy by id after been destroyed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<string> AfterDestroyById(string id)
        {
            await _Tn.CommitAsync();
            return await Task.FromResult(id);
        }

        /// <summary>
        ///     Check if model exist by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<bool> Exist(string id)
        {
            var exist = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.ID == id);

            return exist != null;
        }

        public virtual async Task<IEnumerable<T>> FindByFilter(FilterDefinition<T> filter)
        {
            var sort = Builders<T>.Sort.Descending(a => a.ID);
            return await FindByFilter(filter, sort, 0, 0);
        }

        public virtual async Task<IEnumerable<T>> FindByFilter(FilterDefinition<T> filter, SortDefinition<T> sort)
        {
            return await FindByFilter(filter, sort, 0, 0);
        }

        public virtual async Task<IEnumerable<T>> FindByFilter(FilterDefinition<T> filter, SortDefinition<T> sort,
            int skip)
        {
            return await FindByFilter(filter, sort, 0, 0);
        }

        public virtual async Task<IEnumerable<T>> FindByFilter(FilterDefinition<T> filter, SortDefinition<T> sort,
            int skip, int limit)
        {
            return await DB.Fluent<T>().Match(filter)
                .Sort(sort)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO release unmanaged resources here
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing) _Tn?.Dispose();
        }

        ~SketBaseRepository()
        {
            Dispose(false);
        }
    }
}