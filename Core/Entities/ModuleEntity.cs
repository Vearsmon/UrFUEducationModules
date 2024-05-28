using System.ComponentModel.DataAnnotations;

namespace Core.Objects;

public class ModuleEntity
{
    [Required]
    public Guid Uuid { get; set; }

    [Required]
    [Display(Name = "Название модуля")]
    public string Title { get; set; }
    
    [Required]
    [Display(Name = "Тип модуля")]
    public string Type { get; set; }
}