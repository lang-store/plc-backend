using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using registry.PostgreSQL;
using Microsoft.AspNetCore.Cors;

namespace registry.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [EnableCors("OpenPolicy")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ConceptsController : Controller
    {
        readonly OperationContext Context;
        readonly ILogger<ConceptsController> _logger;

        public ConceptsController(OperationContext OperationContext, ILogger<ConceptsController> logger)
        {
            Context = OperationContext;
            _logger = logger;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult> GetConcepts()
        {
            var list = await Context.concepts.ToListAsync();

            foreach (var item in list)
            {
                item.examples = await GetExamplesByConceptId(item.id);
            }

            return Ok(list);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult> CreateConcept(Concepts concept)
        {
            Context.concepts.Add(concept);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateConcept(Concepts concept)
        {
            var item = await Context.concepts.SingleOrDefaultAsync(concept => concept.id == concept.id);
            if (item == null)
            {
                return NotFound(item);
            }

            item.name = concept.name;
            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteConcept(int id)
        {
            var item = await Context.concepts.FindAsync(id);
            if (item == null)
            {
                return NotFound(id);
            }

            Context.concepts.Remove(item);
            await Context.SaveChangesAsync();

            return Ok(item);
        }

        private async Task<Examples[]> GetExamplesByConceptId(int id)
        {
            var examples = await Context.examples
               .AsNoTracking()
               .Where(expl => expl.conceptId == id)
               .ToArrayAsync();
            return examples;
        }
    }
}
