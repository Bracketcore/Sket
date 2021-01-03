using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using Sket.Core.Repository.Interfaces;


//Todo add a rate limiter
namespace Sket.Core.Controllers
{
    /// <summary>
    ///     Abstract Base Controller
    /// </summary>
    /// <typeparam name="T">Controller model</typeparam>
    /// <typeparam name="TC">Repository class to use</typeparam>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[Controller]")]
    public abstract class SketBaseController<T, TC> : ControllerBase, ISketBaseController<T>, IDisposable
        where T : IEntity
        where TC : ISketBaseRepository<T>
    {
        // public virtual H Hub { get; set; }

        protected SketBaseController(TC repo)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<T>> GetAll()
        {
            return Ok(await Repo.FindAll().ConfigureAwait(false));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<T>> GetById(string id)
        {
            var exist = await Repo.Exist(id).ConfigureAwait(false);

            if (!exist) return NotFound();

            var check = await Repo.FindById(id).ConfigureAwait(false);
            return Ok(check);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "App,Admin,Support")]
        public virtual async Task<ActionResult<T>> Create([FromBody] T doc)
        {
            try
            {
                // var change = JsonConvert.DeserializeObject<T>(doc);

                if (ModelState.IsValid)
                {
                    if (doc == null) return BadRequest();

                    var cre = await Repo.Create(doc);
                    if (cre == null) return BadRequest();

                    return Created(typeof(T).Name + " Created", cre);
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<T>> Update(string id, [FromBody] T replace)
        {
            async Task<ActionResult<T>> command()
            {
                //Check if user is owner
                var exist = await Repo.Exist(id).ConfigureAwait(false);

                if (!exist) return NotFound();

                _ = await Repo.Update(id, replace).ConfigureAwait(false);
                return NoContent();
            }

            if (HttpContext.User.IsInRole("User"))
            {
                var token = HttpContext.Request.Headers["Authorization"].ToString();

                var user = HttpContext.User.Claims.GetEnumerator();
                return await command();
            }

            return await command();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult<T>> Remove(string id)
        {
            async Task<ActionResult<T>> command()
            {
                var exist = await Repo.Exist(id);

                if (!exist) return NotFound();

                _ = await Repo.DestroyById(id);
                return NoContent();
            }

            if (HttpContext.User.IsInRole("User"))
                return await command();
            return await command();
        }

        /// <summary>
        ///     Check if id exist
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Bool</returns>
        [HttpGet("{id}/exist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<ActionResult> Exist(string id)
        {
            var exist = await Repo.Exist(id);
            return exist ? (ActionResult) Ok() : NotFound();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}