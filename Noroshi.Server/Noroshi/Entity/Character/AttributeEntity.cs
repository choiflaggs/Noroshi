using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Character;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.AttributeSchema;

namespace Noroshi.Server.Entity.Character
{
    public class AttributeEntity : AbstractDaoWrapperEntity<AttributeEntity, AttributeDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<AttributeEntity> ReadAndBuildAll()
        {
            return _instantiate((new AttributeDao()).ReadAll());
        }


        public uint ID => _record.ID;
        public uint ClassID => _record.ClassID;
        public uint GroupID => _record.GroupID;
        public uint Lifetime => _record.Lifetime;
        public uint EffectID => _record.EffectID;
        public int Arg1 => _record.Arg1;
        public int Arg2 => _record.Arg2;
        public int Arg3 => _record.Arg3;
        public int Arg4 => _record.Arg4;
        public int Arg5 => _record.Arg5;
        public int Arg6 => _record.Arg6;
        public int Arg7 => _record.Arg7;
        public int Arg8 => _record.Arg8;
        public int Arg9 => _record.Arg9;
        public int Arg10 => _record.Arg10;
        public int Arg11 => _record.Arg11;
        public int Arg12 => _record.Arg12;
        public int Arg13 => _record.Arg13;
        public int Arg14 => _record.Arg14;
        public int Arg15 => _record.Arg15;
        public uint ReceiveDamageCharacterEffectID => _record.ReceiveDamageCharacterEffectID;

        public Core.WebApi.Response.Character.Attribute ToResponseData()
        {
            return new Core.WebApi.Response.Character.Attribute()
            {
                ID = ID,
                ClassID = ClassID,
                GroupID = GroupID,
                Lifetime = Lifetime,
                EffectID = EffectID,
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
                Arg11 = Arg11,
                Arg12 = Arg12,
                Arg13 = Arg13,
                Arg14 = Arg14,
                Arg15 = Arg15,
                ReceiveDamageCharacterEffectID = ReceiveDamageCharacterEffectID,
            };
        }
    }
}
