using System.Net;

namespace ServerOris.Framework.Handlers;

class StaticFileHandler : Handler
{
    private string filePath = "login.html";
    private string siteFolder = Path.Combine(AppContext.BaseDirectory, "static");
    private string serverPath;

    public StaticFileHandler(string serverPath)
    {
        this.serverPath = serverPath;
    }
    public override async Task HandleRequest(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;
        string path = request.Url!.LocalPath.Substring(serverPath.Length);


        if (path == "")
        {
            path = filePath;
        }

        if (path == "login")
        {
            path = filePath;
        }

        bool isFile = Path.HasExtension(path);
        if (isFile)
        {
            string requestedFile;
            try
            {
                requestedFile = Path.GetFullPath(Path.Combine(siteFolder, path));
            }
            catch (ArgumentException)
            {
                response.StatusCode = 404;
                response.Close();
                return;
            }

            if (!requestedFile.StartsWith(siteFolder + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase) || !File.Exists(requestedFile))
            {
                response.StatusCode = 404;
                response.Close();
                return;
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
                    return;
            }

            try
            {
                byte[] buffer = await File.ReadAllBytesAsync(requestedFile);
                response.ContentLength64 = buffer.Length;

                if (request.HttpMethod != "HEAD")
                {
                    await response.OutputStream.WriteAsync(buffer);
                }

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
        else if (Succesor != null)
        {
            await Succesor.HandleRequest(context);
        }
        else
        {
            response.StatusCode = 404;
            response.Close();
        }
    }
}