using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Battle;
using CharacterConstant = Noroshi.Core.Game.Character.Constant;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;
using Noroshi.Server.Entity.Quest;
using Noroshi.Server.Entity.Battle;

namespace Noroshi.Server.Entity.Character
{
    public class PlayerCharacterEntity : AbstractDaoWrapperEntity<PlayerCharacterEntity, PlayerCharacterDao, PlayerCharacterDao.PrimaryKey, PlayerCharacterDao.Record>, IBattleCharacter
    {

        public static PlayerCharacterEntity ReadAndBuild(uint id, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(new[] { id }, readType).FirstOrDefault();
        }
        public static IEnumerable<PlayerCharacterEntity> ReadAndBuildMulti(IEnumerable<uint> ids, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(ids.Select(id => new PlayerCharacterDao.PrimaryKey { ID = id }), readType);
        }

        public static IEnumerable<PlayerCharacterEntity> ReadAndBuildMultiByPlayerID(uint playerId, ReadType readType = ReadType.Slave)
        {
            var dao = new PlayerCharacterDao();
            return _instantiate(dao.ReadByPlayerID(playerId));
        }

        public static PlayerCharacterEntity ReadAndBuildByPlayerIDAndChracterID(uint playerId, uint characterId, ReadType readType = ReadType.Slave)
        {
            var dao = new PlayerCharacterDao();
            var playerCharacterData = dao.ReadByPlayerIDAndChracterID(playerId, characterId, readType);
            return _instantiate(playerCharacterData);
        }
        public static IEnumerable<PlayerCharacterEntity> ReadAndBuildMultiByPlayerIDAndChracterIDs(uint playerId, IEnumerable<uint> characterIds, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new PlayerCharacterDao()).ReadByPlayerIDAndChracterIDs(playerId, characterIds, readType));
        }
        public static IEnumerable<PlayerCharacterEntity> ReadAndBuildMultiByPlayerIDs(IEnumerable<uint> playerIds)
        {
            return _instantiate(new PlayerCharacterDao().ReadByPlayerIDs(playerIds));
        }

        public static PlayerCharacterEntity Create(uint playerId, uint characterId, byte evolutionLevel)
        {
            var create  = _instantiate((new PlayerCharacterDao()).Create(playerId, characterId, evolutionLevel));
            // ミッション:キャラクター所持数
            if (create != null) QuestTriggerEntity.CountUpCharactersNum(playerId);
            return create;
        }
        public static PlayerCharacterEntity Create(uint playerId, uint characterId, byte evolutionLevel, uint shardId)
        {
            var create = _instantiate((new PlayerCharacterDao()).Create(playerId, characterId, evolutionLevel, shardId));
            // ミッション:キャラクター所持数
            if (create != null) QuestTriggerEntity.CountUpCharactersNum(playerId);
            return create;
        }

        public static bool CanCreateCharacter(uint playerId, uint characterId)
        {
            return new PlayerCharacterDao().ReadByPlayerIDAndChracterID(playerId, characterId, ReadType.Slave) == null;
        }

        public bool HasGear(byte index)
        {
            return GetPlayerCharacterGear().Any(gear => gear.GearPosition == index);
        }

        List<PlayerCharacterGearEntity> _playerCharacterGears = new List<PlayerCharacterGearEntity>();


        uint? _initialHp = null;
        ushort? _initialEnergy = null;
        float? _damageCoefficient = null;

        public void SetInitialCondition(uint? initialHp, ushort? initialEnergy, float? damageCoefficient)
        {
            _initialHp = initialHp;
            _initialEnergy = initialEnergy;
            _damageCoefficient = damageCoefficient;
        }


        public override bool Save()
        {
            if (0 < _levelUpNum) QuestTriggerEntity.CountUpCharacterLevelUpNum(_record.PlayerID, _levelUpNum);
            return base.Save();
        }




        public uint ID => _record.ID;
        public uint PlayerID => _record.PlayerID;
        public uint CharacterID => _record.CharacterID;
        public ushort Level => _record.Level;
        public uint Exp => _record.Exp;
        public byte PromotionLevel => _record.PromotionLevel;
        public byte EvolutionLevel => _record.EvolutionLevel;
        public ushort ActionLevel1 => _record.ActionLevel1;
        public ushort ActionLevel2 => _record.ActionLevel2;
        public ushort ActionLevel3 => _record.ActionLevel3;
        public ushort ActionLevel4 => _record.ActionLevel4;
        public ushort ActionLevel5 => _record.ActionLevel5;
        public uint GearID1 => Gear1?.GearID ?? 0;
        public uint GearID2 => Gear2?.GearID ?? 0;
        public uint GearID3 => Gear3?.GearID ?? 0;
        public uint GearID4 => Gear4?.GearID ?? 0;
        public uint GearID5 => Gear5?.GearID ?? 0;
        public uint GearID6 => Gear6?.GearID ?? 0;
        public PlayerCharacterGearEntity Gear1 => GetPlayerCharacterGear().FirstOrDefault(gear => gear.GearPosition == 1);
        public PlayerCharacterGearEntity Gear2 => GetPlayerCharacterGear().FirstOrDefault(gear => gear.GearPosition == 2);
        public PlayerCharacterGearEntity Gear3 => GetPlayerCharacterGear().FirstOrDefault(gear => gear.GearPosition == 3);
        public PlayerCharacterGearEntity Gear4 => GetPlayerCharacterGear().FirstOrDefault(gear => gear.GearPosition == 4);
        public PlayerCharacterGearEntity Gear5 => GetPlayerCharacterGear().FirstOrDefault(gear => gear.GearPosition == 5);
        public PlayerCharacterGearEntity Gear6 => GetPlayerCharacterGear().FirstOrDefault(gear => gear.GearPosition == 6);

        ushort _levelUpNum = 0;
        public IEnumerable<PlayerCharacterGearEntity> GetPlayerCharacterGear()
        {
            return _playerCharacterGears.Count == 0 ? (_playerCharacterGears = PlayerCharacterGearEntity.ReadAndBuildMulti(new[]
            {
                _record.Gear1, _record.Gear2, _record.Gear3, _record.Gear4, _record.Gear5, _record.Gear6
            }).ToList()) : _playerCharacterGears;
        }

        void _changePlayerCharacterGear(byte index, PlayerCharacterGearEntity playerCharacterGearEntity)
        {
            _playerCharacterGears.RemoveAll(gear => gear.GearPosition == index);
            if (playerCharacterGearEntity != null)
            {
                _playerCharacterGears.Add(playerCharacterGearEntity);
            }
        }

        public Core.WebApi.Response.PlayerCharacter ToResponseData()
        {
            return new Core.WebApi.Response.PlayerCharacter
            {
                ID = ID,
                CharacterID = CharacterID,
                Level = Level,
                Exp = Exp,
                ExpInLevel = _getExpInLevel(),
                PromotionLevel = PromotionLevel,
                EvolutionLevel = EvolutionLevel,
                ActionLevel1 = ActionLevel1,
                ActionLevel2 = ActionLevel2,
                ActionLevel3 = ActionLevel3,
                ActionLevel4 = ActionLevel4,
                ActionLevel5 = ActionLevel5,
                GearID1 = GearID1,
                GearID2 = GearID2,
                GearID3 = GearID3,
                GearID4 = GearID4,
                GearID5 = GearID5,
                GearID6 = GearID6,
                PlayerCharacterGears = GetPlayerCharacterGear().Select(e => e.ToResponseData()).ToArray()
            };
        }
        /// <summary>
        /// バトル時のレスポンス。
        /// </summary>
        /// <returns></returns>
        public Core.WebApi.Response.Battle.BattleCharacter ToBattleResponseData()
        {
            return new Core.WebApi.Response.Battle.BattleCharacter
            {
                Type = (byte)BattleCharacterType.Player,
                ID = ID,
                IsBoss = false,
                CharacterID = CharacterID,
                Level = Level,
                PromotionLevel = PromotionLevel,
                EvolutionLevel = EvolutionLevel,
                ActionLevel1 = ActionLevel1,
                ActionLevel2 = ActionLevel2,
                ActionLevel3 = ActionLevel3,
                ActionLevel4 = ActionLevel4,
                ActionLevel5 = ActionLevel5,
                GearID1 = GearID1,
                GearID2 = GearID2,
                GearID3 = GearID3,
                GearID4 = GearID4,
                GearID5 = GearID5,
                GearID6 = GearID6,
                InitialHP = _initialHp,
                InitialEnergy = _initialEnergy,
                DamageCoefficient = _damageCoefficient,
            };
        }
        uint _getExpInLevel()
        {
            return CharacterLevelEntity.GetExpInLevel(Level, Exp);
        }

        public void ChangePromotionLevel(byte level, byte maxLevel)
        {
            if (level <= 0 || level > maxLevel)
            {
                throw new InvalidOperationException();                
            }
            var record = _cloneRecord();
            record.PromotionLevel = level;

            _changeLocalRecord(record);
        }

        public void ChangeEvolutionLevel(byte level, byte maxLevel)
        {
            if (level <= 0 || level > maxLevel)
            {
                throw new InvalidOperationException();
            }
            var record = _cloneRecord();
            record.EvolutionLevel = level;

            _changeLocalRecord(record);
        }

        public byte PromotionRank => CharacterConstant.PROMOTION_RANK_MAP.OrderByDescending(kv => kv.Value).First(kv => kv.Value <= PromotionLevel).Key;


        public void RemoveAllEquipGear()
        {
            var record = _cloneRecord();
            record.Gear1 = 0;
            record.Gear2 = 0;
            record.Gear3 = 0;
            record.Gear4 = 0;
            record.Gear5 = 0;
            record.Gear6 = 0;
            _changeLocalRecord(record);
            _playerCharacterGears = new List<PlayerCharacterGearEntity>();
        }

        public void ChangeGear(byte index, PlayerCharacterGearEntity playerCharacterGearEntity)
        {
            var record = _cloneRecord();
            switch (index)
            {
                case 1:
                    if (record.Gear1 != 0 && playerCharacterGearEntity != null)
                    {
                        throw new InvalidOperationException();
                    }
                    record.Gear1 = playerCharacterGearEntity?.ID ?? 0;
                    break;
                case 2:
                    if (record.Gear2 != 0 && playerCharacterGearEntity != null)
                    {
                        throw new InvalidOperationException();
                    }
                    record.Gear2 = playerCharacterGearEntity?.ID ?? 0;
                    break;
                case 3:
                    if (record.Gear3 != 0 && playerCharacterGearEntity != null)
                    {
                        throw new InvalidOperationException();
                    }
                    record.Gear3 = playerCharacterGearEntity?.ID ?? 0;
                    break;
                case 4:
                    if (record.Gear4 != 0 && playerCharacterGearEntity != null)
                    {
                        throw new InvalidOperationException();
                    }
                    record.Gear4 = playerCharacterGearEntity?.ID ?? 0;
                    break;
                case 5:
                    if (record.Gear5 != 0 && playerCharacterGearEntity != null)
                    {
                        throw new InvalidOperationException();
                    }
                    record.Gear5 = playerCharacterGearEntity?.ID ?? 0;
                    break;
                case 6:
                    if (record.Gear6 != 0 && playerCharacterGearEntity != null)
                    {
                        throw new InvalidOperationException();
                    }
                    record.Gear6 = playerCharacterGearEntity?.ID ?? 0;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            _changePlayerCharacterGear(index, playerCharacterGearEntity);
            _changeLocalRecord(record);
        }

        public void ChangeActionLevel(uint index, ushort level)
        {
            if (level <= 0 || level > Level)
            {
                throw new InvalidOperationException();
            }
            var record = _cloneRecord();
            switch (index)
            {
                case 1:
                    record.ActionLevel1 = level;
                    break;
                case 2:
                    record.ActionLevel2 = level;
                    break;
                case 3:
                    record.ActionLevel3 = level;
                    break;
                case 4:
                    record.ActionLevel4 = level;
                    break;
                case 5:
                    record.ActionLevel5 = level;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            _changeLocalRecord(record);
        }

        /// <summary>
        /// 経験値追加。
        /// </summary>
        /// <param name="exp">追加経験値</param>
        /// <param name="playerLevel">所有プレイヤーレベル</param>
        /// <returns>実際の追加経験値</returns>
        public uint AddExp(uint exp, ushort playerLevel)
        {
            var maxExp = CharacterLevelEntity.GetMaxExpByLevel(playerLevel);
            var addedExp = Math.Min(maxExp - Exp, exp);
            if (addedExp > 0)
            {
                var previousLevel = Level;
                var newRecord = _cloneRecord();
                newRecord.Exp += addedExp;
                newRecord.Level = CharacterLevelEntity.GetLevel(newRecord.Exp);

                if (_levelUpNum != 0) throw new InvalidOperationException();
                _levelUpNum = (ushort)(Level - previousLevel);

                _changeLocalRecord(newRecord);
            }

            return addedExp;
        }
        /// <summary>
        /// 強制的に経験値をセットするメソッド。デバッグメニューでのみ利用可。
        /// </summary>
        /// <param name="exp">経験値</param>
        /// <param name="playerLevel">所有プレイヤーレベル</param>
        /// <returns></returns>
        public bool ForceSetExp(uint exp, ushort playerLevel)
        {
            var maxExp = CharacterLevelEntity.GetMaxExpByLevel(playerLevel);
            if (exp <= maxExp)
            {
                var newRecord = _cloneRecord();
                newRecord.Exp = exp;
                newRecord.Level = CharacterLevelEntity.GetLevel(newRecord.Exp);
                _changeLocalRecord(newRecord);
                return true;
            }
            return false;
        }
    }
}
