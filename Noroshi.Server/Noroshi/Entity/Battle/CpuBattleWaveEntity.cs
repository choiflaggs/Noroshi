using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Battle;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CpuBattleWaveSchema;

namespace Noroshi.Server.Entity.Battle
{
    public class CpuBattleWaveEntity : AbstractDaoWrapperEntity<CpuBattleWaveEntity, CpuBattleWaveDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<CpuBattleWaveEntity> ReadAndBuildMultiByBattleIDs(IEnumerable<uint> battleIds)
        {
            return (new CpuBattleWaveDao()).ReadByBattleIDs(battleIds).Select(r => _instantiate(r));
        }


        Dictionary<uint, CpuCharacterEntity> _cpuCharacterMap;


        public uint BattleID => _record.BattleID;
        public byte No => _record.No;

        public int GetCpuCharacterNum()
        {
            return CpuCharacterIDs.Count();
        }

        public IEnumerable<uint> CpuCharacterIDs
        {
            get
            {
                return _getCpuCharacterIds().Where(id => id != default(uint));
            }
        }

        uint[] _getCpuCharacterIds()
        {
            return new uint[] {
                _record.CpuCharacterID1,
                _record.CpuCharacterID2,
                _record.CpuCharacterID3,
                _record.CpuCharacterID4,
                _record.CpuCharacterID5,
            };
        }

        public IEnumerable<CpuCharacterEntity> GetCpuCharacters()
        {
            return CpuCharacterIDs.Select(id => _cpuCharacterMap[id]);
        }
        public void SetCpuCharacterMap(Dictionary<uint, CpuCharacterEntity> cpuCharacterMap)
        {
            _cpuCharacterMap = cpuCharacterMap;
        }

        uint _getCpuCharacterIdByNo(int no)
        {
            return _getCpuCharacterIds()[no - 1];
        }
        CpuCharacterEntity _getCpuCharacterByNo(int no)
        {
            var cpuCharacterId = _getCpuCharacterIdByNo(no);
            return _cpuCharacterMap.ContainsKey(cpuCharacterId) ? _cpuCharacterMap[cpuCharacterId] : null;
        }

        /// <summary>
        /// バトル初期状態適用。
        /// </summary>
        /// <param name="enemyCharacterConditions">バトル初期状態</param>
        public void ApplyInitialCondition(InitialCondition.CharacterCondition[] enemyCharacterConditions)
        {
            for (var i = 0; i < enemyCharacterConditions.Length; i++)
            {
                _getCpuCharacterByNo(i + 1).SetInitialHPAndEnergy(enemyCharacterConditions[i].HP, enemyCharacterConditions[i].Energy);
            }
        }


        public Core.WebApi.Response.Battle.BattleWave ToResponseData()
        {
            return new Core.WebApi.Response.Battle.BattleWave()
            {
                BattleCharacter1 = _getCpuCharacterResponseData(1),
                BattleCharacter2 = _getCpuCharacterResponseData(2),
                BattleCharacter3 = _getCpuCharacterResponseData(3),
                BattleCharacter4 = _getCpuCharacterResponseData(4),
                BattleCharacter5 = _getCpuCharacterResponseData(5),
            };
        }
        Core.WebApi.Response.Battle.BattleCharacter _getCpuCharacterResponseData(int no)
        {
            var cpuCharacter = _getCpuCharacterByNo(no);
            return cpuCharacter != null ? cpuCharacter.ToResponseData() : null;
        }
    }
}
