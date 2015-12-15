using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.PresentBox;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PresentBoxReceivedLogSchema;

namespace Noroshi.Server.Entity.PresentBox
{
    public class PresentBoxReceivedLogEntity : AbstractDaoWrapperEntity<PresentBoxReceivedLogEntity, PresentBoxReceivedLogDao, Schema.PrimaryKey, Schema.Record>
    {
        public static PresentBoxReceivedLogEntity ReadAndBuild(uint id, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(new[] { id }, readType).FirstOrDefault();
        }
        public static IEnumerable<PresentBoxReceivedLogEntity> ReadAndBuildMulti(IEnumerable<uint> ids, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id }), readType);
        }
        public static IEnumerable<PresentBoxReceivedLogEntity> ReadAndBuildByPlayerID(uint playerId)
        {
            return _instantiate((new PresentBoxReceivedLogDao()).ReadByPlayerID(playerId));
        }
        public static IEnumerable<PresentBoxReceivedLogEntity> CreateMulti(IEnumerable<PresentBoxEntity> presentBoxes)
        {
            return presentBoxes.Select(pb => Create(pb));
        }
        public static PresentBoxReceivedLogEntity Create(PresentBoxEntity presentBox)
        {
            return _instantiate((new PresentBoxReceivedLogDao()).Create(presentBox.ID, presentBox.PlayerID, _serializePossessionData(presentBox.PossessionParams), presentBox.TextID, _serializeTextData(presentBox.TextParams)));
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
        public PossessionParam[] PossessionParams => _deserializePossessionData().PossessionParams;
        public uint CreatedAt => _record.CreatedAt;

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
    }
}
