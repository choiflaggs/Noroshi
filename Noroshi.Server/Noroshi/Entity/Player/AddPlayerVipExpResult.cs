using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Entity.Shop;

namespace Noroshi.Server.Entity.Player
{
    public class AddPlayerVipExpResult
    {
        public readonly ushort PreviousPlayerVipLevel;
        public readonly ushort CurrentPlayerVipLevel;
        public readonly PlayerVipLevelBonusEntity PreviousPlayerVipLevelBonus;
        public readonly PlayerVipLevelBonusEntity CurrentPlayerVipLevelBonus;


        public AddPlayerVipExpResult(ushort previousPlayerLevel, ushort currentPlayerLevel)
        {
            PreviousPlayerVipLevel = previousPlayerLevel;
            CurrentPlayerVipLevel = currentPlayerLevel;
            var playerVipLevelBonuses = PlayerVipLevelBonusEntity.ReadAndBuildMulti(new[] { previousPlayerLevel, currentPlayerLevel});
            PreviousPlayerVipLevelBonus = playerVipLevelBonuses.First(bonus => bonus.Level == previousPlayerLevel);
            CurrentPlayerVipLevelBonus = playerVipLevelBonuses.First(bonus => bonus.Level == CurrentPlayerVipLevel);
        }
        public bool LevelUp => CurrentPlayerVipLevel > PreviousPlayerVipLevel;

        public IEnumerable<Core.WebApi.Response.Players.AddPlayerVipExpResult> ToResponseData(ShopEntity[] Shops)
        {
            var previousStandingShops = Shops.Select(shop => new StandingRaidShop { ID = shop.ID, TextKey = shop.TextKey, Opened = shop.ResidentVipLevel <= PreviousPlayerVipLevelBonus.Level }).ToArray();
            var currentStandingShops = Shops.Select(shop => new StandingRaidShop { ID = shop.ID, TextKey = shop.TextKey, Opened = shop.ResidentVipLevel <= CurrentPlayerVipLevelBonus.Level }).ToArray();
            var results = new List<Core.WebApi.Response.Players.AddPlayerVipExpResult>();
            if (LevelUp)
            {
                if (!PreviousPlayerVipLevelBonus.QuickBattle && CurrentPlayerVipLevelBonus.QuickBattle)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = 0,
                        TextKey = "",
                    });
                }
                if (!PreviousPlayerVipLevelBonus.ConsecutiveQuickBattles && CurrentPlayerVipLevelBonus.ConsecutiveQuickBattles)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = 0,
                        TextKey = "",
                    });
                }
                if (!PreviousPlayerVipLevelBonus.GemToGearEnchant && CurrentPlayerVipLevelBonus.GemToGearEnchant)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = 0,
                        TextKey = "",
                    });
                }
                if (!PreviousPlayerVipLevelBonus.VipGacha && CurrentPlayerVipLevelBonus.VipGacha)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = 0,
                        TextKey = "",
                    });
                }
                foreach (var previousStandingShop in previousStandingShops)
                {
                    if (!previousStandingShop.Opened && currentStandingShops.First(shop => shop.ID == previousStandingShop.ID).Opened)
                    {
                        results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                        {
                            Param = 0,
                            TextKey = "",
                        });
                    }
                }
                if (PreviousPlayerVipLevelBonus.MaxStaminaRecoveryNum < CurrentPlayerVipLevelBonus.MaxStaminaRecoveryNum)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = CurrentPlayerVipLevelBonus.MaxStaminaRecoveryNum,
                        TextKey = "",
                    });
                }
                if (PreviousPlayerVipLevelBonus.MaxBackStoryRecoveryNum < CurrentPlayerVipLevelBonus.MaxBackStoryRecoveryNum)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = CurrentPlayerVipLevelBonus.MaxBackStoryRecoveryNum,
                        TextKey = "",
                    });
                }
                if (PreviousPlayerVipLevelBonus.MaxRentalCharacterNum < CurrentPlayerVipLevelBonus.MaxRentalCharacterNum)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = CurrentPlayerVipLevelBonus.MaxRentalCharacterNum,
                        TextKey = "",
                    });
                }
                if (PreviousPlayerVipLevelBonus.MaxActionLevelPointRecoveryNum < CurrentPlayerVipLevelBonus.MaxActionLevelPointRecoveryNum)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = CurrentPlayerVipLevelBonus.MaxActionLevelPointRecoveryNum,
                        TextKey = ""
                    });
                }
                if (PreviousPlayerVipLevelBonus.MaxResetExpeditionNum < CurrentPlayerVipLevelBonus.MaxResetExpeditionNum)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = CurrentPlayerVipLevelBonus.MaxResetExpeditionNum,
                        TextKey = "",
                    });
                }
                if (PreviousPlayerVipLevelBonus.MaxGreetingNum < CurrentPlayerVipLevelBonus.MaxGreetingNum)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = CurrentPlayerVipLevelBonus.MaxGreetingNum,
                        TextKey = "",
                    });
                }
                if (PreviousPlayerVipLevelBonus.GemBonus < CurrentPlayerVipLevelBonus.GemBonus)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = (uint)(CurrentPlayerVipLevelBonus.GemBonus * 100),
                        TextKey = "",
                    });
                }
                if (PreviousPlayerVipLevelBonus.GuildPointBonus < CurrentPlayerVipLevelBonus.GuildPointBonus)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = (uint)(CurrentPlayerVipLevelBonus.GuildPointBonus * 100),
                        TextKey = "",
                    });
                }
                if (PreviousPlayerVipLevelBonus.ExpeditionPointBonus < CurrentPlayerVipLevelBonus.ExpeditionPointBonus)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = (uint)(CurrentPlayerVipLevelBonus.ExpeditionPointBonus * 100),
                        TextKey = "",
                    });
                }
                if (PreviousPlayerVipLevelBonus.ArenaPointBonus < CurrentPlayerVipLevelBonus.ArenaPointBonus)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = (uint)(CurrentPlayerVipLevelBonus.ArenaPointBonus * 100),
                        TextKey = "",
                    });
                }
                if (PreviousPlayerVipLevelBonus.ExpeditionGoldBonus < CurrentPlayerVipLevelBonus.ExpeditionGoldBonus)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = (uint)(CurrentPlayerVipLevelBonus.ExpeditionGoldBonus * 100),
                        TextKey = "",
                    });
                }
                if (PreviousPlayerVipLevelBonus.DailyReceivableRaidTicketNum < CurrentPlayerVipLevelBonus.DailyReceivableRaidTicketNum)
                {
                    results.Add(new Core.WebApi.Response.Players.AddPlayerVipExpResult
                    {
                        Param = (uint)(CurrentPlayerVipLevelBonus.DailyReceivableRaidTicketNum * 100),
                        TextKey = "",
                    });
                }
            }
            return results;
        }
    }
}