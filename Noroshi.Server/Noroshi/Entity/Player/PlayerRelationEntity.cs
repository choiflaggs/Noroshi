using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Guild;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerRelationSchema;

namespace Noroshi.Server.Entity.Player
{
    /// <summary>
    /// プレイヤー間と関係を表現するクラス。
    /// </summary>
    public class PlayerRelationEntity : AbstractDaoWrapperEntity<PlayerRelationEntity, PlayerRelationDao, Schema.PrimaryKey, Schema.Record>
    {
        public static PlayerRelationEntity ReadOrDefaultAndBuild(uint playerId, uint targetPlayerId)
        {
            return ReadOrDefaultAndBuildMulti(playerId, new[] { targetPlayerId }).First();
        }
        public static IEnumerable<PlayerRelationEntity> ReadOrDefaultAndBuildMulti(uint playerId, IEnumerable<uint> targetPlayerIds)
        {
            return _instantiate((new PlayerRelationDao()).ReadOrDefaultMulti(playerId, targetPlayerIds));
        }

        public static PlayerRelationEntity CreateOrReadAndBuild(uint playerId, uint targetPlayerId)
        {
            return _instantiate((new PlayerRelationDao()).CreateOrRead(playerId, targetPlayerId));
        }


        /// <summary>
        /// 対象プレイヤー ID。
        /// </summary>
        public uint TargetPlayerID => _record.TargetPlayerID;
        /// <summary>
        /// 最後に挨拶した日時。
        /// </summary>
        public uint LastGreetingAt => _record.LastGreetingAt;

        /// <summary>
        /// 挨拶する側の報酬を取得する。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PossessionParam> GetGreetingRewards()
        {
            return new[] { PossessionManager.GetBPParam(Constant.BP_PER_GREETING) };
        }

        /// <summary>
        /// 挨拶可否チェック。
        /// </summary>
        /// <param name="greeting">挨拶する側のプレイヤー状態</param>
        /// <param name="greeted">挨拶される側のプレイヤー状態</param>
        /// <param name="guild">所属ギルド</param>
        /// <param name="lastGreetingRewardReceivedAt">挨拶される側の前回報酬受け取り日時</param>
        /// <returns></returns>
        public bool CanGreet(PlayerStatusEntity greeting, PlayerStatusEntity greeted, GuildEntity guild, uint? lastGreetingRewardReceivedAt)
        {
            if (greeting == null || greeted == null || guild == null) throw new InvalidOperationException();
            // 同一プレイヤーチェック。
            if (greeting.PlayerID == greeted.PlayerID) return false;
            // 同じギルド所属チェック。
            if (!greeting.GuildID.HasValue || !greeted.GuildID.HasValue) return false;
            if (greeting.GuildID.Value != guild.ID || greeted.GuildID.Value != guild.ID) return false;
            // 挨拶数上限チェック。
            if (guild.GetMaxGreetingNum(greeting.VipLevel) <= greeting.GreetingNum) return false;
            // 相手が過去挨拶を確認済みかチェック。
            if (lastGreetingRewardReceivedAt.HasValue && lastGreetingRewardReceivedAt <= LastGreetingAt) return false;
            // 前回挨拶がリセット済みかチェック。
            if (!ContextContainer.GetContext().TimeHandler.HasAlreadyReset(LastGreetingAt)) return false;

            return true;
        }

        /// <summary>
        /// 挨拶実行。
        /// </summary>
        /// <param name="greeting">挨拶する側のプレイヤー状態</param>
        public void Greet(PlayerStatusEntity greeting)
        {
            var record = _cloneRecord();
            record.LastGreetingAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
            // 挨拶する側の挨拶数をインクリメントしておく。
            greeting.IncrementGreetingNum();
        }
    }
}
