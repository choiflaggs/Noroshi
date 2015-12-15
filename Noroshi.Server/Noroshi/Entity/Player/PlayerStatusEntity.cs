using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.Player;
using Noroshi.Core.Game.Guild;
using PlayerConstant = Noroshi.Core.Game.Player.Constant;
using GuildConstant = Noroshi.Core.Game.Guild.Constant;
using Noroshi.Core.WebApi.Response;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Entity.Guild;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Player;
using Noroshi.Server.Entity.Quest;
using Noroshi.Server.Entity.Shop;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.PlayerStatusSchema;

namespace Noroshi.Server.Entity.Player
{
    public class PlayerStatusEntity : AbstractDaoWrapperEntity<PlayerStatusEntity, PlayerStatusDao, Schema.PrimaryKey, Schema.Record>
    {
        public static IEnumerable<PlayerStatusEntity> ReadAndBuildMulti(IEnumerable<uint> playerIds, ReadType readType = ReadType.Slave)
        {
            return _loadAssociatedEntities(ReadAndBuildMulti(playerIds.Select(playerId => new Schema.PrimaryKey { PlayerID = playerId }), readType), readType);
        }
        public static PlayerStatusEntity ReadAndBuild(uint playerId, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(new[] { playerId }, readType).FirstOrDefault();
        }
        public static IEnumerable<PlayerStatusEntity> ReadAndBuildByGuildID(uint guildId, byte rowCount, ReadType readType = ReadType.Slave)
        {
            return _loadAssociatedEntities(_instantiate((new PlayerStatusDao()).ReadByGuildID(guildId, rowCount, readType)), readType);
        }

        public static PlayerStatusEntity Create(uint playerId, uint languageId, uint shardId)
        {
            return _instantiate((new PlayerStatusDao()).Create(playerId, languageId, shardId));
        }

        public static IEnumerable<PlayerStatusEntity> ReadAndBuildByLevelRange(ushort minLevel, ushort maxLevel, byte rowCount)
        {
            // プレイヤーレベルはデータとして保有していないので経験値に変換してから検索する。
            return _loadAssociatedEntities(_instantiate((new PlayerStatusDao()).ReadByExpRange(PlayerLevelEntity.GetNecessaryExp(minLevel), PlayerLevelEntity.GetNecessaryExp(maxLevel), rowCount)), ReadType.Slave);
        }

        static PlayerStatusEntity _loadAssociatedEntities(PlayerStatusEntity entity, ReadType readType)
        {
            return _loadAssociatedEntities(new[] { entity }, readType).FirstOrDefault();
        }
        static IEnumerable<PlayerStatusEntity> _loadAssociatedEntities(IEnumerable<PlayerStatusEntity> entities, ReadType readType)
        {
            if (entities.Count() == 0) return entities;
            if (readType != ReadType.Lock) return entities;
            var guildIds = entities.Where(entity => entity.GuildID.HasValue).Select(entity => entity.GuildID.Value).Distinct();
            var todayActivityLogMap = PlayerActivityDailyLogEntity.CreateOrReadAndBuildTodayActivityLogs(entities.Select(entity => entity.PlayerID)).ToDictionary(padl => padl.PlayerID);
            var todayGuildActivityLogMap = guildIds.Count() > 0 ? GuildActivityDailyLogEntity.CreateOrReadAndBuildTodayActivityLogs(guildIds).ToDictionary(gadl => gadl.GuildID) : new Dictionary<uint, GuildActivityDailyLogEntity>();
            return entities.Select(entity =>
            {
                var todayGuildActivityLog = entity.GuildID.HasValue && todayGuildActivityLogMap.ContainsKey(entity.GuildID.Value) ? todayGuildActivityLogMap[entity.GuildID.Value] : null;
                entity._setTodayActivityLog(todayActivityLogMap[entity.PlayerID], todayGuildActivityLog);
                return entity;
            });
        }

        /// <summary>
        /// チュートリアルステップの進捗を試みる。
        /// 内部で PlayerStatus をロックを掛けてビルド、更新した上で破棄してしまうので、
        /// 外で PlayerStatus を利用している場合は本メソッドを使ってはいけない。
        /// </summary>
        /// <param name="playerId">対象プレイヤーID</param>
        /// <param name="tutorialStep">進めたいステップ</param>
        public static void TryToProceedTutorialStep(uint playerId, TutorialStep tutorialStep)
        {
            var playerStatus = ReadAndBuild(playerId, ReadType.Lock);
            var updatedTutorialStep = playerStatus.TryToProceedTutorialStep(tutorialStep);
            if (updatedTutorialStep.HasValue && !playerStatus.Save()) throw new SystemException(string.Join("\t", "Fail to Save", playerId));
        }

        StaminaHandler _staminaHandler = new StaminaHandler();
        ActionLevelPointHandler _actionLevelPointHandler = new ActionLevelPointHandler();
        BPHandler _bpHandler = new BPHandler();
        TutorialStepHandler _tutorialStep;
        PlayerActivityDailyLogEntity _todayActivityLog;
        GuildActivityDailyLogEntity _todayGuildActivityLog;

        public uint PlayerID => _record.PlayerID;
        public uint LanguageID => _record.LanguageID;
        public string Name => _record.Name;
        public uint Exp => _record.Exp;
        public uint VipExp => _record.VipExp;
        /// <summary>
        /// 所属ギルド ID。ギルド未所属の場合は null となる。
        /// </summary>
        public uint? GuildID => _record.GuildID > 0 ? (uint?)_record.GuildID : null;
        /// <summary>
        /// 所属ギルドにおける役割。一般メンバーの場合は null となる。
        /// </summary>
        public GuildRole? GuildRole => _record.GuildRole > 0 ? (GuildRole?)_record.GuildRole : null;
        public PlayerGuildState GuildState => (PlayerGuildState)_record.GuildState;
        public uint Gold => _record.Gold;
        public uint FreeGem => _record.FreeGem;
        public uint ChargeGem => _record.ChargeGem;
        public uint TotalGem => FreeGem + ChargeGem;
        public uint AvaterCharacterID => _record.AvaterCharacterID;
        public ushort Level => PlayerLevelEntity.GetLevel(Exp);
        public ushort VipLevel => PlayerVipLevelEntity.GetLevel(VipExp);
        public uint GuildPoint => _record.GuildPoint;
        public uint ExpeditionPoint => _record.ExpeditionPoint;
        public uint SoulPoint => _record.SoulPoint;

        private ushort _levelUpNum = 0;

        public PlayerVipLevelBonusEntity PlayerVipLevelBonus => _getVipLevelBonus();

        PlayerVipLevelBonusEntity _getVipLevelBonus()
        {
            return PlayerVipLevelBonusEntity.ReadAndBuild(VipLevel);
        }

        public uint BeginnerGuildDroppedOutAt => _record.BeginnerGuildDroppedOutOn;

        void _setTodayActivityLog(PlayerActivityDailyLogEntity todayActivityLog, GuildActivityDailyLogEntity todayGuildActivityLog)
        {
            if (todayActivityLog.PlayerID != PlayerID)
            {
                throw new InvalidOperationException();
            }
            _todayActivityLog = todayActivityLog;
            if (GuildID.HasValue)
            {
                if (todayGuildActivityLog.GuildID != GuildID.Value)
                {
                    throw new InvalidOperationException();
                }
                _todayGuildActivityLog = todayGuildActivityLog;
            }
        }

        public PlayerStatus ToResponseData()
        {
            return new PlayerStatus
            {
                PlayerID = PlayerID,
                LanguageID = LanguageID,
                Name = Name,
                Level = Level,
                Exp = Exp,
                ExpInLevel = _getExpInLevel(),
                VipLevel = VipLevel,
                VipExp = VipExp,
                VipExpInLevel = _getVipExpInLevel(),
                LastStamina = _record.LastStamina,
                LastStaminaUpdatedAt = _record.LastStaminaUpdatedAt,
                LastStaminaRecoveryNum = _record.LastStaminaRecoveryNum,
                LastStaminaRecoveredAt = _record.LastStaminaRecoveredAt,
                LastGoldRecoveryNum = _record.LastGoldRecoveryNum,
                LastGoldRecoveredAt = _record.LastGoldRecoveredAt,
                MaxStamina = MaxStamina,
                LastBP = _record.LastBP,
                LastBPUpdatedAt = _record.LastBPUpdatedAt,
                LastBPRecoveryNum = _record.LastBPRecoveryNum,
                LastBPRecoveredAt = _record.LastBPRecoveredAt,
                LastActionLevelPoint = _record.LastActionLevelPoint,
                LastActionLevelPointUpdatedAt = _record.LastActionLevelPointUpdatedAt,
                LastActionLevelPointRecoveredAt = _record.LastActionLevelPointRecoveredAt,
                LastActionLevelPointRecoveryNum = _record.LastActionLevelPointRecoveryNum,
                MaxBP = MaxBP,
                Gold = Gold,
                Gem = TotalGem,
                AvaterCharacterID = AvaterCharacterID,
                GuildID = GuildID,
                GuildRole = GuildRole,
                TutorialStep = _record.TutorialStep
            };
        }
        public OtherPlayerStatus ToOtherResponseData()
        {
            return new OtherPlayerStatus
            {
                ID = PlayerID,
                Level = Level,
                Name = Name,
                AvaterCharacterID = AvaterCharacterID,
                GuildID = GuildID,
                GuildRole = GuildRole,
            };
        }
        public Core.WebApi.Response.Guild.GuildMemberPlayerStatus ToGuildMemberResponseData(bool canGreet)
        {
            return new Core.WebApi.Response.Guild.GuildMemberPlayerStatus
            {
                ID = PlayerID,
                Level = Level,
                Name = Name,
                AvaterCharacterID = AvaterCharacterID,
                GuildID = GuildID,
                GuildRole = GuildRole,
                CanGreet = canGreet,
            };
        }

        public PlayerChangeStatusResponse ToLevelUpResponseData()
        {
            return new PlayerChangeStatusResponse
            {
                Exp = Exp,
                Level = Level,
                Stamina = Stamina,
                StaminaMaxCount = MaxStamina
            };
        }

        public IEnumerable<Core.Game.GameContent.GameContent> GetOpenGameContents()
        {
            return Core.Game.GameContent.GameContent.BuildOpenGameContentsByPlayerLevel(Level);
        }

        public TutorialStepHandler GetTutorialStep()
        {
            if (_tutorialStep == null)
            {
                _tutorialStep = new TutorialStepHandler(_record.TutorialStep);
            }
            return _tutorialStep;
        }

        public bool CanLotTutorialGacha()
        {
            return GetTutorialStep().CanLotTutorialGacha;
        }
        public TutorialStep? TryToProceedTutorialStepByClearStageID(uint clearStageId)
        {
            return _tryToProceedTutorialStep(() => _tutorialStep.ProceedByClearStageID(clearStageId));
        }
        public TutorialStep? TryToProceedTutorialStep(TutorialStep nextStep)
        {
            return _tryToProceedTutorialStep(() => _tutorialStep.Proceed(nextStep));
        }
        TutorialStep? _tryToProceedTutorialStep(Func<bool> proceedTutorial)
        {
            var tutorialStep = GetTutorialStep();
            if (proceedTutorial())
            {
                var newRecord = _cloneRecord();
                newRecord.TutorialStep = tutorialStep.Step;
                _changeLocalRecord(newRecord);
                return (TutorialStep)newRecord.TutorialStep;
            }
            return null;
        }

        /// <summary>
        /// ギルド ID をセットする。
        /// </summary>
        /// <param name="guildId"></param>
        public void SetGuildID(uint guildId)
        {
            var record = _cloneRecord();
            record.GuildID = guildId;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// ギルドから離脱する。
        /// </summary>
        public void DropOutFromGuild()
        {
            SetGuildID(0);
            // 役割も外す。
            RemoveGuildRole();
        }

        /// <summary>
        /// 所属ギルドにおける役割をセットする。
        /// </summary>
        /// <param name="guildRole">所属ギルドにおける役割</param>
        public void SetGuildRole(GuildRole guildRole)
        {
            _setGuildRole((byte)guildRole);
        }
        /// <summary>
        /// 所属ギルドにおける役割を外す。
        /// </summary>
        public void RemoveGuildRole()
        {
            _setGuildRole(0);
        }
        public void _setGuildRole(byte guildRole)
        {
            var record = _cloneRecord();
            record.GuildRole = guildRole;
            _changeLocalRecord(record);
        }
        public void SetGuildState(PlayerGuildState guildState)
        {
            var record = _cloneRecord();
            record.GuildState = (byte)guildState;
            record.UpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }
        public bool CanJoinBeginnerGuildChat()
        {
            // 初期値は初心者ギルドに在籍中.
            if (BeginnerGuildDroppedOutAt == 0)
            {
                return true;
            }
           
            return ContextContainer.GetContext().TimeHandler.UnixTime <= BeginnerGuildDroppedOutAt + GuildConstant.BEGINNER_GUILD_CHAT_DROPOUT_AFTER_SPAN.TotalSeconds;
        }
        public bool CanJoinNormaGuildChat(uint guildId)
        {
            if ( GuildID.HasValue == false || GuildID == 0 || GuildID != guildId)
            {
                return false;
            }
            return true;
        }
        public void SetBeginnerGuildDroppedOutAt()
        {
            var record = _cloneRecord();
            record.BeginnerGuildDroppedOutOn = ContextContainer.GetContext().TimeHandler.DayStartUnixTimeInUTC;
            _changeLocalRecord(record);
        }

        /// <summary>
        /// 挨拶数（日時リセットあり）。
        /// </summary>
        public byte GreetingNum => (byte)(ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastGreetedAt) ? 0 : _record.LastGreetingNum);
        /// <summary>
        /// 挨拶数インクリメント。
        /// </summary>>
        public void IncrementGreetingNum()
        {
            var newRecord = _cloneRecord();
            newRecord.LastGreetingNum = (byte)(GreetingNum + 1);
            newRecord.LastGreetedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);

        }
        /// <summary>
        /// 未確認の被挨拶数。
        /// </summary>>
        public uint UnconfirmedGreetedNum => _record.UnconfirmedGreetedNum;
        /// <summary>
        /// 未確認の被挨拶数インクリメント。
        /// </summary>>
        public void IncrementUnconfirmedGreetedNum()
        {
            _setUnconfirmedGreetedNum(UnconfirmedGreetedNum + 1);
        }
        /// <summary>
        /// 未確認の被挨拶数リセット。
        /// </summary>>
        public void ResetUnconfirmedGreetedNum()
        {
            _setUnconfirmedGreetedNum(0);
        }
        void _setUnconfirmedGreetedNum(uint unconfirmedGreetedNum)
        {
            var newRecord = _cloneRecord();
            newRecord.UnconfirmedGreetedNum = unconfirmedGreetedNum;
            _changeLocalRecord(newRecord);
        }
        /// <summary>
        /// 被挨拶報酬受け取り可否チェック。
        /// </summary>
        /// <returns></returns>>
        public bool CanReceiveGreetedReward()
        {
            return UnconfirmedGreetedNum > 0;
        }
        /// <summary>
        /// 被挨拶報酬取得。
        /// </summary>
        /// <returns></returns>>
        public IEnumerable<PossessionParam> GetReceivableGreetedRewardPossessionParams()
        {
            return new[] { PossessionManager.GetGuildPointParam((ushort)(GuildConstant.GUILD_POINT_PER_GREETED * UnconfirmedGreetedNum * ( PlayerVipLevelBonus.GuildPointBonus + 1.00))) };
        }

        /// <summary>
        /// 傭兵雇用報酬受け取り数（日次リセットあり）。
        /// </summary>>
        public byte RentalRewardReceivingNum => (byte)(ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastRentalRewardReceivedAt) ? 0 : _record.LastRentalRewardReceivingNum);
        /// <summary>
        /// 傭兵雇用報酬受け取り数をインクリメント。
        /// </summary>>
        public void IncrementRentalRewardReceivingNum()
        {
            var newRecord = _cloneRecord();
            newRecord.LastRentalRewardReceivingNum = (byte)(RentalRewardReceivingNum + 1);
            _changeLocalRecord(newRecord);
        }

        public byte MaxActionLevelPoint => PlayerVipLevelBonus.MaxActionLevelPointRecoveryNum;
        public ushort ActionLevelPoint => _actionLevelPointHandler.CurrentValue(_record.LastActionLevelPoint, _record.LastActionLevelPointUpdatedAt, MaxActionLevelPoint, ContextContainer.GetContext().TimeHandler.UnixTime);

        public void SetActionLevelPoint(byte actionLevelPoint)
        {
            var record = _cloneRecord();
            record.LastActionLevelPoint = actionLevelPoint;
            record.LastActionLevelPointUpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }

        public uint ActionLevelPointRecoveryNum => (uint) (ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastActionLevelPointRecoveredAt) ? 0 : _record.LastActionLevelPointRecoveryNum);
        public void IncrementActionLevelPointRecoveryNum()
        {
            var record = _cloneRecord();
            record.LastActionLevelPointRecoveryNum = (byte)(ActionLevelPointRecoveryNum + 1);
            record.LastActionLevelPointRecoveredAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }

        public bool ConsumeActionLevelPoint(byte actionLevelPoint)
        {
            if (ActionLevelPoint < actionLevelPoint) return false;
            SetActionLevelPoint((byte)(ActionLevelPoint - actionLevelPoint));
            return true;
        }


        public ushort MaxStamina => _staminaHandler.MaxValue(Level);
        public ushort Stamina => _staminaHandler.CurrentValue(Level, _record.LastStamina, _record.LastStaminaUpdatedAt, ContextContainer.GetContext().TimeHandler.UnixTime);

        public void SetStamina(ushort stamina)
        {
            var record = _cloneRecord();
            record.LastStamina = stamina;
            record.LastStaminaUpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }
        public bool ConsumeStamina(ushort stamina)
        {
            if (Stamina < stamina) return false;
            SetStamina((ushort)(Stamina - stamina));
            // アクティビティログにも記録。
            _todayActivityLog.ConsumeStamina(stamina);
            return true;
        }

        public uint StaminaRecoveryNum => (uint)(ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastStaminaRecoveredAt) ? 0 : _record.LastStaminaRecoveryNum);
        public void IncrementStaminaRecoveryNum()
        {
            var record = _cloneRecord();
            record.LastStaminaRecoveryNum = (byte)(StaminaRecoveryNum + 1);
            record.LastStaminaRecoveredAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }


        public uint GoldRecoveryNum => (uint)(ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastGoldRecoveredAt) ? 0 : _record.LastGoldRecoveryNum);
        public void IncrementGoldRecoveryNum()
        {
            var record = _cloneRecord();
            record.LastGoldRecoveryNum = (byte)(GoldRecoveryNum + 1);
            record.LastGoldRecoveredAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }

        public byte MaxBP => _bpHandler.MaxValue();
        public byte BP => _bpHandler.CurrentValue(_record.LastBP, _record.LastBPUpdatedAt, ContextContainer.GetContext().TimeHandler.UnixTime);

        public void SetBP(byte bp)
        {
            var record = _cloneRecord();
            record.LastBP = bp;
            record.LastBPUpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }
        public bool ConsumeBP(byte bp)
        {
            if (BP < bp) return false;
            SetBP((byte)(BP - bp));
            // アクティビティログにも記録。
            _todayActivityLog.ConsumeBP(bp);
            _todayGuildActivityLog.ConsumeBP(bp);
            return true;
        }
        public uint BPRecaveryNum => (uint)(ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastBPRecoveredAt) ? 0 : _record.LastBPRecoveryNum);
        public uint GoldRecaveryNum => (ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastGoldRecoveredAt) ? 0 : _record.LastGoldRecoveryNum);
        public void IncrementBPRecaveryNum()
        {
            var record = _cloneRecord();
            record.LastBPRecoveryNum  = (byte)(BPRecaveryNum + 1);
            record.LastBPRecoveredAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(record);
        }


        public void UseGem(uint gem)
        {
            if (gem > TotalGem) {
                throw new InvalidOperationException();
            }
            if (gem < FreeGem)
            {
                ChangeFreeGem(FreeGem - gem);
                return;
            }
            var notEnoughGem = gem - FreeGem;
            ChangeFreeGem(0);
            ChangeChargeGem(ChargeGem - notEnoughGem);
        }

        // チェックが呼び出し側に強制してしまうので廃止予定。
        public void ChangeGold(uint gold)
        {
            var record = _cloneRecord();
            record.Gold = gold;
            _changeLocalRecord(record);
        }

        public void SetExp(uint playerExp)
        {
            var newRecord = _cloneRecord();
            newRecord.Exp = playerExp;
            var maxExp = PlayerLevelEntity.GetMaxExp();
            if (newRecord.Exp > maxExp)
            {
                newRecord.Exp = maxExp;
            }
            _changeLocalRecord(newRecord);
        }
        public AddPlayerExpResult AddExp(uint playerExp)
        {
            var previousLevel = Level;
            var previousMaxStamina = MaxStamina;
            SetExp(Exp + playerExp);
            if (previousLevel < Level)
            {
                SetStamina(MaxStamina);

                if (_levelUpNum != 0) throw new InvalidOperationException();
                // ミッション：プレイヤーLevelUp.
                _levelUpNum = (ushort)( Level - previousLevel );
            }
            return new AddPlayerExpResult(previousLevel, Level, previousMaxStamina, MaxStamina);
        }
        public void SetVipExp(uint vipExp)
        {
            var newRecord = _cloneRecord();
            newRecord.VipExp = vipExp;
            var maxExp = PlayerVipLevelEntity.GetMaxExp();
            if (newRecord.VipExp > maxExp)
            {
                newRecord.VipExp = maxExp;
            }
            _changeLocalRecord(newRecord);
        }
        public AddPlayerVipExpResult AddVipExp(uint vipExp)
        {
            var previousVipLevel = VipLevel;
            SetVipExp(Exp + vipExp);
            return new AddPlayerVipExpResult(previousVipLevel, VipLevel);
        }
        uint _getExpInLevel()
        {
            return PlayerLevelEntity.GetExpInLevel(Level, Exp);
        }
        uint _getVipExpInLevel()
        {
            return PlayerVipLevelEntity.GetExpInLevel(VipLevel, VipExp);
        }


        public void ChangeFreeGem(uint gem)
        {
            var record = _cloneRecord();
            record.FreeGem = gem;
            _changeLocalRecord(record);
        }

        public void ChangeChargeGem(uint gem)
        {
            var record = _cloneRecord();
            record.ChargeGem = gem;
            _changeLocalRecord(record);
        }

        public void ChangeAvaterCharacterID(ushort id)
        {
            var record = _cloneRecord();
            record.AvaterCharacterID = id;
            _changeLocalRecord(record);
        }

        public void ChangeName(string name)
        {
            var record = _cloneRecord();
            record.Name = name;
            _changeLocalRecord(record);
        }

        /// <summary>
        /// ゴールド付与。
        /// </summary>
        /// <param name="gold">付与ゴールド</param>
        /// <returns></returns>
        public uint AddGold(uint gold)
        {
            gold = Math.Min(gold, PlayerConstant.MAX_GOLD - Gold);
            _setGold(Gold + gold);
            return gold;
        }
        /// <summary>
        /// ゴールド消費。
        /// </summary>
        /// <param name="gold">消費ゴールド</param>
        public void ConsumeGold(uint gold)
        {
            if (Gold < gold) throw new InvalidOperationException();
            _setGold(Gold - gold);
        }
        void _setGold(uint gold)
        {
            var record = _cloneRecord();
            record.Gold = gold;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// ギルドポイント付与。
        /// </summary>
        /// <param name="guildPoint">付与ギルドポイント</param>
        /// <returns></returns>
        public uint AddGuildPoint(uint guildPoint)
        {
            guildPoint = Math.Min(guildPoint, PlayerConstant.MAX_GUILD_POINT - GuildPoint);
            _setGuildPoint(GuildPoint + guildPoint);
            return guildPoint;
        }
        /// <summary>
        /// ギルドポイント消費。
        /// </summary>
        /// <param name="guildPoint">消費ギルドポイント</param>
        public void ConsumeGuildPoint(uint guildPoint)
        {
            if (GuildPoint < guildPoint) throw new InvalidOperationException();
            _setGuildPoint(GuildPoint - guildPoint);
        }
        void _setGuildPoint(uint guildPoint)
        {
            var record = _cloneRecord();
            record.GuildPoint = guildPoint;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// 冒険ポイント付与。
        /// </summary>
        /// <param name="expeditionPoint">付与冒険ポイント</param>
        /// <returns></returns>
        public uint AddExpeditionPoint(uint expeditionPoint)
        {
            expeditionPoint = Math.Min(expeditionPoint, PlayerConstant.MAX_EXPEDITION_POINT - ExpeditionPoint);
            _setExpeditionPointPoint(ExpeditionPoint + expeditionPoint);
            return expeditionPoint;
        }
        /// <summary>
        /// 冒険ポイント消費。
        /// </summary>
        /// <param name="expeditionPoint">消費冒険ポイント</param>
        public void ConsumeExpeditionPointPoint(uint expeditionPoint)
        {
            if (ExpeditionPoint < expeditionPoint) throw new InvalidOperationException();
            _setExpeditionPointPoint(ExpeditionPoint - expeditionPoint);
        }
        void _setExpeditionPointPoint(uint expeditionPoint)
        {
            var record = _cloneRecord();
            record.ExpeditionPoint = expeditionPoint;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// ソウルポイント付与。
        /// </summary>
        /// <param name="soulPoint">付与ソウルポイント</param>
        /// <returns></returns>
        public uint AddSoulPoint(uint soulPoint)
        {
            soulPoint = Math.Min(soulPoint, PlayerConstant.MAX_SOUL_POINT - SoulPoint);
            _setSoulPoint(SoulPoint + soulPoint);
            return soulPoint;
        }
        /// <summary>
        /// ソウルポイント消費。
        /// </summary>
        /// <param name="soulPoint">消費ソウルポイント</param>
        public void ConsumeSoulPoint(uint soulPoint)
        {
            if (SoulPoint < soulPoint) throw new InvalidOperationException();
            _setSoulPoint(SoulPoint - soulPoint);
        }
        void _setSoulPoint(uint soulPoint)
        {
            var record = _cloneRecord();
            record.SoulPoint = soulPoint;
            _changeLocalRecord(record);
        }

        public override bool Save()
        {
            if (!_todayActivityLog.Save())
            {
            }
            if (_todayGuildActivityLog != null && !_todayGuildActivityLog.Save())
            {
            }

            // ミッション：キャラクターのレベルアップ回数.
            if (0 < _levelUpNum) QuestTriggerEntity.CountUpCharacterLevelUpNum(_record.PlayerID, _levelUpNum);

            return base.Save();

        }
    }
}
