using Flaggs.StateTransition;

namespace Noroshi.BattleScene.CharacterState
{
    public class CharacterState : IState
    {
        public virtual bool IsActionState { get { return false; } }
    }
}
