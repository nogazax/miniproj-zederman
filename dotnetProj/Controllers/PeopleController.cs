
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetProj.Models;
using AutoMapper;
using dotnetProj.TaskControllerHelper;
using dotnetProj.PeopleControllerHelper;

namespace dotnetProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly MyDatabaseContext _context;

        private readonly IMapper _mapper;

        public PeopleController(MyDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // POST: api/people
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Person>> PostPerson(NoIdPerson person) 
        {
            var WithIdPerson = _mapper.Map<Person>(person);
            WithIdPerson.Id = Guid.NewGuid().ToString();
            _context.People.Add(WithIdPerson);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PeopleValidator.PersonExists(WithIdPerson.Id, _context))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest("Invalid fields");
                }
            }
            HttpContext.Response.Headers.Add("Location", $"https://localhost:9000/api/people/{WithIdPerson.Id}");
            HttpContext.Response.Headers.Add("x-Created-Id", WithIdPerson.Id);
            return StatusCode(201);
        }

        // GET: api/people
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoTasksPerson>>> GetPeople()
        {

            var people = await _context.People.ToListAsync();
            return Ok(people.Select(person => _mapper.Map<NoTasksPerson>(person)));
        }

        // GET: api/people/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoTasksPerson))]


        public async Task<ActionResult<NoTasksPerson>> GetPerson(string id)
        {
            var person = _mapper.Map<NoTasksPerson>(await _context.People.FindAsync(id));

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // GET: api/People/{id}/tasks/ 
        [HttpGet("{id}/tasks")] //TODO check that works
        public ActionResult<List<Models.Task>> GetPersonTasks(string id, string? status)
        {
            var tasks = _context.Tasks.Where(t => t.OwnerId == id).ToList();

            if (tasks == null)
            {
                return NotFound();
            }

            return tasks;
        }

        // POST: api/people/{id}/tasks
        [HttpPost("{id}/tasks")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Models.Task>> AddChore(string id, [FromBody] NoIdOwnerIdTask task) //check why returned 201 is uncodumented
        {
            var fullTask = _mapper.Map<Models.Task>(task);
            fullTask.Id = Guid.NewGuid().ToString();
            fullTask.OwnerId = id;
            if (!TaskValidator.IsValidTask(fullTask))
			{
                return BadRequest("Task has missing fields");
			}
            _context.Tasks.Add(fullTask);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (!PeopleValidator.PersonExists(id,_context))
                {
                    return NotFound($"Person with Id {id} does not exist in the DB");
                }
                else
                {
                    return BadRequest("data contains illegal values");
                }
            }
            HttpContext.Response.Headers.Add("Location", $"https://localhost:9000/api/tasks/{fullTask.Id}");
            HttpContext.Response.Headers.Add("x-Created-Id", fullTask.Id);
            return StatusCode(201, "Task created and assigned successfully");
        }

		



		


        // PATCH: api/people/{id}
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<NoTasksPerson>> PatchPerson(string id, [FromBody] NoIdPerson noIdPerson)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(noIdPerson.Email)){
                person.Email = noIdPerson.Email;
			}
            if (!string.IsNullOrEmpty(noIdPerson.Name))
			{
                person.Name = noIdPerson.Name;
			}

            if (!string.IsNullOrEmpty(noIdPerson.FavoriteProgrammingLanguage))
			{
                person.FavoriteProgrammingLanguage = noIdPerson.FavoriteProgrammingLanguage;
			}
            await _context.SaveChangesAsync();



            return Ok(_mapper.Map<NoTasksPerson>(person));
        }


        // DELETE: api/people/{id} 
        [HttpDelete("{id}")] //TODO check if works
        public async Task<IActionResult> DeletePerson(string id)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            var tasksForId = await _context.Tasks.ToListAsync();

            foreach (var task in tasksForId)
			{
                if (task.OwnerId == id)
				{
                    _context.Tasks.Remove(task);
                }
			}

            _context.People.Remove(person);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}
