using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Information;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Information;
using Noroshi.Server.Entity.Player;

namespace Noroshi.Server.Services
{
    public class InformationService
    {
        /// <summary>
        /// 指定プレイヤーのお知らせ一覧を取得する。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <returns></returns>
        public static Core.WebApi.Response.Information.ListResponse List(uint playerId)
        {
            // 開いているお知らせは全てビルド。
            var informations = InformationEntity.ReadAndBuildOpenInformationByCurrentTime(ContextContainer.GetContext().TimeHandler.UnixTime);
            // 仮に開いているお知らせが存在しなければ空配列で即レスポンスを返す。
            if (informations.Count() == 0)
            {
                /* レスポンス */

                return new Core.WebApi.Response.Information.ListResponse
                {
                    Informations = new Core.WebApi.Response.Information.Information[0],
                };
            }

            var informationCategories = informations.Select(information => information.Category).Distinct();
            // お知らせマスタをビルドし、お知らせ種類 => 種類ごとの閲覧時間のマッピングを作る。
            var informationCategoryToPlayerLastConfirmedAt = PlayerConfirmationEntity.ReadLastInformationConfirmedAtMulti(playerId, informationCategories);
            
            /* レスポンス */
            return new Core.WebApi.Response.Information.ListResponse
            {
                Informations = informations.Select(information =>
                {
                    return information.ToResponseData(informationCategoryToPlayerLastConfirmedAt);
                }).ToArray(),
            };
        }

        /// <summary>
        /// お知らせの種類毎に既読時間更新。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <param name="informationCategories">お知らせの種類</param>
        /// <returns></returns>
        public static Core.WebApi.Response.Information.ReadResponse Read(uint playerId, byte[] informationCategories)
        {
            foreach (var informationCategory in informationCategories)
            {
                if(!InformationCategory.IsDefined(typeof(InformationCategory), (int)informationCategory))
                {
                    throw new InvalidOperationException(string.Join("\t", "Information Category Not Found", informationCategory));
                }
            }
            /* トランザクション */
            return ContextContainer.NoroshiTransaction(tx =>
            {
                foreach (var informationCategory in informationCategories)
                {
                    // 受け取り日時更新。
                    PlayerConfirmationEntity.ConfirmInformation(playerId, (InformationCategory)informationCategory, ContextContainer.GetContext().TimeHandler.UnixTime);
                }
                tx.Commit();

                return new Core.WebApi.Response.Information.ReadResponse
                {
                    ConfirmedAt = ContextContainer.GetContext().TimeHandler.UnixTime,
                };
            });
            
        }
    }
}
