using WorkerService;
using WorkerService.Models;
using WorkerService.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<LightManager>();

builder.Services.Configure<AppSettingModel>(builder.Configuration.GetSection("Setting"));

builder.Services.AddHostedService<Worker>();


var host = builder.Build();
await host.RunAsync();
