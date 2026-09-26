namespace ServerOris;

public class Settings
{
    public Server Server { get; set; } = new Server();
}

public class Server
{
    public string Port { get; set; } = "2323";
    public string Host { get; set; } = "127.0.0.1";
    public string Path { get; set; } = "connection/";
}