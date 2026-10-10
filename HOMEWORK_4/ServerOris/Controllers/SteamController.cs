using ServerOris.Framework.Attributes;
using System.Net.Mail.MailAddress;
using System.Net.Mail.MailMessage;
using System.Net.Mail.SmptClient;
namespace ServerOris.Controllers;

public class SteamController
{
    //GET http://localhost:2323/steam/send?to=test@test.ru@hero=
    [Get("send")]
    public void Send([FromQuery] string to, string hero)
    {
        // отправитель - устанавливаем адрес и отображаемое в письме имя
        MailAddress from = new MailAddress("somemail@gmail.com", "Tom");
        // кому отправляем
        MailAddress to = new MailAddress("somemail@yandex.ru");
        // создаем объект сообщения
        MailMessage m = new MailMessage(from, to);
        // тема письма
        m.Subject = "Тест";
        // текст письма
        m.Body = "<h2>Письмо-тест работы smtp-клиента</h2>";
        // письмо представляет код html
        m.IsBodyHtml = true;
        // адрес smtp-сервера и порт, с которого будем отправлять письмо
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        // логин и пароль
        smtp.Credentials = new NetworkCredential("somemail@gmail.com", "mypassword");
        smtp.EnableSsl = true;
        smtp.Send(m);
        Console.Read();
    }
    
}