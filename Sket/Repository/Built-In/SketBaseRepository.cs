using Bracketcore.KetAPI.Interfaces;
using Bracketcore.KetAPI.Model;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bracketcore.KetAPI.Repository
{
    /// <summary>
    /// Based Repository 
    /// </summary>
    /// <typeparam name="T">Repository Model</typeparam>
    public class SketBaseRepository<T> : IBaseRepository<T> where T : SketPersistedModel
    {

        public SketContextModel<T> SketContextModel { get; set; }

        public SketBaseRepository()
        {
            SketContextModel = new SketContextModel<T>();
        }

        /// <summary>
        /// Send every document modification to _context model
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public virtual Task<SketContextModel<T>> BeforeCreate(T doc)
        {
            SketContextModel.Model = doc;
            return Task.FromResult(SketContextModel);
        }

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

        public virtual Task<T> AfterCreate(T doc)
        {
            return Task.FromResult(doc);
        }

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

        public virtual async Task<int> Count()
        {
            var c = await DB.Queryable<T>().ToListAsync();
            return c.Count;
        }

        public virtual async Task<List<T>> FindAll()
        {
            var filter = Builders<T>.Filter.Empty;

            return await DB.Queryable<T>().ToListAsync();
        }

        public virtual async Task<T> FindById(string id)
        {
            return await DB.Find<T>().OneAsync(id);
        }

        public virtual async Task<string> Update(string id, T doc)
        {
            doc.ID = id;
            var filter = Builders<T>.Filter.Eq(i => i.ID, id);

            await DB.Collection<T>().ReplaceOneAsync(filter, doc, new ReplaceOptions() { IsUpsert = true });

            return $"{id} updated";
        }

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

        public virtual Task<string> BeforeDestroyById(string id)
        {
            return Task.FromResult(id);
        }

        public virtual async Task<string> DestroyById(string id)
        {
            var d = await DB.Collection<T>().DeleteOneAsync(i => i.ID == id);
            return $"{d}";
        }

        public virtual Task<string> AfterDestroyById(string id)
        {
            return Task.FromResult(id);
        }

        public virtual async Task<bool> Exist(string id)
        {
            var exist = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.ID == id);

            return exist != null;
        }

    }
}