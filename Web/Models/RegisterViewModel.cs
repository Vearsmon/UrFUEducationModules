namespace UrFUEducationalModules.Models;

// Модель для регистрации. Нужна, чтобы маппить данные форм в объекты
public class RegisterViewModel
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
}