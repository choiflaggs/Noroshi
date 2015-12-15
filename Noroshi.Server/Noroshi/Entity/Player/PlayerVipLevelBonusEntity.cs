using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;
using Noroshi.Server.Entity.Shop;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerVipLevelBonusSchema;


namespace Noroshi.Server.Entity.Player
{
    public class PlayerVipLevelBonusEntity : AbstractDaoWrapperEntity<PlayerVipLevelBonusEntity, PlayerVipLevelBonusDao, Schema.PrimaryKey, Schema.Record>
    {
        static Dictionary<ushort, PlayerVipLevelBonusEntity> _levelToVipLevelBonusMap;

        static PlayerVipLevelBonusEntity()
        {
           _tryToSetCache(); 
        }

        static void _tryToSetCache()
        {
            if (_levelToVipLevelBonusMap == null)
            {
                _levelToVipLevelBonusMap = _instantiate((new PlayerVipLevelBonusDao()).ReadAll()).ToDictionary(bonus => bonus.Level);
            }
        }

        public static IEnumerable<PlayerVipLevelBonusEntity> ReadAndBuildMulti(IEnumerable<ushort> levels)
        {
            return levels.Select(level => _levelToVipLevelBonusMap[level]);
        }

        public static PlayerVipLevelBonusEntity ReadAndBuild(ushort level)
        {
            return _levelToVipLevelBonusMap[level];
        }

        public static IEnumerable<PlayerVipLevelBonusEntity> ReadAndBuildAll()
        {
            return _levelToVipLevelBonusMap.Select(map => map.Value);
        }

        public ushort Level => _record.Level;
        public bool QuickBattle => _record.QuickBattle > 0;
        public bool ConsecutiveQuickBattles => _record.ConsecutiveQuickBattles > 0;
        public bool GemToGearEnchant => _record.GemToGearEnchant > 0;
        public bool VipGacha => _record.VipGacha > 0;
        public byte MaxStaminaRecoveryNum => _record.MaxStaminaRecoveryNum;
        public byte MaxBackStoryRecoveryNum => _record.MaxBackStoryRecoveryNum;
        public byte MaxRentalCharacterNum => _record.MaxRentalCharacterNum;
        public byte MaxResetExpeditionNum => _record.MaxResetExpeditionNum;
        public byte MaxGreetingNum => _record.MaxGreetingNum;
        public float GemBonus => _record.GemBonus;
        public float GuildPointBonus => _record.GuildPointBonus;
        public float ExpeditionPointBonus => _record.ExpeditionPointBonus;
        public float ArenaPointBonus => _record.ArenaPointBonus;
        public float ExpeditionGoldBonus => _record.ExpeditionGoldBonus;
        public ushort DailyReceivableRaidTicketNum => _record.DailyReceivableRaidTicketNum;
        public byte MaxActionLevelPointRecoveryNum => _record.MaxActionLevelPointRecoveryNum;


        public PlayerVipLevelBonus ToResponseData(ShopEntity[] Shops)
        {
            var standingShops = Shops.Select(shop => new StandingRaidShop {ID = shop.ID, TextKey = shop.TextKey, Opened = shop.ResidentVipLevel <= Level}).ToArray();
            return new PlayerVipLevelBonus
            {
                ArenaPointBonus = ArenaPointBonus,
                ConsecutiveQuickBattles = ConsecutiveQuickBattles,
                DailyReceivableRaidTicketNum = DailyReceivableRaidTicketNum,
                ExpeditionGoldBonus = ExpeditionGoldBonus,
                ExpeditionPointBonus = ExpeditionPointBonus,
                GemBonus = GemBonus,
                GemToGearEnchant = GemToGearEnchant,
                GuildPointBonus = GuildPointBonus,
                Level = Level,
                MaxBackStoryRecoveryNum = MaxBackStoryRecoveryNum,
                MaxGreetingNum = MaxGreetingNum,
                MaxActionLevelPointRecoveryNum = MaxActionLevelPointRecoveryNum,
                MaxStaminaRecoveryNum = MaxStaminaRecoveryNum,
                MaxRentalCharacterNum = MaxRentalCharacterNum,
                MaxResetExpeditionNum = MaxResetExpeditionNum,
                QuickBattle = QuickBattle,
                VipGacha = VipGacha,
                StandingRaidShops = standingShops
            };
        }
    }
}