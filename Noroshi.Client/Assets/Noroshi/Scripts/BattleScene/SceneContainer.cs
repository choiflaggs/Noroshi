using Noroshi.WebApi;
using Noroshi.TimeUtil;
using Noroshi.BattleScene.Cache;

namespace Noroshi.BattleScene
{
    /// バトルシーン内でのみ利用する static 領域のコンテナクラス。
    public class SceneContainer : Noroshi.SceneContainer
    {
        /// SceneManager を取得。
        public static SceneManager GetSceneManager()
        {
            return Get<SceneManager>();
        }
        /// 非ゲームロジック領域（通常時は Unity 領域）のクラスをビルドして対応 IF を取得するための Factory を取得。
        public static IFactory GetFactory()
        {
            return Get<IFactory>();
        }
        /// 時間操作のための TimeHandler 取得。
        public static ITimeHandler GetTimeHandler()
        {
            return Get<ITimeHandler>();
        }
        /// Web API を叩くための WebApiRequester 取得。
        public static IWebApiRequester GetWebApiRequester()
        {
            return Get<IWebApiRequester>();
        }

        /// SceneManager にぶら下がっている BattleManager を取得するためのショートカット。
        public static IBattleManager GetBattleManager()
        {
            return GetSceneManager().BattleManager;
        }
        /// SceneManager にぶら下がっている CharacterManager を取得するためのショートカット。
        public static CharacterManager GetCharacterManager()
        {
            return GetSceneManager().CharacterManager;
        }
        /// SceneManager にぶら下がっている CacheManager を取得するためのショートカット。
        public static CacheManager GetCacheManager()
        {
            return GetSceneManager().CacheManager;
        }

        public static void Dispose()
        {
            if (Contains<SceneManager>())
            {
                GetSceneManager().Dispose();
            }
            if (Contains<IFactory>())
            {
                // TODO : Dispose 不要か確認。
            }
            if (Contains<ITimeHandler>())
            {
                // TODO : Dispose 不要か確認。
            }
            if (Contains<IWebApiRequester>())
            {
                // TODO : Dispose 不要か確認。
            }
            // static 領域をクリア。
            Clear();
        }
    }
}
