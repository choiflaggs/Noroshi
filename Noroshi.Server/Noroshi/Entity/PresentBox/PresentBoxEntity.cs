using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Noroshi.Core.Game.PresentBox;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.PresentBox;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PresentBoxSchema;

namespace Noroshi.Server.Entity.PresentBox
{
    public class PresentBoxEntity : AbstractDaoWrapperEntity<PresentBoxEntity, PresentBoxDao, Schema.PrimaryKey, Schema.Record>
    {
        public static PresentBoxEntity ReadAndBuild(uint id, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(new[] { id }, readType).FirstOrDefault();
        }
        public static IEnumerable<PresentBoxEntity> ReadAndBuildMulti(IEnumerable<uint> ids, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id }), readType);
        }
        public static IEnumerable<PresentBoxEntity> ReadAndBuildByPlayerID(uint playerId)
        {
            return _instantiate((new PresentBoxDao()).ReadByPlayerID(playerId));
        }
        public static PresentBoxEntity Create(uint playerId, IEnumerable<PossessionParam> possessionParams, string textId, string[] textParams)
        {
            return _instantiate((new PresentBoxDao()).Create(playerId, _serializePossessionData(possessionParams.ToArray()), textId, _serializeTextData(textParams)));
        }

        public static void AddRewardsAndCommit(IEnumerable<uint> playerIds, IEnumerable<PossessionParam> rewards, string textKey, string[] textParams, Action<uint, PossessionParam> afterCommitAction)
        {
            ContextContainer.ShardTransaction(tx =>
            {
                foreach (var playerId in playerIds)
                {
                    if (Create(playerId, rewards, textKey, textParams) == null)
                    {
                        throw new SystemException(string.Join("\t", "Fail to Create Present Box", playerId));
                    }
                    tx.AddAfterCommitAction(() =>
                    {
                        foreach (var reward in rewards)
                        {
                            afterCommitAction.Invoke(playerId, reward);
                        }
                    });
                }
                tx.Commit();
            });
        }

        static string _serializePossessionData(PossessionData possessionData)
        {
            return JsonConvert.SerializeObject(possessionData);
        }
        static string _serializePossessionData(PossessionParam[] possessionParams)
        {
            return _serializePossessionData(new PossessionData { PossessionParams = possessionParams });
        }
        static string _serializeTextData(TextData textData)
        {
            return JsonConvert.SerializeObject(textData);
        }
        static string _serializeTextData(string[] textParams)
        {
            return _serializeTextData(new TextData { TextParams = textParams });
        }


        public uint ID => _record.ID;
        public uint PlayerID => _record.PlayerID;
        public PossessionParam[] PossessionParams => _deserializePossessionData().PossessionParams;
        public string TextID => _record.TextID;
        public string[] TextParams => _deserializeTextData().TextParams;
        public uint CreatedAt => _record.CreatedAt;

        public bool CanReceive()
        {
            return ContextContainer.GetContext().TimeHandler.UnixTime <= CreatedAt + Constant.MAX_PRESENT_BOX_KEEPING_SPAN.TotalSeconds;
        }

        PossessionData _deserializePossessionData()
        {
            return JsonConvert.DeserializeObject<PossessionData>(_record.PossessionData);
        }
        TextData _deserializeTextData()
        {
            return JsonConvert.DeserializeObject<TextData>(_record.TextData);
        }
        class PossessionData
        {
            public PossessionParam[] PossessionParams { get; set; }
        }
        class TextData
        {
            public string[] TextParams { get; set; }
        }

        public Core.WebApi.Response.PresentBox.PresentBox ToResponseData(PossessionManager possessionManager)
        {
            return new Core.WebApi.Response.PresentBox.PresentBox
            {
                ID = ID,
                PossessionObjects = possessionManager.GetPossessionObjects(PossessionParams).Select(po => po.ToResponseData()).ToArray(),
                TextID = TextID,
                TextParams = TextParams,
                CreatedAt = CreatedAt,
            };
        }
    }
}
