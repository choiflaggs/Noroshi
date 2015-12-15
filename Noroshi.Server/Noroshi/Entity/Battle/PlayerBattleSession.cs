using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Battle;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerBattleSessionSchema;

namespace Noroshi.Server.Entity.Battle
{
    public class PlayerBattleSession : AbstractDaoWrapperEntity<PlayerBattleSession, PlayerBattleSessionDao, Schema.PrimaryKey, Schema.Record>
    {
        public static PlayerBattleSession Create(uint playerId, IEnumerable<PlayerCharacterEntity> playerCharacters)
        {
            var playerCharacterIds = playerCharacters.Select(pc => pc.ID);
            var sessionId = _generateSessionId(playerId, playerCharacterIds);
            var entity = Create(new Schema.Record() {
                PlayerID = playerId,
                SessionID = sessionId,
                PreprocessData = "",
            });
            if (entity == null)
            {
                entity = ReadAndBuildByPlayerID(playerId, ReadType.Lock);
            }
            entity.SetSessionIDAndPlayerCharacterIDs(sessionId, playerCharacterIds);
            var a = entity.Save();
            return entity;
        }
        public static PlayerBattleSession ReadAndBuildByPlayerID(uint playerId, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuild(new Schema.PrimaryKey() { PlayerID = playerId }, readType);
        }

        static string _generateSessionId(uint playerId, IEnumerable<uint> playerCharacterIds)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] bs = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(playerId.ToString()));
            md5.Clear();
            var result = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                result.Append(b.ToString("x2"));
            }
            return result.ToString();
        }

        public uint[] PlayerCharacterIDs => new[]
        {
            _record.PlayerCharacterID1,
            _record.PlayerCharacterID2,
            _record.PlayerCharacterID3,
            _record.PlayerCharacterID4,
            _record.PlayerCharacterID5,
        }
        .Where(playerCharacterId => playerCharacterId > 0).ToArray();

        public void SetSessionIDAndPlayerCharacterIDs(string sessionId, IEnumerable<uint> playerCharacterIds)
        {
            var newRecord = _cloneRecord();

            var newPlayerCharacterIds = new uint[5];
            var i = 0;
            foreach (var id in playerCharacterIds)
            {
                newPlayerCharacterIds[i++] = id;
            }
            newRecord.SessionID = sessionId;
            newRecord.PlayerCharacterID1 = newPlayerCharacterIds[0];
            newRecord.PlayerCharacterID2 = newPlayerCharacterIds[1];
            newRecord.PlayerCharacterID3 = newPlayerCharacterIds[2];
            newRecord.PlayerCharacterID4 = newPlayerCharacterIds[3];
            newRecord.PlayerCharacterID5 = newPlayerCharacterIds[4];

            _changeLocalRecord(newRecord);
        }
        public PossessionParam[] GetDropPossessionParams()
        {
            var preprocessData = JsonConvert.DeserializeObject<PreprocessData>(_record.PreprocessData);
            return preprocessData.DropPossessionParams;
        }
        public void SetDropPossessionParams(IEnumerable<PossessionParam> dropPossessionParams)
        {
            var newRecord = _cloneRecord();
            newRecord.PreprocessData = JsonConvert.SerializeObject(new PreprocessData(){
                DropPossessionParams = dropPossessionParams.ToArray()
            });
            _changeLocalRecord(newRecord);
        }
        public class PreprocessData
        {
            public PossessionParam[] DropPossessionParams { get; set; }
        }
    }
}
