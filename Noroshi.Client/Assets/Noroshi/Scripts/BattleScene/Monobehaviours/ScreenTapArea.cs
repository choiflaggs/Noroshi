using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Noroshi.BattleScene.MonoBehaviours
{
    public class ScreenTapArea : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
            vh.Clear();
        }

#if UNITY_EDITOR
        // Inspector上に何も表示したくないので、空の状態を定義する
        [CustomEditor(typeof(ScreenTapArea))]
        class FullScreenTapAreaEditor : Editor
        {
            public override void OnInspectorGUI () {}
        }
#endif
    }
}
