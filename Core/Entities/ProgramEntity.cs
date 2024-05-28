using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Objects;

public class ProgramEntity
{
    [Required]
    public Guid Uuid { get; set; }

    [Required]
    [Display(Name = "Название программы")]
    public string Title { get; set; }
    
    [Required]
    [Display(Name = "Статус программы")]
    public string Status { get; set; }
    
    [Required]
    [Display(Name = "Шифр программы")]
    public string Cypher { get; set; }
    
    [Required]
    [Display(Name = "Уровень обучения")]
    public string Level { get; set; }
    
    [Required]
    [Display(Name = "Стандарт обучения")]
    public string Standard { get; set; }
    
    [Required]
    [Display(Name = "Институт")]
    public Guid Institute { get; set; }
    
    [Required]
    [Display(Name = "Ответственное лицо")]
    public Guid Head { get; set; }
    
    [Required]
    [Display(Name = "Дата следующей аккредитации")]
    public DateTime AccreditationTime { get; set; }
}

public static class Level
{
    public const string Bachelor = "Бакалавриат";
    public const string AppliedBachelor = "Прикладной бакалавриат";
    public const string Specialist = "Специалитет";
    public const string Master = "Магистратура";
    public const string Graduate = "Аспирантура";
}

public static class Standard
{
    public const string SUOS = "СУОС";
    public const string FGOSVO = "ФГОС ВО";
    public const string SUT = "СУТ";
    public const string FGOSVPO = "ФГОС ВПО";
    public const string FGOS3 = "ФГОС 3++";
}