namespace WorkerService.Models
{
    public class LampScheduleModel
    {
        public int Pin { get; set; }
        public bool IsTurnOn { get; set; } = false;
        public List<TimeDurationModel> TurnOnPeriod { get; set; } = new();
    }
}