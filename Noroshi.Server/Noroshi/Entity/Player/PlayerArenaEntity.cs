using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response;
using ResponseArena = Noroshi.Core.WebApi.Response.Arena;
using ResponsePlayers = Noroshi.Core.WebApi.Response.Players;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Services.Player;
using Noroshi.Server.Daos.Rdb.Arena;
using Noroshi.Core.Game.Arena;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerArenaSchema;

namespace Noroshi.Server.Entity.Player
{
    public class PlayerArenaEntity : AbstractDaoWrapperEntity<PlayerArenaEntity, PlayerArenaDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<PlayerArenaEntity> ReadAndBuildMulti(IEnumerable<uint> playerIds)
        {
            return ReadAndBuildMulti(playerIds.Select(id => new Schema.PrimaryKey { PlayerID = id }));
        }

        public static PlayerArenaEntity ReadAndBuild(uint playerId, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new PlayerArenaDao()).ReadByPK(new Schema.PrimaryKey { PlayerID = playerId }, readType));
        }

        public static IEnumerable<PlayerArenaEntity> ReadAndBuildAll()
        {
            return (new PlayerArenaDao()).ReadAll().Select(r => _instantiate(r));
        }

        public static PlayerArenaEntity BuildOrReadAndBuild(uint playerId)
        {
            return _instantiate((new PlayerArenaDao()).BuildOrRead(playerId));
        }

        public static PlayerArenaEntity CreateOrReadAndBuild(uint playerId)
        {
            return _instantiate((new PlayerArenaDao()).CreateOrRead(playerId));
        }

        public static IEnumerable<PlayerArenaEntity> ReadByRankOverAndRowCountOtherSsearcher(uint rank, uint rowLimit, uint playerId)
        {
            return _instantiate((new PlayerArenaDao()).ReadByRankOverAndRowCountOtherPlayerId(rank, rowLimit, playerId));
        }
        public static IEnumerable<PlayerArenaEntity> ReadByRankUnderAndRowCountOtherSsearcher(uint rank, uint rowLimit, uint playerId)
        {
            return _instantiate((new PlayerArenaDao()).ReadByRankUnderAndRowCountOtherPlayerId(rank, rowLimit, playerId));
        }

        public static uint GetArenaLastRank => PlayerArenaRankEntity.GetLastRank();


        public static Dictionary<uint, IEnumerable<uint>> GetPlayerCharactersInDeckOrGetVirtualDeck(IEnumerable<uint> playerIds)
        {
            var deckMap = new Dictionary<uint, IEnumerable<uint>>();
            var entityMap = ReadAndBuildMulti(playerIds).ToDictionary(e => e.PlayerID);
            var noDeckPlayerIds = playerIds.Where(id => !entityMap.ContainsKey(id));
            var playerCharacters = noDeckPlayerIds.Count() > 0 ? PlayerCharacterEntity.ReadAndBuildMultiByPlayerIDs(noDeckPlayerIds) : new PlayerCharacterEntity[0];

            foreach (var kv in entityMap)
            {
                deckMap.Add(kv.Key, kv.Value.DeckPlayerCharacterIDs);
            }
            foreach (var grouping in playerCharacters.ToLookup(pc => pc.PlayerID))
            {
                deckMap.Add(grouping.Key, grouping.Select(pc => pc.ID).ToArray());
            }
            return deckMap;
        }

        public static IEnumerable<uint> GetCharacterIds(uint[] playerCharacterIds)
        {
            var idList = playerCharacterIds.Where(d => d != 0).Select(id => new PlayerCharacterDao.PrimaryKey { ID = id });
            var primaryKeys = idList as PlayerCharacterDao.PrimaryKey[] ?? idList.ToArray();
            return !primaryKeys.Any() ? new List<uint>() : PlayerCharacterEntity.ReadAndBuildMulti(primaryKeys).Select(e => e.ToResponseData().CharacterID);
        }


        public bool CanAttack()
        {

            // 本日の攻撃回数の状態.
            if ( PlayNum <= Constant.ARENA_DAILY_PLAY_LIMIT_NUM ) return false;

            // クールタイムの状態.
            if ( ContextContainer.GetContext().TimeHandler.UnixTime < _record.CoolTimeAt ) return false;


            return true;
        }


        public bool CanDefense()
        {
            // 前回　戦闘からの経過時間.
            if ( Constant.ARENA_LISTING_NEXT_BATTLE_SPAN.TotalSeconds + ContextContainer.GetContext().TimeHandler.UnixTime < BattleStartedAt) return false;

            return true;
        }

        public static PlayerArenaEntity ChangeDeck(uint playerId, uint[] deckCharacterIds)
        {
            var entity = ReadAndBuild(playerId);
            entity.ChangeDeck(deckCharacterIds);

            entity.Save();
            return entity;
        }

        public static PlayerArenaEntity IncrementPlayNum(uint playerId)
        {
            var entity = ReadAndBuild(playerId);
            entity.IncrementPlayNum();

            entity.Save();
            return entity;
        }

        public uint PlayerID => _record.PlayerID;
        public uint Rank => _record.Rank;
        public uint BestRank => _record.BestRank;
        public uint[] DeckCharacter => GetCharacterIds(new []
        {
            _record.DeckPlayerCharacterID1,
            _record.DeckPlayerCharacterID2,
            _record.DeckPlayerCharacterID3,
            _record.DeckPlayerCharacterID4,
            _record.DeckPlayerCharacterID5
        }).ToArray();

        public uint[] DeckPlayerCharacterIDs => (new[]{
            _record.DeckPlayerCharacterID1,
            _record.DeckPlayerCharacterID2,
            _record.DeckPlayerCharacterID3,
            _record.DeckPlayerCharacterID4,
            _record.DeckPlayerCharacterID5,
        }).Where(id => id > 0).ToArray();


        public uint Win => _record.Win;
        public uint Lose => _record.Lose;
        public uint DefenseWin => _record.DefenseWin;
        public uint DefenseLose => _record.DefenseLose;
        public uint AllHP => _record.AllHP;
        public uint AllStrength => _record.AllStrength;
        public uint PlayNum => (uint)(ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastBattledAt) ? 0 : _record.PlayNum);    // LastPlayResetAtは、ArenaBattleFinishで値が更新する.
        public uint PlayResetNum => (uint)(ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastPlayResetAt) ? 0 : _record.PlayResetNum);
        public uint CoolTimeAt => _record.CoolTimeAt;
        public uint CoolTimeResetNum => (uint)(ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastCoolTimeResetAt) ? 0 : _record.CoolTimeResetNum);

        public uint BattleStartedAt => _record.BattleStartedAt;

        // 課金回数カウンターを最後に使った時間 = リセットタイミング.
        // 最後にバトルした時間　日付をまたげば = リセット


        public PlayerCharacter[] DeckPlayerCharacterEntitys()
        {
            if (DeckCharacter.Length == 0)
            {
                return null;
            }
            var characterData = new PlayerCharacter[5];
            foreach (var index in Enumerable.Range(0, DeckCharacter.Length).Where(index => DeckCharacter[index] > 0))
            {
                characterData[index] = PlayerCharacterEntity.ReadAndBuildByPlayerIDAndChracterID(PlayerID, DeckCharacter[index]).ToResponseData();
            }
            return characterData;
        }


        public ResponseArena.PlayerArena ToResponseData()
        {
            return new ResponseArena.PlayerArena
            {
                Rank                = _record.Rank,
                BestRank            = _record.BestRank,
                Win                 = _record.Win,
                Lose                = _record.Lose,
                DefenseWin          = _record.DefenseWin,
                DefenseLose         = _record.DefenseLose,
                DeckCharacters      = DeckPlayerCharacterEntitys(),
                AllHP               = _record.AllHP,
                AllStrength         = _record.AllStrength,
                CoolTime            = _record.CoolTimeAt,
                PlayCount           = _record.PlayNum,
                PlayMaxCount        = Constant.ARENA_DAILY_PLAY_LIMIT_NUM,

                BattleStartedAt     = _record.BattleStartedAt,
                LastBattledAt       = _record.LastBattledAt,
                PlayResetNum        = _record.PlayResetNum,
                LastPlayResetAt     = _record.LastPlayResetAt,
                CoolTimeResetNum    = _record.CoolTimeResetNum,

            };
        }

        public ResponsePlayers.PlayerArenaOtherResponse ToOtherResponseData()
        {
            return new ResponsePlayers.PlayerArenaOtherResponse
            {
                Rank                = Rank,
                Win                 = Win,
                Lose                = Lose,
                DefenseWin          = DefenseWin,
                DefenseLose         = DefenseLose,
                DeckCharacters      = DeckPlayerCharacterEntitys(),
                AllHP               = AllHP,
                AllStrength         = AllStrength,
                OtherPlayerStatus   = PlayerStatusService.GetOhter(PlayerID)
            };
        }

        public ResponseArena.ArenaServiceResponse ToServiceResponseData(PlayerStatusEntity playerStatus)
        {
            return new ResponseArena.ArenaServiceResponse
            {
                PlayerStatus = playerStatus.ToResponseData(),
                PlayerArena  = ToResponseData(),
            };
        }

        public void IncrementPlayNum()
        {
            // 既に敷居値までカウントしている(異常).
            if (PlayNum <= Constant.ARENA_DAILY_PLAY_LIMIT_NUM) throw new SystemException(string.Join("\t", "Already ArenaPlayNum Daily Limit Value", _record.PlayerID, PlayNum));

            var record = _cloneRecord() as Schema.Record;
            if (record == null) throw new InvalidOperationException();

            record.PlayNum++;

            // クールタイム開始.
            if (PlayNum < Constant.ARENA_DAILY_PLAY_LIMIT_NUM)
                record.CoolTimeAt = (uint)(ContextContainer.GetContext().TimeHandler.UnixTime + Constant.ARENA_COOLTIME_AFTRE_BATTLE_SPAN.TotalSeconds);
            else
                record.CoolTimeAt = 0;  // 敷居値では回らない.

            record.LastBattledAt = ContextContainer.GetContext().TimeHandler.UnixTime;

            _changeLocalRecord(record);
        }

        public void ChangeDeck(uint[] deckCharacterIds)
        {
            var record = _cloneRecord() as Schema.Record;
            if (record == null) throw new InvalidOperationException();
            record.DeckPlayerCharacterID1 = deckCharacterIds.Length == 0 ? 0 : deckCharacterIds[0];
            record.DeckPlayerCharacterID2 = deckCharacterIds.Length <= 1 ? 0 : deckCharacterIds[1];
            record.DeckPlayerCharacterID3 = deckCharacterIds.Length <= 2 ? 0 : deckCharacterIds[2];
            record.DeckPlayerCharacterID4 = deckCharacterIds.Length <= 3 ? 0 : deckCharacterIds[3];
            record.DeckPlayerCharacterID5 = deckCharacterIds.Length <= 4 ? 0 : deckCharacterIds[4];

            // TODO:AllHP計算.

            // TODO:AllStrength.

            _changeLocalRecord(record);
        }

        public void ChangeRank(uint rank)
        {
            // 変化が無ければ無視する.
            if (Rank == rank) return;

            var record = _cloneRecord() as Schema.Record;
            if (record == null) throw new InvalidOperationException();

            record.Rank = rank;

            _changeLocalRecord(record);
        }

        public void ResetPlayNum()
        {
            var record = _cloneRecord();
            if (record == null) throw new InvalidOperationException();

            record.PlayNum = 0;
            record.CoolTimeAt = 0;
            record.PlayResetNum++;      // 傾斜用カウンター.
            record.LastPlayResetAt = ContextContainer.GetContext().TimeHandler.UnixTime;       //  利用日付の保存　日付またぎのリセットに利用.
            _changeLocalRecord(record);
        }


        public void ResetCoolTime()
        {
            var record = _cloneRecord();
            if (record == null) throw new InvalidOperationException();

            record.CoolTimeAt = 0;
            record.CoolTimeResetNum++;  // 傾斜用カウンター.
            record.LastCoolTimeResetAt = ContextContainer.GetContext().TimeHandler.UnixTime;    // 利用日付の保存　日付またぎのリセットに利用.
            _changeLocalRecord(record);
        }

        
        // todo 後で直す
        public void ResetTimeCheck()
        {
            //var updateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 5, 0, 0);
            //if (UpdatedAt >= updateTime || updateTime >= DateTime.Now)
            //    return;
            //AddPlayCount((ResetNum + 1) * 5 - PlayNum);
            //AddResetCount();
        }

    }
}