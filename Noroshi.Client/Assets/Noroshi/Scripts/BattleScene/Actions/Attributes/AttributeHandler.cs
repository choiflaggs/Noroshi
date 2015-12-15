using System.Collections.Generic;
using UniLinq;
using UniRx;

namespace Noroshi.BattleScene.Actions.Attributes
{
    public class AttributeHandler
    {
        IActionTarget _target;
        List<IAttribute> _attributes = new List<IAttribute>();
        Subject<IAttribute> _onAddAttributeSubject = new Subject<IAttribute>();
        Subject<IAttribute> _onRemoveAttributeSubject = new Subject<IAttribute>();
        Subject<ChangeableValueEvent> _onChangeHPSubject = new Subject<ChangeableValueEvent>();
        CompositeDisposable _disposables = new CompositeDisposable();
        Noroshi.Core.WebApi.Response.Character.Attribute[] _mastarDatas = null;

        public AttributeHandler(IActionTarget target)
        {
            _target = target;
        }

        /// Attribute 追加時に該当 Attribute がプッシュされる Observable を取得。
        public IObservable<IAttribute> GetOnAddAttributeObservable()
        {
            return _onAddAttributeSubject.AsObservable();
        }
        /// Attribute 削除時に該当 Attribute がプッシュされる Observable を取得。
        public IObservable<IAttribute> GetOnRemoveAttributeObservable()
        {
            return _onRemoveAttributeSubject.AsObservable();
        }
        public IObservable<ChangeableValueEvent> GetOnChangeHPObservable()
        {
            return _onChangeHPSubject.AsObservable();
        }

        public IObservable<AttributeHandler> LoadData()
        {
            return GlobalContainer.RepositoryManager.AttributeRepository.LoadAll().Do(mastarData => _mastarDatas = mastarData).Select(_ => this);
        }

        IAttribute _build(uint attributeId, float coefficient )
        {
            return AttributeBuilder.Build(_mastarDatas.FirstOrDefault(attr => attr.ID == attributeId), coefficient);
        }

        /// Attribute 追加
        public IObservable<IAttribute> AddAttribute(IAttribute attribute)
        {
            if (attribute == null)
            {
                return Observable.Empty<IAttribute>();
            }
            // 重複チェック
            if (attribute.GroupID.HasValue && _attributes.Any(a => a.GroupID == attribute.GroupID))
            {
                var oldAttribute = _attributes.First(a => a.GroupID == attribute.GroupID);
                _removeAttribute(oldAttribute);
            }
            attribute.GetOnForceExit().Subscribe(a => _removeAttribute(a)).AddTo(_disposables);
            _addAttribute(attribute);
            if (!attribute.Lifetime.HasValue) return Observable.Return(attribute);
            return SceneContainer.GetTimeHandler().Timer(attribute.Lifetime.Value).Select(_ =>
            {
                _removeAttribute(attribute);
                return attribute;
            });
        }
        void _addAttribute(IAttribute attribute)
        {
            attribute.GetOnChangeHPObservable().Subscribe(_onChangeHPSubject.OnNext);
            _attributes.Add(attribute);
            attribute.OnEnter(_target);
            _onAddAttributeSubject.OnNext(attribute);
        }

        /// 全ての Attribute を削除。
        public void RemoveAttributes()
        {
            foreach (var attribute in _attributes)
            {
                attribute.OnExit(_target);
                _onRemoveAttributeSubject.OnNext(attribute);
            }
            _attributes.Clear();
            _disposables.Clear();
        }
        void _removeAttribute(IAttribute attribute)
        {
            if (!_attributes.Contains(attribute)) return;
            attribute.OnExit(_target);
            _onRemoveAttributeSubject.OnNext(attribute);
            _attributes.Remove(attribute);
        }

        /// ActionEvent を受け取った際に呼ばれる。
        public void ReceiveActionEvent(ActionEvent actionEvent)
        {
            if (actionEvent.AttributeID.HasValue && !actionEvent.IsForceRemoveAttribute)
            {
                actionEvent.SetAttribute(_build(actionEvent.AttributeID.Value, actionEvent.AttributeCoefficient.Value));
            }
            if (actionEvent.IsForceRemoveAttribute)
            {
                _removeAttribute(actionEvent.Attribute);
                actionEvent.RemoveAttribute();
            }
            var attributes = new List<IAttribute>(_attributes);
            foreach (var attribute in attributes)
            {
                attribute.OnReceiveActionEvent(_target, actionEvent);
            }
        }

        public bool HasMissDamage()
        {
            return _attributes.Any(a => a.GetType() == typeof(MissDamage));
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}