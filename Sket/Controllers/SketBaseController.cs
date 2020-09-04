using Bracketcore.Sket.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Bracketcore.Sket.Entity;


//Todo add a rate limiter
namespace Bracketcore.Sket.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Controller model</typeparam>
    /// <typeparam name="TC">Controller model</typeparam>
    [ApiController]
    [Route("api/[Controller]")]
    //[Authorize]
    public abstract class SketBaseController<T, TC> : ControllerBase
        where T : SketPersistedModel
        where TC : SketBaseRepository<T>
    {
        protected TC Repo { get; set; }
        // public virtual H Hub { get; set; }

        public SketBaseController(TC repo)
        {
            Repo = repo;
            // Hub = hub;
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
        public virtual async Task<IActionResult> Create([FromBody] T doc)
        {
            if (doc == null) return BadRequest();

            var cre = await Repo.Create(doc);
            if (cre == null) return BadRequest();
            return Created(typeof(T).Name + " Created", JsonConvert.SerializeObject(cre));
        }

        [Authorize(Roles = "User,Admin,Support")]
        [HttpPut("{id}")]
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
    }
}