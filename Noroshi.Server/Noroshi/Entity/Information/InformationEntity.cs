using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Information;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.information;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.InformationSchema;

namespace Noroshi.Server.Entity.Information
{
    public class InformationEntity : AbstractDaoWrapperEntity<InformationEntity, InformationDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<InformationEntity> ReadAndBuildOpenInformationByCurrentTime(uint currentTime)
        {
            return (new InformationDao()).ReadOpenInformation(currentTime).Select(r => _instantiate(r));
        }
        public static InformationEntity ReadAndBuild(uint InformationId)
        {
            return ReadAndBuild(new Schema.PrimaryKey { ID = InformationId });
        }
       
        public uint ID => _record.ID;
        public string TitleTextKey => "Master.Information." + _record.TextKey + ".Title";
        public string BodyTextKey => "Master.Information." + _record.TextKey + ".Body";
        public string BannerInformation => _record.BannerInformation;
        public InformationCategory Category => (InformationCategory)_record.Category;
        public uint OpenedAt => _record.OpenedAt;
        public uint ClosedAt => _record.ClosedAt;

        bool _hasReadInformation(uint? playerLastConfirmedAt)
        {
            if(playerLastConfirmedAt == null)
            {
                return false;
            }
            return playerLastConfirmedAt >= OpenedAt;
        }

        public Core.WebApi.Response.Information.Information ToResponseData(Dictionary<InformationCategory, uint?> informationCategoryToPlayerLastConfirmedAt)
        {
            return new Core.WebApi.Response.Information.Information
            {
                ID = ID,
                Category = Category,
                BannerInformation = BannerInformation,
                OpenedAt = OpenedAt,
                ClosedAt = ClosedAt,
                TitleTextKey = TitleTextKey,
                BodyTextKey = BodyTextKey,
                HasReadInformation = _hasReadInformation(informationCategoryToPlayerLastConfirmedAt[Category]),
            };
        }
    }
}
