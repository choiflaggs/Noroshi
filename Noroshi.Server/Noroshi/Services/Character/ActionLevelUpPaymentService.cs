using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Entity.Character;

namespace Noroshi.Server.Services.Character
{
    public class ActionLevelUpPaymentService
    {
        public static ActionLevelUpPayment[] MasterData() => ActionLevelUpPaymentEntity.ReadAndBuildAll().Select(e => e.ToResponseData()).ToArray();
    }
}