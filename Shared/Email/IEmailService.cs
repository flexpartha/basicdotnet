namespace UserApi.Shared.Email;

public interface IEmailService
{
    Task SendLoginNotificationAsync(string name, string username, string email);
}
