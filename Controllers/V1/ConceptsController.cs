using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using registry.PostgreSQL;
using Microsoft.AspNetCore.Cors;
using System;

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

        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> GetConcept(int id)
        {
            var item = await Context.concepts
               .AsNoTracking()
               .SingleOrDefaultAsync(cnpt => cnpt.id == id);

            if (item == null)
            {
                return NotFound(id);
            }

            item.examples = await GetExamplesByConceptId(item.id);
            return Ok(item);
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

            var item = await Context.concepts
                .AsNoTracking()
                .SingleOrDefaultAsync(cntp => cntp.name == concept.name && cntp.languageId == concept.languageId);

            if (item == null)
            {
                return NotFound(concept);
            }

            return Ok(item);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateConcept(Concepts concept)
        {
            var item = await Context.concepts.SingleOrDefaultAsync(cntp => cntp.id == concept.id);
            if (item == null)
            {
                return NotFound(item);
            }

            item.name = concept.name;
            item.category = concept.category;
            item.method = concept.method;
            item.languageId = concept.languageId;

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
