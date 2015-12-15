using System.Collections.Generic;
using UniLinq;

namespace Noroshi.BattleScene.Actions
{
    public class ActionBuilder
    {
        public static IAction[] BuildMulti(IEnumerable<Core.WebApi.Response.Character.Action> datas)
        {
            return datas.Select(data => _build(data)).ToArray();
        }
        static IAction _build(Core.WebApi.Response.Character.Action data)
        {
            IAction action = null;
            switch (data.ClassID)
            {
            case 1:
                action = new RangeAttack(data);
                break;
            case 2:
                action = new IndirectRangeAttack(data);
                break;
            case 3:
                action = new GiveAttribute(data);
                break;
            case 4:
                action = new FixedDamage(data);
                break;
            case 5:
                action = new RatioDamage(data);
                break;
            case 6:
                action = new RangeRatioDamage(data);
                break;
            case 7:
                action = new ChargeAttack(data);
                break;
            case 8:
                action = new Attack(data);
                break;
            case 10:
                action = new ChargeRangeAttack(data);
                break;
            case 11:
                action = new WideAttack(data);
                break;
            case 12:
                action = new StealFixedDamage(data);
                break;
            case 13:
                action = new IndirectAttack(data);
                break;
            case 14:
                action = new StraightRangeAttack(data);
                break;
            case 15:
                action = new ChargeThroughAttack(data);
                break;
            case 16:
                action = new StealWideAttack(data);
                break;
            case 17:
                action = new BehindAttack(data);
                break;
            case 18:
                action = new RangeWideAttack(data);
                break;
            case 19:
                action = new DelayAppearanceAttack(data);
                break;
            case 20:
                action = new ChargeDefenseAttack(data);
                break;
            case 21:
                action = new RangeGiveAttribute(data);
                break;
            case 1001:
                action = new ShadowMaker(data);
                break;
            case 2001:
                action = new MultiAction(data);
                break;
            case 2002:
                action = new SequentialMultiAction(data);
                break;
            case 2003:
                action = new RandomAction(data);
                break;
            case 2004:
                action = new StraightRandomAction(data);
                break;
            case 2005:
                action = new SequentialRandomAction(data);
                break;
            case 2006:
                action = new ChargeSequentialMultiAction(data);
                break;
            case 2007:
                action = new BackwardSequentialMultiAction(data);
                break;
            case 2008:
                action = new StraightSequentialAction(data);
                break;
            case 3001:
                action = new OverrideArg(data);
                break;
            default:
                break;
            }
            return action;
        }
    }
}