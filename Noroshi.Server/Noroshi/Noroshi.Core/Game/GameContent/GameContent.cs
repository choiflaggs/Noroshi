using System;
using System.Collections.Generic;
using System.Linq;

namespace Noroshi.Core.Game.GameContent
{
    public class GameContent
    {
        const string TEXT_KEY_PREFIX = "Game.GameContent.";
        static readonly Dictionary<GameContentID, ushort> OPEN_PLAYER_LEVEL_MAP = new Dictionary<GameContentID, ushort>
        {
            { GameContentID.Arena, 10 },
            { GameContentID.Training, 14 },
            { GameContentID.Trial, 14 },
            { GameContentID.BeginnerGuild, 15 },
            { GameContentID.Expedition, 20 },
            { GameContentID.NormalGuild, 20 },
            { GameContentID.DefensiveWar, 25 },
        };

        public static IEnumerable<GameContent> BuildAll()
        {
            var gameContents = new List<GameContent>();
            foreach (GameContentID gameContentId in Enum.GetValues(typeof(GameContentID)))
            {
                gameContents.Add(new GameContent(gameContentId));
            }
            return gameContents;
        }
        public static GameContent Build(uint id)
        {
            return BuildAll().Where(gc => gc.ID == id).FirstOrDefault();
        }
        public static IEnumerable<GameContent> BuildMulti(IEnumerable<uint> ids)
        {
            var map = BuildAll().ToDictionary(gc => gc.ID);
            return ids.Select(id => map[id]);
        }
        public static IEnumerable<GameContent> BuildOpenGameContentsByPlayerLevel(ushort currentPlayerLevel)
        {
            return BuildAll().Where(gc => !gc.OpenPlayerLevel.HasValue || gc.OpenPlayerLevel.Value <= currentPlayerLevel);
        }
        public static IEnumerable<GameContent> BuildOpenGameContentsByPlayerLevel(ushort previousPlayerLevel, ushort currentPlayerLevel)
        {
            return BuildAll().Where(gc => gc.OpenPlayerLevel.HasValue && previousPlayerLevel < gc.OpenPlayerLevel.Value && gc.OpenPlayerLevel.Value <= currentPlayerLevel);
        }

        public static bool IsOpen(GameContentID gameContentId, ushort playerLevel)
        {
            return OPEN_PLAYER_LEVEL_MAP[gameContentId] <= playerLevel;
        }

        public static ushort GetNormalGuildOpenPlayerLevel()
        {
            return OPEN_PLAYER_LEVEL_MAP[GameContentID.NormalGuild];
        }

        public uint ID { get; private set; }
        public string TextKey { get; private set; }
        public ushort? OpenPlayerLevel { get; private set; }

        GameContent(GameContentID gameContentId)
        {
            ID = (uint)gameContentId;
            TextKey = TEXT_KEY_PREFIX + gameContentId.ToString();
            OpenPlayerLevel = OPEN_PLAYER_LEVEL_MAP.ContainsKey(gameContentId) ? (ushort?)OPEN_PLAYER_LEVEL_MAP[gameContentId] : null;
        }
    }
}
