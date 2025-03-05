using WorkerService;
using WorkerService.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<LightManager>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
