using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.RaidBoss;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Story;

namespace Noroshi.Server.Entity.BattleContent
{
	public class StoryBattleContent : AbstractBattleContent
	{
		readonly uint _id;
		readonly uint _playerId;

		readonly StoryStageEntity _stage;
		public PlayerStatusEntity PlayerData;

		public StoryBattleContent(uint playerId, uint id)
		{
			_playerId = playerId;
			_id = id;

			_stage = StoryStageEntity.ReadAndBuild(id);
		}
		public override uint GetBattleID()
		{
			return _stage.BattleID;
		}
		public override ushort GetPlayerExp()
		{
			return _stage.Stamina;
		}

		public override AdditionalInformation GetAdditionalInformation()
		{
			return new AdditionalInformation
			{

				BattleTitleTextKey = _stage.TextKey + ".Name",
			};
		}

		public override IEnumerable<CpuCharacterEntity> GetOwnCpuCharacters()
		{
			var cpuCharacterIds = _stage.CpuCharacterIDs.Where(id => id > 0);

			return cpuCharacterIds.Any() ? CpuCharacterEntity.ReadAndBuildMulti(cpuCharacterIds) : new CpuCharacterEntity[0];
		}


		public override bool CanBattle(PlayerStatusEntity playerStatus, uint paymentNum)
		{
			var data = StoryStageEntity.ReadAndBuildAll().FirstOrDefault(d => d.ID == _id);
			return data != null && playerStatus != null && playerStatus.Stamina >= data.Stamina;
		}
		public override BattleAutoMode GetBattleAutoMode()
		{
			return BattleAutoMode.Selectable;
		}


		public override void FinishBattle(VictoryOrDefeat victoryOrDefea, byte rank, BattleResult result, PlayerStatusEntity playerStatus, IEnumerable<PlayerCharacterEntity> ownPlayerCharacters, GuildEntity guild)
		{

			var stageData = StoryStageEntity.ReadAndBuildAll().FirstOrDefault(d => d.ID == _id);
			switch (victoryOrDefea) {
				case VictoryOrDefeat.Win:
					var nextStageData =

						StoryStageEntity.ReadAndBuildByEpisodeID(stageData.EpisodeID)
							.Where(s => s.ID > stageData.ID)
							.OrderBy(s => s.ID)
							.FirstOrDefault();

					if (nextStageData != null) {

						PlayerStoryStageEntity.Get(_playerId, nextStageData.ID);
					}

					var playerStageData = PlayerStoryStageEntity.ChangedProgress(_playerId, _id, rank);
//                    var playerStatus = PlayerStatusEntity.AddGold(playerId, battleData.Money);
					playerStatus.ConsumeStamina(stageData.Stamina);

					GuildRaidBossEntity.TryToDiscoveryNormal(playerStatus);
					playerStageData.Save();
					break;
				case VictoryOrDefeat.Loss:
					break;
				case VictoryOrDefeat.Draw:
					break;
				case VictoryOrDefeat.TimeUp:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(victoryOrDefea), victoryOrDefea, null);
			}
			return;

			//var stageData = StageEntity.ReadAndBuildAll().FirstOrDefault(d => d.StageID == _id);
			//var battleData = CpuBattle.ReadAndBuild(new CpuBattleDao.PrimaryKey {ID = stageData.BattleID});
			//if (stageData == null) {
			//    throw new InvalidOperationException();
			//}
			//switch (victoryOrDefea) {
			//    case VictoryOrDefeat.Loss:
			//        break;
			//    case VictoryOrDefeat.Win:
			//        PlayerData.ChangeExp(PlayerData.Exp + battleData.Exp);
			//        PlayerData.ChangeGold(PlayerData.Gold + battleData.Money);
			//        break;
			//    case VictoryOrDefeat.Escape:
			//        PlayerData.AddStamina((ushort)(stageData.Stamina - stageData.EscapeStamina));
			//        break;
			//    default:
			//        throw new InvalidOperationException();
			//}
			//PlayerData.Save();
		}
		public override IEnumerable<PossessionParam> GetRewards(PlayerStatusEntity playerStatus, VictoryOrDefeat victoryOrDefeat)
		{
            var rewards = new List<PossessionParam>();
            if (victoryOrDefeat != VictoryOrDefeat.Win) return rewards;

            rewards.Add(PossessionManager.GetPlayerExpParam(_stage.Stamina));
            var tutorialStep = playerStatus.TryToProceedTutorialStepByClearStageID(_stage.ID);
            if (tutorialStep.HasValue)
            {
                var tutorial = new TutorialEntity(tutorialStep.Value);
                var tutorialRewards = tutorial.GetPossessionParams();
                if (tutorialRewards.Count() > 0) rewards.AddRange(tutorialRewards);
            }
            return rewards;
		}
	}
}
