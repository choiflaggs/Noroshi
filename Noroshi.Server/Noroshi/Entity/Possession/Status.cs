using System;
using Noroshi.Core.Game.Possession;

namespace Noroshi.Server.Entity.Possession
{
    public class Status : IPossessionObject
    {
        public Status(uint id, uint num, uint possessionNum)
        {
            ID = id;
            Num = num;
            PossessingNum = possessionNum;
        }

        public PossessionCategory Category => PossessionCategory.Status;
        public uint ID { get; private set; }
        public uint Num { get; private set; }

        public string TextKey
        {
            get
            {
                switch ((PossessionStatusID)ID)
                {
                    case PossessionStatusID.PlayerExp:
                        return "PlayerExp";
                    case PossessionStatusID.Gold:
                        return "Gold";
                    case PossessionStatusID.CommonGem:
                        return "Gem";
                    case PossessionStatusID.FreeGem:
                        return "Gem";
                    case PossessionStatusID.BP:
                        return "BP";
                    case PossessionStatusID.Stamina:
                        return "Stamina";
                    case PossessionStatusID.PlayerVipExp:
                        return "PlayerVipExp";
                    default:
                        throw new ArgumentException();
                }
            }
        }
        public uint PossessingNum { get; private set; }

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
