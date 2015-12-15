using System;
using System.Collections.Generic;
using Noroshi.Server.Contexts;

namespace Noroshi.Server.Roles.DayOfWeekOpenable.Extensions
{
    public static class DayOfWeekOpenableExtensions
    {
        public static bool IsDayOfWeekOpen(this IDayOfWeekOpenable dayOfWeekOpenable)
        {
            return GetOpenDayOfWeeks(dayOfWeekOpenable).Contains(ContextContainer.GetContext().TimeHandler.LocalDayOfWeek);
        }

        public static List<DayOfWeek> GetOpenDayOfWeeks(this IDayOfWeekOpenable dayOfWeekOpenable)
        {
            var dayOfWeeks = new List<DayOfWeek>();
            if (dayOfWeekOpenable.IsOpenOnSunday) dayOfWeeks.Add(DayOfWeek.Sunday);
            if (dayOfWeekOpenable.IsOpenOnMonday) dayOfWeeks.Add(DayOfWeek.Monday);
            if (dayOfWeekOpenable.IsOpenOnTuesday) dayOfWeeks.Add(DayOfWeek.Tuesday);
            if (dayOfWeekOpenable.IsOpenOnWednesday) dayOfWeeks.Add(DayOfWeek.Wednesday);
            if (dayOfWeekOpenable.IsOpenOnThursday) dayOfWeeks.Add(DayOfWeek.Thursday);
            if (dayOfWeekOpenable.IsOpenOnFriday) dayOfWeeks.Add(DayOfWeek.Friday);
            if (dayOfWeekOpenable.IsOpenOnSaturday) dayOfWeeks.Add(DayOfWeek.Saturday);
            return dayOfWeeks;
        }
    }
}
