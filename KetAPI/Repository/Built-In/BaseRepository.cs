using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bracketcore.KetAPI.Interfaces;
using Bracketcore.KetAPI.Model;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MongoDB.Entities;

namespace Bracketcore.KetAPI.Repository
{
    /// <summary>
    /// Based Repository 
    /// </summary>
    /// <typeparam name="T">Repository Model</typeparam>
    public  class BaseRepository<T> : IBaseRepository<T> where T : PersistedModel
    {
        
        public  ContextModel<T> ContextModel { get; set; }

        public BaseRepository()
        {
            ContextModel = new ContextModel<T>();
        }

        /// <summary>
        /// Send every document modification to _context model
        /// </summary>
        /// <param name="Doc"></param>
        /// <returns></returns>
        public virtual Task<ContextModel<T>> BeforeCreate(T Doc)
        {
            ContextModel.Model = Doc;
            return Task.FromResult(ContextModel);
        }

        public virtual async Task<T> Create(T Doc)
        {
            try
            {
                var before = await BeforeCreate(Doc);
                await DB.SaveAsync(before.Model);
                var after = await AfterCreate(Doc);
                
                return after;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public virtual Task<T> AfterCreate(T Doc)
        {
            return Task.FromResult(Doc);
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

        public virtual async Task<T> FindById(string Id)
        {
            return await DB.Find<T>().OneAsync(Id);
        }

        public virtual async Task<string> Update(string Id, T Doc)
        {
            Doc.ID = Id;
            var filter = Builders<T>.Filter.Eq(i => i.ID, Id);

            await DB.Collection<T>().ReplaceOneAsync(filter, Doc, new ReplaceOptions() {IsUpsert = true});

            return $"{Id} updated";
        }

        public virtual async Task<string> BulkUpdate(IEnumerable<T> Doc)
        {
            var ls = new List<string>();
            foreach (var Document in Doc)
            {
                await Update(Document.ID, Document);
                ls.Add(Document.ID);
            }

            return $"Updated {string.Join(",", ls.ToArray())}";
        }

        public virtual async Task<string> DestroyAll(IEnumerable<T> Doc)
        {
            var ls = new List<string>();

            foreach (var Docs in Doc)
            {
                await DestroyById(Docs.ID);
                ls.Add(Docs.ID);
            }

            return $"{string.Join(",", ls.ToArray())} Deleted";
        }

        public virtual Task<string> BeforeDestroyById(string Id)
        {
            return Task.FromResult(Id);
        }

        public virtual async Task<string> DestroyById(string Id)
        {
            var d = await DB.Collection<T>().DeleteOneAsync(i => i.ID == Id);
            return $"{d}";
        }

        public virtual Task<string> AfterDestroyById(string Id)
        {
            return Task.FromResult(Id);
        }

        public virtual async Task<bool> Exist(string Id)
        {
            var exist = await DB.Queryable<T>().FirstOrDefaultAsync(i => i.ID == Id);

            return exist != null;
        }
        
    }
}