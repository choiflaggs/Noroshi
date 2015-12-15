using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.LoginBonus;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.LoginBonus;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.LoginBonusRewardSchema;

namespace Noroshi.Server.Entity.LoginBonus
{
    public class LoginBonusRewardEntity : AbstractDaoWrapperEntity<LoginBonusRewardEntity, LoginBonusRewardDao, Schema.PrimaryKey, Schema.Record>
    {
        public static LoginBonusRewardEntity ReadAndBuild(uint id)
        {
            return ReadAndBuild(new Schema.PrimaryKey { ID = id });
        }
        public static IEnumerable<LoginBonusRewardEntity> ReadAndBuildByLoginBonusIDs(IEnumerable<uint> loginBonusIds)
        {
            return (new LoginBonusRewardDao()).ReadByLoginBonusIDs(loginBonusIds).Select(r => _instantiate(r));
        }

        public uint ID => _record.ID;
        public uint LoginBonusID => _record.LoginBonusID;
        public byte Threshold => _record.Threshold;
        public PossessionCategory PossessionCategory => (PossessionCategory)_record.PossessionCategory;
        public uint PossessionID => _record.PossessionID;
        public uint PossessionNum => _record.PossessionNum;

        public PossessionParam GetPossessionParam(bool vipRewardFlag = false)
        {

            return new PossessionParam
            {
                Category = PossessionCategory,
                ID = PossessionID,
                Num = _vipPossessionNum(vipRewardFlag),
            };
        }
        uint _vipPossessionNum(bool vipRewardFlag)
        {
            if (vipRewardFlag)
            {
                return PossessionNum * Constant.INCREASE_MAGNIFICATION;
            }
            return PossessionNum;
        }
        public static bool HasVipBonus(uint threshold, ushort vipLevel, LoginBonusCategory category)
        {
            ushort vipNeedLevel = 1;
            switch (category)
            {
                case LoginBonusCategory.Monthly:
                    vipNeedLevel = Constant.MONTHLY_DAY_TO_VIP_LEVEL_MAP
                        .OrderByDescending(kv => kv.Key)
                        .First(kv => kv.Key <= threshold)
                        .Value;
                    break;
                case LoginBonusCategory.StartUp:
                    vipNeedLevel = Constant.START_UP_DAY_TO_VIP_LEVEL_MAP
                        .OrderByDescending(kv => kv.Key)
                        .First(kv => kv.Key <= threshold)
                        .Value;
                    break;
            }
            return vipNeedLevel <= vipLevel;
        }
    }
}
