using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Noroshi.UI {
    public class RaidGuildRankPanel : MonoBehaviour {
        [SerializeField] Image imgGuildRank;
        [SerializeField] Text txtAfterGuildExp;
        [SerializeField] GameObject guildExpBar;
        [SerializeField] Sprite[] guildRankSpriteList;

        private ushort needCooperationPoint;

        public void SetGuildRank(Noroshi.Core.WebApi.Response.Guild.Guild guildData) {
            switch(guildData.GuildRank) {
                case Noroshi.Core.Game.Guild.GuildRank.S:
                    needCooperationPoint = 0;
                    imgGuildRank.sprite = guildRankSpriteList[0];
                    break;
                case Noroshi.Core.Game.Guild.GuildRank.A:
                    needCooperationPoint = Noroshi.Core.Game.Guild.Constant.GUILD_RANK_TO_NECESSARY_COOPERATION_POINT_MAP[Noroshi.Core.Game.Guild.GuildRank.S];
                    imgGuildRank.sprite = guildRankSpriteList[1];
                    break;
                case Noroshi.Core.Game.Guild.GuildRank.B:
                    needCooperationPoint = Noroshi.Core.Game.Guild.Constant.GUILD_RANK_TO_NECESSARY_COOPERATION_POINT_MAP[Noroshi.Core.Game.Guild.GuildRank.A];
                    imgGuildRank.sprite = guildRankSpriteList[2];
                    break;
                case Noroshi.Core.Game.Guild.GuildRank.C:
                    needCooperationPoint = Noroshi.Core.Game.Guild.Constant.GUILD_RANK_TO_NECESSARY_COOPERATION_POINT_MAP[Noroshi.Core.Game.Guild.GuildRank.B];
                    imgGuildRank.sprite = guildRankSpriteList[3];
                    break;
                case Noroshi.Core.Game.Guild.GuildRank.D:
                    needCooperationPoint = Noroshi.Core.Game.Guild.Constant.GUILD_RANK_TO_NECESSARY_COOPERATION_POINT_MAP[Noroshi.Core.Game.Guild.GuildRank.C];
                    imgGuildRank.sprite = guildRankSpriteList[4];
                    break;
                case Noroshi.Core.Game.Guild.GuildRank.E:
                    needCooperationPoint = Noroshi.Core.Game.Guild.Constant.GUILD_RANK_TO_NECESSARY_COOPERATION_POINT_MAP[Noroshi.Core.Game.Guild.GuildRank.D];
                    imgGuildRank.sprite = guildRankSpriteList[5];
                    break;
            }
            var expBarWidth = guildExpBar.GetComponent<RectTransform>().sizeDelta.x;
            var xx = needCooperationPoint == 0 ? 0 : expBarWidth - expBarWidth * (float)guildData.CooperationPoint / (float)needCooperationPoint;
            txtAfterGuildExp.text = (needCooperationPoint - guildData.CooperationPoint).ToString();
            guildExpBar.transform.localPosition = new Vector3(-xx, 0, 0);
        }

    }
}
