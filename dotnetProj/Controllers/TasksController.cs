using Microsoft.AspNetCore.Mvc;
using dotnetProj.Models;
using AutoMapper;
using dotnetProj.TaskControllerHelper;
using dotnetProj.PeopleControllerHelper;

namespace dotnetProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly MyDatabaseContext _context;

        private readonly IMapper _mapper;

        public TasksController(MyDatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ITask>> GetTask(string id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound();
            }
            if (task.Type.Equals("Chore", StringComparison.OrdinalIgnoreCase))
			{
                return _mapper.Map<Chore>(task);
			}
            return _mapper.Map<HomeWork>(task);
        }

         [HttpGet("{id}/owner")]
        public async Task<ActionResult<string>> GetOwnerId(string id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound($"A task with the id {id} does not exist.");
            }
            return Ok(task.OwnerId); //TODO verify if we need to return JSON or just string
        }

        // GET: api/Tasks/5/status
        [HttpGet("{id}/status")]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<string>> GetTaskStatus(string id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound($"A task with the id {id} does not exist.");
            }
            return Ok(task.Status); //TODO verify if we need to return JSON or just string
        }

        // PATCH: api/tasks/{id}
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<ActionResult<ITask>> PatchTask(string id, [FromBody] NoIdTask PatchContent)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound($"A task with the id {id} does not exist.");
            }

            // owner

            if (PatchContent.OwnerId != null) //check if new owner is valid
			{
                if (await checkValidOwner(PatchContent.OwnerId))
				{
                    task.OwnerId = PatchContent.OwnerId;
                }
				else
				{
                    return NotFound($"A person with the id {PatchContent.OwnerId} does not exist");
				}
            }

            task.Status = PatchContent.Status ?? task.Status;

            // update fields

            if (PatchContent.Type == null || PatchContent.Type.Equals(task.Type, StringComparison.OrdinalIgnoreCase)) //patching a task from the same type
			{

                if (task.Type.Equals("Chore", StringComparison.OrdinalIgnoreCase)) //swapping chore fields
				{
                    task.Description = PatchContent.Description ?? task.Description;
                    task.Size = PatchContent.Size ?? task.Size;
                }
                else //swapping HW fields
				{
                    task.Course = PatchContent.Course ?? task.Course;
                    task.DueDate = PatchContent.DueDate != null ? PatchContent.DueDate.ToString().Split(' ')[0] : task.DueDate;
                    task.Details = PatchContent.Details ?? task.Details;
				}
            }


            else //changing type of task
            {
                swapChoreHW(task,PatchContent);
                if (TaskValidator.IsValidTask(task))
				{
					try
					{
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception)
					{
                        return BadRequest("invalid fields");
					}
                }
                return BadRequest("missing fields");
            }

            return task.Type.Equals("Chore", StringComparison.OrdinalIgnoreCase) ? _mapper.Map<Chore>(task): _mapper.Map<HomeWork>(task);
        }

        private void swapChoreHW(Models.Task originalTask, NoIdTask newTask)
		{
			if (newTask.Type.Equals("chore", StringComparison.OrdinalIgnoreCase))
            { //chore to HW
                originalTask.Course = null;
                originalTask.DueDate = null;
                originalTask.Details = null;
                originalTask.Description = newTask.Description;
                originalTask.Size = newTask.Size;
			}
			else
			{
                originalTask.Course = newTask.Course;
                originalTask.DueDate = newTask.DueDate.ToString().Split(' ')[0];
                originalTask.Details = newTask.Details;
                originalTask.Description = null;
                originalTask.Size = null;
            }
		}

		private async Task<bool> checkValidOwner(string ownerId)
		{
            var newOwner = await _context.People.FindAsync(ownerId);
            return newOwner != null;

        }

        // PUT: api/Tasks/5
        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> ChangeTaskStatus(string id, [FromBody] string status)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound($"A task with the id {id} does not exist.");
            }
            task.Status = status;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest("invalid status");
            }
            return NoContent();
        }

        [HttpPut("{id}/owner")]
        [ProducesResponseType(typeof(string),StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public async Task<ActionResult<string>> ChangeTaskOwner(string id, [FromBody] string newOwnerId)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound($"A task with the id {id} does not exist.");
            }
            
            if (!PeopleValidator.PersonExists(newOwnerId, _context))
			{
                return NotFound($"Person with Id {id} does not exist in the DB");
			}

            task.OwnerId = newOwnerId;
            task.Owner = await _context.People.FindAsync(newOwnerId);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest("DB threw an unexpected error");
            }
            return NoContent();
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NotFound($"A task with the id {id} does not exist.");
        }
    }
}
