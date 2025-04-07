using System.ComponentModel.DataAnnotations;

namespace CollegeSystemApi.Models.Common;

public abstract class BaseEntity:IEntity
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? CreatedBy { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public string UpdatedBy { get; set; } = string.Empty;
      
}