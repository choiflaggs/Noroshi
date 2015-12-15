using System;
using System.Linq;
using Noroshi.Core.WebApi.Response;
using Noroshi.Core.WebApi.Response.Character;
using Noroshi.Core.Game.Player;
using Noroshi.Server.Contexts;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Item;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Quest;

namespace Noroshi.Server.Services.Item
{
    public class SoulService
    {

        public static PlayerSoul[] GetAll(uint playerId)
        {
            return PlayerSoulEntity.ReadAndBuildAll(playerId).Select(data => data.ToResponseData()).ToArray();
        }

        public static Core.WebApi.Response.Soul[] MasterData()
        {
            return SoulEntity.ReadAndBuildAll().Select(e => e.ToResponseData()).ToArray();
        }

        public static CreateCharacter UseSoulWithCreateCharacter(uint playerId, uint soulId)
        {
            var soulData = SoulEntity.ReadAndBuild(soulId);
            if (soulData == null) {
                throw new InvalidOperationException("Not Found Soul's Data : " + soulId);
            }

            if (!PlayerCharacterEntity.CanCreateCharacter(playerId, soulData.CharacterID)) {
                throw new InvalidOperationException("Cannot Create Character : " + soulData.CharacterID);
            }
            var characterData = CharacterEntity.ReadAndBuild(soulData.CharacterID);
            var evolutionData =
                CharacterEvolutionTypeEntity.ReadAndBuild(characterData.EvolutionType, characterData.InitialEvolutionLevel);

            var possessionManager = new PossessionManager(playerId, soulData.GetAllPossessableParams(evolutionData.NecessaryGold, evolutionData.Soul));
            possessionManager.Load();

            if (!possessionManager.CanRemove(soulData.GetUsingSoulPossessionParam(evolutionData.Soul)) || !possessionManager.CanRemove(soulData.GetUsingGoldPosssesionParam(evolutionData.NecessaryGold))) {
                throw new InvalidOperationException("Cannot Remove Possession : " + soulId);
            }

            ContextContainer.ShardTransaction(tx =>
            {
                possessionManager.Remove(soulData.GetUsingGoldPosssesionParam(evolutionData.NecessaryGold));
                possessionManager.Remove(soulData.GetUsingSoulPossessionParam(evolutionData.Soul));
                possessionManager.Add(soulData.GetCharacterPosssesionParam());
                tx.Commit();
            });

            return new CreateCharacter
            {
                UseSoulPossessionObject = possessionManager.GetPossessionObject(soulData.GetUsingSoulPossessionParam(evolutionData.Soul)).ToResponseData(),
                CreateCharacterPossessionObject = possessionManager.GetPossessionObject(soulData.GetCharacterPosssesionParam()).ToResponseData(),
                UseGoldPossessionObject = possessionManager.GetPossessionObject(soulData.GetUsingGoldPosssesionParam(evolutionData.NecessaryGold)).ToResponseData()
            };
        }

        public static CharacterEvolutionLevelUpResponse UseSoulWithEvolutionLevel(uint playerId, uint soulId)
        {
            var soulData = SoulEntity.ReadAndBuild(soulId);
            if (soulData == null) {
                throw new NullReferenceException("Soul's Data Not Found : " + soulId);
            }
            var playerCharacterData = PlayerCharacterEntity.ReadAndBuildByPlayerIDAndChracterID(playerId, soulData.CharacterID, ReadType.Lock);
            if (playerCharacterData == null) {
                throw new InvalidOperationException();
            }
            var characterData = CharacterEntity.ReadAndBuild(soulData.CharacterID);
            if (characterData == null) {
                throw new InvalidOperationException();
            }
            var evolutionData =
                CharacterEvolutionTypeEntity.ReadAndBuild(characterData.EvolutionType, (byte)(playerCharacterData.EvolutionLevel + 1));
            if (evolutionData == null) {
                throw new NullReferenceException("Character EvolutionData Not Found");
            }

            var possessionManager = new PossessionManager(playerId, evolutionData.GetAllPossessionParams(soulId));
            possessionManager.Load();

            if (!possessionManager.CanRemove(evolutionData.GetUsingSoulPossessionParam(soulId)) || !possessionManager.CanRemove(evolutionData.GetUsingGoldPosssesionParam())) {
                throw new InvalidOperationException("Cannot Remove Possession : " + soulId);
            }

            playerCharacterData.ChangeEvolutionLevel((byte)(playerCharacterData.EvolutionLevel + 1), evolutionData.EvolutionLevel);
            ContextContainer.ShardTransaction(tx =>
            {
                possessionManager.Remove(evolutionData.GetUsingSoulPossessionParam(soulId));
                possessionManager.Remove(evolutionData.GetUsingGoldPosssesionParam());

                playerCharacterData.Save();

                // クエスト：指定レボリューションレベルのキャラクター数.
                QuestTriggerEntity.CountUpPlayerCharacterEvolutionLevelNum(playerCharacterData.PlayerID, playerCharacterData.EvolutionLevel);

                // チュートリアル進捗処理。
                PlayerStatusEntity.TryToProceedTutorialStep(playerId, TutorialStep.EvolutionLevelUP);

                tx.Commit();
            });
            return new CharacterEvolutionLevelUpResponse
            {
                UsedGold = possessionManager.GetPossessionObject(evolutionData.GetUsingGoldPosssesionParam()).ToResponseData(),
                UsedSoul = possessionManager.GetPossessionObject(evolutionData.GetUsingSoulPossessionParam(soulId)).ToResponseData(),
                PlayerCharacter = playerCharacterData.ToResponseData()
            };
        }

    }
}