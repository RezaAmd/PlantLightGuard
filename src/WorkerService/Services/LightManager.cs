﻿using System.Device.Gpio;
using WorkerService.Models;

namespace WorkerService.Services
{
    public class LightManager
    {
        private readonly ILogger<LightManager> _logger;
        private readonly GpioController _controller;
        public LightManager(ILogger<LightManager> logger)
        {
            _controller = new();
            _logger = logger;
        }

        public async Task RunByScheduleAsync(LampScheduleModel lampModel,
            CancellationToken cancellationToken = default)
        {
            if(lampModel.TurnOnPeriod.Count == 0)
            {
                _logger.LogError("No schedules found!");
                return;
            }

            // Print schedules.
            PrintTimeSchedules(lampModel.TurnOnPeriod);

            // Prepare pin for output on board.
            PreparePinForOutput(lampModel.Pin);

            // Turn off lamp at first.
            TurnOffLamp(lampModel);

            _logger.LogInformation("Investigating lamp schedules ...");

            while (!cancellationToken.IsCancellationRequested)
            {
                bool isInTurnOnPeriod = IsInIntervalNow(lampModel.TurnOnPeriod);
                if (isInTurnOnPeriod)
                {
                    if (lampModel.IsTurnOn)
                    {
                        await WaitAMinuteAsync(cancellationToken);
                        continue;
                    }

                    // Turn on lamp.
                    TurnOnLamp(lampModel);
                }
                else
                {
                    if (lampModel.IsTurnOn == false)
                    {
                        await WaitAMinuteAsync(cancellationToken);
                        continue;
                    }

                    // Turn off lamp.
                    TurnOffLamp(lampModel);
                }
                await WaitAMinuteAsync(cancellationToken);
            }
        }

        #region Util's
        private void PreparePinForOutput(int pin)
        {
            _controller.OpenPin(pin, PinMode.Output);
        }

        private bool IsInIntervalNow(List<TimeDurationModel> timeDurationList)
        {
            _logger.LogInformation("Investigating lamp schedules ...");
            List<bool> isInIntervalList = new();
            foreach (var timeDuration in timeDurationList)
            {
                isInIntervalList.Add(IsInIntervalNow(timeDuration));
            }
            return isInIntervalList.Any(b => b == true);
        }
        private bool IsInIntervalNow(TimeDurationModel timeDuration)
        {
            TimeOnly timeNow = TimeOnly.Parse(DateTime.Now.ToLongTimeString());

            return timeNow >= timeDuration.Start && timeNow <= timeDuration.End;
        }

        private void TurnOnLamp(LampScheduleModel lampModel)
        {
            _controller.Write(lampModel.Pin, PinValue.High);
            lampModel.IsTurnOn = true;
            _logger.LogInformation($"The lamp (#{lampModel.Pin}) turned on.");
        }
        private void TurnOffLamp(LampScheduleModel lampModel)
        {
            _controller.Write(lampModel.Pin, PinValue.Low);
            lampModel.IsTurnOn = false;
            _logger.LogInformation("The lamp (#{lampModel.Pin}) turned off.");
        }

        private async Task WaitAMinuteAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Sleep for 1 minute.");
            await Task.Delay(60_000, cancellationToken);
        }

        private void PrintTimeSchedules(List<TimeDurationModel> Schedule)
        {
            _logger.LogInformation($"{Schedule.Count} Schedules exist.");
            foreach (var timeDuration in Schedule)
            {
                _logger.LogInformation($"From {timeDuration.Start} To {timeDuration.End}");
            }
        }
        #endregion
    }
}
