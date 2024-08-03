using System.ComponentModel.DataAnnotations;

namespace ProgressTrackerSubjectService.Models;

public class SubjectModel
{
    [Key]
    public int Id { get; set; }
    
    [StringLength(60, MinimumLength = 3)]
    [Required]
    public string Name { get; set; }

    [Range(1, 10)] 
    [Required]
    public int Credits { get; set; }
    
    [Range(1, 150)]
    [Required]
    public int LearningHours { get; set; }
    
    public SubjectModel(string name, int credits, int learningHours)
    {
        Name = name;
        Credits = credits;
        LearningHours = learningHours;
    }
}