using Noroshi.Core.WebApi.Request.Item;
using Noroshi.Core.WebApi.Response;
using UniRx;

namespace Noroshi.Gear
{
    class WebApiRequester
    {
        public static IObservable<GearEnchantAddExpResponse> GearEnchantExp(uint playerGearId, uint[] gearIds,
            uint[] gearEnchantMaterialIds, uint[] gearPieceIds)
        {
            var requestParams = new GearEnchantExpRequest {PlayerGearID = playerGearId, GearIDs = gearIds, GearEnchantMaterialIDs = gearEnchantMaterialIds, GearPieceIDs = gearPieceIds};
            return _getWebApiRequester().Post<GearEnchantExpRequest, GearEnchantAddExpResponse>("Gear/GearEnchantExp", requestParams);
        }
        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }

    }
}
