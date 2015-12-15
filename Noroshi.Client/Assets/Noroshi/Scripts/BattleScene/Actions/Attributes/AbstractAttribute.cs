using UniRx;

namespace Noroshi.BattleScene.Actions.Attributes
{
    public abstract class AbstractAttribute : IAttribute
    {
        Core.WebApi.Response.Character.Attribute _data;
        Subject<IAttribute> _onForceExit = new Subject<IAttribute>();
        protected float _coefficient;

        public AbstractAttribute(Core.WebApi.Response.Character.Attribute data, float coefficient)
        {
            _data        = data;
            _coefficient = coefficient;
        }

        public uint? GroupID { get { return _data.GroupID > 0 ? (uint?)_data.GroupID : null; } }
        public int? Lifetime { get { return _data.Lifetime > 0 ? (int?)_data.Lifetime : null; } }
        public uint CharacterEffectID { get { return _data.EffectID; } }
        public abstract bool IsNegative { get; }
        public virtual bool CharacterEffectOncePlay { get { return false; } }

        public abstract void OnEnter(IActionTarget target);
        public abstract void OnExit (IActionTarget target);
        public virtual void OnReceiveActionEvent(IActionTarget target, ActionEvent actionEvent) {}

        public IObservable<IAttribute> GetOnForceExit()
        {
            return _onForceExit.AsObservable();
        }
        protected void _forceExit()
        {
            _onForceExit.OnNext(this);
        }

        public virtual IObservable<ChangeableValueEvent> GetOnChangeHPObservable()
        {
            return Observable.Empty<ChangeableValueEvent>();
        }

        protected int _arg1  { get { return _data.Arg1 ; } }
        protected int _arg2  { get { return _data.Arg2 ; } }
        protected int _arg3  { get { return _data.Arg3 ; } }
        protected int _arg4  { get { return _data.Arg4 ; } }
        protected int _arg5  { get { return _data.Arg5 ; } }
        protected int _arg6  { get { return _data.Arg6 ; } }
        protected int _arg7  { get { return _data.Arg7 ; } }
        protected int _arg8  { get { return _data.Arg8 ; } }
        protected int _arg9  { get { return _data.Arg9 ; } }
        protected int _arg10 { get { return _data.Arg10; } }
        protected int _arg11 { get { return _data.Arg11; } }
        protected int _arg12 { get { return _data.Arg12; } }
        protected int _arg13 { get { return _data.Arg13; } }
        protected int _arg14 { get { return _data.Arg14; } }
        protected int _arg15 { get { return _data.Arg15; } }

        protected uint? _receiveDamageCharacterEffectID { get { return _data.ReceiveDamageCharacterEffectID == 0 ? null : (uint?)_data.ReceiveDamageCharacterEffectID; } }
    }
}