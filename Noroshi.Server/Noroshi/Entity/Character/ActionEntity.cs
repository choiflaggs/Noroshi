using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Character;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.ActionSchema;

namespace Noroshi.Server.Entity.Character
{
    public class ActionEntity : AbstractDaoWrapperEntity<ActionEntity, ActionDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<ActionEntity> ReadAndBuildAll()
        {
            return (new ActionDao()).ReadAll().Select(r => _instantiate(r));
        }

        public uint ID { get { return _record.ID; } }
        public string TextKey => "Master.Action." + _record.TextKey;
        public uint ClassID { get { return _record.ClassID; } }
        public byte TriggerID { get { return _record.TriggerID; } }
        public float? ExecutableProbability => _record.ExecutableProbability > 0 ? (float?)_record.ExecutableProbability : null;
        public byte? ExecutableNum => _record.ExecutableNum > 0 ? (byte?)_record.ExecutableNum : null;
        public byte? ExecutorTargetable => _record.ExecutorTargetable > 0 ? (byte?)_record.ExecutorTargetable : null;
        public byte? TargetSortType => _record.TargetSortType > 0 ? (byte?)_record.TargetSortType : null;
        public byte? MaxTargetNum => _record.MaxTargetNum > 0 ? (byte?)_record.MaxTargetNum : null;
        public byte DamageType => _record.DamageType;
        public byte DamageMagicalAttribute => _record.DamageMagicalAttribute;
        public byte TargetStateID { get { return _record.TargetStateID; } }
        public uint AttributeID1 => _record.AttributeID1;
        public float AttributeIntercept1 => _record.AttributeIntercept1;
        public float AttributeSlope1 => _record.AttributeSlope1;
        public uint AttributeID2 => _record.AttributeID2;
        public float AttributeIntercept2 => _record.AttributeIntercept2;
        public float AttributeSlope2 => _record.AttributeSlope2;
        public int Arg1 { get { return _record.Arg1; } }
        public int Arg2 { get { return _record.Arg2; } }
        public int Arg3 { get { return _record.Arg3; } }
        public int Arg4 { get { return _record.Arg4; } }
        public int Arg5 { get { return _record.Arg5; } }
        public int Arg6 { get { return _record.Arg6; } }
        public int Arg7 { get { return _record.Arg7; } }
        public int Arg8 { get { return _record.Arg8; } }
        public int Arg9 { get { return _record.Arg9; } }
        public int Arg10 { get { return _record.Arg10; } }
        public float Intercept1 { get { return _record.Intercept1; } }
        public float Slope1 { get { return _record.Slope1; } }
        public float Intercept2 { get { return _record.Intercept2; } }
        public float Slope2 { get { return _record.Slope2; } }
        public float Intercept3 { get { return _record.Intercept3; } }
        public float Slope3 { get { return _record.Slope3; } }

        public uint? SoundID => _record.SoundID > 0 ? (uint?)_record.SoundID : null;
        public uint? ExecutionSoundID => _record.ExecutionSoundID > 0 ? (uint?)_record.ExecutionSoundID : null;
        public uint? HitSoundID => _record.HitSoundID > 0 ? (uint?)_record.HitSoundID : null;

        public Core.WebApi.Response.Character.Action ToResponseData()
        {
            return new Core.WebApi.Response.Character.Action
            {
                ID = ID,
                ClassID = ClassID,
                TriggerID = TriggerID,
                ExecutableProbability = ExecutableProbability,
                ExecutableNum = ExecutableNum,
                ExecutorTargetable = ExecutorTargetable,
                TextKey = TextKey,
                TargetSortType = TargetSortType,
                MaxTargetNum = MaxTargetNum,
                DamageType = DamageType,
                DamageMagicalAttribute = DamageMagicalAttribute,
                TargetStateID = TargetStateID,
                AttributeID1 = AttributeID1,
                AttributeIntercept1 = AttributeIntercept1,
                AttributeSlope1 = AttributeSlope1,
                AttributeID2 = AttributeID2,
                AttributeIntercept2 = AttributeIntercept2,
                AttributeSlope2 = AttributeSlope2,
                Arg1 = Arg1,
                Arg2 = Arg2,
                Arg3 = Arg3,
                Arg4 = Arg4,
                Arg5 = Arg5,
                Arg6 = Arg6,
                Arg7 = Arg7,
                Arg8 = Arg8,
                Arg9 = Arg9,
                Arg10 = Arg10,
                Intercept1 = Intercept1,
                Slope1 = Slope1,
                Intercept2 = Intercept2,
                Slope2 = Slope2,
                Intercept3 = Intercept3,
                Slope3 = Slope3,
                HitCharacterEffectID = _record.HitCharacterEffectID,
                SoundID = SoundID,
                ExecutionSoundID = ExecutionSoundID,
                HitSoundID = HitSoundID,
            };
        }
    }
}
