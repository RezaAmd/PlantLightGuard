using Microsoft.Extensions.Options;
using WorkerService.Models;
using WorkerService.Services;

namespace WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly AppSettingModel _appSettingModel;
        private readonly LightManager _lightManager;
        public Worker(ILogger<Worker> logger,
            IOptions<AppSettingModel> appSettingOptions,
            LightManager lightManager)
        {
            _logger = logger;
            _appSettingModel = appSettingOptions.Value;
            _lightManager = lightManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Prepare lamp schedule model instance.
            LampScheduleModel lampModel = new();
            lampModel.Pin = _appSettingModel.LightPin;
            lampModel.TurnOnPeriod = _appSettingModel.LightTurnOnSchedule;

            // Run managing lamp.
            await _lightManager.RunByScheduleAsync(lampModel, stoppingToken);
        }
    }
}