using System.Diagnostics;
using copilot_client;
using copilot_client_services.Handlers;
using copilot_client_services.interfaces.services;
using copilot_client_services.services;
using copilot_client_shared.Settings;
using Microsoft.Agents.CopilotStudio.Client;

var builder = Host.CreateApplicationBuilder(args);
var settings = new CopilotConnectionSettings(builder.Configuration.GetSection("CopilotStudioClientSettings"));
builder.Services.AddSingleton(settings);
builder.Services.AddHostedService<Worker>();
builder.Services.AddTransient<IMyCopilotClientService, MyCopilotClientService>();
builder.Services.AddHttpClient("mcs").ConfigurePrimaryHttpMessageHandler(() =>
{
    return new MyCustomTokenHandler(settings);
});
builder.Services.AddTransient<CopilotClient>(serviceProvider =>
{
    var settings = new CopilotConnectionSettings(builder.Configuration.GetSection("CopilotStudioClientSettings"));
    var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<CopilotClient>();
    return new CopilotClient(
        settings,
        serviceProvider.GetRequiredService<IHttpClientFactory>(),
        logger,
        "mcs"
    );
});


var host = builder.Build();
host.Run();
