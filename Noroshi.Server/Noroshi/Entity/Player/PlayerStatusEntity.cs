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
            // �v���C���[���x���̓f�[�^�Ƃ��ĕۗL���Ă��Ȃ��̂Ōo���l�ɕϊ����Ă��猟������B
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
        /// �`���[�g���A���X�e�b�v�̐i�������݂�B
        /// ������ PlayerStatus �����b�N���|���ăr���h�A�X�V������Ŕj�����Ă��܂��̂ŁA
        /// �O�� PlayerStatus �𗘗p���Ă���ꍇ�͖{���\�b�h���g���Ă͂����Ȃ��B
        /// </summary>
        /// <param name="playerId">�Ώۃv���C���[ID</param>
        /// <param name="tutorialStep">�i�߂����X�e�b�v</param>
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
        /// �����M���h ID�B�M���h�������̏ꍇ�� null �ƂȂ�B
        /// </summary>
        public uint? GuildID => _record.GuildID > 0 ? (uint?)_record.GuildID : null;
        /// <summary>
        /// �����M���h�ɂ���������B��ʃ����o�[�̏ꍇ�� null �ƂȂ�B
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
        /// �M���h ID ���Z�b�g����B
        /// </summary>
        /// <param name="guildId"></param>
        public void SetGuildID(uint guildId)
        {
            var record = _cloneRecord();
            record.GuildID = guildId;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// �M���h���痣�E����B
        /// </summary>
        public void DropOutFromGuild()
        {
            SetGuildID(0);
            // �������O���B
            RemoveGuildRole();
        }

        /// <summary>
        /// �����M���h�ɂ�����������Z�b�g����B
        /// </summary>
        /// <param name="guildRole">�����M���h�ɂ��������</param>
        public void SetGuildRole(GuildRole guildRole)
        {
            _setGuildRole((byte)guildRole);
        }
        /// <summary>
        /// �����M���h�ɂ�����������O���B
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
            // �����l�͏��S�҃M���h�ɍݐВ�.
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
        /// ���A���i�������Z�b�g����j�B
        /// </summary>
        public byte GreetingNum => (byte)(ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastGreetedAt) ? 0 : _record.LastGreetingNum);
        /// <summary>
        /// ���A���C���N�������g�B
        /// </summary>>
        public void IncrementGreetingNum()
        {
            var newRecord = _cloneRecord();
            newRecord.LastGreetingNum = (byte)(GreetingNum + 1);
            newRecord.LastGreetedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            _changeLocalRecord(newRecord);

        }
        /// <summary>
        /// ���m�F�̔툥�A���B
        /// </summary>>
        public uint UnconfirmedGreetedNum => _record.UnconfirmedGreetedNum;
        /// <summary>
        /// ���m�F�̔툥�A���C���N�������g�B
        /// </summary>>
        public void IncrementUnconfirmedGreetedNum()
        {
            _setUnconfirmedGreetedNum(UnconfirmedGreetedNum + 1);
        }
        /// <summary>
        /// ���m�F�̔툥�A�����Z�b�g�B
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
        /// �툥�A��V�󂯎��ۃ`�F�b�N�B
        /// </summary>
        /// <returns></returns>>
        public bool CanReceiveGreetedReward()
        {
            return UnconfirmedGreetedNum > 0;
        }
        /// <summary>
        /// �툥�A��V�擾�B
        /// </summary>
        /// <returns></returns>>
        public IEnumerable<PossessionParam> GetReceivableGreetedRewardPossessionParams()
        {
            return new[] { PossessionManager.GetGuildPointParam((ushort)(GuildConstant.GUILD_POINT_PER_GREETED * UnconfirmedGreetedNum * ( PlayerVipLevelBonus.GuildPointBonus + 1.00))) };
        }

        /// <summary>
        /// �b���ٗp��V�󂯎�萔�i�������Z�b�g����j�B
        /// </summary>>
        public byte RentalRewardReceivingNum => (byte)(ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.LastRentalRewardReceivedAt) ? 0 : _record.LastRentalRewardReceivingNum);
        /// <summary>
        /// �b���ٗp��V�󂯎�萔���C���N�������g�B
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
            // �A�N�e�B�r�e�B���O�ɂ��L�^�B
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
            // �A�N�e�B�r�e�B���O�ɂ��L�^�B
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

        // �`�F�b�N���Ăяo�����ɋ������Ă��܂��̂Ŕp�~�\��B
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
                // �~�b�V�����F�v���C���[LevelUp.
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
        /// �S�[���h�t�^�B
        /// </summary>
        /// <param name="gold">�t�^�S�[���h</param>
        /// <returns></returns>
        public uint AddGold(uint gold)
        {
            gold = Math.Min(gold, PlayerConstant.MAX_GOLD - Gold);
            _setGold(Gold + gold);
            return gold;
        }
        /// <summary>
        /// �S�[���h����B
        /// </summary>
        /// <param name="gold">����S�[���h</param>
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
        /// �M���h�|�C���g�t�^�B
        /// </summary>
        /// <param name="guildPoint">�t�^�M���h�|�C���g</param>
        /// <returns></returns>
        public uint AddGuildPoint(uint guildPoint)
        {
            guildPoint = Math.Min(guildPoint, PlayerConstant.MAX_GUILD_POINT - GuildPoint);
            _setGuildPoint(GuildPoint + guildPoint);
            return guildPoint;
        }
        /// <summary>
        /// �M���h�|�C���g����B
        /// </summary>
        /// <param name="guildPoint">����M���h�|�C���g</param>
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
        /// �`���|�C���g�t�^�B
        /// </summary>
        /// <param name="expeditionPoint">�t�^�`���|�C���g</param>
        /// <returns></returns>
        public uint AddExpeditionPoint(uint expeditionPoint)
        {
            expeditionPoint = Math.Min(expeditionPoint, PlayerConstant.MAX_EXPEDITION_POINT - ExpeditionPoint);
            _setExpeditionPointPoint(ExpeditionPoint + expeditionPoint);
            return expeditionPoint;
        }
        /// <summary>
        /// �`���|�C���g����B
        /// </summary>
        /// <param name="expeditionPoint">����`���|�C���g</param>
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
        /// �\�E���|�C���g�t�^�B
        /// </summary>
        /// <param name="soulPoint">�t�^�\�E���|�C���g</param>
        /// <returns></returns>
        public uint AddSoulPoint(uint soulPoint)
        {
            soulPoint = Math.Min(soulPoint, PlayerConstant.MAX_SOUL_POINT - SoulPoint);
            _setSoulPoint(SoulPoint + soulPoint);
            return soulPoint;
        }
        /// <summary>
        /// �\�E���|�C���g����B
        /// </summary>
        /// <param name="soulPoint">����\�E���|�C���g</param>
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

            // �~�b�V�����F�L�����N�^�[�̃��x���A�b�v��.
            if (0 < _levelUpNum) QuestTriggerEntity.CountUpCharacterLevelUpNum(_record.PlayerID, _levelUpNum);

            return base.Save();

        }
    }
}
