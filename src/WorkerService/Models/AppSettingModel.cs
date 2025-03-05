namespace WorkerService.Models
{
    public class AppSettingModel
    {
        public int LightPin { get; set; } = 17;
        public List<TimeDurationModel> LightTurnOnSchedule { get; set; } = new();
    }
}
