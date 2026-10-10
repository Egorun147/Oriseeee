using ServerOris.Framework.Attributes;
namespace ServerOris.Controllers;

[HttpController("auth")]
public class AuthController
{
    [Get("login")]
    public string Login()
    {
        string filePath = Path.Combine(
            AppContext.BaseDirectory, "static", "login.html");

        return File.ReadAllText(filePath);
    }
    [Post("login")]
    public void Login(string login, string password)
    {
        Console.WriteLine("login: " + login);
        Console.WriteLine("password: " + password);
    }
}