using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Entity.Item;

namespace Noroshi.Server.Services.Item
{
    public class GearPieceService
    {
        public static PlayerGearPiece[] GetAll(uint playerId)
        {
            return PlayerGearPieceEntity.ReadAndBuildAll(playerId).Select(data => data.ToResponseData()).ToArray();
        }

        public static GearPiece[] MasterData()
        {
            return GearPieceEntity.ReadAndBuildAll().Select(data => data.ToResponseData()).ToArray();
        }
    }
}