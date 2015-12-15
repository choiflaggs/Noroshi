using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Possession;
using Noroshi.Core.Game.Battle;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Item;
using Noroshi.Server.Entity.Gacha;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Battle;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.CpuBattleSchema;

namespace Noroshi.Server.Entity.Battle
{
    public class CpuBattleEntity : AbstractBattleEntity
    {
        public static CpuBattleEntity ReadAndBuild(uint id)
        {
            return ReadAndBuildMulti(new[] { id }).FirstOrDefault();
        }
        public static IEnumerable<CpuBattleEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
        {
            var battles = DaoWrapperEntity.ReadAndBuildMulti(ids);
            return battles.Select(battle => new CpuBattleEntity(battle));
        }


        readonly DaoWrapperEntity _battle;

        CpuBattleEntity(DaoWrapperEntity battle)
        {
            _battle = battle;
        }

        public uint ID => _battle.ID;
        public override uint Gold => _battle.Gold;

        public override uint GetCharacterExp()
        {
            return _battle.CharacterExp;
        }

        public override void ApplyInitialCondition(InitialCondition.CharacterCondition[][] enemyCharacterConditions)
        {
            foreach (var wave in _battle.GetWaves())
            {
                if (wave.No <= enemyCharacterConditions.Length)
                {
                    wave.ApplyInitialCondition(enemyCharacterConditions[wave.No - 1]);
                }
            }
        }

        public override IEnumerable<PossessionParam> GetDroppableRewards()
        {
            return _battle.GetDroppablePossessionParams();
        }
        public override List<List<List<PossessionParam>>> LotDropRewards()
        {
            return _battle.LotRewardsForDrop();
        }

        public List<List<PossessionParam>> LotRewardsForAutoBattle(byte count)
        {
            return _battle.LotRewardsForAutoBattle(count);
        }

        public Core.WebApi.Response.Battle.CpuBattle ToResponseData(PossessionManager possessionManager)
        {
            return _battle.ToResponseData(possessionManager);
        }

        public IEnumerable<CpuCharacterEntity> GetCpuCharacters(byte minWaveNo = 1)
        {
            return _battle.GetCpuCharacters(minWaveNo);
        }

        class DaoWrapperEntity : AbstractDaoWrapperEntity<DaoWrapperEntity, CpuBattleDao, Schema.PrimaryKey, Schema.Record>
        {
            const byte MIN_BOSS_LOT_NUM = 1;

            public static IEnumerable<DaoWrapperEntity> ReadAndBuildMulti(IEnumerable<uint> ids)
            {
                var entities = ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id }));
                if (!entities.Any())
                    return entities;
                return _loadAssociatedEntities(entities);
            }
            static IEnumerable<DaoWrapperEntity> _loadAssociatedEntities(IEnumerable<DaoWrapperEntity> entities)
            {
                var battleIds = entities.Select(entity => entity.ID);
                var waves = CpuBattleWaveEntity.ReadAndBuildMultiByBattleIDs(battleIds);
                var cpuCharacterMap = CpuCharacterEntity.ReadAndBuildMulti(waves.SelectMany(wave => wave.CpuCharacterIDs).Distinct()).ToDictionary(cc => cc.ID);
                var battleIdToWaves = waves.Select(wave =>
                {
                    wave.SetCpuCharacterMap(cpuCharacterMap);
                    return wave;
                }).ToLookup(wave => wave.BattleID);
                var gachaIds = entities.Select(entity => entity.GachaID).Distinct();
                var gachaMap = GachaEntity.ReadAndBuildMulti(gachaIds).ToDictionary(gacha => gacha.ID);
                var battleIdToStoriesMap = CpuBattleStoryEntity.ReadAndBuildByBattleIDs(battleIds).ToLookup(story => story.BattleID);

                return entities.Select(entity =>
                {
                    var gacha = gachaMap.ContainsKey(entity.GachaID) ? gachaMap[entity.GachaID] : null;
                    entity._setAssociatedEntities(battleIdToWaves[entity.ID], gacha, battleIdToStoriesMap[entity.ID]);
                    return entity;
                });
            }


            IEnumerable<CpuBattleWaveEntity> _waves;
            GachaEntity _gacha;
            IEnumerable<CpuBattleStoryEntity> _stories;


            public uint ID => _record.ID;
            public uint CharacterExp => _record.CharacterExp;
            public uint Gold => _record.Gold;
            public uint FieldID => _record.FieldID;
            public uint GachaID => _record.GachaID;

            void _setAssociatedEntities(IEnumerable<CpuBattleWaveEntity> waves, GachaEntity gacha, IEnumerable<CpuBattleStoryEntity> stories)
            {
                _waves = waves;
                _gacha = gacha;
                _stories = stories;
            }

            CpuCharacterEntity _getBoss()
            {
                var lastWave = _waves.Last();
                return lastWave.GetCpuCharacters().FirstOrDefault(cc => cc.ID == _record.BossCpuCharacterID);
            }

            int _getBossNum()
            {
                return 1;
            }
            int _getZakoNum()
            {
                return _waves.Sum(w => w.GetCpuCharacterNum()) - _getBossNum();
            }

            public IEnumerable<CpuBattleWaveEntity> GetWaves()
            {
                return _waves;
            }

            public IEnumerable<CpuCharacterEntity> GetCpuCharacters(byte minWaveNo = 1)
            {
                return _waves.Where(wave => wave.No >= minWaveNo).SelectMany(wave => wave.GetCpuCharacters());
            }

            public IEnumerable<PossessionParam> GetDroppablePossessionParams()
            {
                if (_gacha == null) return new PossessionParam[0];
                return _gacha.GetPossessionParamCandidates();
            }

            public List<List<PossessionParam>> LotRewardsForAutoBattle(byte num)
            {
                var boss = _getBoss();
                var soul = boss == null ? null : SoulEntity.ReadAndBuildByCharacterID(boss.CharacterID);
                var totalRewards = new List<List<PossessionParam>>();
                for (var i = 0; i < num; i++)
                {
                    // まずはバトルガチャ抽選数を決める。
                    var totalLotNum = ContextContainer.GetContext().RandomGenerator.Next(_record.MinDropNum, _record.MaxDropNum);
                    var bossLotNum = (byte)Math.Max((totalLotNum - _getZakoNum()), MIN_BOSS_LOT_NUM);
                    var zakoLotNum = (byte)Math.Max(totalLotNum - bossLotNum, 0);

                    // ボス報酬抽選。
                    var bossRewards = _lotBossRewards(boss, soul, bossLotNum);

                    // 非ボス報酬抽選。
                    var zakoRewards = _lotGacha(zakoLotNum);

                    // 並び替えしながらマージ。
                    var rewards = new List<PossessionParam>();
                    rewards.AddRange(bossRewards.Where(pp => pp.Category == PossessionCategory.Soul));
                    rewards.AddRange(bossRewards.Where(pp => pp.Category != PossessionCategory.Soul));
                    rewards.AddRange(zakoRewards);
                    totalRewards.Add(rewards);
                }
                return totalRewards;
            }

            public List<List<List<PossessionParam>>> LotRewardsForDrop()
            {
                var boss = _getBoss();
                var soul = boss == null ? null : SoulEntity.ReadAndBuildByCharacterID(boss.CharacterID);

                // まずはバトルガチャ抽選数を決める。
                var totalLotNum = ContextContainer.GetContext().RandomGenerator.Next(_record.MinDropNum, _record.MaxDropNum);
                var bossLotNum = (byte)Math.Max((totalLotNum - _getZakoNum()), MIN_BOSS_LOT_NUM);
                var zakoLotNum = (byte)Math.Max(totalLotNum - bossLotNum, 0);

                // ボス報酬抽選。
                var bossRewards = _lotBossRewards(boss, soul, bossLotNum);

                // 非ボス報酬抽選。
                var zakoRewards = new Queue<PossessionParam>(_lotGacha(zakoLotNum));

                // クライアントへ返すデータを構築。
                var rewardsForClient = new List<List<List<PossessionParam>>>();
                foreach (var wave in _waves.Reverse())
                {
                    var waveRewards = new List<List<PossessionParam>>();
                    foreach (var cpuCharacter in wave.GetCpuCharacters())
                    {
                        if (boss != null && cpuCharacter == boss)
                        {
                            if (bossRewards.Count > 0)
                            {
                                waveRewards.Add(new List<PossessionParam>(bossRewards));
                                bossRewards.Clear();
                            }
                            else
                            {
                                waveRewards.Add(new List<PossessionParam>());
                            }
                        }
                        else
                        {
                            if (zakoRewards.Count > 0)
                            {
                                waveRewards.Add(new List<PossessionParam> { zakoRewards.Dequeue() });
                            }
                            else
                            {
                                waveRewards.Add(new List<PossessionParam>());
                            }
                        }
                    }
                    rewardsForClient.Add(waveRewards);
                }
                return ((IEnumerable<List<List<PossessionParam>>>)rewardsForClient).Reverse().ToList();
            }
            List<PossessionParam> _lotBossRewards(CpuCharacterEntity boss, SoulEntity soul, byte gachaLotNum)
            {
                var rewards = new List<PossessionParam>();
                if (boss != null)
                {
                    if (soul != null)
                    {
                        if (ContextContainer.GetContext().RandomGenerator.Lot(_record.BossSoulDropRatio))
                        {
                            rewards.Add(new PossessionParam
                            {
                                Category = PossessionCategory.Soul,
                                ID = soul.SoulID,
                                Num = 1
                            });
                        }
                    }
                    rewards.AddRange(_lotGacha(gachaLotNum));
                }
                return rewards;
            }

            IEnumerable<PossessionParam> _lotGacha(byte lotNum)
            {
                if (_gacha == null) return new PossessionParam[0];
                var gachaItemContents = _gacha.Lot(lotNum);
                return gachaItemContents.Select(gc => gc.GetPossessableParam());
            }

            public Core.WebApi.Response.Battle.CpuBattle ToResponseData(PossessionManager possessionManager)
            {
                var boss = _getBoss();
                if (boss != null) boss.SetBoss();
                var beforeBattleStory = _stories.FirstOrDefault(story => story.Trigger == CpuBattleStoryEntity.StoryTrigger.BeforeBattle);
                var beforeBossWaveStory = _stories.FirstOrDefault(story => story.Trigger == CpuBattleStoryEntity.StoryTrigger.BeforeBossWave);
                var afterBossDieStory = _stories.FirstOrDefault(story => story.Trigger == CpuBattleStoryEntity.StoryTrigger.AfterBossDie);
                var afterBattleStory = _stories.FirstOrDefault(story => story.Trigger == CpuBattleStoryEntity.StoryTrigger.AfterBattle);
                return new Core.WebApi.Response.Battle.CpuBattle()
                {
                    ID = ID,
                    CharacterExp = CharacterExp,
                    Gold = Gold,
                    DroppablePossessionObjects = possessionManager.GetPossessionObjects(GetDroppablePossessionParams()).Select(po => po.ToResponseData()).ToArray(),
                    FieldID = FieldID,
                    Waves = _waves.Select(w => w.ToResponseData()).ToArray(),
                    BeforeBattleStory = beforeBattleStory?.ToResponseData(),
                    BeforeBossWaveStory = beforeBossWaveStory?.ToResponseData(),
                    AfterBossDieStory = afterBossDieStory?.ToResponseData(),
                    AfterBattleStory = afterBattleStory?.ToResponseData(),
                };
            }
        }
    }
}
