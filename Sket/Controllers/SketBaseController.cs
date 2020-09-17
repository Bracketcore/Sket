using System;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


//Todo add a rate limiter
namespace Bracketcore.Sket.Controllers
{
    /// <summary>
    /// Abstract Base Controller
    /// </summary>
    /// <typeparam name="T">Controller model</typeparam>
    /// <typeparam name="TC">Controller model</typeparam>
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public abstract class SketBaseController<T, TC> : ControllerBase, IDisposable
        where T : SketPersistedModel
        where TC : SketBaseRepository<T>
    {
        // public virtual H Hub { get; set; }

        public SketBaseController(TC repo)
        {
            Repo = repo;
            // Hub = hub;
        }

        protected TC Repo { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            return Ok(await Repo.FindAll().ConfigureAwait(false));
        }

        [HttpGet("{id}")]
        public virtual async Task<IActionResult> GetById(string id)
        {
            var exist = await Repo.Exist(id).ConfigureAwait(false);

            if (!exist) return NotFound();

            var check = await Repo.FindById(id).ConfigureAwait(false);
            return Ok(check);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create([FromBody] T doc)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (doc == null) return BadRequest();

                    var cre = await Repo.Create(doc);
                    if (cre == null) return BadRequest();
                    return Created(typeof(T).Name + " Created", JsonConvert.SerializeObject(cre));
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [Authorize(Roles = "User,Admin,Support")]
        [HttpPut("{id}")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Update(string id, T replace)
        {
            //Check if user is owner
            var exist = await Repo.Exist(id).ConfigureAwait(false);

            if (!exist) return NotFound();
            _ = await Repo.Update(id, replace).ConfigureAwait(false);
            return NoContent();
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpDelete("{id}")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Remove(string id)
        {
            var exist = await Repo.Exist(id).ConfigureAwait(false);

            if (!exist) return NotFound();
            _ = await Repo.DestroyById(id).ConfigureAwait(false);
            return NoContent();
        }

        [Authorize(Roles = "App")]
        [HttpGet("exist/{id}")]
        public virtual IActionResult Exist(string id)
        {
            return Ok(Repo.Exist(id));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) Repo?.Dispose();
        }
    }
}