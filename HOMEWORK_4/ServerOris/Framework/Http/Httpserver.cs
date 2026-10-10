using System.Net;
using ServerOris.Framework.Handlers;
namespace ServerOris;

public class HttpServer
{
    private readonly HttpListener server;
    private string filePath = "login.html";
    private string siteFolder = Path.Combine(AppContext.BaseDirectory, "static");
    private string serverPath;

    public HttpServer(string urlPrefix)
    {
        server = new HttpListener();
        server.Prefixes.Add(urlPrefix);
        serverPath = new Uri(urlPrefix).AbsolutePath;
    }

    public void Start()
    {
        if (!File.Exists(Path.Combine(siteFolder, filePath)))
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
        var handler = new StaticFileHandler(serverPath);
        handler.Succesor = new ControllerHandler();
        while (server.IsListening)
        {
            HttpListenerContext context;
            try
            {
                context = await server.GetContextAsync();
            }
            catch (HttpListenerException) when (!server.IsListening)
            {
                break;
            }

            var request = context.Request;
            var response = context.Response;

            string path = request.Url!.LocalPath.Substring(serverPath.Length);

            // Показываем страницы и формы, но не картинки и стили.
            if (request.HttpMethod == "POST" || !Path.HasExtension(path) ||
                Path.GetExtension(path).Equals(".html", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Запрос пользователя: " + request.HttpMethod + " " + request.Url.LocalPath);
            }
            
            await handler.HandleRequest(context);
            
        }
    }

    public void Stop()
    {
        server.Stop();
        Console.WriteLine("Сервер всё...");
    }
}
