using System.ComponentModel.DataAnnotations;

namespace ProgressTrackerSubjectService.ViewModels;

public class SubjectEditModel
{
    [Key]
    [Required]
    public int Id { get; set; }
    
    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string Name { get; set; } = null!;

    [Range(1, 10)] 
    [Required]
    public int Credits { get; set; }

    [Display(Name = "Learning Hours")]
    [Range(1, 150)]
    [Required]
    public int LearningHours { get; set; }
}