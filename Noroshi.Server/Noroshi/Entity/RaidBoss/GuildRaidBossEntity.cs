using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.RaidBoss;
using Noroshi.Core.Game.Battle;
using RaidBossConstant = Noroshi.Core.Game.RaidBoss.Constant;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Character;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Battle;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.RaidBoss;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildRaidBossSchema;

namespace Noroshi.Server.Entity.RaidBoss
{
    /// <summary>
    /// ギルドに出現するレイドボスを扱うクラス。
    /// </summary>
    public class GuildRaidBossEntity : AbstractDaoWrapperEntity<GuildRaidBossEntity, GuildRaidBossDao, Schema.PrimaryKey, Schema.Record>
    {
        static uint _calculateGuildClusterId(uint bp)
        {
            // TODO : 設定。
            return 1;
        }
        /// <summary>
        /// ビルドする。
        /// </summary>
        /// <param name="id">ギルドレイドボス ID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static GuildRaidBossEntity ReadAndBuild(uint id, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(new uint[] { id }, readType).FirstOrDefault();
        }
        /// <summary>
        /// ビルドする。
        /// </summary>
        /// <param name="ids">ギルドレイドボス ID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static IEnumerable<GuildRaidBossEntity> ReadAndBuildMulti(IEnumerable<uint> ids, ReadType readType = ReadType.Slave)
        {
            return _loadAssociatedEntities(ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id }), readType));
        }
        /// <summary>
        /// ギルド ID 指定で一括ビルドする。ギャップロックを掛けてしまうケースがあることに注意。
        /// </summary>
        /// <param name="guildId">対象ギルド ID</param>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static IEnumerable<GuildRaidBossEntity> ReadAndBuildAliveByGuildID(uint guildId, ReadType readType = ReadType.Slave)
        {
            var recordLifetime = RaidBossConstant.GUILD_RAID_BOSS_RECORD_LIFE_TIME;
            return _loadAssociatedEntities(_instantiate((new GuildRaidBossDao()).ReadByGuildIDAndStateWithLifetime(guildId, RaidBossState.Alive, (uint)recordLifetime.TotalSeconds, readType)));
        }
        /// <summary>
        /// 通常レイドボスの発見を試みる。
        /// </summary>
        /// <param name="playerStatus">発見試行プレイヤー</param>
        /// <returns></returns>
        public static GuildRaidBossEntity TryToDiscoveryNormal(PlayerStatusEntity playerStatus)
        {
            if (!playerStatus.GuildID.HasValue) return null;
            var todayGuildActivity = GuildActivityDailyLogEntity.ReadAndBuildTodayByGuildID(playerStatus.GuildID.Value);
            var todayBp = todayGuildActivity != null ? todayGuildActivity.BPConsuming : 0;
            var guildClusterId = _calculateGuildClusterId(todayBp);
            // アクティブなレイドボス数を取得。厳密性を担保するためにギャップロックはあきらめる。
            var activeRaidBossNum = (byte)ReadAndBuildAliveByGuildID(playerStatus.GuildID.Value, ReadType.Lock).Where(grb => grb.IsActive()).Count();
            if (activeRaidBossNum >= RaidBossConstant.MAX_ACTIVE_RAID_BOSS_NUM_PER_GUILD)
            {
                return null;
            }
            var raidBoss = RaidBossEntity.LotNormal(guildClusterId, activeRaidBossNum);
            if (raidBoss == null) return null;
            var entity = _instantiate((new GuildRaidBossDao()).Create(playerStatus.PlayerID, playerStatus.GuildID.Value, raidBoss.ID));
            entity._setRaidBoss(raidBoss);
            return entity;
        }
        /// <summary>
        /// 巨大レイドボスの発見を試みる。
        /// </summary>
        /// <param name="playerStatus">発見試行プレイヤー</param>
        /// <param name="guild">発見試行プレイヤーの所属ギルド</param>
        /// <param name="defeatedRaidBossLevel">撃破レイドボスレベル</param>
        /// <returns></returns>
        public static GuildRaidBossEntity TryToDiscoverySpecial(PlayerStatusEntity playerStatus, GuildEntity guild, byte defeatedRaidBossLevel)
        {
            if (!playerStatus.GuildID.HasValue) return null;
            if (playerStatus.GuildID.Value != guild.ID) throw new InvalidOperationException();
            var todayGuildActivity = GuildActivityDailyLogEntity.ReadAndBuildTodayByGuildID(playerStatus.GuildID.Value);
            var todayBp = todayGuildActivity != null ? todayGuildActivity.BPConsuming : 0;
            var guildClusterId = _calculateGuildClusterId(todayBp);
            // 友情ポイントを消費可能なら確実に特別ボスを引き当てる。
            var forceHit = guild.CanConsumeCooperationPoint();
            if (forceHit)
            {
                guild.ConsumeCooperationPoint();
            }
            var raidBoss = RaidBossEntity.LotSpecial(defeatedRaidBossLevel, forceHit);
            if (raidBoss == null) return null;
            var entity = _instantiate((new GuildRaidBossDao()).Create(playerStatus.PlayerID, playerStatus.GuildID.Value, raidBoss.ID));
            entity._setRaidBoss(raidBoss);
            return entity;
        }

        static IEnumerable<GuildRaidBossEntity> _loadAssociatedEntities(IEnumerable<GuildRaidBossEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var raidBossMap = RaidBossEntity.ReadAndBuildMulti(entities.Select(e => e.RaidBossID)).ToDictionary(rb => rb.ID);
            return entities.Select(entity =>
            {
                entity._setRaidBoss(raidBossMap[entity.RaidBossID]);
                return entity;
            });
        }


        RaidBossEntity _raidBoss;

        void _setRaidBoss(RaidBossEntity raidBoss)
        {
            if (RaidBossID != raidBoss.ID) throw new InvalidOperationException();
            _raidBoss = raidBoss;
        }


        /// <summary>
        /// ギルドレイドボス ID。
        /// </summary>
        public uint ID => _record.ID;
        /// <summary>
        /// ギルド ID。
        /// </summary>
        public uint GuildID => _record.GuildID;
        /// <summary>
        /// レイドボス ID。
        /// </summary>
        public uint RaidBossID => _record.RaidBossID;
        /// <summary>
        /// レイドボス状態。
        /// </summary>
        public RaidBossState State => (RaidBossState)_record.State;
        /// <summary>
        /// 出現日時。
        /// </summary>
        public uint CreatedAt => _record.CreatedAt;
        /// <summary>
        /// コンボ数。
        /// </summary>
        public uint ComboNum => _record.ComboNum;

        /// <summary>
        /// 多言語対応用テキストキー
        /// </summary>
        public string TextKey => _raidBoss.TextKey;
        /// <summary>
        /// レイドボス種別。
        /// </summary>
        public RaidBossGroupType Type => _raidBoss.Type;
        /// <summary>
        /// レベル。
        /// </summary>
        public byte Level => _raidBoss.Level;
        /// <summary>
        /// 生存期間（出現後、この期間を過ぎると逃走する）。
        /// </summary>
        public TimeSpan Lifetime => _raidBoss.Lifetime;
        /// <summary>
        /// CPU バトル ID。
        /// </summary>
        public uint CpuBattleID => _raidBoss.CpuBattleID;
        /// <summary>
        /// 発見プレイヤー ID。
        /// </summary>
        public uint DiscoveryPlayerID => _record.DiscoveryPlayerID;

        /// <summary>
        /// 逃亡日時。出現日時から算出した日時に加え、デイリーリセット日時、マスタークローズ日時を加味。
        /// </summary>
        public uint EscapedAt
        {
            get
            {
                var escapedAt = ContextContainer.GetContext().TimeHandler.UnixTime + (uint)_raidBoss.Lifetime.TotalSeconds;
                var dailyClosedAt = ContextContainer.GetContext().TimeHandler.GetNextResetUnixTime(CreatedAt);
                var closedAt = _raidBoss.ClosedAt.HasValue ? Math.Min(_raidBoss.ClosedAt.Value, dailyClosedAt) : dailyClosedAt;
                return Math.Min(escapedAt, closedAt);
            }
        }
        /// <summary>
        /// 生存、かつ逃亡前かどうか。
        /// </summary>
        /// <returns></returns>
        public bool IsActive()
        {
            return ContextContainer.GetContext().TimeHandler.UnixTime < EscapedAt && !IsDefeated();
        }
        /// <summary>
        /// 撃破済みか。
        /// </summary>
        /// <returns></returns>
        public bool IsDefeated()
        {
            return State == RaidBossState.Defeated;
        }
        /// <summary>
        /// 最大 HP（登場 CPU キャラクターの最大 HP 合計値）
        /// </summary>
        public uint MaxHP => _calculateHP(_raidBoss.GetCpuCharacters());
        /// <summary>
        /// 現在 HP（登場 CPU キャラクターの現売 HP 合計値）
        /// </summary>
        public uint CurrentHP
        {
            get
            {
                if (IsDefeated()) return 0;
                var battleData = _getBattleData();
                return _calculateHP(_raidBoss.GetCpuCharacters(battleData?.WaveNo ?? 0 + 1)) + (uint)(battleData?.HPs.Sum(hp => hp) ?? 0);
            }
        }
        uint _calculateHP(IEnumerable<CpuCharacterEntity> cpuCharacters)
        {
            var characterMap = CharacterEntity.ReadAndBuildMulti(cpuCharacters.Select(cc => cc.CharacterID)).ToDictionary(c => c.ID);
            return (uint)cpuCharacters.Sum(cc =>
            {
                var status = new Core.Game.Character.CharacterStatus(cc.ToResponseData(), characterMap[cc.CharacterID].ToResponseData());
                return status.MaxHP;
            });
        }

        /// <summary>
        /// バトル初期状態を取得する。前回までのバトル結果を反映したものを返す。
        /// </summary>
        /// <param name="bp">消費 BP</param>
        /// <returns></returns>
        public InitialCondition GetBattleInitialCondition(IEnumerable<uint> playerCharacterIds, byte bp)
        {
            var battleData = _getBattleData();
            if (battleData == null) return null;
            var initialCondition = new InitialCondition
            {
                OwnPlayerCharacterConditions = playerCharacterIds.Select(id => new InitialCondition.PlayerCharacterCondition
                {
                    PlayerCharacterID = id,
                    DamageCoefficient = GetDamageCoefficient(bp),
                }).ToArray(),
                EnemyCharacterConditions = new InitialCondition.CharacterCondition[][] { battleData.HPs.Select(hp => new InitialCondition.CharacterCondition { HP = hp }).ToArray() },
            };
            return initialCondition;
        }

        /// <summary>
        /// バトル内ダメージ係数。
        /// </summary>
        /// <param name="bp">使用BP</param>
        /// <returns></returns>
        public float GetDamageCoefficient(byte bp)
        {
            var bpCoefficient = RaidBossConstant.CONSUMING_BP_TO_DAMAGE_COEFFICIENT_MAP[bp];
            // TODO : コンボ係数計算。
            var comboCoefficient = (Type != RaidBossGroupType.Special) ? 1 : (1 + (float)ComboNum / 10);
            return comboCoefficient * bpCoefficient;
        }

        /// <summary>
        /// 獲得し得る全ての報酬。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PossessionParam> GetAllRewards()
        {
            return _raidBoss.GetAllRewards().Select(r => r.GetPossessionParam());
        }
        /// <summary>
        /// 獲得し得る発見者報酬。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PossessionParam> GetDiscoveryRewards()
        {
            return _raidBoss.GetDiscoveryRewards().Select(r => r.GetPossessionParam());
        }
        /// <summary>
        /// 獲得し得る参加者報酬。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PossessionParam> GetEntryRewards()
        {
            return _raidBoss.GetEntryRewards().Select(r => r.GetPossessionParam());
        }
        /// <summary>
        /// 発見者報酬を抽選する。
        /// </summary>
        /// <param name="playerGuildRaidBoss">プレイヤーギルドレイドボス情報</param>
        /// <param name="vipLevel">VIP レベル</param>
        /// <returns></returns>
        public IEnumerable<PossessionParam> LotAcquirableDiscoveryRewards(PlayerGuildRaidBossEntity playerGuildRaidBoss, ushort vipLevel)
        {
            if (playerGuildRaidBoss == null || DiscoveryPlayerID != playerGuildRaidBoss.PlayerID) return new PossessionParam[0];
            var candidates = _raidBoss.GetDiscoveryRewards();
            return candidates.Where(r => ContextContainer.GetContext().RandomGenerator.Lot(r.Probability)).Select(r => r.GetPossessionParam());
        }
        /// <summary>
        /// 参加者報酬を抽選する。
        /// </summary>
        /// <param name="playerGuildRaidBoss">プレイヤーギルドレイドボス情報</param>
        /// <param name="vipLevel">VIP レベル</param>
        /// <returns></returns>
        public IEnumerable<PossessionParam> LotAcquirableEntryRewards(PlayerGuildRaidBossEntity playerGuildRaidBoss, ushort vipLevel)
        {
            if (playerGuildRaidBoss == null) return new PossessionParam[0];
            var damageRatio = (float)playerGuildRaidBoss.Damage / MaxHP;
            var candidates = _raidBoss.GetEntryRewards();
            var rewards = candidates.Where(r => ContextContainer.GetContext().RandomGenerator.Lot(r.Probability)).Select(r => r.GetPossessionParam());
            return _applyDamageRatioToRewards(rewards, damageRatio);
        }
        IEnumerable<PossessionParam> _applyDamageRatioToRewards(IEnumerable<PossessionParam> rewards, float damageRatio)
        {
            // TODO : 計算式
            var coefficient = (1 + damageRatio);
            return rewards.Select(pp =>
            {
                // ギルドポイントだけ付与数を補正する。
                if (PossessionManager.IsGuildPointParam(pp))
                {
                    pp.Num = (uint)(pp.Num * coefficient);
                }
                return pp;
            });
        }

        BattleData _getBattleData()
        {
            return _deserializeFromText<BattleData>(_record.BattleData);
        }
        EntryData _getEntryData()
        {
            return _deserializeFromText<EntryData>(_record.EntryData);
        }

        /// <summary>
        /// バトルデータをセット。
        /// </summary>
        /// <param name="playerId">バトルプレイヤー ID</param>
        /// <param name="waveNo">バトル終了時のウェーブ番号</param>
        /// <param name="enemyCharacterStates">敵（レイドボス側）キャラクター状態</param>
        /// <returns>計算上、撃破したら真を返す。</returns>
        public bool SetBattleData(uint playerId, byte waveNo, BattleResult.EnemyCharacterState[] enemyCharacterStates)
        {
            _setEntryData(playerId);
            // 敵毎にダメージ減算。
            var battleData = _getBattleData();
            var nextHPs = battleData != null
                ? enemyCharacterStates.Select((ecs, i) =>
                {
                    return Math.Max(battleData.HPs[i] - (ecs.InitialHP - ecs.RemainingHP), 0);
                }).ToArray()
                : enemyCharacterStates.Select(ecs => ecs.RemainingHP).ToArray();
            if (nextHPs.All(hp => hp == 0))
            {
                return true;
            }
            var newRecord = _cloneRecord();
            newRecord.BattleData = _serializeToText(new BattleData
            {
                WaveNo = waveNo,
                HPs = nextHPs,
            });
            _changeLocalRecord(newRecord);
            return false;
        }
        /// <summary>
        /// 撃破する。
        /// </summary>
        /// <param name="playerId">撃破プレイヤー ID</param>
        /// <param name="guild">撃破プレイヤー所属ギルド</param>
        /// <returns>重複なしの初回撃破時のみ真が返る。</returns>
        public bool Defeat(uint playerId, GuildEntity guild)
        {
            // 重複撃破もあり得る。
            if (State == RaidBossState.Defeated) return false;
            _setEntryData(playerId);
            var newRecord = _cloneRecord();
            newRecord.State = (byte)RaidBossState.Defeated;
            newRecord.DefeatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);
            var battlePlayerNum = (byte)_getEntryData().BattlePlayerIDs.Count();
            guild.AddConsumeCooperationPoint(_getCooperationPoint(battlePlayerNum));
            // 撃破数カウントアップ。
            GuildActivityDailyLogEntity.IncrementTodayDefeatRaidBossNum(ID);
            return true;
        }
        void _setEntryData(uint playerId)
        {
            var previousEntryData = _getEntryData();
            var newRecord = _cloneRecord();
            if (_record.LastBattlePlayerID != playerId) newRecord.ComboNum++;
            newRecord.LastBattlePlayerID = playerId;
            var battlePlayerIds = previousEntryData != null ? new List<uint>(previousEntryData.BattlePlayerIDs) : new List<uint>();
            battlePlayerIds.Add(playerId);
            newRecord.EntryData = _serializeToText(new EntryData
            {
                BattlePlayerIDs = battlePlayerIds.Distinct().ToArray(),
            });
            _changeLocalRecord(newRecord);
        }
        ushort _getCooperationPoint(byte battlePlayerNum)
        {
            return RaidBossConstant.MIN_RAID_BOSS_BATTLE_PLAYER_NUM_TO_COOPERATION_POIN_MAP
                .OrderByDescending(kv => kv.Key)
                .First(kv => kv.Key <= battlePlayerNum)
                .Value;
        }

        /// <summary>
        /// バトル可否チェック。
        /// </summary>
        /// <param name="playerStatus">バトルプレイヤー</param>
        /// <param name="playerGuildRaidBoss">プレイヤーギルドレイドボス状態</param>
        /// <param name="bp">消費 BP</param>
        /// <returns></returns>
        public bool CanBattle(PlayerStatusEntity playerStatus, PlayerGuildRaidBossEntity playerGuildRaidBoss, uint bp)
        {
            if (!playerStatus.GuildID.HasValue || playerStatus.GuildID.Value != GuildID) return false;
            if (!IsActive()) return false;
            // 発見者は初回だけ BP なしを許可。
            if (bp == 0)
            {
                if (DiscoveryPlayerID == playerStatus.PlayerID && playerGuildRaidBoss.State == PlayerGuildRaidBossState.None)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// 報酬受け取り可否チェック。
        /// </summary>
        /// <param name="playerGuildRaidBoss">プレイヤーギルドレイドボス状態</param>
        /// <param name="vipLevel">VIP レベル</param>
        /// <returns></returns>
        public bool CanReceiveRewards(PlayerGuildRaidBossEntity playerGuildRaidBoss, ushort vipLevel)
        {
            if (!playerGuildRaidBoss.CanReceiveRewards()) return false;
            return IsDefeated();
        }
        /// <summary>
        /// 報酬を受け取る。
        /// </summary>
        /// <param name="playerGuildRaidBoss">プレイヤーギルドレイドボス状態</param>
        public void ReceiveRewards(PlayerGuildRaidBossEntity playerGuildRaidBoss)
        {
            playerGuildRaidBoss.ReceiveRewards();
        }

        public Core.WebApi.Response.RaidBoss.RaidBoss ToResponseData(PlayerGuildRaidBossEntity playerGuildRaidBoss, PossessionManager possessionManager)
        {
            return new Core.WebApi.Response.RaidBoss.RaidBoss
            {
                ID = ID,
                RaidBossID = RaidBossID,
                CreatedAt = CreatedAt,
                ComboNum = ComboNum,
                TextKey = _raidBoss.TextKey,
                Type = Type,
                Level = Level,
                Lifetime = (uint)Lifetime.TotalSeconds,
                EscapedAt = EscapedAt,
                IsDefeated = IsDefeated(),
                CurrentHP = CurrentHP,
                MaxHP = MaxHP,                
                DiscoveryRewards = possessionManager.GetPossessionObjects(GetDiscoveryRewards()).Select(po => po.ToResponseData()).ToArray(),
                EntryRewards = possessionManager.GetPossessionObjects(GetEntryRewards()).Select(po => po.ToResponseData()).ToArray(),
                OwnPlayerDamage = playerGuildRaidBoss?.Damage,
            };
        }

        class BattleData
        {
            public byte WaveNo { get; set; }
            public uint[] HPs { get; set; }
        }
        class EntryData
        {
            public uint[] BattlePlayerIDs { get; set; }
        }
    }
}
