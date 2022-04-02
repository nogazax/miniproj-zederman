
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetProj.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using System.Runtime.CompilerServices;

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

        public async Task<ActionResult<Models.Task>> AddChore([FromBody] Models.Task noIdPerson) //check why returned 201 is uncodumented
        {
            var person = new Person();
            var WithId = _mapper.Map<Person>(person);
            WithId.Id = Guid.NewGuid().ToString();
            _context.People.Add(WithId);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PersonExists(WithId.Id))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest("shshshs");
                }
            }
            HttpContext.Response.Headers.Add("Location", $"https://localhost:9000/api/people/{WithId.Id}");
            HttpContext.Response.Headers.Add("x-Created-Id", WithId.Id);
            return StatusCode(201);
        }

    

        // POST: api/people
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Person>> PostPerson(NoIdPerson person) //check why returned 201 is uncodumented
        {
            var WithId = _mapper.Map<Person>(person);
            WithId.Id = Guid.NewGuid().ToString();
            _context.People.Add(WithId);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
				if (PersonExists(WithId.Id))
				{
					return Conflict();
				}
				else
				{
                    return BadRequest("shshshs");
                }
            }
            HttpContext.Response.Headers.Add("Location",$"https://localhost:9000/api/people/{WithId.Id}");
            HttpContext.Response.Headers.Add("x-Created-Id", WithId.Id);
            return StatusCode(201);
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

        private bool PersonExists(string id)
        {
            return _context.People.Any(e => e.Id == id);
        }
    }
}
