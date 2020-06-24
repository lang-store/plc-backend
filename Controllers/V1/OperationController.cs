using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using registry.PostgreSQL;
using System;

namespace registry.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    public class OperationController : Controller
    {
        readonly OperationContext Context;
        readonly ILogger<OperationController> _logger;

        public OperationController(OperationContext OperationContext, ILogger<OperationController> logger)
        {
            Context = OperationContext;
            _logger = logger;
        }

        [HttpGet("languages")]
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

        [HttpPost("languages")]
        [Produces("application/json")]
        public async Task<ActionResult> CreateConcept(Languages language)
        {
            Context.languages.Add(language);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("languages")]
        public async Task<IActionResult> UpdateOperation(Languages language)
        {
            var lang = await Context.languages.SingleOrDefaultAsync(lang => lang.id == language.id);
            if (lang == null)
            {
                return NotFound(language);
            }

            var item = await Context.languages.FindAsync(language.id);
            if (item == null)
            {
                return NotFound(language.id);
            }

            item.name = language.name;
            await Context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("concepts")]
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

        [HttpPost("examples")]
        [Produces("application/json")]
        public async Task<ActionResult> CreateConcept(Concepts concept)
        {
            Context.concepts.Add(concept);
            await Context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("examples")]
        [Produces("application/json")]
        public async Task<ActionResult> GetExamples()
        {
            var list = await Context.examples.ToListAsync();
            return Ok(list);
        }

        [HttpPost("examples")]
        [Produces("application/json")]
        public async Task<ActionResult> CreateExample(Examples example)
        {
            Context.examples.Add(example);
            await Context.SaveChangesAsync();
            return Ok();
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

        //     [HttpGet("{system}")]
        //     [Produces("application/json")]
        //     public async Task<ActionResult> GetBySystem(string system)
        //     {
        //         var sys = await Context.systems.SingleOrDefaultAsync(sys => sys.name == system);
        //         if (sys == null)
        //         {
        //             return NotFound(system);
        //         }

        //         var list = await Context.operations
        //             .AsNoTracking()
        //             .Where(op => op.system == sys)
        //             .ToListAsync();

        //         return Ok(ToShowOperation(list));
        //     }

        //     [HttpGet("id/{id}")]
        //     [Produces("application/json")]
        //     public async Task<ActionResult> GetOne(int id)
        //     {
        //         var operation = await Context.operations.FindAsync(id);

        //         if (operation == null)
        //         {
        //             return NotFound();
        //         }

        //         return Ok(new OperationToShow(operation));
        //     }

        //     [HttpGet("{system}/{classId}/{shortName}/")]
        //     [Produces("application/json")]
        //     public async Task<ActionResult> GetByParameters(string system, string classId, string shortName)
        //     {
        //         var sys = await Context.systems.SingleOrDefaultAsync(sys => sys.name == system);
        //         if (sys == null)
        //         {
        //             return NotFound(system);
        //         }

        //         var operation = await Context.operations
        //             .AsNoTracking()
        //             .SingleOrDefaultAsync(op => op.system == sys
        //                 && (op.classId == classId || op.childs != null && op.childs.Contains(classId))
        //                 && (op.shortName == shortName || op.parentShortName == shortName));

        //         if (operation == null)
        //         {
        //             return NotFound();
        //         }

        //         return Ok(new OperationToShow(operation));
        //     }

        //     [HttpPost("{system}/hotkey")]
        //     [Produces("application/json")]
        //     public async Task<ActionResult> Contains(string system, OperationBase[] operations)
        //     {
        //         var sys = await Context.systems.SingleOrDefaultAsync(sys => sys.name == system);
        //         if (sys == null)
        //         {
        //             return NotFound(system);
        //         }

        //         List<OperationHotKey> converted = new List<OperationHotKey>();

        //         foreach (var item in operations)
        //         {
        //             var opDB = await Context.operations
        //                   .AsNoTracking()
        //                   .SingleOrDefaultAsync(op =>
        //                       op.system == sys
        //                       && op.classId == item.classId
        //                       && (op.shortName == item.shortName || op.parentShortName == item.shortName));

        //             if (opDB != null)
        //             {
        //                 converted.Add(new OperationHotKey(opDB));
        //             }
        //         }

        //         return Ok(converted.ToArray());
        //     }

        //     [HttpPost("{system}")]
        //     [Produces("application/json")]
        //     public async Task<ActionResult<OperationToShow>> CreateOperation(string system, OperationToShow operation)
        //     {
        //         var sys = await Context.systems.SingleOrDefaultAsync(sys => sys.name == system);
        //         if (sys == null)
        //         {
        //             return NotFound(system);
        //         }

        //         var currentOperation = await Context.operations
        //             .AsNoTracking()
        //             .SingleOrDefaultAsync(op => op.system == sys && op.classId == operation.classId && op.shortName == operation.shortName);

        //         if (currentOperation != null)
        //         {
        //             return ValidationProblem($"Operation [{operation.classId}][{operation.shortName}] already exist.");
        //         }

        //         Context.operations.Add(new Operation
        //         {
        //             classId = operation.classId,
        //             shortName = operation.shortName,
        //             parentShortName = operation.parentShortName,
        //             source = operation.source,
        //             conversionDate = operation.conversionDate,
        //             modificationDate = operation.modificationDate,
        //             hotKey = operation.hotKey,
        //             childs = operation.childs,
        //             lastAuthor = operation.lastAuthor,
        //             insertDate = DateTime.Now,
        //             system = sys
        //         });
        //         await Context.SaveChangesAsync();
        //         return Ok(operation);
        //     }

        //     [HttpPut("{system}/{id}")]
        //     public async Task<IActionResult> UpdateOperation(string system, int id, OperationToShow operation)
        //     {
        //         var sys = await Context.systems.SingleOrDefaultAsync(sys => sys.name == system);
        //         if (sys == null)
        //         {
        //             return NotFound(system);
        //         }

        //         var item = await Context.operations.FindAsync(id);
        //         if (item == null)
        //         {
        //             return NotFound(id);
        //         }

        //         item.classId = operation.classId;
        //         item.shortName = operation.shortName;
        //         item.parentShortName = operation.parentShortName;
        //         item.modificationDate = operation.modificationDate;
        //         item.conversionDate = operation.conversionDate;
        //         item.insertDate = DateTime.Now;
        //         item.lastAuthor = operation.lastAuthor;
        //         item.hotKey = operation.hotKey;
        //         item.source = operation.source;
        //         item.system = sys;

        //         try
        //         {
        //             await Context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
        //         {
        //             return BadRequest();
        //         }

        //         return Ok();
        //     }

        //     [HttpDelete("{id}")]
        //     public async Task<ActionResult> DeleteOperation(int id)
        //     {
        //         var item = await Context.operations.FindAsync(id);
        //         if (item == null)
        //         {
        //             return NotFound(id);
        //         }

        //         Context.operations.Remove(item);
        //         await Context.SaveChangesAsync();

        //         return Ok(item);
        //     }

        //     private List<OperationToShow> ToShowOperation(List<Operation> list)
        //     {
        //         List<OperationToShow> operationsList = new List<OperationToShow>();
        //         list.ForEach(oper => operationsList.Add(new OperationToShow(oper)));
        //         return operationsList;
        //     }
        //     private bool TodoItemExists(long id) => Context.systems.Any(e => e.id == id);
    }
}
