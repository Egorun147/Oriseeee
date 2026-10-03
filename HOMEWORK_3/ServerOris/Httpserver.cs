using System.Net;

namespace ServerOris;

public class HttpServer
{
    private readonly HttpListener server;
    private string filePath = "hello.html";
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
            Console.WriteLine("Пришел запрос: " + path);

            if (path == "")
            {
                path = filePath;
            }

            string requestedFile;
            try
            {
                requestedFile = Path.GetFullPath(Path.Combine(siteFolder, path));
            }
            catch (ArgumentException)
            {
                response.StatusCode = 404;
                response.Close();
                continue;
            }

            // Отдаём файлы только из папки static.
            if (!requestedFile.StartsWith(siteFolder + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase) || !File.Exists(requestedFile))
            {
                response.StatusCode = 404;
                response.Close();
                continue;
            }

            string extension = Path.GetExtension(requestedFile).ToLower();
            switch (extension)
            {
                case ".html":
                    response.ContentType = "text/html; charset=utf-8";
                    break;
                case ".css":
                    response.ContentType = "text/css; charset=utf-8";
                    break;
                case ".js":
                    response.ContentType = "text/javascript; charset=utf-8";
                    break;
                case ".png":
                    response.ContentType = "image/png";
                    break;
                case ".jpg":
                case ".jpeg":
                    response.ContentType = "image/jpeg";
                    break;
                case ".webp":
                    response.ContentType = "image/webp";
                    break;
                case ".svg":
                    response.ContentType = "image/svg+xml";
                    break;
                case ".ico":
                    response.ContentType = "image/x-icon";
                    break;
                default:
                    response.StatusCode = 404;
                    response.Close();
                    continue;
            }

            try
            {
                byte[] buffer = await File.ReadAllBytesAsync(requestedFile);
                response.ContentLength64 = buffer.Length;

                if (request.HttpMethod != "HEAD")
                {
                    await response.OutputStream.WriteAsync(buffer);
                }

                Console.WriteLine("Запрос обработан!");
            }
            catch (IOException)
            {
                Console.WriteLine("Не удалось прочитать или отправить файл");
            }
            catch (HttpListenerException)
            {
                Console.WriteLine("Клиент отключился");
            }
            finally
            {
                response.Close();
            }
        }
    }

    public void Stop()
    {
        server.Stop();
        Console.WriteLine("Сервер всё...");
    }
}
