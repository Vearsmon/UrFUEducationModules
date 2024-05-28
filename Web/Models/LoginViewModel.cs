namespace UrFUEducationalModules.Models;

// Модель для входа. Нужна, чтобы маппить данные форм в объекты
public class LoginViewModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
}