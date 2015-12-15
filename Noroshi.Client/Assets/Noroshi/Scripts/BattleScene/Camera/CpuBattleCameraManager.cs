using UniRx;

namespace Noroshi.BattleScene.Camera
{
    public class CpuBattleCameraManager : AbstractCameraManager
    {
        public override void Prepare()
        {
            base.Prepare();
            var battleManager = (CpuBattleManager)SceneContainer.GetBattleManager();
            if (battleManager.GetBeforeBattleStory() != null)
            {
                // バトル前ストーリーが存在する場合、通常よりも右ポジションから場面がスタートするので、カメラもその位置へ移動しておき、
                // （仮想的に Wave0 とするが、Wave0 にはゲームロジックが存在しない）
                byte waveNoForStory = 0;
                _cameraController.MoveX(WaveField.GetPositionX(waveNoForStory), 0.001f);
                // ストーリー後に Wave1 へカメラを動かす。
                byte nextWaveNo = 1;
                SceneContainer.GetSceneManager().GetOnExitBeforeBattleStoryObservable()
                .SelectMany(_ => _moveCamera(nextWaveNo))
                .SelectMany(_ => SceneContainer.GetBattleManager().RunOwnCharactersToCorrectPosition())
                .Subscribe(_ => SceneContainer.GetSceneManager().FinishReady()).AddTo(_disposables);                
            }
        }
    }
}
