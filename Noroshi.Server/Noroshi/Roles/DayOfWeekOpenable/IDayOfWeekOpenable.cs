namespace Noroshi.Server.Roles.DayOfWeekOpenable
{
    public interface IDayOfWeekOpenable
    {
        bool IsOpenOnSunday { get; }
        bool IsOpenOnMonday { get; }
        bool IsOpenOnTuesday { get; }
        bool IsOpenOnWednesday { get; }
        bool IsOpenOnThursday { get; }
        bool IsOpenOnFriday { get; }
        bool IsOpenOnSaturday { get; }
    }
}
