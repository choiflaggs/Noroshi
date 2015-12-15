namespace Noroshi.Server.Roles.OneTimeOpenable
{
    public interface IOneTimeOpenable
    {
        uint? OpenedAt { get; }
        uint? ClosedAt { get; }
    }
}
