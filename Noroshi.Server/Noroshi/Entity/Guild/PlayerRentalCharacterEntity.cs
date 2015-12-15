using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Guild;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Guild;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerRentalCharacterSchema;

namespace Noroshi.Server.Entity.Guild
{
    /// <summary>
    /// 傭兵状態を扱うクラス。
    /// </summary>
    public class PlayerRentalCharacterEntity : AbstractDaoWrapperEntity<PlayerRentalCharacterEntity, PlayerRentalCharacterDao, Schema.PrimaryKey, Schema.Record>
    {
        public static PlayerRentalCharacterEntity ReadAndBuild(uint playerId, byte no, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(playerId, new byte[] { no }, readType).FirstOrDefault();
        }
        public static IEnumerable<PlayerRentalCharacterEntity> ReadAndBuildMulti(uint playerId, IEnumerable<byte> nos, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(nos.Select(no => new Schema.PrimaryKey { PlayerID = playerId, No = no }), readType);
        }
        public static IEnumerable<PlayerRentalCharacterEntity> ReadAndBuildByPlayerStatus(PlayerStatusEntity playerStatus, ReadType readType)
        {
            return ReadAndBuildMulti(playerStatus.PlayerID, _getAvailablePlayerRentalCharacterNos(playerStatus), readType);
        }

        public static IEnumerable<PlayerRentalCharacterEntity> ReadAndBuildActiveByPlayerID(uint playerId)
        {
            return ReadAndBuildActiveByPlayerIDs(new uint[] { playerId });
        }
        public static IEnumerable<PlayerRentalCharacterEntity> ReadAndBuildActiveByPlayerIDs(IEnumerable<uint> playerIds)
        {
            return _instantiate((new PlayerRentalCharacterDao()).ReadByPlayerIDs(playerIds)).Where(entity => entity.IsActive());
        }

        public static PlayerRentalCharacterEntity ReadAndBuildByPlayerCharacterID(uint playerCharacterId, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildByPlayerCharacterIDs(new[] { playerCharacterId }, readType).FirstOrDefault();
        }
        public static IEnumerable<PlayerRentalCharacterEntity> ReadAndBuildByPlayerCharacterIDs(IEnumerable<uint> playerCharacterIds, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new PlayerRentalCharacterDao()).ReadByPlayerCharacterIDs(playerCharacterIds, readType));
        }

        /// <summary>
        /// 傭兵レコード作成（傭兵派遣）可否を確認。
        /// </summary>
        /// <param name="playerStatus">派遣主プレイヤー状態</param>
        /// <param name="no">傭兵番号</param>
        /// <param name="playerCharacter">派遣されるプレイヤーキャラクター</param>
        /// <returns></returns>
        public static bool CanCreate(PlayerStatusEntity playerStatus, byte no, PlayerCharacterEntity playerCharacter)
        {
            // 番号チェック。
            if (no < Constant.MIN_PLAYER_RENTAL_CHARACTER_NO || _getMaxPlayerRentalCharacterNo(playerStatus) < no)
            {
                throw new ArgumentOutOfRangeException(string.Join("\t", "Invalid Player Rental Character No", playerStatus.PlayerID, no));
            }
            // 所有者チェック。
            if (playerStatus.PlayerID != playerCharacter.PlayerID)
            {
                throw new InvalidOperationException(string.Join("\t", "No Authority", playerStatus.PlayerID, playerCharacter.ID));
            }
            return true;
        }
        /// <summary>
        /// 傭兵レコード作成。派遣と同義。
        /// </summary>
        /// <param name="playerStatus">派遣主プレイヤー状態</param>
        /// <param name="no">傭兵番号</param>
        /// <param name="playerCharacter">派遣されるプレイヤーキャラクター</param>
        /// <returns></returns>
        public static PlayerRentalCharacterEntity Create(PlayerStatusEntity playerStatus, byte no, PlayerCharacterEntity playerCharacter)
        {
            if (!CanCreate(playerStatus, no, playerCharacter))
            {
                throw new InvalidOperationException(string.Join("\t", "Cannot Create", playerStatus.PlayerID, no, playerCharacter.ID));
            }
            return _instantiate((new PlayerRentalCharacterDao()).Create(playerStatus.PlayerID, no, playerCharacter.ID));
        }
        static byte[] _getAvailablePlayerRentalCharacterNos(PlayerStatusEntity playerStatus)
        {
            var count = _getMaxPlayerRentalCharacterNo(playerStatus) - Constant.MIN_PLAYER_RENTAL_CHARACTER_NO + 1;
            return Enumerable.Range(Constant.MIN_PLAYER_RENTAL_CHARACTER_NO, count).Select(no => (byte)no).ToArray();
        }
        static byte _getMaxPlayerRentalCharacterNo(PlayerStatusEntity playerStatus)
        {
            return PlayerVipLevelBonusEntity.ReadAndBuild(playerStatus.VipLevel).MaxRentalCharacterNum;
        }
        /// <summary>
        /// 傭兵キャラクターの雇用数インクリメントを試みる。
        /// </summary>
        /// <param name="playerCharacterId">対象傭兵プレイヤーキャラクターID</param>
        /// <returns></returns>
        public static void TryToIncrementRentalNum(uint playerCharacterId)
        {
            // 事前に SLAVE チェックが済んでいる前提。SLAVE チェックをすり抜けたギャップロックは許容する。
            var entity = ReadAndBuildByPlayerCharacterID(playerCharacterId, ReadType.Lock);
            if (entity == null) return;
            entity.IncrementRentalNum();
            if (!entity.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Update", entity.PlayerID, entity.No, entity.PlayerCharacterID, entity.RentalNum));
            }
        }

        public uint PlayerID => _record.PlayerID;
        public byte No => _record.No;
        public uint PlayerCharacterID => _record.PlayerCharacterID;
        /// <summary>
        /// 派遣期間。
        /// </summary>
        public TimeSpan RegisterTime => TimeSpan.FromSeconds(ContextContainer.GetContext().TimeHandler.UnixTime - _record.CreatedAt);
        /// <summary>
        /// 雇用回数。
        /// </summary>
        public uint RentalNum => _record.RentalNum;

        /// <summary>
        /// 全傭兵報酬を取得。
        /// </summary>
        /// <param name="playerStatus">報酬受け取りプレイヤー状態</param>
        /// <param name="characterLevel">派遣キャラクターのレベル</param>
        /// <returns></returns>
        public List<PossessionParam> GetAllRewards(PlayerStatusEntity playerStatus, ushort characterLevel)
        {
            var possessionParams = new List<PossessionParam>();
            if (GetFixedRewards(characterLevel).Count() > 0) possessionParams.AddRange(GetFixedRewards(characterLevel));
            if (GetRentalRewards(playerStatus, characterLevel).Count() > 0) possessionParams.AddRange(GetRentalRewards(playerStatus, characterLevel));
            return possessionParams;
        }
        /// <summary>
        /// 傭兵最低報酬を取得。
        /// </summary>
        /// <param name="characterLevel">派遣キャラクターのレベル</param>
        /// <returns></returns>
        public PossessionParam[] GetFixedRewards(ushort characterLevel)
        {
            return new[]
            {
                PossessionManager.GetGoldParam((uint)(Constant.RENTAL_FIXED_REWARD_GOLD_PER_CHARACTER_LEVEL * characterLevel)),
                PossessionManager.GetFreeGemParam(Constant.RENTAL_FIXED_REWARD_GEM),
            };
        }
        /// <summary>
        /// 傭兵雇用報酬を取得。
        /// </summary>
        /// <param name="playerStatus">報酬受け取りプレイヤー状態</param>
        /// <param name="characterLevel">派遣キャラクターのレベル</param>
        /// <returns></returns>
        public PossessionParam[] GetRentalRewards(PlayerStatusEntity playerStatus, ushort characterLevel)
        {
            var rentalNum = (byte)Math.Min(RentalNum, (Constant.MAX_RECEIVABLE_RENTAL_REWARD_NUM - playerStatus.RentalRewardReceivingNum));
            if (rentalNum == 0) return new PossessionParam[0];
            return new[]
            {
                PossessionManager.GetGoldParam((uint)Constant.RENTAL_REWARD_GOLD_PER_CHARACTER_LEVEL * characterLevel * rentalNum),
                PossessionManager.GetFreeGemParam((byte)(Constant.RENTAL_REWARD_GEM * rentalNum)),
            };
        }

        /// <summary>
        /// アクティブ判定。
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return RegisterTime < Constant.RENTAL_SPAN;
        }
        /// <summary>
        /// 派遣解除可否判定。
        /// </summary>
        /// <returns></returns>
        public bool CanRemove()
        {
            return !IsActive();
        }
        /// <summary>
        /// 派遣解除。
        /// </summary>
        /// <returns></returns>
        public bool Remove()
        {
            return Delete();
        }

        /// <summary>
        /// 雇用数インクリメント。
        /// </summary>
        public void IncrementRentalNum()
        {
            var record = _cloneRecord();
            record.RentalNum++;
            _changeLocalRecord(record);
        }

        public Core.WebApi.Response.Guild.RentalCharacter ToResponseData(PlayerCharacterEntity playerCharacter)
        {
            if (playerCharacter.ID != PlayerCharacterID) throw new ArgumentException();
            return new Core.WebApi.Response.Guild.RentalCharacter
            {
                No = No,
                PlayerCharacter = playerCharacter.ToResponseData(),
                CreatedAt = _record.CreatedAt,
            };
        }
    }
}
