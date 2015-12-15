namespace Noroshi.Core.Game.Player
{
    public enum Country
    {
        Japan = 1,
    }
    public enum Language
    {
        Japanese = 1,
    }
    /// <summary>
    /// チュートリアル進捗度。
    /// </summary>
    public enum TutorialStep
    {
        ClearStoryStage1        =  11,
        ClearStoryStage2        =  21,
        EquipGear               =  22,
        ClearStoryStage3        =  31,
        ActionLevelUP           =  32,
        ClearStoryStage4        =  41,
        PromotionLevelUP        =  42,
        ClearStoryStage5        =  51,
        ClearStoryStage6        =  61,
        ConsumeDrug             =  62,
        ClearStoryStage7        =  71,
        LotGacha                =  72,
        ClearStoryStage8        =  81,
        ReceiveQuestReward      =  82,
        ClearStoryStage9        =  91,
        ReceiveDailyQuestReward =  92,
        ClearStoryStage10       = 101,
        EvolutionLevelUP        = 102,
    }

    /// <summary>
    /// デバイストークン種別。
    /// </summary>
    public enum DeviceTokenType
    {
        iOS = 1,
        Android = 2,
    }
    /// <summary>
    /// デバイストークン状態。
    /// </summary>
    public enum DeviceTokenStatus
    {
        /// <summary>
        /// 正常。
        /// </summary>
        Normal = 1,
        /// <summary>
        /// エラー。
        /// </summary>
        Error = 2,
    }
}
