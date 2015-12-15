using System.Collections.Generic;
using UniRx;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Character;
using Noroshi.BattleScene.Drop;
using Noroshi.BattleScene.CharacterEffect;
using Noroshi.BattleScene.Sound;
using CameraShakeByActionType = Noroshi.BattleScene.Actions.CameraShakeByActionType;

namespace Noroshi.BattleScene
{
    public interface IBattleManager : IManager
    {
        /// 現在の Wave 番号
        byte CurrentWaveNo { get; }

        DropHandler DropHandler { get; }
        BattleResult BattleResult { get; }

        BattleAutoMode BattleAutoMode { get; }
        ushort PlayerExp { get; }

        IObservable<byte> GetOnCountDownObservable();
        IObservable<bool> GetOnToggleNextWaveButtonVisibleObservable();
        IObservable<byte> GetOnCompletePrepareNextWaveObservable();
       /// バトル終了時にプッシュされる Observable を取得
        IObservable<VictoryOrDefeat> GetOnFinishBattleObservable();
        IObservable<CharacterEffectEvent> GetOnCommandCharacterEffectObservable();
        IObservable<SoundEvent> GetOnCommandSoundObservable();
        IObservable<float> GetOnSlowObservable();
        IObservable<byte> GetOnEnterWaveBattleObservable();
        IObservable<bool> GetOnWaitRetryObservable();
        IObservable<Noroshi.Core.WebApi.Response.Battle.IBattleFinishResponse> SendResult();
        IObservable<CameraShakeByActionType> GetOnTryCameraShakeObservable();

        /// 最後の Wave 番号
        int WaveNum { get; }
        /// 現在の Wave
        Wave CurrentWave { get; }

        IObservable<VictoryOrDefeat> Start();

        void StartWave();

        void TryToTransitToNextWave();

        IObservable<IBattleManager> StartInterval();
        IObservable<Wave> SwitchWave();
        float GetSwitchWaveTime();

        IObservable<IBattleManager> RunOwnCharactersToCorrectPosition();

        void SetAutoMode(bool isOn);

        /// Ready 状態の終了を待つべきかどうか判定。
        bool ShouldWaitToFinishReady();

        Character GetPickUpCharacter();

        void LogicUpdate();
        Noroshi.Core.WebApi.Response.Battle.AdditionalInformation AdditionalInformation{ get; }
    }
}
