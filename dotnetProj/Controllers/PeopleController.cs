
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetProj.Models;
using AutoMapper;
using dotnetProj.TaskControllerHelper;
using dotnetProj.PeopleControllerHelper;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Cors;

namespace dotnetProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {

        private readonly SqlContext _context;

        private readonly IMapper _mapper;

        public PeopleController(SqlContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // POST: api/people
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Nullable))]
        public async Task<IActionResult> PostPerson(NoIdPerson person) 
        {
            var WithIdPerson = _mapper.Map<Person>(person);
            WithIdPerson.Id = Guid.NewGuid().ToString();
            _context.People.Add(WithIdPerson);
            try
            {
                checkEmail(person.Emails);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PeopleValidator.PersonExists(WithIdPerson.Id, _context))
                {
                    return StatusCode(400,"Conflict with id of two persons");
                }
                else
                {
                    return StatusCode(400,"Required data fields are missing");

                }
            }

            catch (ArgumentException)
			{
                return StatusCode(400,"Invalid Email address");
            }
            HttpContext.Response.Headers.Add("Location", $"https://localhost:9000/api/people/{WithIdPerson.Id}");
            HttpContext.Response.Headers.Add("x-Created-Id", WithIdPerson.Id);
            return StatusCode(201, "Person created successfully");
        }

		private void checkEmail(string? email)
		{
            if (email == null || !(new EmailAddressAttribute().IsValid(email)))
			{
				Console.WriteLine("invalid email");
                throw new ArgumentException("invalid Email format");
			}
        }

        // GET: api/people
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NoTasksPerson>>> GetPeople()
        {

            var people = (await _context.People.ToListAsync()).Select(person => _mapper.Map<NoTasksPerson>(person)).ToList();
            var tasks = await _context.Tasks.ToListAsync();
            foreach (var person in people)
			{
                person.ActiveTaskCount =  tasks.FindAll(task => task.OwnerId.Equals(person.Id)).Count();
            }
            return Ok(people);
        }

        // GET: api/people/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NoTasksPerson))]
        public async Task<ActionResult<NoTasksPerson>> GetPerson(string id)
        {
            var person = _mapper.Map<NoTasksPerson>(await _context.People.FindAsync(id));

            if (person == null)
            {
                return NotFound();
            }
            var tasks = await _context.Tasks.ToListAsync();
            person.ActiveTaskCount = tasks.FindAll(task => task.OwnerId.Equals(person.Id)).Count();
            return Ok(person);
        }

        // GET: api/People/{id}/tasks/ 
        [HttpGet("{id}/tasks")] //TODO check that works
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ITask))]
        public ActionResult<List<ITask>> GetPersonTasks(string id, taskStatus? status)
        {
            var tasks = _context.Tasks.Where(t => t.OwnerId == id).ToList();

            if (tasks.Count == 0)
            {
                return NotFound($"Did not find any tasks for id: {id}");
            }
            
            if (status != null)
			{
                tasks = tasks.Where(t => string.Equals(t.Status, status.ToString(),StringComparison.OrdinalIgnoreCase)).ToList();
                if (tasks.Count == 0)
                {
                    return NotFound($"Did not find any tasks for id: {id}, with status: {status}");
                }
            }

            var ItaskList = GenerateItasks(tasks);
            return ItaskList;
        }

		private List<ITask> GenerateItasks(List<Models.Task> tasks)
		{
			var taskList = new List<ITask>();
            foreach (var task in tasks)
			{
                if (task.Type.Equals("homework", StringComparison.OrdinalIgnoreCase))
				{
                    taskList.Add(_mapper.Map<HomeWork>(task));
				}
				else
				{
                    taskList.Add(_mapper.Map<Chore>(task));

                }
            }
            return taskList;
		}

		public enum taskStatus
		{
            Active,
            Done
		}
        // POST: api/people/{id}/tasks
        [HttpPost("{id}/tasks")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<Models.Task>> AddChore(string id, [FromBody] NoIdOwnerIdTask task)
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
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<NoTasksPerson>> PatchPerson(string id, [FromBody] NoIdPerson noIdPerson)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound($"Requested person is not present, with id {id}.");
            }

            if (!string.IsNullOrEmpty(noIdPerson.Emails)){
                checkEmail(noIdPerson.Emails);
                person.Emails = noIdPerson.Emails;
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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> DeletePerson(string id)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound($"Requested person is not present, with id {id}.");
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

            return Ok($"removed person with id {id}.");
        }

    }
}
