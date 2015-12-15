using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Entity.Possession
{
    public class CharacterHandler : AbstractPossessionHandler<Character>
    {
        const uint MAX_NUM = 1;

        Dictionary<uint, PlayerCharacterEntity> _characterIdToPlayerCharacterMap;
        Dictionary<uint, CharacterEntity> _characterMap;

        public CharacterHandler(uint playerId, IEnumerable<uint> candidateCharacterIds) : base(playerId, candidateCharacterIds)
        {
        }

        public override PossessionCategory PossessionCategory => PossessionCategory.Character;

        protected override void _load(IEnumerable<uint> candidateIds, bool beforeUpdate)
        {
            // とりあえず Insert する作戦が使えないのでロックはかけない。
            // ここでロックを掛けない場合、ないと思って付与を試みるが実は既に付与されていた、ということがあり得るが、
            // _add() 時に検知して return false する方針でお茶を濁す。
            var readType = beforeUpdate ? ReadType.Master : ReadType.Slave;
            _characterMap = CharacterEntity.ReadAndBuildMulti(candidateIds).ToDictionary(item => item.ID);
            _characterIdToPlayerCharacterMap = PlayerCharacterEntity.ReadAndBuildMultiByPlayerIDAndChracterIDs(PlayerID, candidateIds, readType).ToDictionary(pc => pc.CharacterID);
        }

        public override IPossessionObject GetPossessionObject(uint characterId, uint num)
        {
            return _characterMap.ContainsKey(characterId) ? new Character(_characterMap[characterId], GetCurrentNum(characterId)) : null;
        }

        public override uint GetCurrentNum(uint characterId)
        {
            return (uint)(_characterIdToPlayerCharacterMap.ContainsKey(characterId) ? 1 : 0);
        }
        public override uint GetMaxNum(uint characterId)
        {
            return MAX_NUM;
        }

        protected override bool _add(IEnumerable<IPossessionParam> possessionParams)
        {
            return possessionParams.Select(possessionParam => _characterMap[possessionParam.ID]).All(character => PlayerCharacterEntity.Create(PlayerID, character.ID, character.InitialEvolutionLevel) != null);
        }

        protected override bool _remove(IEnumerable<IPossessionParam> possessableParams)
        {
            return false;
        }
    }
}
