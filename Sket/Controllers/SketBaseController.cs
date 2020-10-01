using System;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;
using Bracketcore.Sket.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


//Todo add a rate limiter
namespace Bracketcore.Sket.Controllers
{
    /// <summary>
    ///     Abstract Base Controller
    /// </summary>
    /// <typeparam name="T">Controller model</typeparam>
    /// <typeparam name="TC">Repository class to use</typeparam>
    [Produces("application/json")]
    [ApiController]
    [Route("api/[Controller]")]
    [Authorize]
    public abstract class SketBaseController<T, TC> : ControllerBase, ISketBaseController<T>
        where T : SketPersistedModel
        where TC : ISketBaseRepository<T>
    {
        // public virtual H Hub { get; set; }

        protected SketBaseController(TC repo)
        {
            Repo = repo;
            // Hub = hub;
        }

        protected TC Repo { get; set; }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> GetAll()
        {
            return Ok(await Repo.FindAll().ConfigureAwait(false));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> GetById(string id)
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
        public virtual async Task<IActionResult> Create(T doc)
        {
            try
            {
                // var change = JsonConvert.DeserializeObject<T>(doc);

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

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual async Task<IActionResult> Update(string id, T replace)
        {
            async Task<IActionResult> command()
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
        public virtual async Task<IActionResult> Remove(string id)
        {
            async Task<IActionResult> command()
            {
                var exist = await Repo.Exist(id).ConfigureAwait(false);

                if (!exist) return NotFound();

                _ = await Repo.DestroyById(id).ConfigureAwait(false);
                return NoContent();
            }

            if (HttpContext.User.IsInRole("User"))
                return await command();
            return await command();
        }

        [HttpGet("exist/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public virtual IActionResult Exist(string id)
        {
            return Ok(Repo.Exist(id));
        }
    }
}