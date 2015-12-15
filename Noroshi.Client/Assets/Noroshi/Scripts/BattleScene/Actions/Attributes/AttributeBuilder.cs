namespace Noroshi.BattleScene.Actions.Attributes
{
    public class AttributeBuilder
    {
        public static IAttribute Build(Core.WebApi.Response.Character.Attribute data, float coefficient)
        {
            IAttribute attribute = null;
            switch (data.ClassID)
            {
            case 1:
                attribute = new StatusBoost(data, coefficient);
                break;
            case 2:
                attribute = new PeriodicDamage(data, coefficient);
                break;
            case 3:
                attribute = new LimitedShield(data, coefficient);
                break;
            case 4:
                attribute = new UnlimitedShield(data, coefficient);
                break;
            case 5:
                attribute = new SpeedChanger(data, coefficient);
                break;
            case 6:
                attribute = new StateTransitionBlock(data, coefficient);
                break;
            case 7:
                attribute = new AttributeShield(data, coefficient);
                break;
            case 8:
                attribute = new BreakableStun(data, coefficient);
                break;
            case 9:
                attribute = new Charm(data, coefficient);
                break;
            case 10:
                attribute = new Stun(data, coefficient);
                break;
            case 11:
                attribute = new StatusChanger(data, coefficient);
                break;
            case 12:
                attribute = new MissDamage(data, coefficient);
                break;
            default:
                break;
            }
            return attribute;
        }
    }
}
