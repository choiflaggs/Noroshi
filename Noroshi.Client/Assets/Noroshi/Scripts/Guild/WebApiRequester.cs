using UniRx;
using Noroshi.Core.WebApi.Response.Guild;
using Noroshi.Core.Game.Player;

namespace Noroshi.Guild
{
    public class WebApiRequester
    {
        /// <summary>
        /// 自プレイヤーが所属するギルド情報を取得する。
        /// </summary>
        public static IObservable<GetOwnResponse> GetOwn()
        {
            return _getWebApiRequester().Request<GetOwnResponse>("Guild/GetOwn");
        }
        /// <summary>
        /// 指定ギルド ID でギルド情報を取得する。
        /// </summary>
        /// <param name="guildId">検索ギルド ID</param>
        public static IObservable<GetResponse> Get(uint guildId)
        {
            var request = new GuildIDRequest { GuildID = guildId };
            return _getWebApiRequester().Request<GuildIDRequest, GetResponse>("Guild/Get", request);
        }
        /// <summary>
        /// おすすめギルドを取得する。
        /// </summary>
        public static IObservable<GetRecommendedGuildsResponse> GetRecommendedGuilds()
        {
            return _getWebApiRequester().Request<GetRecommendedGuildsResponse>("Guild/GetRecommendedGuilds");
        }
        /// <summary>
        /// 初心者ギルドへ加入する。
        /// </summary>
        public static IObservable<JoinResponse> JoinBeginnerGuild()
        {
            return _getWebApiRequester().Post<JoinResponse>("Guild/JoinBeginnerGuild");
        }
        /// <summary>
        /// 自動参加処理を実行し、何かしらの新ギルドに入った状態にする。
        /// 内部的にはまず加入可能なオススメギルドを検索し、検索ヒットすればそこへ加入、なければ自分で新ギルドを設立する。
        /// </summary>
        public static IObservable<JoinAutomaticallyResponse> JoinAutomatically()
        {
            var request = new JoinAutomaticallyRequest { CountryID = (uint)Country.Japan };
            return _getWebApiRequester().Post<JoinAutomaticallyRequest, JoinAutomaticallyResponse>("Guild/JoinAutomatically", request);
        }
        class JoinAutomaticallyRequest
        {
            public uint CountryID { get; set; }
        }
        /// <summary>
        /// ギルドを設立する。
        /// </summary>
        /// <param name="isOpen">オープン or クローズ</param>
        /// <param name="necessaryPlayerLevel">加入に必要なプレイヤーレベル</param>
        /// <param name="name">ギルド名</param>
        /// <param name="introduction">紹介文</param>
        public static IObservable<CreateResponse> Create(bool isOpen, ushort necessaryPlayerLevel, string name, string introduction)
        {
            var request = new CreateRequest
            {
                IsOpen = isOpen,
                CountryID = (uint)Country.Japan,
                NecessaryPlayerLevel = necessaryPlayerLevel,
                Name = name,
                Introduction = introduction,
            };
            return _getWebApiRequester().Post<CreateRequest, CreateResponse>("Guild/Create", request);
        }
        class CreateRequest
        {
            public bool IsOpen { get; set; }
            public uint CountryID { get; set; }
            public ushort NecessaryPlayerLevel { get; set; }
            public string Name { get; set; }
            public string Introduction { get; set; }
        }
        /// <summary>
        /// ギルドに加入する。
        /// </summary>
        /// <param name="guildId">加入先ギルド ID</param>
        public static IObservable<JoinResponse> Join(uint guildId)
        {
            var request = new GuildIDRequest { GuildID = guildId };
            return _getWebApiRequester().Post<GuildIDRequest, JoinResponse>("Guild/Join", request);
        }
        /// <summary>
        /// ギルドに加入リクエストを送る。
        /// </summary>
        /// <param name="guildId">加入リクエスト先ギルド ID</param>
        public static IObservable<HandleRequestResponse> Request(uint guildId)
        {
            var request = new GuildIDRequest { GuildID = guildId };
            return _getWebApiRequester().Post<GuildIDRequest, HandleRequestResponse>("Guild/Request", request);
        }
        /// <summary>
        /// 既に送っている加入リクエストをキャンセルする。
        /// </summary>
        public static IObservable<HandleRequestResponse> CancelRequest()
        {
            return _getWebApiRequester().Post<HandleRequestResponse>("Guild/CancelRequest");
        }
        /// <summary>
        /// 加入リクエストを承認する。リーダーと幹部しか利用できない。
        /// </summary>
        /// <param name="targetPlayerId">加入承認対象プレイヤー ID</param>
        public static IObservable<HandleReceivedRequestResponse> AcceptRequest(uint targetPlayerId)
        {
            var request = new TargetPlayerIDRequest { TargetPlayerID = targetPlayerId };
            return _getWebApiRequester().Post<TargetPlayerIDRequest, HandleReceivedRequestResponse>("Guild/AcceptRequest", request);
        }
        /// <summary>
        /// 加入リクエストを拒否する。リーダーと幹部しか利用できない。
        /// </summary>
        /// <param name="targetPlayerId">加入拒否対象プレイヤー ID</param>
        public static IObservable<HandleReceivedRequestResponse> RejectRequest(uint targetPlayerId)
        {
            var request = new TargetPlayerIDRequest { TargetPlayerID = targetPlayerId };
            return _getWebApiRequester().Post<TargetPlayerIDRequest, HandleReceivedRequestResponse>("Guild/RejectRequest", request);
        }
        /// <summary>
        /// ギルド設定を変更する。リーダーしか利用できない。
        /// </summary>
        /// <param name="isOpen">オープン or クローズ（null で更新しない）</param>
        /// <param name="necessaryPlayerLevel">加入に必要なプレイヤーレベル（null で更新しない）</param>
        /// <param name="name">ギルド名（null か空文字で更新しない）</param>
        /// <param name="introduction">紹介文（null か空文字で更新しない）</param>
        public static IObservable<ConfigureResponse> Configure(bool? isOpen, ushort? necessaryPlayerLevel, string name, string introduction)
        {
            var request = new ConfigureRequest
            {
                IsOpen = isOpen,
                CountryID = null,
                NecessaryPlayerLevel = necessaryPlayerLevel,
                Name = name,
                Introduction = introduction,
            };
            return _getWebApiRequester().Post<ConfigureRequest, ConfigureResponse>("Guild/Configure", request);
        }
        class ConfigureRequest
        {
            public bool? IsOpen { get; set; }
            public uint? CountryID { get; set; }
            public ushort? NecessaryPlayerLevel { get; set; }
            public string Name { get; set; }
            public string Introduction { get; set; }
        }
        /// <summary>
        /// 幹部を任命する。リーダーしか利用できない。
        /// </summary>
        /// <param name="targetPlayerId">新幹部プレイヤー ID</param>
        public static IObservable<AddExecutiveRoleResponse> AddExecutiveRole(uint targetPlayerId)
        {
            var request = new TargetPlayerIDRequest { TargetPlayerID = targetPlayerId };
            return _getWebApiRequester().Post<TargetPlayerIDRequest, AddExecutiveRoleResponse>("Guild/AddExecutiveRole", request);
        }
        /// <summary>
        /// 幹部を解任する。リーダーしか利用できない。
        /// </summary>
        /// <param name="targetPlayerId">解任対象プレイヤー ID</param>
        public static IObservable<RemoveExecutiveRoleResponse> RemoveExecutiveRole(uint targetPlayerId)
        {
            var request = new TargetPlayerIDRequest { TargetPlayerID = targetPlayerId };
            return _getWebApiRequester().Post<TargetPlayerIDRequest, RemoveExecutiveRoleResponse>("Guild/RemoveExecutiveRole", request);
        }
        /// <summary>
        /// リーダー権限を譲渡する。リーダーしか利用できない。
        /// </summary>
        /// <param name="targetPlayerId">新リーダープレイヤー ID</param>
        public static IObservable<ChangeLeaderResponse> ChangeLeader(uint targetPlayerId)
        {
            var request = new TargetPlayerIDRequest { TargetPlayerID = targetPlayerId };
            return _getWebApiRequester().Post<TargetPlayerIDRequest, ChangeLeaderResponse>("Guild/ChangeLeader", request);
        }
        /// <summary>
        /// メンバーを除名する。リーダーしか利用できない。
        /// </summary>
        /// <param name="targetPlayerId">除名対象プレイヤー ID</param>
        public static IObservable<LayOffResponse> LayOff(uint targetPlayerId)
        {
            var request = new TargetPlayerIDRequest { TargetPlayerID = targetPlayerId };
            return _getWebApiRequester().Post<TargetPlayerIDRequest, LayOffResponse>("Guild/LayOff", request);
        }
        /// <summary>
        /// バトルへ連れて行くことが可能な傭兵キャラクターを取得する。
        /// </summary>
        public static IObservable<GetTakableRentalCharactersResponse> GetTakableRentalCharacters()
        {
            return _getWebApiRequester().Request<GetTakableRentalCharactersResponse>("Guild/GetTakableRentalCharacters");
        }
        /// <summary>
        /// 派遣中の傭兵キャラクターを取得する。
        /// </summary>
        public static IObservable<GetRentalCharactersResponse> GetRentalCharacters()
        {
            return _getWebApiRequester().Request<GetRentalCharactersResponse>("Guild/GetRentalCharacters");
        }
        /// <summary>
        /// 傭兵キャラクターを派遣する。
        /// </summary>
        /// <param name="no">傭兵番号（枠指定）</param>
        /// <param name="playerCharacterId">傭兵として派遣するプレイヤーキャラクター ID</param>
        public static IObservable<AddRentalCharacterResponse> AddRentalCharacter(byte no, uint playerCharacterId)
        {
            var request = new AddRentalCharacterRequest { No = no, PlayerCharacterID = playerCharacterId };
            return _getWebApiRequester().Post<AddRentalCharacterRequest, AddRentalCharacterResponse>("Guild/AddRentalCharacter", request);
        }
        class AddRentalCharacterRequest
        {
            public byte No { get; set; }
            public uint PlayerCharacterID { get; set; }
        }
        /// <summary>
        /// 派遣期間の過ぎた派遣中傭兵キャラクターを連れ戻す。傭兵関連ビューの表示タイミングで呼ぶ想定。
        /// </summary>
        public static IObservable<RemoveRentalCharacterResponse> RemoveRentalCharacters()
        {
            return _getWebApiRequester().Post<RemoveRentalCharacterResponse>("Guild/RemoveRentalCharacters");
        }
        /// <summary>
        /// 挨拶をする。
        /// </summary>
        /// <param name="targetPlayerId">挨拶対象プレイヤー ID</param>
        public static IObservable<AddRentalCharacterResponse> Greet(uint targetPlayerId)
        {
            var request = new TargetPlayerIDRequest { TargetPlayerID = targetPlayerId };
            return _getWebApiRequester().Post<TargetPlayerIDRequest, AddRentalCharacterResponse>("Guild/Greet", request);
        }
        /// <summary>
        /// 被挨拶報酬を受け取る。
        /// </summary>
        public static IObservable<ReceiveGreetedRewardResponse> ReceiveGreetedReward()
        {
            return _getWebApiRequester().Post<ReceiveGreetedRewardResponse>("Guild/ReceiveGreetedReward");
        }

        class TargetPlayerIDRequest
        {
            public uint TargetPlayerID { get; set; }
        }
        class GuildIDRequest
        {
            public uint GuildID { get; set; }
        }
        static WebApi.WebApiRequester _getWebApiRequester()
        {
            return new WebApi.WebApiRequester();
        }
    }
}
