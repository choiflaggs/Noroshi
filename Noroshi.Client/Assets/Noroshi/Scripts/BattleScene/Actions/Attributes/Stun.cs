namespace Noroshi.BattleScene.Actions.Attributes
{
    public class Stun : AbstractAttribute
    {
        public Stun(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
        }

        public override void OnEnter(IActionTarget target)
        {
            target.TryToTransitToStop();
        }

        public override void OnExit(IActionTarget target)
        {
            target.TryToTransitFromStop();
        }

        public override bool IsNegative { get { return true; } }
    }
}