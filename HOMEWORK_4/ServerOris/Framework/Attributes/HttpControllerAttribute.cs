namespace ServerOris.Framework.Attributes;

public class HttpControllerAttribute : Attribute
{
    public string Route { get; }

    public HttpControllerAttribute(string route)
    {
        Route = route;
    }
}