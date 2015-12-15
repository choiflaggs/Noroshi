using System;
using System.Collections.Generic;
using UniRx;
using Noroshi.Core.Game.Enums;
using Noroshi.Core.Game.Battle;
using UniLinq;

namespace Noroshi.BattleScene.Actions
{
    public enum DamageType
    {
        Physical = 1,
        Magical = 2,
    }
    public enum TargetSortType
    {
        CurrentHPAsc = 1,
        MaxHPDesc = 2,
        PositionFront = 3,
        Random = 4,
    }
    public enum ActionEffectType
    {
        ViewOnly = 1,
        ViewAndCameraShake = 2,
        CameraShakeOnly = 3,
    }

    public enum CameraShakeByActionType
    {
        Play = 1,
        Pause = 2,
        Replay = 3,
    }

    public abstract class AbstractAction : IAction
    {
        Core.WebApi.Response.Character.Action _data;
        Dictionary<byte, int> _overrideArgs = new Dictionary<byte, int>();
        uint? _overrideAttributeId = null;
        protected ActionAnimation _animation;
        IActionView _actionView = null;
        Subject<CameraShakeByActionType> _onTryCameraShakeSubject = new Subject<CameraShakeByActionType>();
        public byte Rank { get; private set; }
        public int Level { get; private set; }

        public AbstractAction(Core.WebApi.Response.Character.Action data)
        {
            _data = data;
        }
        public void SetRank(byte rank) { Rank = rank; }

        public uint ID { get { return _data.ID; } }

        public Trigger Trigger { get { return (Trigger)_data.TriggerID; } }
        public float? ExecutableProbability { get { return _data.ExecutableProbability; } }
        public byte? ExecutableNum { get { return _data.ExecutableNum; } }
        public byte? ExecutorTargetable { get { return _data.ExecutorTargetable; } }
        protected TargetSortType? _targetSortType { get { return (TargetSortType?)_data.TargetSortType; } }
        protected byte? _maxTargetNum { get { return _data.MaxTargetNum; } }
        public DamageType? DamageType { get { return _data.DamageType > 0 ? (DamageType?)_data.DamageType : null; } }
        protected DamageMagicalAttribute? _damageMagicalAttribute { get { return _data.DamageMagicalAttribute > 0 ? (DamageMagicalAttribute?)_data.DamageMagicalAttribute : null; } }
        protected TargetStateID? _targetStateId { get { return _data.TargetStateID > 0 ? (TargetStateID?)_data.TargetStateID : null; } }
        protected uint? _attributeId { get { return _overrideAttributeId.HasValue ? _overrideAttributeId.Value : _data.AttributeID1 > 0 ? (uint?)_data.AttributeID1 : null; } }
        protected float _attributeIntercept { get { return _data.AttributeIntercept1; } }
        protected float _attributeSlope { get { return _data.AttributeSlope1; } }

        protected int _arg1  { get { return _overrideArgs.ContainsKey(1) ? _overrideArgs[1] : _data.Arg1 ; } }
        protected int _arg2  { get { return _overrideArgs.ContainsKey(2) ? _overrideArgs[2] : _data.Arg2 ; } }
        protected int _arg3  { get { return _overrideArgs.ContainsKey(3) ? _overrideArgs[3] : _data.Arg3 ; } }
        protected int _arg4  { get { return _overrideArgs.ContainsKey(4) ? _overrideArgs[4] : _data.Arg4 ; } }
        protected int _arg5  { get { return _overrideArgs.ContainsKey(5) ? _overrideArgs[5] : _data.Arg5 ; } }
        protected int _arg6  { get { return _overrideArgs.ContainsKey(6) ? _overrideArgs[6] : _data.Arg6 ; } }
        protected int _arg7  { get { return _overrideArgs.ContainsKey(7) ? _overrideArgs[7] : _data.Arg7 ; } }
        protected int _arg8  { get { return _overrideArgs.ContainsKey(8) ? _overrideArgs[8] : _data.Arg8 ; } }
        protected int _arg9  { get { return _overrideArgs.ContainsKey(9) ? _overrideArgs[9] : _data.Arg9 ; } }
        protected int _arg10 { get { return _overrideArgs.ContainsKey(10) ? _overrideArgs[10] : _data.Arg10; } }

        float _intercept1 { get { return _data.Intercept1; } }
        float _slope1     { get { return _data.Slope1    ; } }
        float _intercept2 { get { return _data.Intercept2; } }
        float _slope2     { get { return _data.Slope2    ; } }
        float _intercept3 { get { return _data.Intercept3; } }
        float _slope3     { get { return _data.Slope3    ; } }

        protected uint? _hitCharacterEffectID { get { return _data.HitCharacterEffectID; } }
        protected uint? _hitSoundId { get { return _data.HitSoundID == 0 ? null : (uint?)_data.HitSoundID; } }
        public uint? SoundID { get { return _data.SoundID; } }
        public uint? ExecutionSoundID { get { return _data.ExecutionSoundID; } }

        protected float _getLevelDrivenAttributeParam() { return _attributeIntercept + _attributeSlope * (Level - 1); }
        protected float _getLevelDrivenParam1() { return _intercept1 + _slope1 * (Level - 1); }
        protected float _getLevelDrivenParam2() { return _intercept2 + _slope2 * (Level - 1); }
        protected float _getLevelDrivenParam3() { return _intercept3 + _slope3 * (Level - 1); }

        protected virtual ActionEffectType? _actionEffectType { get { return null; } }
        protected void _tryCameraShake()
        {
            if (_shouldUseCameraShake()) _onTryCameraShakeSubject.OnNext(CameraShakeByActionType.Play);
        }

        public void SetLevel(int level)
        {
            Level = level;
        }
        public void SetAnimation(ActionAnimation animation)
        {
            _animation = animation;
        }
        public virtual IObservable<KeyValuePair<byte, Dictionary<byte, int>>> GetOnOverrideArgsObservable()
        {
            return Observable.Empty<KeyValuePair<byte, Dictionary<byte, int>>>();
        }
        public virtual IObservable<KeyValuePair<byte, uint>> GetOnOverrideAttributeIdsObservable()
        {
            return Observable.Empty<KeyValuePair<byte, uint>>();
        }
        public void OverrideArg(byte argNo, int overrideValue)
        {
            if (_overrideArgs.ContainsKey(argNo)) _overrideArgs.Remove(argNo);
            _overrideArgs.Add(argNo, overrideValue);
        }
        public void OverrideAttributeId(uint attributeId)
        {
            _overrideAttributeId = (uint?)attributeId;
        }

        public abstract IActionTarget[] GetTargets(IActionTargetFinder actionTargetFinder, IActionExecutor executor);
        public virtual void PreProcess(IActionExecutor executor, IActionTarget[] targetCandidates) {}
        public abstract void Execute(IActionTargetFinder actionTargetFinder, IActionExecutor executor, IActionTarget[] targets);
        public virtual void PostProcess(IActionExecutor executor) {}
        public virtual void Cancel(IActionExecutor executor) {}
        public virtual void Reset(IActionExecutor executor) {}

        public virtual void EnterTimeStop()
        {
            if (_shouldUseActionView()) _actionView.PauseOn();
            if (_shouldUseCameraShake()) _onTryCameraShakeSubject.OnNext(CameraShakeByActionType.Pause);
        }
        public virtual void ExitTimeStop()
        {
            if (_shouldUseActionView()) _actionView.PauseOff();
            if (_shouldUseCameraShake()) _onTryCameraShakeSubject.OnNext(CameraShakeByActionType.Replay);
        }

        // TODO : リポジトリ抽象化
        public virtual IObservable<IAction> LoadAdditionalDatas(Repositories.Server.ActionRepository repository)
        {
            return Observable.Return<IAction>(this);
        }
        public virtual IObservable<IAction> LoadAssets(IActionExecutor executor, IActionFactory factory)
        {
            if (!_shouldUseActionView()) return Observable.Return<IAction>(this);
            return factory.BuildActionView(executor.CharacterID).Select(v => 
                                                                        {
                _actionView = v;
                _actionView.Disappear();
                return (IAction)this;
            });
        }

        public IObservable<CameraShakeByActionType> GetOnTryCameraShakeObservable()
        {
            return _onTryCameraShakeSubject.AsObservable();
        }

        public virtual string AnimationName
        {
            get
            {
                var names = new string[]{
                    Constant.ACTION_RANK_0_ANIMATION_NAME,
                    Constant.ACTION_RANK_1_ANIMATION_NAME,
                    Constant.ACTION_RANK_2_ANIMATION_NAME,
                    Constant.ACTION_RANK_3_ANIMATION_NAME,
                    Constant.ACTION_RANK_4_ANIMATION_NAME,
                    Constant.ACTION_RANK_5_ANIMATION_NAME,
                };
                return names[Rank];
            }
        }

        protected void _tryToSetActionViewSkinLevel(int skinLevel)
        {
            if (_shouldUseActionView()) _actionView.SetSkin(skinLevel);
        }

        protected void _tryActionViewAppearance(Grid.GridPosition grid, Grid.Direction direction, string animationName, int sortOrder)
        {
            if (_shouldUseActionView()) _actionView.Appear(grid, direction, animationName, sortOrder);
        }
        protected void _tryActionViewAppearance(UnityEngine.Vector2 position, Grid.Direction direction, string animationName, int sortOrder)
        {
            if (_shouldUseActionView()) _actionView.Appear(position, direction, animationName, sortOrder);
        }
        protected IObservable<IActionView> _tryToLaunchActionView(IActionExecutorView executorView, Grid.GridPosition grid, Grid.Direction direction, string animationName, int sortOrder)
        {
            return _shouldUseActionView() ? _actionView.Launch(executorView, grid, direction, animationName, sortOrder) : Observable.Empty<IActionView>();
        }
        protected IObservable<IActionView> _tryToLaunchActionView(IActionExecutorView executorView, Grid.GridPosition grid, Grid.Direction direction, string animationName, int sortOrder, byte? animationSequenceNumber)
        {
            return _shouldUseActionView() ? _actionView.Launch(executorView, grid, direction, animationName, sortOrder, animationSequenceNumber) : Observable.Empty<IActionView>();
        }
        protected void _tryActionViewDisappearance()
        {
            if (_shouldUseActionView()) _actionView.Disappear();
        }

        protected bool _shouldUseCameraShake()
        {
            if (_actionEffectType.HasValue)
            {
                return (_actionEffectType.Value == ActionEffectType.ViewAndCameraShake ||
                        _actionEffectType.Value == ActionEffectType.CameraShakeOnly);
            }
            return false;
        }
        
        protected bool _shouldUseActionView()
        {
            if (_actionEffectType.HasValue)
            {
                return (_actionEffectType.Value == ActionEffectType.ViewOnly || 
                        _actionEffectType.Value == ActionEffectType.ViewAndCameraShake);
            }
            return false;
        }

        public override string ToString()
        {
            return String.Format("ID:{0}, Type:{1}", ID, GetType().Name);
        }
    }
}