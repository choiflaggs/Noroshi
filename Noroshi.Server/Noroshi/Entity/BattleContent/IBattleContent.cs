using System.Collections.Generic;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Possession;

namespace Noroshi.Server.Entity.BattleContent
{
    public interface IBattleContent
    {
        uint GetBattleID();

        IEnumerable<CpuCharacterEntity> GetOwnCpuCharacters();
        InitialCondition GetInitialCondition(uint[] playerCharacterIds, uint paymentNum);
        AdditionalInformation GetAdditionalInformation();
        BattleAutoMode GetBattleAutoMode();
        bool IsLoopBattle();
        ushort GetPlayerExp();
        IEnumerable<PossessionParam> GetRewards(PlayerStatusEntity playerStatus, VictoryOrDefeat victoryOrDefeat);
        bool IsValidCharacters(IEnumerable<CharacterEntity> characters);
        bool CanBattle(PlayerStatusEntity playerStatus, uint paymentNum);
        void PreProcess();
        void FinishBattle(VictoryOrDefeat victoryOrDefeat, byte rank, BattleResult result, PlayerStatusEntity playerStatus, IEnumerable<PlayerCharacterEntity> ownPlayerCharacters, GuildEntity guild);
    }
}
