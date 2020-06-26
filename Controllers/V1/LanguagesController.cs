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
    public class LanguagesController : Controller
    {
        readonly OperationContext Context;
        readonly ILogger<LanguagesController> _logger;

        public LanguagesController(OperationContext OperationContext, ILogger<LanguagesController> logger)
        {
            Context = OperationContext;
            _logger = logger;
        }

        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult> GetLanguages()
        {
            var list = await Context.languages.ToListAsync();

            foreach (var item in list)
            {
                item.concepts = await GetConceptsByLanguageId(item.id);
            }

            return Ok(list);
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult> CreateConcept(Languages language)
        {
            Context.languages.Add(language);
            await Context.SaveChangesAsync();

            var item = await Context.languages
                .AsNoTracking()
                .SingleOrDefaultAsync(lang => lang.name == language.name);

            if (item == null)
            {
                return NotFound(language);
            }

            return Ok(item);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateLanguage(Languages language)
        {
            var lang = await Context.languages.SingleOrDefaultAsync(lang => lang.id == language.id);
            if (lang == null)
            {
                return NotFound(language);
            }

            lang.name = language.name;
            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLanguage(int id)
        {
            var item = await Context.languages.FindAsync(id);
            if (item == null)
            {
                return NotFound(id);
            }

            Context.languages.Remove(item);
            await Context.SaveChangesAsync();

            return Ok(item);
        }

        private async Task<Concepts[]> GetConceptsByLanguageId(int id)
        {
            var concepts = await Context.concepts
               .AsNoTracking()
               .Where(con => con.languageId == id)
               .ToArrayAsync();

            foreach (var item in concepts)
            {
                item.examples = await GetExamplesByConceptId(item.id);
            }

            return concepts;
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
