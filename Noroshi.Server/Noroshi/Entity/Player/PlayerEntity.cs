using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerSchema;

namespace Noroshi.Server.Entity.Player
{
    /// <summary>
    /// プレイヤー情報クラス。
    /// </summary>
    public class PlayerEntity : AbstractDaoWrapperEntity<PlayerEntity, PlayerDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// プレイヤー ID で参照してビルドする。
        /// </summary>
        /// <param name="id">プレイヤー ID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static PlayerEntity ReadAndBuild(uint id, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuild(new Schema.PrimaryKey { ID = id }, readType);
        }
        /// <summary>
        /// セッション ID で参照してビルドする。ログイン後の簡易認証に利用。
        /// </summary>
        /// <param name="sessionId">セッション ID</param>
        /// <returns></returns>
        public static PlayerEntity ReadAndBuildBySessionID(string sessionId)
        {
            return _instantiate((new PlayerDao()).ReadBySessionID(sessionId));
        }
        /// <summary>
        /// UDID で参照してビルドする。ログイン時に利用。
        /// </summary>
        /// <param name="udid">UDID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static PlayerEntity ReadAndBuildByUDID(string udid, ReadType readType = ReadType.Slave)
        {
            return _instantiate((new PlayerDao()).ReadByUDID(udid, readType));
        }

        /// <summary>
        /// プレイヤー作成。
        /// </summary>
        /// <param name="udid">UDID</param>
        /// <param name="sessionId">初回セッションID</param>
        /// <param name="shardId">所属シャードID</param>
        /// <returns></returns>
        public static PlayerEntity Create(string udid, string sessionId, uint shardId)
        {
            return _instantiate((new PlayerDao()).Create(udid, sessionId, shardId));
        }


        /// <summary>
        /// プレイヤーID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// 所属シャードID。
        /// </summary>
        public uint ShardID => _record.ShardID;
        /// <summary>
        /// セッションID。
        /// </summary>
        public string SessionID => _record.SessionID;
        /// <summary>
        /// 端末識別 ID。
        /// </summary>
        public string UDID => _record.UDID;


        /// <summary>
        /// セッションIDをセットする。
        /// </summary>
        /// <param name="sessionId">新セッションID</param>
        public void SetSessionID(string sessionId)
        {
            var record = _cloneRecord();
            record.SessionID = sessionId;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// 端末識別 ID をセットする。
        /// </summary>
        /// <param name="udid">新端末識別 ID</param>
        public void SetUDID(string udid)
        {
            var record = _cloneRecord();
            record.UDID = udid;
            _changeLocalRecord(record);
        }

        /// <summary>
        /// どの端末にも紐付かないであろうプレイヤーへ変換する（デバッグ専用メソッド）。
        /// </summary>
        public void ChangeToDummyPlayer()
        {
            var record = _cloneRecord();
            record.UDID = $"DUMMY-{ID}";
            record.SessionID = $"DUMMY-{ID}";
            _changeLocalRecord(record);
        }
    }
}
