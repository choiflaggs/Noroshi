using UnityEngine;
using System.Collections;
using UnityEditor;
using Noroshi.BattleScene.MonoBehaviours;

namespace Noroshi.Editor.BattleScene.Extension
{
    public class FieldEffectDuplicate
    {
        [UnityEditor.MenuItem("GameObject/FieldEffectDuplicate", false, 20)]
        static void _duplicate()
        {
            var duplicateNum = 3;
            var selectionObj = Selection.activeGameObject;
            if (selectionObj == null)
            {
                EditorUtility.DisplayDialog("Error", "複製対象となるオブジェクトが選択されていません", "OK");
                return;
            }
            if (selectionObj.transform.parent == null)
            {
                EditorUtility.DisplayDialog("Error", "複製対象に対して親オブジェクトが見当たりません", "OK");
                return;
            }
            var fieldEffectMover = selectionObj.transform.parent.GetComponent<FieldEffectMover>();
            if (fieldEffectMover == null)
            {
                EditorUtility.DisplayDialog("Error", "複製対象となるオブジェクトの親に対し<FieldEffectMover>コンポーネントが見当たりません", "OK");
                return;
            }
            for (var i = 0; i < duplicateNum; i++)
            {
                var obj = GameObject.Instantiate(selectionObj) as GameObject;
                obj.name = selectionObj.name + (i + 1).ToString();
                obj.transform.SetParent(selectionObj.transform.parent);
                obj.transform.localPosition = new Vector3(Noroshi.BattleScene.WaveField.HORIZONTAL_LENGTH * (i + 1), 0, 0);
            }
        }
    }
}