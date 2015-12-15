namespace Noroshi.Core.Game.Possession
{
    public enum PossessionCategory
    {
        Gear = 1,
        GearPiece = 2,
        GearEnchantMaterial = 3,
        Soul = 4,
        Drug = 5,
        ExchangeCashGift = 6,
        RaidTicket = 7,
        Character = 8,
        Status = 11
    }
    public enum PossessionStatusID
    {
        PlayerExp = 1,
        Gold = 2,
        CommonGem = 3,
        FreeGem = 4,
        // 危険過ぎるので課金ジェムはここでは扱わない。
//      ChargeGem = 5,
        BP = 6,
        Stamina = 7,
        PlayerVipExp = 8,
    }
}
