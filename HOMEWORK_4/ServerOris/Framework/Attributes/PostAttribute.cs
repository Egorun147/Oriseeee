namespace ServerOris.Framework.Attributes;

public class PostAttribute : Attribute
{
    public string Route { get; }

    public PostAttribute(string route)
    {
        Route = route;
    }
}