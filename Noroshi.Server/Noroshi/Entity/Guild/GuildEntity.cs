using System;
using System.Collections.Generic;
using System.Linq;
using Noroshi.Core.Game.GameContent;
using Noroshi.Core.Game.Player;
using Noroshi.Core.Game.Guild;
using Noroshi.Server.Contexts;
using Noroshi.Server.Entity.Player;
using Noroshi.Server.Entity.Possession;
using Noroshi.Server.Daos.Rdb;
using Noroshi.Server.Daos.Rdb.Guild;
using Constant = Noroshi.Core.Game.Guild.Constant;
using Schema = Noroshi.Server.Daos.Rdb.Schemas.GuildSchema;

namespace Noroshi.Server.Entity.Guild
{
    public class GuildEntity : AbstractDaoWrapperEntity<GuildEntity, GuildDao, Schema.PrimaryKey, Schema.Record>
    {
        const byte TRY_NUM_TO_ACQUIRE_RECOMMENDED_GUILD = 3;
        const byte ROW_NUM_TO_READ_RECOMMENDED_GUILD = 100;

        enum MemberAndRequestNumCluster
        {
            Lonely = 1,
            Micro = 2,
            Tiny = 3,
            Small = 4,
            Medium = 5,
            Big = 6,
            Huge = 7,
        }

        public static GuildEntity ReadAndBuild(uint id, ReadType readType = ReadType.Slave)
        {
            return ReadAndBuildMulti(new uint[] { id }, readType).FirstOrDefault();
        }
        public static IEnumerable<GuildEntity> ReadAndBuildMulti(IEnumerable<uint> ids, ReadType readType = ReadType.Slave)
        {
            return _loadAssociatedEntities(ReadAndBuildMulti(ids.Select(id => new Schema.PrimaryKey { ID = id }), readType));
        }

        /// <summary>
        /// おすすめギルドを取得する。内部的に複数回参照するのでややコストが高いことに注意。
        /// </summary>
        /// <param name="playerStatus">対象プレイヤー</param>
        /// <param name="num">取得数</param>
        /// <returns></returns>
        public static IEnumerable<GuildEntity> ReadAndBuildRecommendedGuilds(PlayerStatusEntity playerStatus, byte num)
        {
            var memberAndRequestNumClusters = (new[]
            {
                MemberAndRequestNumCluster.Lonely,
                MemberAndRequestNumCluster.Micro,
                MemberAndRequestNumCluster.Tiny,
                MemberAndRequestNumCluster.Small,
                MemberAndRequestNumCluster.Medium,
                MemberAndRequestNumCluster.Big,
            })
            .OrderBy(_ => ContextContainer.GetContext().RandomGenerator.NextDouble()).Take(TRY_NUM_TO_ACQUIRE_RECOMMENDED_GUILD);
            var allGuilds = new List<GuildEntity>();
            foreach (var cluster in memberAndRequestNumClusters)
            {
                var guilds = _instantiate((new GuildDao()).ReadByCategoryAndMemberAndRequestNumClusterAndMaxAveragePlayerLevel(GuildCategory.NormalOpen, (byte)cluster, playerStatus.Level, ROW_NUM_TO_READ_RECOMMENDED_GUILD))
                    .Where(guild => !playerStatus.GuildID.HasValue || playerStatus.GuildID.Value != guild.ID && guild.NecessaryPlayerLevel <= playerStatus.Level);
                allGuilds.AddRange(guilds);
                if (allGuilds.Count() >= num) return allGuilds.Take(num);
            }
            allGuilds.AddRange(_instantiate((new GuildDao()).ReadByCategoryAndMaxAveragePlayerLevel(GuildCategory.NormalOpen, playerStatus.Level, ROW_NUM_TO_READ_RECOMMENDED_GUILD)));
            return _loadAssociatedEntities(allGuilds.Where(guild => !playerStatus.GuildID.HasValue || playerStatus.GuildID.Value != guild.ID && guild.NecessaryPlayerLevel <= playerStatus.Level).Take(num));
        }

        public static IEnumerable<GuildEntity> ReadAndBuildAllNormalGuilds()
        {
            return _instantiate((new GuildDao()).ReadAllNormalGuilds());
        }

        /// <summary>
        /// 通常ギルド設立に必要な支払い内容を取得。
        /// </summary>
        /// <returns></returns>
        public static PossessionParam GetPaymentToCreateNormalGuild()
        {
            return PossessionManager.GetCommonGemParam(Constant.NECESSARY_GEM_TO_CREATE_NORMAL_GUILD);
        }

        /// <summary>
        /// 通常ギルド新規設立可否チェック。
        /// </summary>
        /// <param name="owner">設立プレイヤー</param>
        /// <param name="currentGuild">設立プレイヤーが所属する現ギルド（所属ギルドがない場合は null）</param>
        /// <returns></returns>
        public static bool CanCreateNormalGuild(PlayerStatusEntity owner, GuildEntity currentGuild)
        {
            // 通常ギルドがオープンしている必要がある。
            if (!GameContent.IsOpen(GameContentID.NormalGuild, owner.Level)) return false;
            // 現ギルドを脱退できるかチェック。
            if (currentGuild != null && !currentGuild.CanDropOut(owner)) return false;
            return true;
        }
        /// <summary>
        /// 通常ギルドを新規設立する。
        /// </summary>
        /// <param name="owner">設立プレイヤー</param>
        /// <param name="currentGuild">設立プレイヤーが所属する現ギルド（所属ギルドがない場合は null）</param>
        /// <param name="isOpen">オープン or クローズ</param>
        /// <param name="countryId">国籍 ID</param>
        /// <param name="necessaryPlayerLevel">必要プレイヤーレベル</param>
        /// <param name="name">ギルド名</param>
        /// <param name="introduction">紹介文</param>
        /// <returns></returns>
        public static GuildEntity CreateNormalGuild(PlayerStatusEntity owner, GuildEntity currentGuild, GuildRequestEntity request, GuildEntity requestingGuild, bool isOpen, uint countryId, ushort necessaryPlayerLevel, string name, string introduction)
        {
            var newGuild = _createNormalGuild(owner.PlayerID, isOpen);
            if (newGuild == null)
            {
                throw new SystemException(string.Join("\t", "Cannot Create Normal Guild", owner.PlayerID, isOpen));
            }
            newGuild.SetCountryID(countryId);
            newGuild.SetNecessaryPlayerLevel(necessaryPlayerLevel);
            newGuild.SetName(name);
            newGuild.SetIntroduction(introduction);
            // 新規ギルド加入チェック。
            if (!newGuild.CanJoinAsOwner(owner, currentGuild))
            {
                throw new SystemException(string.Join("\t", "Cannot Join Normal Guild", owner.PlayerID, newGuild.ID, isOpen));
            }
            // 新ギルド参加。
            newGuild.Join(owner, currentGuild, request, requestingGuild);
            return newGuild;
        }
        static GuildEntity _createNormalGuild(uint ownerPlayerId, bool isOpen)
        {
            return _loadAssociatedEntity(_instantiate((new GuildDao()).CreateNormalGuild(ownerPlayerId, isOpen)));
        }

        /// <summary>
        /// 初心者ギルドをビルドする。ロック指定の場合はなければ作成してしまうことに注意。
        /// </summary>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public static GuildEntity ReadAndBuildBeginnerGuild(ReadType readType)
        {
            if (readType == ReadType.Lock)
            {
                return _loadAssociatedEntity(_instantiate((new GuildDao()).ReadOrCreateBeginnerGuild()));
            }
            else
            {
                return _loadAssociatedEntity(_instantiate((new GuildDao()).ReadBeginnerGuild()));
            }
        }

        /// <summary>
        /// 加入に必要なプレイヤーレベル設定チェック。
        /// </summary>
        /// <param name="necessaryPlayerLevel">加入に必要なプレイヤーレベル</param>
        /// <returns></returns>
        public static bool IsValidNecessaryPlayerLevel(ushort necessaryPlayerLevel)
        {
            return 1 <= necessaryPlayerLevel && necessaryPlayerLevel <= PlayerLevelEntity.GetMaxLevel();
        }
        /// <summary>
        /// ギルド名チェック。
        /// </summary>
        /// <param name="name">ギルド名</param>
        /// <returns></returns>
        public static bool IsValidName(string name)
        {
            return Constant.MIN_NAME_LENGTH <= name.Length && name.Length <= Constant.MAX_NAME_LENGTH;
        }
        /// <summary>
        /// 紹介文チェック。
        /// </summary>
        /// <param name="introduction">紹介文</param>
        /// <returns></returns>
        public static bool IsValidIntroduction(string introduction)
        {
            return Constant.MIN_INTRODUCTION_LENGTH <= introduction.Length && introduction.Length <= Constant.MAX_INTRODUCTION_LENGTH;
        }

        static MemberAndRequestNumCluster _getMemberAndRequestNumCluster(uint memberAndRequestNum)
        {
            if (memberAndRequestNum <= 1)
            {
                return MemberAndRequestNumCluster.Lonely;
            }
            else if (memberAndRequestNum <= 3)
            {
                return MemberAndRequestNumCluster.Micro;
            }
            else if (memberAndRequestNum <= 5)
            {
                return MemberAndRequestNumCluster.Tiny;
            }
            else if (memberAndRequestNum <= 10)
            {
                return MemberAndRequestNumCluster.Small;
            }
            else if (memberAndRequestNum <= Constant.MAX_NORMAL_GUILD_MEMBER_NUM / 2)
            {
                return MemberAndRequestNumCluster.Medium;
            }
            else if (memberAndRequestNum < Constant.MAX_NORMAL_GUILD_MEMBER_NUM)
            {
                return MemberAndRequestNumCluster.Big;
            }
            else
            {
                return MemberAndRequestNumCluster.Huge;
            }
        }

        static GuildEntity _loadAssociatedEntity(GuildEntity entity)
        {
            if (entity == null) return null;
            return _loadAssociatedEntities(new[] { entity }).FirstOrDefault();
        }
        static IEnumerable<GuildEntity> _loadAssociatedEntities(IEnumerable<GuildEntity> entities)
        {
            if (entities.Count() == 0) return entities;
            var activityMap = GuildActivityDailyLogEntity.ReadOrInstantiateDefaultAndBuildMultiYesterday(entities.Select(entity => entity.ID)).ToDictionary(activity => activity.GuildID);
            return entities.Select(entity =>
            {
                entity._setYesterdayActivity(activityMap[entity.ID]);
                return entity;
            });
        }


        GuildActivityDailyLogEntity _yesterdayActivity;
        void _setYesterdayActivity(GuildActivityDailyLogEntity activity)
        {
            _yesterdayActivity = activity;
        }

        public uint ID => _record.ID;
        public GuildCategory Category => (GuildCategory)_record.Category;
        public Country Country => (Country)_record.CountryID;
        public ushort NecessaryPlayerLevel => _record.NecessaryPlayerLevel;
        public string Name => _record.Name;
        public string Introduction => _record.Introduction;
        public uint LeaderPlayerID => _record.LeaderPlayerID;
        public uint MemberNum => _record.MemberNum;
        public uint? MaxMemberNum => Category == GuildCategory.Beginner ? null : (uint?)Constant.MAX_NORMAL_GUILD_MEMBER_NUM;
        public byte RequestNum => _record.RequestNum;
        public uint CreatedAt => _record.CreatedAt;

        /// <summary>
        /// ギルドランク取得。必ず SLAVE 参照にしているので更新時に信用してはいけない。
        /// </summary>
        public GuildRank GuildRank => new CooperationPoint(_yesterdayActivity.CooperationPoint).GetGuildRank();

        /// <summary>
        /// 友情ポイント。
        /// </summary>
        public ushort CooperationPoint => ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.CooperationPointUpdatedAt) ? (ushort)0 : _record.CooperationPoint;
        /// <summary>
        /// 友情ポイント使用回数。
        /// </summary>
        public ushort CooperationPointConsumedNum => ContextContainer.GetContext().TimeHandler.HasAlreadyReset(_record.CooperationPointUpdatedAt) ? (ushort)0 : _record.CooperationPointConsumedNum;
        /// <summary>
        /// 最大友情ポイント（消費回数に応じて変動する）。
        /// </summary>
        public ushort MaxCooperationPoint => (ushort)(100 + CooperationPointConsumedNum * 10);
        /// <summary>
        /// 友情ポイントを加算する。
        /// </summary>
        /// <param name="cooperationPoint">友情ポイント</param>
        public void AddConsumeCooperationPoint(ushort cooperationPoint)
        {
            var record = _cloneRecord();
            record.CooperationPoint = (ushort)(CooperationPoint + cooperationPoint);
            record.CooperationPointConsumedNum = CooperationPointConsumedNum;
            record.CooperationPointUpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
            // ログにも記録。
            GuildActivityDailyLogEntity.AddTodayConsumeCooperationPoint(ID, cooperationPoint);
        }
        /// <summary>
        /// 友情ポイント消費可否チェック。
        /// </summary>
        /// <returns></returns>
        public bool CanConsumeCooperationPoint()
        {
            return MaxCooperationPoint <= CooperationPoint;
        }
        /// <summary>
        /// 友情ポイントを消費する。
        /// </summary>
        public void ConsumeCooperationPoint()
        {
            var record = _cloneRecord();
            record.CooperationPoint = 0;
            record.CooperationPointConsumedNum = (ushort)(CooperationPointConsumedNum + 1);
            record.CooperationPointUpdatedAt = ContextContainer.GetContext().TimeHandler.UnixTime;
        }

        /// <summary>
        /// 最大挨拶可能数。
        /// </summary>
        /// <param name="vipLevel">挨拶する側の VIP レベル</param>
        /// <returns></returns>
        public byte GetMaxGreetingNum(ushort vipLevel)
        {
            // TODO : VIP レベル考慮。
            return Category == GuildCategory.Beginner ? Constant.MAX_GREETING_NUM_PER_DAY_IN_BEGINNER_GUILD : PlayerVipLevelBonusEntity.ReadAndBuild(vipLevel).MaxGreetingNum;
        }

        /// <summary>
        /// 所属メンバー情報を取得する。
        /// </summary>
        /// <param name="readType">参照タイプ</param>
        /// <returns></returns>
        public IEnumerable<PlayerStatusEntity> GetMemberPlayerStatuses(ReadType readType = ReadType.Slave)
        {
            // 初心者ギルドの場合は上限ありで取得するので全員取れないことに注意。
            return PlayerStatusEntity.ReadAndBuildByGuildID(ID, Constant.MAX_NORMAL_GUILD_MEMBER_NUM, readType);
        }

        /// <summary>
        /// （通常ギルドにおいて）オープンクローズをセットする。
        /// </summary>
        /// <param name="isOpen">オープンクローズ</param>
        public void SetOpen(bool isOpen)
        {
            if (Category == GuildCategory.Beginner) throw new InvalidOperationException();
            var record = _cloneRecord();
            record.Category = (byte)(isOpen ? GuildCategory.NormalOpen : GuildCategory.NormalClose);
            _changeLocalRecord(record);
        }
        /// <summary>
        /// 国籍 ID をセットする。
        /// </summary>
        /// <param name="countryId">国籍 ID</param>
        public void SetCountryID(uint countryId)
        {
            var record = _cloneRecord();
            record.CountryID = countryId;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// ギルドに参加するための最低プレイヤーレベルをセットする。
        /// </summary>
        /// <param name="playerLevel">ギルドに参加するために必要な最低プレイヤーレベル</param>
        public void SetNecessaryPlayerLevel(ushort playerLevel)
        {
            var record = _cloneRecord();
            record.NecessaryPlayerLevel = playerLevel;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// ギルド名をセットする。
        /// </summary>
        /// <param name="name">ギルド名</param>
        public void SetName(string name)
        {
            var record = _cloneRecord();
            record.Name = name;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// ギルド紹介文をセットする。
        /// </summary>
        /// <param name="introduction">ギルド紹介文</param>
        public void SetIntroduction(string introduction)
        {
            var record = _cloneRecord();
            record.Introduction = introduction;
            _changeLocalRecord(record);
        }
        /// <summary>
        /// 平均プレイヤーレベルをセットする。
        /// </summary>
        /// <param name="averagePlayerLevel">平均プレイヤーレベル</param>
        public void SetAveragePlayerLevel(ushort averagePlayerLevel)
        {
            var record = _cloneRecord();
            record.AveragePlayerLevel = averagePlayerLevel;
            _changeLocalRecord(record);
        }

        /// <summary>
        /// メンバーとしての加入可否チェック。
        /// </summary>
        /// <param name="member">加入メンバー</param>
        /// <param name="currentGuild">加入メンバー現所属ギルド（所属していない場合は null で渡される）</param>
        /// <returns></returns>
        public bool CanJoinAsMember(PlayerStatusEntity member, GuildEntity currentGuild)
        {
            switch (Category)
            {
                case GuildCategory.Beginner:
                    return _canJoinAsBeginnerGuild(member, currentGuild);
                case GuildCategory.NormalOpen:
                    return _canJoinAsNormalOpenGuild(member, currentGuild);
                default:
                    throw new InvalidOperationException();
            }
        }
        /// <summary>
        /// 設立者としての加入可否チェック。
        /// </summary>
        /// <param name="member">加入設立者</param>
        /// <param name="currentGuild">加入設立者現所属ギルド（所属していない場合は null で渡される）</param>
        /// <returns></returns>
        public bool CanJoinAsOwner(PlayerStatusEntity owner, GuildEntity currentGuild)
        {
            switch (Category)
            {
                case GuildCategory.NormalOpen:
                    return _canJoinAsNormalOpenGuild(owner, currentGuild);
                case GuildCategory.NormalClose:
                    // クローズであろうが気にしない。
                    return _canJoinAsNormalOpenGuild(owner, currentGuild);
                default:
                    throw new InvalidOperationException();
            }
        }
        bool _canJoinAsBeginnerGuild(PlayerStatusEntity joiner, GuildEntity currentGuild)
        {
            // ビギナーギルドのみが対象。
            if (Category != GuildCategory.Beginner) throw new InvalidOperationException();
            // 既にリクエストを出していることはないはず。
            if (joiner.GuildState != PlayerGuildState.Default) throw new InvalidOperationException();
            // 初心者ギルドは初参加のみ。
            if (currentGuild != null) return false;
            // ビギナーギルドがオープンしている必要がある。
            if (!GameContent.IsOpen(GameContentID.BeginnerGuild, joiner.Level)) return false;
            // 通常ギルドがオープンしていない必要がある。
            if (GameContent.IsOpen(GameContentID.NormalGuild, joiner.Level)) return false;
            return true;
        }
        bool _canJoinAsNormalOpenGuild(PlayerStatusEntity joiner, GuildEntity currentGuild)
        {
            // 通常オープンギルドのみが対象。
            if (Category != GuildCategory.NormalOpen) throw new InvalidOperationException();
            return _canJoinAsNormalGuildWithoutCheckPlayerGuildState(joiner, currentGuild);
        }
        bool _canJoinAsNormalCloseGuildWithoutCheckPlayerGuildState(PlayerStatusEntity joiner, GuildEntity currentGuild)
        {
            // 通常クローズギルドのみが対象。
            if (Category != GuildCategory.NormalClose) throw new InvalidOperationException();
            return _canJoinAsNormalGuildWithoutCheckPlayerGuildState(joiner, currentGuild);
        }
        bool _canJoinAsNormalGuildWithoutCheckPlayerGuildState(PlayerStatusEntity joiner, GuildEntity currentGuild)
        {
            // 通常ギルドのみが対象。
            if (Category != GuildCategory.NormalOpen && Category != GuildCategory.NormalClose) throw new InvalidOperationException();
            // 通常ギルドがオープンしている必要がある。
            if (!GameContent.IsOpen(GameContentID.NormalGuild, joiner.Level)) return false;
            // 現ギルドを離脱チェック。
            if (currentGuild != null && !currentGuild.CanDropOut(joiner)) return false;
            // プレイヤーレベルチェック。
            if (joiner.Level < NecessaryPlayerLevel) return false;
            // 最大メンバー数が存在しない、もしくは、既に最大メンバー数以上の場合は受け入れない。
            if (!MaxMemberNum.HasValue || MaxMemberNum.Value <= MemberNum + RequestNum) return false;
            return true;
        }
        /// <summary>
        /// 該当プレイヤーをギルドへ加入させる。
        /// </summary>
        /// <param name="playerStatus">加入プレイヤー</param>
        /// <param name="currentGuild">加入プレイヤーが現在所属しているギルド</param>
        public void Join(PlayerStatusEntity playerStatus, GuildEntity currentGuild, GuildRequestEntity request, GuildEntity requestingGuild)
        {
            // 引数チェック。
            if (playerStatus.GuildID.HasValue && (currentGuild == null || playerStatus.GuildID.Value != currentGuild.ID))
            {
                throw new InvalidOperationException();
            }
            // ギルド所属なら離脱。
            if (playerStatus.GuildID.HasValue)
            {
                currentGuild.DropOutAsNotLeader(playerStatus);
            }
            playerStatus.SetGuildID(ID);
            // リクエストは落としてしまう。
            if (request != null)
            {
                requestingGuild.DeleteRequest(playerStatus, request);
                // 他処理と連動することはないので保存してしまう。
                if (!requestingGuild.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save Guild", requestingGuild.ID));
                }
            }
            // リーダー固有処理。
            if (Category != GuildCategory.Beginner && MemberNum == 0)
            {
                playerStatus.SetGuildRole(GuildRole.Leader);
                // まずはここで平均レベルをセット。後はデイリーバッチにお任せ。
                SetAveragePlayerLevel(playerStatus.Level);
            }
            var record = _cloneRecord();
            record.MemberNum++;
            record.MemberAndRequestNumCluster = (byte)_getMemberAndRequestNumCluster(record.MemberNum + RequestNum);
            _changeLocalRecord(record);
        }

        /// <summary>
        /// リクエスト受け付け可否チェック。
        /// </summary>
        /// <param name="requester">リクエストプレイヤー</param>
        /// <param name="currentGuild">リクエストプレイヤーの現所属ギルド</param>
        /// <returns></returns>
        public bool CanReceiveRequest(PlayerStatusEntity requester, GuildEntity currentGuild)
        {
            // 既にリクエストを出している場合は参加できない。
            if (requester.GuildState != PlayerGuildState.Default) return false;
            // 加入条件を満たす必要がある。
            if (!_canJoinAsNormalCloseGuildWithoutCheckPlayerGuildState(requester, currentGuild)) return false;
            return true;
        }
        public void ReceiveRequest(PlayerStatusEntity requester)
        {
            requester.SetGuildState(PlayerGuildState.Request);
            var record = _cloneRecord();
            record.RequestNum++;
            record.MemberAndRequestNumCluster = (byte)_getMemberAndRequestNumCluster(MemberNum + record.RequestNum);
            _changeLocalRecord(record);
        }
        /// <summary>
        /// 加入リクエスト受諾可否チェック。
        /// </summary>
        /// <param name="requester">加入リクエストプレイヤー</param>
        /// <param name="currentGuild">加入リクエストプレイヤーの現所属ギルド</param>
        /// <param name="request">加入リクエスト</param>
        /// <returns></returns>
        public bool CanAcceptRequest(PlayerStatusEntity requester, GuildEntity currentGuild, GuildRequestEntity request)
        {
            // 既にリクエストを出していなければいけない
            if (requester.GuildState != PlayerGuildState.Request) return false;
            if (request == null || request.PlayerID != requester.PlayerID || request.GuildID != ID) return false;
            // 加入条件を満たす必要がある。
            if (!_canJoinAsNormalCloseGuildWithoutCheckPlayerGuildState(requester, currentGuild)) return false;
            return true;
        }
        /// <summary>
        /// 加入リクエストを受諾する。
        /// </summary>
        /// <param name="requester">加入リクエストプレイヤー</param>
        /// <param name="currentGuild">加入リクエストプレイヤーの現所属ギルド</param>
        /// <param name="request">加入リクエスト</param>
        /// <param name="requestingGuild">加入リクエスト先ギルド</param>
        public void AcceptRequest(PlayerStatusEntity requester, GuildEntity currentGuild, GuildRequestEntity request, GuildEntity requestingGuild)
        {
            Join(requester, currentGuild, request, requestingGuild);
        }
        /// <summary>
        /// リクエスト削除。引数である GuildRequestEntity オブジェクトは Delete() されることに注意。
        /// </summary>
        /// <param name="requester">削除対象リクエスト送り元プレイヤー</param>
        /// <param name="request">削除対象リクエスト</param>
        public void DeleteRequest(PlayerStatusEntity requester, GuildRequestEntity request)
        {
            if (request.GuildID != ID) throw new InvalidOperationException();
            if (request.PlayerID != requester.PlayerID) throw new InvalidOperationException();
            requester.SetGuildState(PlayerGuildState.Default);
            if (!request.Delete())
            {
                throw new SystemException(string.Join("\t", "Fail to Delete", request.PlayerID, request.GuildID));
            }
            var record = _cloneRecord();
            record.RequestNum--;
            record.MemberAndRequestNumCluster = (byte)_getMemberAndRequestNumCluster(MemberNum + record.RequestNum);
            _changeLocalRecord(record);
        }

        /// <summary>
        /// ギルドからの離脱可否チェック。
        /// </summary>
        /// <param name="playerStatus">離脱プレイヤー</param>
        /// <returns></returns>
        public bool CanDropOut(PlayerStatusEntity playerStatus)
        {
            if (!playerStatus.GuildID.HasValue || playerStatus.GuildID.Value != ID) return false;
            // メンバーがいる場合、リーダーは原則離脱できない。したければ強制離脱メソッドを使うしかない。
            if (MemberNum > 1 && playerStatus.GuildRole.HasValue && playerStatus.GuildRole.Value == GuildRole.Leader) return false;
            return true;
        }
        /// <summary>
        /// ギルドからの強制離脱。内部的に Save() もしくは Delete() が呼ばれていることに注意。
        /// </summary>
        /// <param name="playerStatus">離脱プレイヤー</param>
        /// <param name="otherMembers">他メンバー</param>
        /// <returns></returns>
        public bool ForceDropOut(PlayerStatusEntity playerStatus, IEnumerable<PlayerStatusEntity> otherMembers)
        {
            if (otherMembers.Any(ps => ps.PlayerID == playerStatus.PlayerID)) throw new InvalidOperationException();

            // リーダーが辞める場合は引継ぎ。
            if (_isLeader(playerStatus) && otherMembers.Count() > 0)
            {
                var nextLeader = otherMembers.Where(ps => ps.GuildRole.HasValue).OrderBy(ps => ps.PlayerID).FirstOrDefault();
                if (nextLeader == null) nextLeader = otherMembers.OrderBy(ps => ps.PlayerID).FirstOrDefault();
                if (!CanChangeLeader(playerStatus, nextLeader)) throw new SystemException();
                ChangeLeader(playerStatus, nextLeader);
                if (!nextLeader.Save())
                {
                    throw new SystemException(string.Join("\t", "Fail to Save Player Status", playerStatus.PlayerID, nextLeader.PlayerID, ID));
                }
            }
            return _dropOut(playerStatus);
        }
        /// <summary>
        /// ギルドからの離脱。内部的に Save() もしくは Delete() が呼ばれていることに注意。
        /// </summary>
        /// <param name="playerStatus">離脱プレイヤー</param>
        /// <returns></returns>
        public bool DropOutAsNotLeader(PlayerStatusEntity playerStatus)
        {
            if (MemberNum > 1 && _isLeader(playerStatus)) throw new InvalidOperationException();
            return _dropOut(playerStatus);
        }
        bool _dropOut(PlayerStatusEntity playerStatus)
        {
            // プレイヤー状態を離脱へ。
            playerStatus.DropOutFromGuild();
            // 初心者ギルドからの離脱はチャット用に記録しておく。
            if (Category == GuildCategory.Beginner)
            {
                playerStatus.SetBeginnerGuildDroppedOutAt();
            }
            // メンバー数デクリメント。
            var record = _cloneRecord();
            record.MemberNum--;
            record.MemberAndRequestNumCluster = (byte)_getMemberAndRequestNumCluster(record.MemberNum + RequestNum);
            _changeLocalRecord(record);
            // 初心者ギルドではない場合、メンバー数がゼロになったギルドは消してしまう。
            if (Category != GuildCategory.Beginner && MemberNum == 0)
            {
                return Delete();
            }
            else
            {
                return Save();
            }
        }

        /// <summary>
        /// リーダー変更可否チェック。
        /// </summary>
        /// <param name="currentLeader">現リーダー</param>
        /// <param name="nextLeader">新リーダー</param>
        /// <returns></returns>
        public bool CanChangeLeader(PlayerStatusEntity currentLeader, PlayerStatusEntity nextLeader)
        {
            // 所属チェック。
            if (!currentLeader.GuildID.HasValue || !nextLeader.GuildID.HasValue) return false;
            if (currentLeader.GuildID.Value != ID || nextLeader.GuildID.Value != ID) return false;
            // 現リーダーチェック。
            if (!_isLeader(currentLeader)) return false;
            // 新リーダーチェック。
            if (_isLeader(nextLeader)) return false;
            return true;
        }
        bool _isLeader(PlayerStatusEntity playerStatus)
        {
            if (playerStatus.PlayerID != LeaderPlayerID) return false;
            if (!playerStatus.GuildRole.HasValue || playerStatus.GuildRole.Value != GuildRole.Leader) return false;
            return true;
        }
        /// <summary>
        /// リーダー変更。
        /// </summary>
        /// <param name="currentLeader">現リーダー</param>
        /// <param name="nextLeader">新リーダー</param>
        public void ChangeLeader(PlayerStatusEntity currentLeader, PlayerStatusEntity nextLeader)
        {
            // 役割付け替え。
            currentLeader.RemoveGuildRole();
            nextLeader.SetGuildRole(GuildRole.Leader);
            // ギルド側の情報も更新。
            var record = _cloneRecord();
            record.LeaderPlayerID = nextLeader.PlayerID;
            _changeLocalRecord(record);
        }

        public Core.WebApi.Response.Guild.Guild ToResponseData()
        {
            return new Core.WebApi.Response.Guild.Guild
            {
                ID = ID,
                Category = Category,
                Country = Country,
                NecessaryPlayerLevel = NecessaryPlayerLevel,
                Name = Name,
                Introduction = Introduction,
                MemberNum = MemberNum,
                MaxMemberNum = MaxMemberNum,
                GuildRank = GuildRank,
                MaxCooperationPoint = MaxCooperationPoint,
                CooperationPoint = CooperationPoint,
            };
        }
    }
}
