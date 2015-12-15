using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Noroshi.BattleScene;
using UniRx;
using UniRx.Triggers;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class FieldEffectMover : UIView
    {
        float _horizontalMoveMax;
        [SerializeField] GameObject[] _childEffectList = null;
        [SerializeField] float _speed = 1.0f;

        new void Awake()
        {
            base.Awake();
            // 移動限界値は現在設定されている1Waveの横幅を利用して算出するルールとする
            _horizontalMoveMax = -WaveField.HORIZONTAL_LENGTH * _childEffectList.Length;
            if (_transform.parent == null) return;

            var fieldEffectMovers = _transform.parent.GetComponentsInChildren<FieldEffectMover>();
            if (fieldEffectMovers.Length == 1)
            {
                var obj = Instantiate(_gameObject) as GameObject;
                obj.transform.SetParent(_transform.parent);
                var loaclPosition = GetLocalPosition();
                obj.transform.localPosition = new Vector2(loaclPosition.x + System.Math.Abs(_horizontalMoveMax), loaclPosition.y);
            }
        }

        void Start()
        {

            this.UpdateAsObservable()
            .Subscribe(_ => 
            {
                var localPosition = GetLocalPosition();
                var nextPosition = localPosition + Vector2.left * this._speed;
                if (nextPosition.x <= _horizontalMoveMax)
                {
                    nextPosition = new Vector2(localPosition.x + System.Math.Abs(_horizontalMoveMax) * 2, localPosition.y);
                }
                SetLocalPosition(nextPosition);
            });
        }
    }
}