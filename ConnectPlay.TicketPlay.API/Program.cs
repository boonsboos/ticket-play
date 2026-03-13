using QuestPDF.Infrastructure;

namespace ConnectPlay.TicketPlay.API;

public class Program
{
    public static void Main(string[] args)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var builder = Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>(); // Configure the rest in Startup
            });

        var app = builder.Build();

        app.Run();
    }
}