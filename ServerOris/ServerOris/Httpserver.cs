using System.Net;
using System.Text;

namespace ServerOris;

public class HttpServer
{
    private readonly HttpListener server;

    public HttpServer(string urlPrefix)
    {
        server = new HttpListener();
        server.Prefixes.Add(urlPrefix);
    }

    public void Start()
    {
        server.Start();
        Console.WriteLine("Сервер запущен");
    }

    public async Task ListenAsync()
    {
        while (true)
        {
            var context = await server.GetContextAsync();

            HttpListenerResponse response = context.Response;
            
            string htmlFileText = File.ReadAllText("hello.html");

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
        Console.ReadLine();
    }
}
