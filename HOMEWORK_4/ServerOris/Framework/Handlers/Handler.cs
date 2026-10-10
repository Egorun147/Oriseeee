using System.Net;

namespace ServerOris.Framework.Handlers;

abstract class Handler
{
    public Handler Succesor { get; set; }
    public abstract Task HandleRequest(HttpListenerContext context);
}