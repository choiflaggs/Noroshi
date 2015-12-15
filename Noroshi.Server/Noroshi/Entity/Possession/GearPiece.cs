using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Entity.Possession
{
    public class GearPiece : IPossessionObject
    {
        private readonly GearPieceEntity _gearPiece;

        public GearPiece(GearPieceEntity gearPiece, uint num, uint possessionNum)
        {
            _gearPiece = gearPiece;
            Num = num;
            PossessingNum = possessionNum;
        }

        public PossessionCategory Category => PossessionCategory.GearPiece;
        public uint ID => _gearPiece.GearPieceID;
        public uint Num
        { get; }

        public string TextKey => _gearPiece.TextKey;
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
