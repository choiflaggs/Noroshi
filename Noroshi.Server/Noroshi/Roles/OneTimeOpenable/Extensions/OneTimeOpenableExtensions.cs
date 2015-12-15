using Noroshi.Server.Contexts;

namespace Noroshi.Server.Roles.OneTimeOpenable.Extensions
{
    public static class OneTimeOpenableExtensions
    {
        public static bool IsOneTimeOpen(this IOneTimeOpenable oneTimeOpenable)
        {
            var timeHandler = ContextContainer.GetContext().TimeHandler;
            if (oneTimeOpenable.OpenedAt.HasValue)
            {
                if (timeHandler.UnixTime < oneTimeOpenable.OpenedAt.Value) return false;
            }
            if (oneTimeOpenable.ClosedAt.HasValue)
            {
                if (oneTimeOpenable.ClosedAt.Value <= timeHandler.UnixTime) return false;
            }
            return true;
        }
    }
}
