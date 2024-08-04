using System.ComponentModel.DataAnnotations;

namespace ProgressTrackerSubjectService.ViewModels;

public class SubjectAddModel
{
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public int SubjectId { get; set; }
}