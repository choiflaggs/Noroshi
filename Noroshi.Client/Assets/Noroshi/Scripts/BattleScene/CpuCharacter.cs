using UniRx;
using Noroshi.Core.WebApi.Response.Battle;

namespace Noroshi.BattleScene
{
    /// CPU キャラクターを扱うクラス。
    public class CpuCharacter : Character
    {
        /// 死亡時逃走演出フラグ。
        public bool IsDeadEscape { get; private set; }

        /// サーバーから受け取った BattleCharacter でインスタンス化。
        public CpuCharacter(BattleCharacter battleCharacter)
        {
            _battleCharacter = battleCharacter;
        }

        /// ボスかどうか。
        public bool IsBoss { get { return _battleCharacter.IsBoss; } }


        /// キャラクターステータスをロードする。
        protected override IObservable<CharacterStatus> _loadCharacterStatus()
        {
            return GlobalContainer.RepositoryManager.LoadCharacterStatus(_battleCharacter);
        }

        /// 死亡前逃走演出フラグを立てる。
        public void SetEscapeBeforeDeadOn()
        {
            IsDeadEscape = true;
            _view.SetEscapeBeforeDeadOn();
        }
        /// 死亡前逃走を実行する。
        public void EscapeBeforeDead()
        {
            _view.PlayEscapeBeforeDead();
        }
    }
}
