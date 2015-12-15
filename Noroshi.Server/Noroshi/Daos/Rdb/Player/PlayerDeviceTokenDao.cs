using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Player;
using Noroshi.Server.Contexts;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerDeviceTokenSchema;

namespace Noroshi.Server.Daos.Rdb.Player
{
    public class PlayerDeviceTokenDao : AbstractShardDBDao<Schema.PrimaryKey, Schema.Record>
    {
        protected override string _tableName => Schema.TableName;

        /// <summary>
        /// 複数のプレイヤー ID で参照する。
        /// </summary>
        /// <param name="playerIds">対象プレイヤー ID</param>
        /// <returns></returns>
        public IEnumerable<Schema.Record> ReadByPlayerIDs(IEnumerable<uint> playerIds)
        {
            return _select("PlayerID IN @PlayerIDs", new { PlayerIDs = playerIds });
        }

        /// <summary>
        /// （ユニーク制約のかかっている）デバイストークン種別とトークンで参照する。
        /// </summary>
        /// <param name="type">対象デバイストークン種別</param>
        /// <param name="tokens">トークン</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public IEnumerable<Schema.Record> ReadByTypeAndTokens(DeviceTokenType type, IEnumerable<string> tokens, ReadType readType)
        {
            return _select("Type = @Type AND Token IN @Tokens", new { Type = (byte)type, Tokens = tokens }, readType);
        }

        /// <summary>
        /// レコード作成を試行し、失敗（既にレコードが存在）したらロックを掛けて参照する。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="type">対象デバイストークン種別</param>
        /// <returns></returns>
        public Schema.Record CreateOrRead(uint playerId, DeviceTokenType type)
        {
            return Create(_getDefaultRecord(playerId, type)) ?? ReadByPK(new Schema.PrimaryKey { PlayerID = playerId, Type = (byte)type }, ReadType.Lock);
        }

        /// <summary>
        /// デフォルトレコード内容を取得する。
        /// </summary>
        /// <param name="playerId">対象プレイヤー ID</param>
        /// <param name="type">対象デバイストークン種別</param>
        /// <returns></returns>
        Schema.Record _getDefaultRecord(uint playerId, DeviceTokenType type)
        {
            return new Schema.Record
            {
                PlayerID = playerId,
                Type = (byte)type,
                Status = (byte)DeviceTokenStatus.Normal,
                Token = "",
                CreatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
            };
        }
    }
}
