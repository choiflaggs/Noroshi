using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.WebApi.Response.Battle;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Entity.BattleContent;
using Noroshi.Server.Entity.Quest;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;

namespace Noroshi.Server.Services
{
    public class BattleService
    {
        public static CpuBattleStartResponse StartCpuBattle(BattleCategory category, uint id, uint playerId, uint[] playerCharacterIds, uint? rentalPlayerId, uint paymentNum)
        {
            return _start(category, id, playerId, playerCharacterIds, rentalPlayerId, paymentNum, CpuBattleEntity.ReadAndBuild, (battle, battleContent, dropPossessionParams, playerCharacters, ownCpuCharacters, possessionManager) =>
            {
                // 付加情報取得
                var additionalInformation = battleContent.GetAdditionalInformation();
                // CPU 含めた自キャラ
                var ownCharacters = new List<BattleCharacter>(playerCharacters.Select(pc => pc.ToBattleResponseData()));
                ownCharacters.AddRange(ownCpuCharacters.Select(cc => cc.ToResponseData()));

                return new CpuBattleStartResponse
                {
                    Battle = ((CpuBattleEntity)battle).ToResponseData(possessionManager),
                    DropPossessionObjects = dropPossessionParams
                        .Select(ppss =>
                        {
                            return ppss.Select(pps =>
                            {
                                return pps.Select(pp => possessionManager.GetPossessionObject(pp).ToResponseData()).ToList();
                            }).ToList();
                        }).ToList(),
                    PlayerExp = battleContent.GetPlayerExp(),
                    AdditionalInformation = additionalInformation != null ? additionalInformation.ToResponseData() : null,
                    BattleAutoMode = (byte)battleContent.GetBattleAutoMode(),
                    OwnCharacters = ownCharacters.ToArray(),
                };
            });
        }

        public static PlayerBattleStartResponse StartPlayerBattle(BattleCategory category, uint id, uint playerId, uint[] playerCharacterIds, uint? rentalPlayerId, uint paymentNum)
        {
            return _start(category, id, playerId, playerCharacterIds, rentalPlayerId, paymentNum, PlayerBattleEntity.ReadAndBuild, (battle, battleContent, dropPossessionParams, playerCharacters, ownCpuCharacters, possessionManager) =>
            {
                // 付加情報取得
                var additionalInformation = battleContent.GetAdditionalInformation();
                // CPU 含めた自キャラ
                var ownCharacters = new List<BattleCharacter>(playerCharacters.Select(pc => pc.ToBattleResponseData()));
                ownCharacters.AddRange(ownCpuCharacters.Select(cc => cc.ToResponseData()));

                return new PlayerBattleStartResponse
                {
                    Battle = ((PlayerBattleEntity)battle).ToResponseData(),
                    DropPossessionObjects = dropPossessionParams
                        .Select(ppss =>
                        {
                            return ppss.Select(pps =>
                            {
                                return pps.Select(pp => possessionManager.GetPossessionObject(pp).ToResponseData()).ToList();
                            }).ToList();
                        }).ToList(),
                    PlayerExp = battleContent.GetPlayerExp(),
                    AdditionalInformation = additionalInformation != null ? additionalInformation.ToResponseData() : null,
                    BattleAutoMode = (byte)battleContent.GetBattleAutoMode(),
                    LoopBattle = battleContent.IsLoopBattle(),
                    OwnCharacters = ownCharacters.ToArray(),
                };
            });
        }
        static T _start<T>(
            BattleCategory category, uint id, uint playerId, uint[] playerCharacterIds, uint? rentalPlayerCharacterId, uint paymentNum,
            Func<uint, IBattleEntity> readAndBuildBattle,
            Func<IBattleEntity, IBattleContent, List<List<List<PossessionParam>>>, IEnumerable<PlayerCharacterEntity>, IEnumerable<CpuCharacterEntity>, PossessionManager, T> makeResponse
        )
        {
            var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId);

            /* 引数チェック */

            // プレイヤーキャラクター ID 数チェック
            if (playerCharacterIds.Length < 1 || Constant.MAX_CHARACTER_NUM_PER_DECK < playerCharacterIds.Length)
            {
                throw new ArgumentException("Invalid Length Player Character IDs : " + string.Join("\t", playerCharacterIds));
            }
            // プレイヤーキャラクター ID 重複指定チェック
            if (playerCharacterIds.Length != playerCharacterIds.Distinct().Count())
            {
                throw new ArgumentException("Duplicate Player Character IDs : " + string.Join("\t", playerCharacterIds));
            }

            /* バトルコンテンツ情報取得 */

            // 対象バトルコンテンツをビルド。
            var battleContent = BattleContentBuilder.BuildBattleContent(playerId, category, id);

            // 遷移上事前チェックはしてあるはずなので、改めてのチェックで引っかかった場合は例外とする。
            if (battleContent == null || !battleContent.CanBattle(playerStatus, paymentNum))
            {
                throw new InvalidOperationException("Invalid Battle Content : " + id);
            }
            // バトル ID 取得
            var battleId = battleContent.GetBattleID();

            /* バトル情報取得 */

            // 対象バトルをビルド
            var battle = readAndBuildBattle(battleId);
            // 対象バトル存在チェック
            if (battle == null)
            {
                throw new NullReferenceException("Battle Not Found : " + battleId);
            }

            // ドロップ内容抽選。
            var dropPossessionParams = battle.LotDropRewards();

            /* プレイヤーキャラクター情報取得 */

            // バトルに参加させるプレイヤーキャラクターをビルド
            var playerCharacters = PlayerCharacterEntity.ReadAndBuildMulti(playerCharacterIds).ToList();

            // 対象プレイヤーキャラクター存在チェック
            if (playerCharacters.Count() != playerCharacterIds.Length)
            {
                throw new ArgumentException("Player Character Not Found : " + string.Join("\t", playerCharacterIds.Except(playerCharacters.Select(pc => pc.ID))));
            }
            // キャラクター所有者チェック
            if (playerCharacters.Any(playerCharacter => playerCharacter.PlayerID != playerId))
            {
                throw new ArgumentException("Invalid Owner Player Character IDs : " + string.Join("\t", playerCharacters.Where(pc => pc.PlayerID != playerId).Select(pc => pc.ID)));
            }

            // レンタルキャラクター取得。
            PlayerCharacterEntity rentalPlayerCharacter = null;
            if (rentalPlayerCharacterId.HasValue)
            {
                var rentalCharacter = PlayerRentalCharacterEntity.ReadAndBuildByPlayerCharacterID(rentalPlayerCharacterId.Value);
                if (rentalCharacter.PlayerCharacterID == playerId)
                {
                    throw new InvalidOperationException(string.Join("\t", "Invalid Rental Player Character ID", rentalCharacter.PlayerCharacterID));
                }
                rentalPlayerCharacter = PlayerCharacterEntity.ReadAndBuild(rentalCharacter.PlayerCharacterID);
                if (rentalPlayerCharacter == null)
                {
                    throw new SystemException(string.Join("\t", "Invalid Rental Player Character", rentalCharacter.PlayerCharacterID));
                }
                if (playerCharacters.Any(pc => pc.CharacterID == rentalPlayerCharacter.CharacterID))
                {
                    throw new InvalidOperationException(string.Join("\t", "Invalid Rental Player Character ID", rentalCharacter.PlayerCharacterID));
                }
                // TODO : レベル補正。
                playerCharacters.Add(rentalPlayerCharacter);
            }
            // 味方 CPU キャラ取得。
            var ownCpuCharacters = battleContent.GetOwnCpuCharacters().Where(cc => playerCharacters.All(pc => cc.CharacterID != pc.CharacterID));
            // 最終的に上限を超えていないかチェック。
            if (playerCharacters.Count() + ownCpuCharacters.Count() > Constant.MAX_CHARACTER_NUM_PER_DECK)
            {
                throw new InvalidOperationException("Invalid Own Characters");
            }

            // キャラクター制約チェック。
            var characters = CharacterEntity.ReadAndBuildMulti(playerCharacters.Select(pc => pc.CharacterID));
            if (!battleContent.IsValidCharacters(characters))
            {
                throw new InvalidOperationException("Invalid Own Characters");
            }

            /* Possession */

            var possessionParams = new List<PossessionParam>(battle.GetDroppableRewards());
            possessionParams.AddRange(dropPossessionParams.SelectMany(ppss => ppss).SelectMany(pps => pps));
            var possessionManager = new PossessionManager(playerId, possessionParams);
            possessionManager.Load();

            /* トランザクション */

            InitialCondition initialCondition = null;
            ContextContainer.ShardTransaction(tx =>
            {
                battleContent.PreProcess();
                var result = battle.StartBattle(playerId, playerCharacters, dropPossessionParams);
                if (rentalPlayerCharacter != null)
                {
                    PlayerRentalCharacterEntity.TryToIncrementRentalNum(rentalPlayerCharacter.ID);
                }



                initialCondition = battleContent.GetInitialCondition(playerCharacterIds, paymentNum);

                tx.Commit();
            });

            /* レスポンス */

            // 初期状態適用。
            if (initialCondition != null)
            {
                if (initialCondition.OwnPlayerCharacterConditions != null)
                {
                    var ownPlayerCharacterConditionMap = initialCondition.OwnPlayerCharacterConditions.ToDictionary(opcc => opcc.PlayerCharacterID);
                    foreach (var playerCharacter in playerCharacters.Where(pc => ownPlayerCharacterConditionMap.ContainsKey(pc.ID)))
                    {
                        var condition = ownPlayerCharacterConditionMap[playerCharacter.ID];
                        playerCharacter.SetInitialCondition(condition.HP, condition.Energy, condition.DamageCoefficient);
                    }
                }
                if (initialCondition.EnemyCharacterConditions != null)
                {
                    battle.ApplyInitialCondition(initialCondition.EnemyCharacterConditions);
                }
            }


            return makeResponse(battle, battleContent, dropPossessionParams, playerCharacters, ownCpuCharacters, possessionManager);
        }

        public static CpuBattleFinishResponse FinishCpuBattle(BattleCategory category, uint id, uint playerId, VictoryOrDefeat victoryOrDefeat, byte rank, BattleResult battleResult)
        {
            return _finish(category, id, playerId, victoryOrDefeat, rank, battleResult, CpuBattleEntity.ReadAndBuild, (battle, battleContent, possessionManager, rewards) =>
            {
                var addPlayerExpResult = possessionManager.GetAddPlayerExpResult();
                return new CpuBattleFinishResponse()
                {
                    Battle = ((CpuBattleEntity)battle).ToResponseData(possessionManager),
                    AddPlayerExpResult = addPlayerExpResult != null ? addPlayerExpResult.ToResponseData() : null,
                };
            });
        }
        public static PlayerBattleFinishResponse FinishPlayerBattle(BattleCategory category, uint id, uint playerId, VictoryOrDefeat victoryOrDefeat, byte rank, BattleResult battleResult)
        {
            return _finish(category, id, playerId, victoryOrDefeat, rank, battleResult, PlayerBattleEntity.ReadAndBuild, (battle, battleContent, possessionManager, rewards) =>
            {
                var addPlayerExpResult = possessionManager.GetAddPlayerExpResult();
                return new PlayerBattleFinishResponse()
                {
                    Battle = ((PlayerBattleEntity)battle).ToResponseData(),
                    AddPlayerExpResult = addPlayerExpResult != null ? addPlayerExpResult.ToResponseData() : null,
                };
            });
        }

        static T _finish<T>(
            BattleCategory category, uint id, uint playerId,
            VictoryOrDefeat victoryOrDefeat, byte rank, BattleResult result,
            Func<uint, IBattleEntity> readAndBuildBattle,
            Func<IBattleEntity, IBattleContent, PossessionManager, IEnumerable<PossessionParam>, T> makeResponse
        )
        {
            /* バトルコンテンツ情報取得 */

            // 対象バトルコンテンツをビルド。
            var battleContent = BattleContentBuilder.BuildBattleContent(playerId, category, id);

            // 遷移上事前チェックはしてあるはずなので、改めてのチェックで引っかかった場合は例外とする。
            if (battleContent == null)
            {
                throw new InvalidOperationException("Invalid Battle Content : " + id);
            }
            // バトル ID 取得
            var battleId = battleContent.GetBattleID();

            /* バトル情報取得 */

            // 対象バトルをビルド
            var battle = readAndBuildBattle(battleId);

            // 対象バトル存在チェック
            if (battle == null)
            {
                throw new NullReferenceException("Battle Not Found : " + battleId);
            }

            /* トランザクション */

            PossessionManager possessionManager = null;
            var rewards = new List<PossessionParam>();
            ContextContainer.ShardTransaction(tx =>
            {
                battle.LoadSession(playerId, ReadType.Lock);
                var playerStatus = PlayerStatusEntity.ReadAndBuild(playerId, ReadType.Lock);
                var guild = playerStatus.GuildID.HasValue ? GuildEntity.ReadAndBuild(playerStatus.GuildID.Value, ReadType.Lock) : null;
                var playerCharacters = PlayerCharacterEntity.ReadAndBuildMulti(battle.GetPlayerCharacterIDs(), ReadType.Lock);
                var battleRewards = battle.FinishBattle(victoryOrDefeat);
                battleContent.FinishBattle(victoryOrDefeat, rank, result, playerStatus, playerCharacters, guild);
                var battleContentRewards = battleContent.GetRewards(playerStatus, victoryOrDefeat);

                if (!playerStatus.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save", playerStatus.PlayerID));
                }
                if (guild != null && !guild.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save Guild" , guild.ID));
                }

                if (victoryOrDefeat == VictoryOrDefeat.Win)
                {
                    // キャラクター経験値アップ。
                    if (battle.GetCharacterExp() > 0)
                    {
                        foreach (var playerCharacter in playerCharacters)
                        {
                            playerCharacter.AddExp(battle.GetCharacterExp(), playerStatus.Level);
                            if (!playerCharacter.Save())
                            {
                                throw new SystemException(string.Join("\t", "Fail to Save", playerCharacter.ID));
                            }
                        }
                    }
                }

                // 報酬付与
                if (battleRewards.Count() > 0) rewards.AddRange(battleRewards);
                if (battleContentRewards.Count() > 0) rewards.AddRange(battleContentRewards);

                var possessionParams = new List<PossessionParam>();
                possessionParams.AddRange(rewards);
                possessionParams.AddRange(battle.GetDroppableRewards());
                possessionManager = new PossessionManager(playerId, possessionParams);
                possessionManager.Load();
                possessionManager.Add(rewards);

                // クエスト状態更新
                QuestTriggerEntity.CountUpBattleFinishNum(playerId, category);

                tx.Commit();
            });

            /* レスポンス */

            return makeResponse(battle, battleContent, possessionManager, rewards);
        }
    }
}
