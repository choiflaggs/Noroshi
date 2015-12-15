
namespace Noroshi.BattleScene.Actions.Attributes
{
    public class AttributeShield : AbstractAttribute 
    {
        public AttributeShield(Core.WebApi.Response.Character.Attribute data, float coefficient) : base(data, coefficient)
        {
        }

        public override void OnReceiveActionEvent(IActionTarget target, ActionEvent actionEvent)
        {
            var addAttribute = actionEvent.Attribute;
            if (addAttribute != null && addAttribute.IsNegative)
            {
                actionEvent.RemoveAttribute();
            }
        }

        public override void OnEnter(IActionTarget target)
        {
        }

        public override void OnExit(IActionTarget target)
        {
        }

        public override bool IsNegative { get { return false; } }
    }
}
