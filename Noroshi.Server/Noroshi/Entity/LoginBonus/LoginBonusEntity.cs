using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.LoginBonus;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.LoginBonus;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.LoginBonusSchema;

namespace Noroshi.Server.Entity.LoginBonus
{
    public class LoginBonusEntity : AbstractDaoWrapperEntity<LoginBonusEntity, LoginBonusDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<LoginBonusEntity> ReadAndBuildOpenLoginBonusByCurrentTime(uint currentTime)
        {
            return _loadAssociatedEntities((new LoginBonusDao()).ReadOpenLoginBonus(currentTime).Select(r => _instantiate(r)));
        }
        public static LoginBonusEntity ReadAndBuild(uint LoginBonusId)
        {
            var entity = ReadAndBuild(new Schema.PrimaryKey { ID = LoginBonusId });
            if (entity == null) return null;
            return _loadAssociatedEntities(new LoginBonusEntity[] { entity }).First();
        }
        static IEnumerable<LoginBonusEntity> _loadAssociatedEntities(IEnumerable<LoginBonusEntity> entities)
        {
            if (entities.Count() == 0) return new LoginBonusEntity[0];
            var rewards = LoginBonusRewardEntity.ReadAndBuildByLoginBonusIDs(entities.Select(e => e.ID));
            var rewardLookup = rewards.ToLookup(reward => reward.LoginBonusID);
            return entities.Select(entity =>
            {
                entity._setRewards(rewardLookup[entity.ID]);
                return entity;
            });
        }

        IEnumerable<LoginBonusRewardEntity> _rewards;

        void _setRewards(IEnumerable<LoginBonusRewardEntity> rewards)
        {
            _rewards = rewards;
        }

        public uint ID => _record.ID;
        public string TextKey => "Master.LoginBonus." + _record.TextKey;
        public LoginBonusCategory Category => (LoginBonusCategory)_record.Category;

        public IEnumerable<PossessionParam> GetPossessionParams()
        {
            return _rewards.Select(reward => reward.GetPossessionParam());
        }

        public IEnumerable<LoginBonusRewardEntity> GetRewardsByThreshold(byte threshold)
        {
            return _rewards.Where(reward => reward.Threshold == threshold);
        }

        public Core.WebApi.Response.LoginBonus.LoginBonus ToResponseData(PlayerLoginBonusEntity playerLoginBonus, PossessionManager possessionManager, ushort playerVipLevel)
        {
            var thresholdToPossessionParamsMap = _rewards.ToLookup(reward => reward.Threshold)
                .ToDictionary(group => group.Key, group => group.Select(reward => reward.GetPossessionParam(LoginBonusRewardEntity.HasVipBonus(reward.Threshold, playerVipLevel, Category))));
            return new Core.WebApi.Response.LoginBonus.LoginBonus
            {
                ID = ID,
                TextKey = TextKey,
                Category = Category,
                CurrentNum = playerLoginBonus.CurrentNum,
                Rewards = thresholdToPossessionParamsMap.Select(reward => new Core.WebApi.Response.LoginBonus.LoginBonusReward
                {
                    Threshold = reward.Key,
                    HasAlreadyReceivedReward = playerLoginBonus.HasAlreadyReceivedReward(reward.Key),
                    CanReceiveReward = playerLoginBonus.CanReceiveReward(reward.Key),
                    HasVipReward = LoginBonusRewardEntity.HasVipBonus(reward.Key, playerVipLevel, Category),
                    PossessionObjects = possessionManager.GetPossessionObjects(reward.Value).Select(po => po.ToResponseData()).ToArray()
                }).ToArray(),
            };
        }
    }
}
