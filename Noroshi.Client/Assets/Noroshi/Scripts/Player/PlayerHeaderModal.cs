using UnityEngine;
using System.Collections;
using UniRx;
using DG.Tweening;

public class PlayerHeaderModal : MonoBehaviour {
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] BtnCommon btnClose;

    public Subject<bool> OnClose = new Subject<bool>();

    private GameObject _overlay;

    private void Start() {
        btnClose.OnClickedBtn.Subscribe(_ => {
            DOTween.To(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 0, 0.3f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => {
                    gameObject.SetActive(false);
                    _overlay.SetActive(false);
                });
        });
    }

    public void OnOpen(GameObject overlay) {
        _overlay = overlay;
        _overlay.SetActive(true);
        gameObject.SetActive(true);
        DOTween.To(() => canvasGroup.alpha, a => canvasGroup.alpha = a, 1, 0.3f)
            .SetEase(Ease.InCubic);
    }
}
