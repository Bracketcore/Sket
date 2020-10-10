using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace Bracketcore.Sket.Repository
{
    /// <summary>
    ///     Based Repository
    /// </summary>
    /// <typeparam name="T">Repository Model</typeparam>

    public class SketBaseRepository<T> : ISketBaseRepository<T> where T : SketPersistedModel
    {
        public SketBaseRepository()
        {
            SketContextModel = new SketContextModel<T>();
        }

        public SketContextModel<T> SketContextModel { get; set; }

        /// <summary>
        ///     Set model modification before its been created.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public virtual Task<SketContextModel<T>> BeforeCreate(T doc)
        {
            SketContextModel.Model = doc;
            return Task.FromResult(SketContextModel);
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
                await DB.SaveAsync(before.Model);
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
        public virtual Task<T> AfterCreate(T doc)
        {
            return Task.FromResult(doc);
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
            doc.ID = id;
            var filter = Builders<T>.Filter.Eq(i => i.ID, id);

            await DB.Collection<T>().ReplaceOneAsync(filter, doc, new ReplaceOptions {IsUpsert = true});

            return $"{id} updated";
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
            var d = await DB.Collection<T>().DeleteOneAsync(i => i.ID == id);
            return $"{d}";
        }

        /// <summary>
        ///     modify model to destroy by id after been destroyed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual Task<string> AfterDestroyById(string id)
        {
            return Task.FromResult(id);
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


        //
        // protected virtual void Dispose(bool disposing)
        // {
        //     if (disposing)
        //     {
        //     }
        // }
        //
        // /// <summary>
        // /// Dispose method
        // /// </summary>
        // public void Dispose()
        // {
        //     Dispose(true);
        //     GC.SuppressFinalize(this);
        // }
    }
}