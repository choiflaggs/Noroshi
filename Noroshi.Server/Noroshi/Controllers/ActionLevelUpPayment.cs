using Noroshi.Server.Services.Character;

namespace Noroshi.Server.Controllers
{
    public class ActionLevelUpPayment : AbstractController
    {
        public Core.WebApi.Response.ActionLevelUpPayment[] MasterData() => ActionLevelUpPaymentService.MasterData();
    }
}