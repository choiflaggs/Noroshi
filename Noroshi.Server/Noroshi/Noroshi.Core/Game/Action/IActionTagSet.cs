using System.Collections.Generic;
using Noroshi.Core.Game.Enums;

namespace Noroshi.Core.Game.Action
{
    public interface IActionTagSet
    {
        bool HasTag(int index);
        IEnumerable<ActionTargetAttribute> GetActionTargetAttributes();
    }
}
