using System.Collections.Generic;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Character;

namespace Noroshi.Server.Entity.BattleContent
{
    public class ArenaBattleContent : AbstractBattleContent
    {
        readonly uint _playerId;
        readonly uint _id;

        public ArenaBattleContent(uint playerId, uint id)
        {
            _playerId = playerId;
            _id = id;
        }

        public override InitialCondition GetInitialCondition(uint[] playerCharacterIds, uint paymentNum)
        {
            return null;
        }

        public override uint GetBattleID()
        {
            return _id;
        }

        public override bool CanBattle(PlayerStatusEntity playerStatus, uint paymentNum)
        {

            // 攻撃を行うプレイヤーの本日の攻撃回数を確認.
            var playerArenaData = PlayerArenaEntity.CreateOrReadAndBuild(_playerId);
            var playerOtherArenaData = PlayerArenaEntity.CreateOrReadAndBuild(_id);

            return playerArenaData.CanAttack() && playerOtherArenaData.CanDefense();
        }
        public override BattleAutoMode GetBattleAutoMode()
        {
            return BattleAutoMode.Auto;
        }

        public override void FinishBattle(VictoryOrDefeat victoryOrDefeat, byte rank, BattleResult result, PlayerStatusEntity playerStatus, IEnumerable<PlayerCharacterEntity> ownPlayerCharacters, GuildEntity guild)
        {
            // 勝っても負けても行う処理.

            // 戦闘履歴を保存.


            // 勝利処理.

            if (victoryOrDefeat != VictoryOrDefeat.Win) return;
            var playerArenaData = PlayerArenaEntity.CreateOrReadAndBuild(_playerId);
            var playerOtherArenaData = PlayerArenaEntity.CreateOrReadAndBuild(_id);
            var playerRank = playerArenaData.Rank;
            var playerOtherRank = playerOtherArenaData.Rank;
            if (playerRank <= playerOtherRank) return;
            playerArenaData.ChangeRank(playerOtherRank);
            playerOtherArenaData.ChangeRank(playerRank);
            playerArenaData.Save();
            playerOtherArenaData.Save();
        }
    }
}
