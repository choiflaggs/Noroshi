using System.Collections.Generic;
using Noroshi.Core.Game.Sound;
using Noroshi.Server.Daos.Rdb;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.SoundSchema;

namespace Noroshi.Server.Entity
{
    public class SoundEntity : AbstractDaoWrapperEntity<SoundEntity, SoundDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<SoundEntity> ReadAndBuildAll()
        {
            return _instantiate((new SoundDao()).ReadAll());
        }

        public Core.WebApi.Response.Master.Sound ToResponseData()
        {
            return new Core.WebApi.Response.Master.Sound
            {
                ID = _record.ID,
                Path = _record.Path,
                ChannelNum = _record.ChannelNum,
                PlayType = (PlayType)_record.PlayType,
            };
        }
    }
}
