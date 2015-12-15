using System.Collections.Generic;
using System.Linq;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Character;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CharacterEffectSchema;

namespace Noroshi.Server.Entity.Character
{
    public class CharacterEffectEntity : AbstractDaoWrapperEntity<CharacterEffectEntity, CharacterEffectDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<CharacterEffectEntity> ReadAndBuildAll()
        {
            return (new CharacterEffectDao()).ReadAll().Select(r => _instantiate(r));
        }

        public uint ID { get { return _record.ID; } }
        public uint PrefabID { get { return _record.PrefabID; } }
        public string AnimationName { get { return _record.AnimationName; } }
        public byte MultiAnimation { get { return _record.MultiAnimation; } }
        public bool HasText { get { return _record.HasText > 0; } }
        public short OrderInCharacterLayer { get { return _record.OrderInCharacterLayer; } }
        public byte Position { get { return _record.Position; } }
        public bool FixedRotationY { get { return _record.FixedRotationY > 0; } }

        public Core.WebApi.Response.CharacterEffect ToResponseData()
        {
            return new Core.WebApi.Response.CharacterEffect()
            {
                ID = ID,
                PrefabID = PrefabID,
                AnimationName = AnimationName,
                MultiAnimation = MultiAnimation,
                HasText = HasText,
                OrderInCharacterLayer = OrderInCharacterLayer,
                Position = Position,
                FixedRotationY = FixedRotationY,
            };
        }
    }
}