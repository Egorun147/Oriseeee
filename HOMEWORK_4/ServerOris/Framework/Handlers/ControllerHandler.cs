using System.Net;
using System.Reflection;
using ServerOris.Framework.Attributes;
using System.Text;

namespace ServerOris.Framework.Handlers;

class ControllerHandler : Handler
{
    public override Task HandleRequest(HttpListenerContext context)
    {
        string[] parts = context.Request.Url!.AbsolutePath
            .Split('/', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length != 3)
        {
            context.Response.StatusCode = 404;
            context.Response.Close();
            return Task.CompletedTask;
        }

        string controllerRoute = parts[1];
        string methodRoute = parts[2];
        Type? controllerType = null;

        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            var attribute = type.GetCustomAttribute<HttpControllerAttribute>();

            if (attribute != null && attribute.Route == controllerRoute)
            {
                controllerType = type;
                break;
            }
        }
        
        if (controllerType == null)
        {
            context.Response.StatusCode = 404;
            context.Response.Close();
            return Task.CompletedTask;
        }

        MethodInfo? controllerMethod = null;

        if (context.Request.HttpMethod == "GET")
        {
            foreach (MethodInfo method in controllerType.GetMethods())
            {
                var attribute = method.GetCustomAttribute<GetAttribute>();

                if (attribute != null && attribute.Route == methodRoute)
                {
                    controllerMethod = method;
                    break;
                }
            }
        }
        
        else if (context.Request.HttpMethod == "POST")
        {
            foreach (MethodInfo method in controllerType.GetMethods())
            {
                var attribute = method.GetCustomAttribute<PostAttribute>();

                if (attribute != null && attribute.Route == methodRoute)
                {
                    controllerMethod = method;
                    break;
                }
            }
        }
        
        if (controllerMethod == null)
        {
            context.Response.StatusCode = 404;
            context.Response.Close();
            return Task.CompletedTask;
        }

        var controller = Activator.CreateInstance(controllerType);
        
        if (context.Request.HttpMethod == "POST")
        {
            using StreamReader reader = new StreamReader(
                context.Request.InputStream,
                context.Request.ContentEncoding);

            string data = reader.ReadToEnd();

            string login = "";
            string password = "";

            foreach (string field in data.Split('&'))
            {
                string[] pair = field.Split('=', 2);

                if (pair.Length != 2)
                {
                    continue;
                }

                string name = WebUtility.UrlDecode(pair[0]);
                string value = WebUtility.UrlDecode(pair[1]);

                if (name == "email")
                {
                    login = value;
                }

                if (name == "password")
                {
                    password = value;
                }
            }

            controllerMethod.Invoke(controller, new object[] { login, password });

            context.Response.StatusCode = 303;
            context.Response.RedirectLocation = "/connection/auth/login";
            context.Response.Close();

            return Task.CompletedTask;
        }
        
        string html = (string)controllerMethod.Invoke(controller, null)!;

        byte[] buffer = Encoding.UTF8.GetBytes(html);

        context.Response.StatusCode = 200;
        context.Response.ContentType = "text/html; charset=utf-8";
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.Close();

        return Task.CompletedTask;
    }
}