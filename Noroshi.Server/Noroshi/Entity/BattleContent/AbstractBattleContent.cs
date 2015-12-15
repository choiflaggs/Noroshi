using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Battle;
using Noroshi.Core.Game.Enums;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Possession;

namespace Noroshi.Server.Entity.BattleContent
{
    public abstract class AbstractBattleContent : IBattleContent
    {
        public abstract bool CanBattle(PlayerStatusEntity playerStatus, uint paymentNum);
        public abstract void FinishBattle(VictoryOrDefeat victoryOrDefeat, byte rank, BattleResult result, PlayerStatusEntity playerStatus, IEnumerable<PlayerCharacterEntity> ownPlayerCharacters, GuildEntity guild);
        public abstract uint GetBattleID();

        public virtual BattleAutoMode GetBattleAutoMode()
        {
            return BattleAutoMode.Selectable;
        }
        public virtual bool IsLoopBattle()
        {
            return false;
        }

        public virtual bool IsValidCharacters(IEnumerable<CharacterEntity> characters)
        {
            var decaCharacters = characters.Where(character => character.TagSet.IsDeca);
            return decaCharacters.Count() <= Constant.MAX_DECA_CHARACTER_NUM_PER_FORCE;
        }

        public virtual InitialCondition GetInitialCondition(uint[] playerCharacterIds, uint paymentNum)
        {
            return null;
        }

        public virtual ushort GetPlayerExp()
        {
            return 0;
        }
        public virtual AdditionalInformation GetAdditionalInformation()
        {
            return null;
        }

        public virtual IEnumerable<CpuCharacterEntity> GetOwnCpuCharacters()
        {
            return new CpuCharacterEntity[0];
        }

        public virtual IEnumerable<PossessionParam> GetRewards(PlayerStatusEntity playerStatus, VictoryOrDefeat victoryOrDefeat)
        {
            return new PossessionParam[0];
        }

        public virtual void PreProcess()
        {
        }
    }
}
