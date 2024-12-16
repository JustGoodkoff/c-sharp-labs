using EveryoneToTheHackathon.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EveryoneToTheHackathon.Host;


public static class Program
{
    static void Main(string[] args)
    {

        // инициализация настроек
        HostApplicationBuilderSettings settings = new()
        {
            Args = args,
            ApplicationName = "Nsu.HackathonProblem",
            Configuration = new ConfigurationManager(),
            ContentRootPath = Directory.GetCurrentDirectory(),
        };

        // загрузка конфигурации
        settings.Configuration.AddJsonFile("appsettings.json", true, true);
        settings.Configuration.AddCommandLine(args);
        
        // создание билдера хоста
        HostApplicationBuilder builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(settings);
        
        // добавление сервиса хоста
        builder.Services.AddHostedService<HackathonService>(
            h => new HackathonService(
                h.GetRequiredService<ILogger<HackathonService>>(),
                h.GetRequiredService<IHackathon>()
                )
            );

        builder.Services.AddTransient<IHackathon, Hackathon>(
            h => new Hackathon(
                h.GetRequiredService<HRManager>(),
                h.GetRequiredService<HRDirector>()
                )
            ); 
            
        builder.Services.AddTransient<ITeamBuildingStrategy, ProposeAndRejectAlgorithm>();
        builder.Services.AddTransient<HRManager>();
        builder.Services.AddTransient<HRDirector>();
        
        IHost host = builder.Build();
        host.Run();
    }
}
