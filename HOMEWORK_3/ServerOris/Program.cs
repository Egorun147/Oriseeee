using System.Net;
using System.Text;
using System.Text.Json;
using ServerOris;

string settingsJson = File.ReadAllText("settings.json");

Settings setting = JsonSerializer.Deserialize<Settings>(settingsJson);

string urlPrefix =
    $"http://{setting.Server.Host}:{setting.Server.Port}/{setting.Server.Path}";

HttpServer server = new HttpServer(urlPrefix);

server.Start();

Console.WriteLine("Вот ссилка: " + urlPrefix);

Task listenTask = server.ListenAsync();

string command = "";

while (command != "exit")
{
    command = Console.ReadLine();
}

server.Stop();
