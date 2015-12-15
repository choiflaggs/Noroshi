namespace Noroshi.Core.WebApi.Response
{
    public class DailyQuest
    {
        public uint ID { get; set; }
        public uint FacilityID { get; set; }
        public uint RequiredCount { get; set; }
        public string Description { get; set; }
        public DailyQuestReward[] RewardItems { get; set; }
    }
}