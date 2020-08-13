using System;
using System.Threading.Tasks;
using Bracketcore.KetAPI.Model;
using Bracketcore.KetAPI.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


//Todo add a rate limiter
namespace Bracketcore.KetAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Controller model</typeparam>
    /// <typeparam name="TC">Controller model</typeparam>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class BaseController<T, TC> : ControllerBase
        where T : PersistedModel 
        where TC : BaseRepository<T>
    {
        public TC Repo { get; set; }
        // public virtual H Hub { get; set; }

        public BaseController(TC repo)
        {
            Repo = repo;
            // Hub = hub;
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost]
        public virtual async Task<IActionResult> Create([FromBody] T doc)
        {
            if (doc == null) return BadRequest();

            var cre = await Repo.Create(doc);
            if (cre == null) return BadRequest();
            return Created(typeof(T).Name + " Created", JsonConvert.SerializeObject(cre));
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(string id, T replace)
        {
            var exist = await Repo.Exist(id).ConfigureAwait(false);

            if (exist)
            {
                var up = await Repo.Update(id, replace).ConfigureAwait(false);
                return Ok(up);
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Remove(string id)
        {
            var exist = await Repo.Exist(id).ConfigureAwait(false);

            if (!exist) return NotFound();
            var del = await Repo.DestroyById(id).ConfigureAwait(false);
            return Ok(del);
        }

        [HttpGet("exist/{id}")]
        public virtual IActionResult Exist(string id)
        {
            return Ok(Repo.Exist(id));
        }
    }
}