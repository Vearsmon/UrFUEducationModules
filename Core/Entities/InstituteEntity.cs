using System.ComponentModel.DataAnnotations;

namespace Core.Objects;

public class InstituteEntity
{
    [Required]
    public Guid Uuid { get; set; }

    [Required]
    [Display(Name = "Название института")]
    public string Title { get; set; }
}