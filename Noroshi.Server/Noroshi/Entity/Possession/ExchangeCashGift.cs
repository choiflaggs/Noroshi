using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Entity.Possession
{
    public class ExchangeCashGift : IPossessionObject
    {
        private readonly ExchangeCashGiftEntity _exchangeCashGift;

        public ExchangeCashGift(ExchangeCashGiftEntity exchangeCashGift, uint num, uint possessionNum)
        {
            _exchangeCashGift = exchangeCashGift;
            Num = num;
            PossessingNum = possessionNum;
        }

        public PossessionCategory Category => PossessionCategory.ExchangeCashGift;
        public uint ID => _exchangeCashGift.ExchangeCashGiftID;
        public uint Num
        { get; }

        public string TextKey => _exchangeCashGift.TextKey;
        public uint PossessingNum
        { get; }

        public Core.WebApi.Response.Possession.PossessionObject ToResponseData()
        {
            return new Core.WebApi.Response.Possession.PossessionObject
            {
                Category = (byte)Category,
                ID = ID,
                Num = Num,

                Name = TextKey,
                PossessingNum = PossessingNum,
            };
        }
    }
}
