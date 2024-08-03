using System.ComponentModel.DataAnnotations;

namespace ProgressTrackerSubjectService.Models;

public class UserSubjectsModel
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    public int SubjectIds { get; set; }
    
    
    public UserSubjectsModel(int userId, int subjectIds)
    {
        UserId = userId;
        SubjectIds = subjectIds;
    }
}