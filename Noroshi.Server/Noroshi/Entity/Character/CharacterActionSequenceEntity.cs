using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Character;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterActionSequenceSchema;

namespace Noroshi.Server.Entity.Character
{
    public class CharacterActionSequenceEntity : AbstractDaoWrapperEntity<CharacterActionSequenceEntity, CharacterActionSequenceDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<CharacterActionSequenceEntity> ReadAndBuildAll()
        {
            return (new CharacterActionSequenceDao()).ReadAll().Select(r => _instantiate(r));
        }

        public uint CharacterID { get { return _record.CharacterID; } }
        public byte TargetActionNum { get { return _record.TargetActionNum; } }
        public byte SecondLoopStartPosition { get { return _record.SecondLoopStartPosition; } }
        public sbyte ActionSequence1 { get { return _record.ActionSequence1; } }
        public sbyte ActionSequence2 { get { return _record.ActionSequence2; } }
        public sbyte ActionSequence3 { get { return _record.ActionSequence3; } }
        public sbyte ActionSequence4 { get { return _record.ActionSequence4; } }
        public sbyte ActionSequence5 { get { return _record.ActionSequence5; } }
        public sbyte ActionSequence6 { get { return _record.ActionSequence6; } }
        public sbyte ActionSequence7 { get { return _record.ActionSequence7; } }

        public Core.WebApi.Response.Character.CharacterActionSequence ToResponseData()
        {
            return new Core.WebApi.Response.Character.CharacterActionSequence()
            {
                CharacterID = CharacterID,
                TargetActionNum = TargetActionNum,
                SecondLoopStartPosition = SecondLoopStartPosition,
                ActionSequence1 = ActionSequence1,
                ActionSequence2 = ActionSequence2,
                ActionSequence3 = ActionSequence3,
                ActionSequence4 = ActionSequence4,
                ActionSequence5 = ActionSequence5,
                ActionSequence6 = ActionSequence6,
                ActionSequence7 = ActionSequence7,
            };
        }
    }
}
