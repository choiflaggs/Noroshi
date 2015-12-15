using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Information;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerConfirmationSchema;

namespace Noroshi.Server.Entity.Player
{
    /// <summary>
    /// 既読管理などに利用する最終確認日時管理クラス。
    /// </summary>
    public class PlayerConfirmationEntity : AbstractDaoWrapperEntity<PlayerConfirmationEntity, PlayerConfirmationDao, Schema.PrimaryKey, Schema.Record>
    {
        /// <summary>
        /// 参照ID。利用シーンが増えている毎に追加。
        /// </summary>
        enum ReferenceID
        {
            Greeted = 1,
            InformationEvent = 2,
            InformationCampaign = 3,
            InformationImportant = 4,
            InformationApology = 5,
        }
        static readonly Dictionary<InformationCategory, ReferenceID> INFORMATION_CATEGORY_TO_REFERENCE_ID_MAP = new Dictionary<InformationCategory, ReferenceID>
        {
            { InformationCategory.Event, ReferenceID.InformationEvent },
            { InformationCategory.Campaign, ReferenceID.InformationCampaign },
            { InformationCategory.Important, ReferenceID.InformationImportant },
            { InformationCategory.Apology, ReferenceID.InformationApology },
        };
        /// <summary>
        /// 被挨拶最終確認日時取得。
        /// </summary>
        /// <param name="playerId">被挨拶プレイヤーID</param>
        /// <returns></returns>
        public static uint? ReadLastGreetedConfirmedAt(uint playerId)
        {
            var entity = ReadAndBuild(new Schema.PrimaryKey { PlayerID = playerId, ReferenceID = (byte)ReferenceID.Greeted });
            return entity != null ? (uint?)entity._confirmedAt : null;
        }
        /// <summary>
        /// 被挨拶最終確認日時取得。複数取得バージョン。
        /// </summary>
        /// <param name="playerIds">被挨拶プレイヤーID</param>
        /// <returns></returns>
        public static Dictionary<uint, uint?> ReadLastGreetedConfirmedAtMulti(IEnumerable<uint> playerIds)
        {
            var map = ReadAndBuildMulti(playerIds.Select(playerId => new Schema.PrimaryKey { PlayerID = playerId, ReferenceID = (byte)ReferenceID.Greeted })).ToDictionary(entity => entity._playerId);
            return playerIds.ToDictionary(playerId => playerId, playerId => map.ContainsKey(playerId) ? (uint?)map[playerId]._confirmedAt : null);
        }
        /// <summary>
        /// 被挨拶確認。
        /// </summary>
        /// <param name="playerId">被挨拶プレイヤーID</param>
        /// <param name="confirmedAt">確認日次</param>
        public static void ConfirmGreeted(uint playerId, uint confirmedAt)
        {
            _confirm(playerId, (byte)ReferenceID.Greeted, confirmedAt);
        }

        /// <summary>
        /// 最終お知らせ閲覧日時取得。
        /// </summary>
        /// <param name="playerId">閲覧プレイヤーID</param>
        /// <returns></returns>
        public static uint? ReadLastInformationConfirmedAt(uint playerId, InformationCategory informationCategory)
        {
            InformationCategory[] informationCategories = new InformationCategory[] { informationCategory };
            var informationCategoryToPlayerLastConfirmedAt = ReadLastInformationConfirmedAtMulti(playerId, informationCategories);
            return informationCategoryToPlayerLastConfirmedAt[informationCategory];
        }
        public static Dictionary<InformationCategory, uint?> ReadLastInformationConfirmedAtMulti(uint playerId, IEnumerable<InformationCategory> informationCategories)
        {
            uint defaultAt;
            var map = ReadAndBuildMulti(informationCategories.Select(informationCategory => new Schema.PrimaryKey { PlayerID = playerId, ReferenceID = (byte)INFORMATION_CATEGORY_TO_REFERENCE_ID_MAP[informationCategory] })).ToDictionary(entity => entity._record.ReferenceID, entity => entity._confirmedAt);
            return informationCategories.ToDictionary(informationCategory => informationCategory, informationCategory => map.TryGetValue((byte)INFORMATION_CATEGORY_TO_REFERENCE_ID_MAP[informationCategory], out defaultAt) ? (uint?)map[(byte)INFORMATION_CATEGORY_TO_REFERENCE_ID_MAP[informationCategory]] : null);
        }

        /// <summary>
        /// お知らせ閲覧。
        /// </summary>
        /// <param name="playerId">閲覧プレイヤーID</param>
        /// <param name="confirmedAt">閲覧日次</param>
        public static void ConfirmInformation(uint playerId, InformationCategory informationCategory, uint confirmedAt)
        {
            byte referenceID = (byte)INFORMATION_CATEGORY_TO_REFERENCE_ID_MAP[informationCategory];
            _confirm(playerId, referenceID, confirmedAt);
        }

        static void _confirm(uint playerId, byte referenceId, uint confirmedAt)
        {
            var entity = _instantiate((new PlayerConfirmationDao()).CreateOrRead(playerId, referenceId));
            entity._setConfirmedAt(confirmedAt);
            if (!entity.Save())
            {
                throw new SystemException(string.Join("\t", "Fail to Update", playerId, referenceId));
            }
        }

        uint _playerId => _record.PlayerID;
        uint _confirmedAt => _record.ConfirmedAt;

        void _setConfirmedAt(uint confirmedAt)
        {
            var newRecord = _cloneRecord();
            newRecord.ConfirmedAt = confirmedAt;
            _changeLocalRecord(newRecord);
        }
    }
}
