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

        [HttpGet("view/{id}")]
        // [Authorize]
        public async Task<IActionResult> ViewSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
            {
                _logger.LogWarning($"Subject with ID {id} not found.");
                return NotFound();
            }
            return Ok(new { subject.Id, subject.Name, subject.Credits, subject.LearningHours });
        }

        [HttpPost("create")]
        // [Authorize]
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

        [HttpGet("edit/{id}")]
        // [Authorize]
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

        [HttpPut("edit")]
        // [Authorize]
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
