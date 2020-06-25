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
    public class ExamplesController : Controller
    {
        readonly OperationContext Context;
        readonly ILogger<ExamplesController> _logger;

        public ExamplesController(OperationContext OperationContext, ILogger<ExamplesController> logger)
        {
            Context = OperationContext;
            _logger = logger;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult> GetExamples()
        {
            var list = await Context.examples.ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult> CreateExample(Examples example)
        {
            Context.examples.Add(example);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateExample(Examples example)
        {
            var item = await Context.examples.SingleOrDefaultAsync(empl => empl.id == example.id);
            if (item == null)
            {
                return NotFound(item);
            }

            item.example = example.example;
            item.notes = example.notes;
            item.conceptId = example.conceptId;

            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteExample(int id)
        {
            var item = await Context.examples.FindAsync(id);
            if (item == null)
            {
                return NotFound(id);
            }

            Context.examples.Remove(item);
            await Context.SaveChangesAsync();

            return Ok(item);
        }
    }
}
