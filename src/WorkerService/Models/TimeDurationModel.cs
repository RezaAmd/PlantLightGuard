namespace WorkerService.Models
{
    public record TimeDurationModel
    {
        public TimeOnly Start { get; set; }
        public TimeOnly End { get; set; }
    }
}