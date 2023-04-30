using System;

namespace Common.Helpers.Models
{
    public class SchedulingOptions
    {
        public const string Name = "CronScheduling";

        public string Schedule { get; set; } = "0 5 ? * *";
        public TimeSpan Timeout { get; set; } = TimeSpan.FromHours(23);
        public TimeSpan RestartDelay { get; set; } = TimeSpan.FromHours(1);
        public bool AutoStart { get; set; } = true;
    }
}
