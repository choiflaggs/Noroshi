using System.Collections.Generic;
using UniRx;

namespace Noroshi.BattleScene.Actions
{
    public enum Trigger
    {
        Normal = 1,
        Active = 2,
        First = 3,
        Dead = 4,
        PhysicalDamage = 5,
        EnemyDead = 6,
        FirstWithAnimation = 7
    }
    public interface IAction
    {
        uint ID { get; }
        int Level { get; }
        byte Rank { get; }
        Trigger Trigger { get; }
        float? ExecutableProbability { get; }
        byte? ExecutableNum { get; }
        byte? ExecutorTargetable { get; }
        string AnimationName { get; }
        DamageType? DamageType { get; }
        uint? SoundID { get; }
        uint? ExecutionSoundID { get; }
        void SetRank(byte rank);
        void SetLevel(int level);
        void SetAnimation(ActionAnimation animation);
        IActionTarget[] GetTargets(IActionTargetFinder actionTargetFinder, IActionExecutor executor);
        void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates);
        void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets);
        void PostProcess(IActionExecutor executor);
        void Cancel(IActionExecutor executor);
        void Reset(IActionExecutor executor);
        IObservable<IAction> LoadAdditionalDatas(Repositories.Server.ActionRepository repository);
        IObservable<IAction> LoadAssets(IActionExecutor executor, IActionFactory factory);
        IObservable<CameraShakeByActionType> GetOnTryCameraShakeObservable();
        IObservable<KeyValuePair<byte, Dictionary<byte, int>>> GetOnOverrideArgsObservable();
        IObservable<KeyValuePair<byte, uint>> GetOnOverrideAttributeIdsObservable();
        void OverrideArg(byte argNo, int overrideValue);
        void OverrideAttributeId(uint attributeId);
        void EnterTimeStop();
        void ExitTimeStop();
    }
}
