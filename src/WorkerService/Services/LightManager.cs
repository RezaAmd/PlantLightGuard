using System.Device.Gpio;
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
                        // TODO: Just log the app still running...
                        continue;
                    }

                    // Turn on lamp.
                    TurnOnLamp(lampModel);
                }
                else
                {
                    if (lampModel.IsTurnOn == false)
                    {
                        // TODO: Just log the app still running...
                        continue;
                    }

                    // Turn off lamp.
                    TurnOffLamp(lampModel);
                }

                _logger.LogInformation("Sleep for 1 minute.");
                await Task.Delay(60_000, cancellationToken); // شبیه‌سازی بازه نوری
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
            _logger.LogInformation("The lamp turned on.");
        }
        private void TurnOffLamp(LampScheduleModel lampModel)
        {
            _controller.Write(lampModel.Pin, PinValue.Low);
            lampModel.IsTurnOn = false;
            _logger.LogInformation("The lamp turned off.");
        }
        #endregion
    }
}
