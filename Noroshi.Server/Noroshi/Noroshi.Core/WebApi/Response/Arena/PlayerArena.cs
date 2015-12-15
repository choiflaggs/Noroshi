using System;

namespace Noroshi.Core.WebApi.Response.Arena
{
    public class PlayerArena
    {
        public uint Rank { get; set; }
        public uint BestRank { get; set; }
        public PlayerCharacter[] DeckCharacters { get; set; }
        public uint Win { get; set; }
        public uint Lose { get; set; }
        public uint DefenseWin { get; set; }
        public uint DefenseLose { get; set; }
        public uint AllHP { get; set; }
        public uint AllStrength { get; set; }
        public uint PlayCount { get; set; }
        public uint CoolTime { get; set; }
        public uint PlayMaxCount { get; set; }

        public uint BattleStartedAt { get; set; }
        public uint LastBattledAt { get; set; }
        public uint PlayResetNum { get; set; }
        public uint LastPlayResetAt { get; set; }
        public uint CoolTimeAt { get; set; }
        public uint CoolTimeResetNum { get; set; }
        public uint LastCoolTimeResetAt { get; set; }


    }
}