using System.Linq;
using LightNode.Server;
using Noroshi.Core.WebApi.Response.Master;
using Noroshi.Core.Game.Player;
using Noroshi.Server.Entity;
using Noroshi.Server.Entity.FrameWork;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Character;

namespace Noroshi.Server.Controllers
{
    public class Master : AbstractController
    {
        [Get]
        public TextMasterResponse TextMaster()
        {
            uint languageId = (uint)Language.Japanese;
            return new TextMasterResponse
            {
                DynamicTexts = DynamicTextEntity.ReadAndBuildByLanguageID(languageId).Select(dt => dt.ToResponseData()).ToArray(),
            };
        }
        [Get]
        public LevelMasterResponse LevelMaster()
        {
            return new LevelMasterResponse
            {
                PlayerLevels = PlayerLevelEntity.ReadAndBuildAll().Select(pl => pl.ToResponseData()).ToArray(),
                PlayerVipLevels = PlayerVipLevelEntity.ReadAndBuildAll().Select(pl => pl.ToResponseData()).ToArray(),
                CharacterLevels = CharacterLevelEntity.ReadAndBuildAll().Select(pl => pl.ToResponseData()).ToArray(),
            };
        }
        [Get]
        public SoundMasterResponse SoundMaster()
        {
            return new SoundMasterResponse
            {
                Sounds = SoundEntity.ReadAndBuildAll().Select(pl => pl.ToResponseData()).ToArray(),
            };
        }
    }
}
