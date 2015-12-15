using UniRx;

namespace Noroshi.BattleScene
{
    /// プレイヤーキャラクターを扱うクラス。
    public class PlayerCharacter : Character
    {
        /// サーバーから受け取った BattleCharacter でインスタンス化。
        public PlayerCharacter(Core.WebApi.Response.Battle.BattleCharacter battleCharacter)
        {
            _battleCharacter = battleCharacter;
        }

        /// プレイヤーキャラクター ID。
        public uint PlayerCharacterID { get { return _battleCharacter.ID; } }

        /// キャラクターステータスをロードする。
        protected override IObservable<CharacterStatus> _loadCharacterStatus()
        {
            return GlobalContainer.RepositoryManager.LoadCharacterStatus(_battleCharacter);
        }
    }
}
