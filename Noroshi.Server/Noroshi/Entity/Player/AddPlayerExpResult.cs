using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.GameContent;
using Noroshi.Server.Entity.Story;

namespace Noroshi.Server.Entity.Player
{
    public class AddPlayerExpResult
    {
        public readonly ushort PreviousPlayerLevel;
        public readonly ushort CurrentPlayerLevel;
        public readonly ushort PreviousMaxStamina;
        public readonly ushort CurrentMaxStamina;
        IEnumerable<GameContent> _openGameContents;

        public AddPlayerExpResult(ushort previousPlayerLevel, ushort currentPlayerLevel, ushort previousMaxStamina, ushort currentMaxStamina)
        {
            PreviousPlayerLevel = previousPlayerLevel;
            CurrentPlayerLevel = currentPlayerLevel;
            PreviousMaxStamina = previousMaxStamina;
            CurrentMaxStamina = currentMaxStamina;
            if (LevelUp)
            {
                _openGameContents = GameContent.BuildOpenGameContentsByPlayerLevel(PreviousPlayerLevel, CurrentPlayerLevel);
            }
            else
            {
                _openGameContents = new GameContent[0];
            }
        }

        public bool LevelUp => CurrentPlayerLevel > PreviousPlayerLevel;

        public IEnumerable<GameContent> GetOpenGameContents()
        {
            return _openGameContents;
        }
        public IEnumerable<StoryChapterEntity> GetOpenChapters()
        {
            return new StoryChapterEntity[0];
        }
        public IEnumerable<StoryEpisodeEntity> GetOpenEpisodes()
        {
            return new StoryEpisodeEntity[0];
        }

        public Core.WebApi.Response.Players.AddPlayerExpResult ToResponseData()
        {
            return new Core.WebApi.Response.Players.AddPlayerExpResult
            {
                PreviousPlayerLevel = PreviousPlayerLevel,
                CurrentPlayerLevel = CurrentPlayerLevel,
                PreviousMaxStamina = PreviousMaxStamina,
                CurrentMaxStamina = CurrentMaxStamina,
                LevelUp = LevelUp,
                OpenGameContentIDs = GetOpenGameContents().Select(gc => gc.ID).ToArray(),
            };
        }
    }
}
