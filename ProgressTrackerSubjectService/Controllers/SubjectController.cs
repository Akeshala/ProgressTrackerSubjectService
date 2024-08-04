using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProgressTrackerSubjectService.Data;
using ProgressTrackerSubjectService.Models;
using ProgressTrackerSubjectService.ViewModels;

namespace ProgressTrackerSubjectService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(AppDbContext context, ILogger<SubjectController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("view/{userid}/{id}")]
        [Authorize]
        public async Task<IActionResult> ViewSubject(int userid, int id)
        {
            var userSubjects = await _context.UserSubjects.FindAsync(userid);
            if (userSubjects != null && !userSubjects.SubjectIds.Contains(id))
            {
                _logger.LogWarning($"Subject with ID {id} not found for {userid}.");
                return NotFound();
            }
            
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                _logger.LogWarning($"Subject with ID {id} not found.");
                return NotFound();
            }
            return Ok(new { subject.Id, subject.Name, subject.Credits, subject.LearningHours });
        }
        
        [HttpGet("all/{userid}")]
        [Authorize]
        public async Task<IActionResult> GetAll(int userid)
        {
            var userSubjects = await _context.UserSubjects.FindAsync(userid);
            if (userSubjects == null)
            {
                _logger.LogWarning($"No Subject found for User with ID {userid}.");
                return NotFound();
            }

            var subjectIds = userSubjects.SubjectIds;
            var subjects = _context.Subjects.Where(e => subjectIds.Contains(e.Id)).ToList();
            if (subjects.Count == 0)
            {
                _logger.LogWarning($"No Subject found for User with ID {userid}.");
                return NotFound();
            }
            
            return Ok(subjects);
        }

        // use this for internal operations
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] SubjectCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for Register.");
                return BadRequest(ModelState);
            }

            if (_context.Subjects.Any(u => u.Name == model.Name))
            {
                _logger.LogWarning($"Subject already exists: {model.Name}");
                return Conflict("Subject already exists.");
            }

            var subject = new SubjectModel(model.Name, model.Credits, model.LearningHours);

            await _context.Subjects.AddAsync(subject);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Subject added: {subject.Name}");
            return CreatedAtAction(nameof(ViewSubject), new { id = subject.Id }, subject);
        }
        
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] SubjectAddModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for Register.");
                return BadRequest(ModelState);
            }

            var userSubjects = await _context.UserSubjects.FindAsync(model.UserId);
            if (userSubjects == null)
            {
                userSubjects = new UserSubjectsModel(model.UserId, [model.SubjectId]);
                await _context.UserSubjects.AddAsync(userSubjects);
            }
            else
            {
                var userSubjectIds = userSubjects.SubjectIds;
                if (!userSubjectIds.Contains(model.SubjectId))
                {
                    userSubjectIds.Add(model.SubjectId);
                    userSubjects.SubjectIds = userSubjectIds;
                    _context.Update(userSubjects);
                }
                else
                {
                    _logger.LogInformation($"Subject with the ID: {model.SubjectId} already exists.");
                }
            }
            
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Subject added: {model.SubjectId}");
            return Ok(userSubjects);
        }
        
        [HttpDelete("remove/{id}/{userid}")]
        [Authorize]
        public async Task<IActionResult> Remove(int id, int userid)
        {
            var userSubjects = await _context.UserSubjects.FindAsync(userid);
            if (userSubjects == null)
            {
                _logger.LogWarning($"No subjects found for {userid}.");
                return NotFound();
            }
            
            if (!userSubjects.SubjectIds.Contains(id))
            {
                _logger.LogWarning($"Subject with ID {id} not found for User with ID {userid}.");
                return NotFound();
            }
            
            var userSubjectIds = userSubjects.SubjectIds;
            userSubjectIds.Remove(id);
            userSubjects.SubjectIds = userSubjectIds;
            _context.Update(userSubjects);
            
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Subject removed: {id}");
            return Ok(userSubjects);
        }

        // use this for internal operations
        [HttpGet("edit/{id}")]
        [Authorize]
        public async Task<IActionResult> GetEditUser(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                _logger.LogWarning($"Subject with ID {id} not found.");
                return NotFound();
            }

            var subjectEditModel = new SubjectEditModel
            {
                Id = subject.Id,
                Name = subject.Name,
                Credits = subject.Credits,
                LearningHours = subject.LearningHours,
            };

            return Ok(subjectEditModel);
        }

        // use this for internal operations
        [HttpPut("edit")]
        [Authorize]
        public async Task<IActionResult> EditSubject([FromBody] SubjectEditModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for EditSubject.");
                return BadRequest(ModelState);
            }

            var existingSubject = await _context.Subjects.FindAsync(model.Id);
            if (existingSubject == null)
            {
                _logger.LogWarning($"Subject with ID {model.Id} not found.");
                return NotFound();
            }

            existingSubject.Name = model.Name;
            existingSubject.Credits = model.Credits;
            existingSubject.LearningHours = model.LearningHours;

            _context.Update(existingSubject);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Subject updated: {existingSubject.Name}");
            return Ok(existingSubject);
        }
    }
}
