using System.Collections.Generic;
using UniLinq;
using UniRx;
using UnityEngine;
using DG.Tweening;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class ResultUIView : ModalUIView, UI.IResultUIView
    {
        [SerializeField] UnityEngine.UI.Text _playerExp;
        [SerializeField] UnityEngine.UI.Text _money;

        [SerializeField] ResultCharacterUIView _ownCharacter1;
        [SerializeField] ResultCharacterUIView _ownCharacter2;
        [SerializeField] ResultCharacterUIView _ownCharacter3;
        [SerializeField] ResultCharacterUIView _ownCharacter4;
        [SerializeField] ResultCharacterUIView _ownCharacter5;
        [SerializeField] ResultCharacterUIView _enemyCharacter1;
        [SerializeField] ResultCharacterUIView _enemyCharacter2;
        [SerializeField] ResultCharacterUIView _enemyCharacter3;
        [SerializeField] ResultCharacterUIView _enemyCharacter4;
        [SerializeField] ResultCharacterUIView _enemyCharacter5;
        [SerializeField] ResultItemUIView _item1;
        [SerializeField] ResultItemUIView _item2;
        [SerializeField] ResultItemUIView _item3;
        [SerializeField] ResultItemUIView _item4;
        [SerializeField] ResultItemUIView _item5;
        [SerializeField] EvaluationStarAnimation[] _evaluationStars;
        [SerializeField] UIView _enemyCharacterContainer;
        [SerializeField] UIView _ownCharacterContainer;
        [SerializeField] UnityEngine.UI.Image _statsTextImage;
        [SerializeField] UnityEngine.UI.Image _victoryLineImage;
        [SerializeField] UnityEngine.UI.Image _winLossTextImage;
        [SerializeField] UnityEngine.UI.Image _winTextAddtiveImage;
        [SerializeField] UIView _bottomItemContainer;
        [SerializeField] UnityEngine.UI.Text _tipsMessage;


        Dictionary<byte, ResultCharacterUIView> _characterNoToOwnResultCharacterUI;
        Dictionary<byte, ResultCharacterUIView> _characterNoToEnemyResultCharacterUI;
        Dictionary<byte, ResultItemUIView> _itemNoToResultItemUI;

        Vector2 _enemyCharacterContainerDefaultAnchorPos = Vector2.zero;
        Vector2 _ownCharacterContainerDefaultAnchorPos = Vector2.zero;
        readonly float _characterContainerMoveDuration = 0.5f;
        readonly float _windowShakeAnimDuration = 0.015f;
        byte _rank = 0;

        new void Awake()
        {
            base.Awake();
            _characterNoToOwnResultCharacterUI = new Dictionary<byte, ResultCharacterUIView>{
                {1, _ownCharacter1},
                {2, _ownCharacter2},
                {3, _ownCharacter3},
                {4, _ownCharacter4},
                {5, _ownCharacter5},
            };
            _characterNoToEnemyResultCharacterUI = new Dictionary<byte, ResultCharacterUIView>{
                {1, _enemyCharacter1},
                {2, _enemyCharacter2},
                {3, _enemyCharacter3},
                {4, _enemyCharacter4},
                {5, _enemyCharacter5},
            };
            _itemNoToResultItemUI = new Dictionary<byte, ResultItemUIView>{
                {1, _item1},
                {2, _item2},
                {3, _item3},
                {4, _item4},
                {5, _item5},
            };

        }

        void Start()
        {
            byte starNo = 0;
            foreach(var evaluationStar in _evaluationStars)
            {
                evaluationStar.SetActive(starNo < _rank);
                starNo++;
            }
            _bottomItemContainer.SetActive(false);
            _enemyCharacterContainerDefaultAnchorPos = _enemyCharacterContainer.GetRectTransform().anchoredPosition;
            _ownCharacterContainerDefaultAnchorPos = _ownCharacterContainer.GetRectTransform().anchoredPosition;
            _enemyCharacterContainer.GetRectTransform().anchoredPosition = new Vector2(200, _enemyCharacterContainerDefaultAnchorPos.y);
            _ownCharacterContainer.GetRectTransform().anchoredPosition = new Vector2(-869, _ownCharacterContainerDefaultAnchorPos.y);
        }

        public void SetBattleRank(byte rank)
        {
            _rank = rank;
        }
        public void SetPlayerExp(ushort exp)
        {
            if (_playerExp == null) return;
            _playerExp.text = exp.ToString();
        }
        public void SetMoney(uint money)
        {
            if (_money == null) return;
            _money.text = money.ToString();
        }
        public void SetItemIDs(IEnumerable<uint> itemIds)
        {
            if (_itemNoToResultItemUI.Values.Where(ui => ui != null).Count() == 0) return;
            var itemNum = itemIds.Count();
            var maxItemUINum = _itemNoToResultItemUI.Count();
            for (byte no = 1; no <= maxItemUINum; no++)
            {
                if (no <= itemNum)
                {
                    _itemNoToResultItemUI[no].SetActive(true);
                }
                else
                {
                    _itemNoToResultItemUI[no].SetActive(false);
                }
            }
        }
        public void SetOwnCharacterThumbnails(IEnumerable<CharacterThumbnail> characterThumbnail)
        {
            var characterNum = characterThumbnail.Count();
            var maxCharacterUINum = _characterNoToOwnResultCharacterUI.Count();
            var characterThumbnailArray = characterThumbnail.ToArray();
            for (byte no = 1; no <= maxCharacterUINum; no++)
            {
                if (no <= characterNum)
                {
                    _characterNoToOwnResultCharacterUI[no].SetActive(true);
                    _characterNoToOwnResultCharacterUI[no].SetCharacterThumbnail(characterThumbnailArray[no - 1]);
                }
                else
                {
                    _characterNoToOwnResultCharacterUI[no].SetActive(false);
                }
            }
        }
        public void SetEnemyCharacterThumbnails(IEnumerable<CharacterThumbnail> characterThumbnail)
        {
            var characterNum = characterThumbnail.Count();
            var maxCharacterUINum = _characterNoToEnemyResultCharacterUI.Count();
            var characterThumbnailArray = characterThumbnail.ToArray();
            for (byte no = 1; no <= maxCharacterUINum; no++)
            {
                if (no <= characterNum)
                {
                    _characterNoToEnemyResultCharacterUI[no].SetActive(true);
                    _characterNoToEnemyResultCharacterUI[no].SetCharacterThumbnail(characterThumbnailArray[no - 1]);
                }
                else
                {
                    _characterNoToEnemyResultCharacterUI[no].SetActive(false);
                }
            }
        }
        public void SetOwnCharacterProgress(byte characterNo, float previousExpRatio, float currentExpRatio, ushort levelUpNum)
        {
            _characterNoToOwnResultCharacterUI[characterNo].SetProgress(previousExpRatio, currentExpRatio, levelUpNum);
        }
        public void SetOwnCharacterStatistics(byte characterNo, uint damage, float damageRatio)
        {
            _characterNoToOwnResultCharacterUI[characterNo].SetStatistics(damage, damageRatio);
        }
        public void SetEnemyCharacterStatistics(byte characterNo, uint damage, float damageRatio)
        {
            _characterNoToEnemyResultCharacterUI[characterNo].SetStatistics(damage, damageRatio);
        }
        
        public void SetLossTipsMessage(string tipsMessage)
        {
            if (_tipsMessage != null) _tipsMessage.text = tipsMessage;
        }

        public IObservable<UI.IResultUIView> LoadAssets()
        {
            return Observable.WhenAll(
                _characterNoToOwnResultCharacterUI.Values.Select(ui => ui.LoadSprite())
                .Concat(
                    _characterNoToEnemyResultCharacterUI.Values.Select(ui => ui.LoadSprite())
                )
            )
            .Select(_ => (UI.IResultUIView)this);
        }

        IObservable<bool> _playWinLoseAnimation()
        {
            Subject<bool> onAnimEnd = new Subject<bool>();

            _victoryLineImage.DOColor(Color.white, 0.1f);

            if (_winTextAddtiveImage != null)
            {
                _winLossTextImage.DOColor(Color.white, 0.1f);
                _winTextAddtiveImage.DOColor(Color.white, 0.1f)
                    .OnComplete( () =>
                    {
                        _winTextAddtiveImage.DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), 0.2f)
                            .OnComplete(() =>
                            {
                                onAnimEnd.OnNext(true);
                                onAnimEnd.OnCompleted();
                            });
                    });
            }
            else
            {
                _winLossTextImage.DOColor(Color.black, 0.1f).OnComplete( () =>
                {
                    _winLossTextImage.DOColor(Color.white, 0.2f).OnComplete( () =>
                    {
                        onAnimEnd.OnNext(true);
                        onAnimEnd.OnCompleted();
                    });
                });
            }
            return onAnimEnd.AsObservable();
        }

        IObservable<bool> _playOnecCharacterContainerAnimation(Vector2 endAnchorPos, RectTransform characterContainerRectTransform)
        {
            Subject<bool> onComplete = new Subject<bool>();
            characterContainerRectTransform.DOAnchorPos(endAnchorPos, _characterContainerMoveDuration).OnComplete( () =>
            {
                onComplete.OnNext(true);
                onComplete.OnCompleted();
            });

            return onComplete.AsObservable();
        }
        
        IObservable<bool> _playAllCharacterContainerAnimation()
        {
            _statsTextImage.enabled = true;
            return Observable.WhenAll(
                    _playOnecCharacterContainerAnimation(_enemyCharacterContainerDefaultAnchorPos, _enemyCharacterContainer.GetRectTransform()),
                    _playOnecCharacterContainerAnimation(_ownCharacterContainerDefaultAnchorPos, _ownCharacterContainer.GetRectTransform())
                )
                .Select(_ => true);
        }

        IObservable<bool> _playAllEvaluationStarAnimation()
        {
            return _playOnceEvaluationStarAnimation(0)
                    .SelectMany(_ => _playOnceEvaluationStarAnimation(1))
                    .SelectMany(_ => _playOnceEvaluationStarAnimation(2));
        }

        IObservable<bool> _playOnceEvaluationStarAnimation(int starNo)
        {
            if (_rank == 0 || _evaluationStars == null || _evaluationStars.Length == 0) return Observable.Return<bool>(false);
            if (starNo >= _rank) return Observable.Return<bool>(false);
            _evaluationStars[starNo].SetActive(true);
            return _evaluationStars[starNo].PlayAnimation()
                .Do(_ => 
                {
                    _window.DOKill();
                    _window.GetRectTransform().anchoredPosition = Vector2.zero;
                    _window.GetRectTransform().DOAnchorPos(new Vector2(0, -20), _windowShakeAnimDuration).SetLoops(4, LoopType.Yoyo);
                })
                .Select(_ => true);
        }

        IObservable<bool> _playOwnCharacterSliderAnimation()
        {
            return Observable.WhenAll(
                    _characterNoToOwnResultCharacterUI.Values
                    .Where(ownCharacter => ownCharacter.isActiveAndEnabled)
                    .Select(ownCharacter => ownCharacter.PlaySliderAnimation())
                )
                .Select(_ => true);
        }

        IObservable<bool> _playEnemyCharacterSliderAnimation()
        {
            return Observable.WhenAll(
                    _characterNoToEnemyResultCharacterUI.Values
                    .Where(enemyCharacter => enemyCharacter.isActiveAndEnabled)
                    .Select(enemyCharacter => enemyCharacter.PlaySliderAnimation())
                )
                .Select(_ => true);
        }

        IObservable<bool> _playCharacterSliderAnimation()
        {
            return Observable.WhenAll(
                    _playOwnCharacterSliderAnimation(),
                    _playEnemyCharacterSliderAnimation()
                )
                .Select(_ => true);
        }

        public override IObservable<UI.IModalUIView> Open()
        {
            return base.Open()
                .SelectMany(_ => _playAllEvaluationStarAnimation())
                .SelectMany(_ => _playAllCharacterContainerAnimation())
                .SelectMany(_ => _playCharacterSliderAnimation())
                .SelectMany(_ => _playWinLoseAnimation())
                .Do(_ => _bottomItemContainer.SetActive(true))
                .Select(__ => (UI.IModalUIView)this);
        }

    }
}