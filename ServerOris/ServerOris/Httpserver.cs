using System.Net;
using System.Text;

namespace ServerOris;

public class HttpServer
{
    private readonly HttpListener server;
    private string filePath = "hello.html";

    public HttpServer(string urlPrefix)
    {
        server = new HttpListener();
        server.Prefixes.Add(urlPrefix);
    }

    public void Start()
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Ошибка: файл {filePath} не найден!");
            Console.WriteLine("Сервер не может быть запущен.");
            return;
        }

        server.Start();
        Console.WriteLine("Сервер запущен");
    }

    public async Task ListenAsync()
    {
        if (!server.IsListening)
        {
            return;
        }

        while (server.IsListening)
        {
            var context = await server.GetContextAsync();

            var request = context.Request;
            var response = context.Response;

            Console.WriteLine("Пришел запрос: " + request.Url.LocalPath);

            if (request.Url.LocalPath.EndsWith("favicon.ico"))
            {
                response.Close();
                continue;
            }

            string htmlFileText = File.ReadAllText(filePath);

            byte[] buffer = Encoding.UTF8.GetBytes(htmlFileText);

            response.ContentType = "text/html; charset=utf-8";
            response.ContentLength64 = buffer.Length;

            using Stream output = response.OutputStream;

            await output.WriteAsync(buffer);
            await output.FlushAsync();

            Console.WriteLine("Запрос обработан!");
        }
    }

    public void Stop()
    {
        server.Stop();
        Console.WriteLine("Сервер всё...");
    }
}