using System.ComponentModel.DataAnnotations;

namespace Core.Objects;

public class HeadEntity
{
    [Required]
    public Guid Uuid { get; set; }

    [Required]
    [Display(Name = "Полное имя ответственного лица")]
    public string Fullname { get; set; }
}