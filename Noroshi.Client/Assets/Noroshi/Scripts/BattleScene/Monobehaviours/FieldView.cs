using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class FieldView : MonoBehaviour, IFieldView
    {
        [SerializeField] float _brightenTime = 0.5f;
        [SerializeField] float _darkenTime = 0.5f;
        [SerializeField] Color _darkColor = new Color(0.3f, 0.3f, 0.3f);
        SpriteRenderer[] _renderers;
        Dictionary<SpriteRenderer, Color> _defaultColors = new Dictionary<SpriteRenderer, Color>();

        void Start()
        {
            _renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in _renderers)
            {
                _defaultColors.Add(renderer, renderer.color);
            }
        }
        public void Brighten()
        {
            foreach (var renderer in _renderers)
            {
                renderer.DOColor(_defaultColors[renderer], _brightenTime);
            }
        }
        public void Darken()
        {
            foreach (var renderer in _renderers)
            {
                var darkColor = new Color(_darkColor.r, _darkColor.g, _darkColor.b, _defaultColors[renderer].a);
                renderer.DOColor(darkColor, _darkenTime);
            }
        }
    }
}