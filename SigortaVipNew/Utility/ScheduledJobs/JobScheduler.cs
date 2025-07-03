using Quartz;
using Quartz.Impl;
using System;

namespace SigortaVip.Utility.ScheduledJobs
{
    class JobScheduler
    {
        //public static void StartPageReloader()
        //{
        //    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
        //    scheduler.Start();

        //    IJobDetail job = JobBuilder.Create<ActivePageReloaderJob>().Build();

        //    ITrigger trigger = TriggerBuilder.Create()
        //        .WithDailyTimeIntervalSchedule
        //          (s =>
        //             s
        //            .OnEveryDay()
        //            .WithIntervalInMinutes(4)
        //          )
        //        .Build();

        //    scheduler.ScheduleJob(job, trigger);
        //}
        //public static void StartPageReloaderOnce()
        //{
        //    IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
        //    scheduler.Start();

        //    IJobDetail job = JobBuilder.Create<ActivePageReloaderJob>().Build();

        //    ITrigger trigger = TriggerBuilder.Create()
        //        .StartAt(DateTimeOffset.Now.AddMinutes(20))
        //        .Build();

        //    scheduler.ScheduleJob(job, trigger);
        //}
    }
}
