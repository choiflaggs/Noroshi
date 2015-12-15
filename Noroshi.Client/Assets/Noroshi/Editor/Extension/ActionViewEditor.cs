using System;
using System.Collections.Generic;
using Noroshi.BattleScene.Actions;
using Noroshi.BattleScene.MonoBehaviours;
using UniLinq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Noroshi.BattleScene;

namespace Noroshi.Editor.BattleScene.Extension
{
    // 見た目を合わせるためにAsynchronousMoveParam側で使用している列挙値に対し-1した値を定義しておく--
    public enum MoveEditorType
    {
        Parabola = 0,
        Linear = 1,
    }
    
    public enum PositionEditorType
    {
        Target = 0,
        ScreenCenter = 1,
        ScreenUpperPosition = 2,
    }
    //---

    [CustomEditor(typeof(ActionView))]
    public class ActionViewEditor : UnityEditor.Editor
    {
        List<string> _actionNameList = new List<string>()
        {
            Constant.ACTION_RANK_0_ANIMATION_NAME,
            Constant.ACTION_RANK_1_ANIMATION_NAME,
            Constant.ACTION_RANK_2_ANIMATION_NAME,
            Constant.ACTION_RANK_3_ANIMATION_NAME,
            Constant.ACTION_RANK_4_ANIMATION_NAME,
        };
        void _drawUnreversibleTransforms(SerializedProperty prop)
        {
            var foldoutName = ObjectNames.NicifyVariableName(prop.displayName);
            switch (prop.propertyType)
            {
            case SerializedPropertyType.Generic:
                prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, foldoutName);
                if (!prop.isExpanded) break;
                if (prop.isArray)
                {
                    prop.arraySize = EditorGUILayout.IntField("Size", prop.arraySize);
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < prop.arraySize; i++)
                    {
                        _drawUnreversibleTransforms(prop.GetArrayElementAtIndex(i));
                    }
                    EditorGUI.indentLevel--;
                }
                break;
            case SerializedPropertyType.Integer:
                prop.intValue = EditorGUILayout.IntField(foldoutName, prop.intValue);
                break;
            case SerializedPropertyType.ObjectReference:
                EditorGUILayout.PropertyField(prop);
                break;
            }
        }

        // ActionViewのクラス変数として定義されているクラスの中身を表示する
        void _drawAsynchronousMoveParam(string propertyName)
        {
            var prop = serializedObject.FindProperty(propertyName);
            prop.isExpanded = EditorGUILayout.Foldout(prop.isExpanded, prop.displayName);
            if (!prop.isExpanded) return;
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(prop.FindPropertyRelative("_useAngle"));

            var moveTypeProp = prop.FindPropertyRelative("_moveType");
            EditorGUILayout.PropertyField(moveTypeProp);

            EditorGUILayout.PropertyField(prop.FindPropertyRelative("_duration"));
            
            EditorGUILayout.PropertyField(prop.FindPropertyRelative("_horizontalEasing"));
            switch ((MoveEditorType)moveTypeProp.enumValueIndex)
            {
            case MoveEditorType.Parabola:
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("_verticalInEasing"));
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("_verticalOutEasing"));
                break;
            case MoveEditorType.Linear:
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("_verticalEasing"));
                break;
            default:
                throw new ArgumentOutOfRangeException();
            }

            var positionTypeProp = prop.FindPropertyRelative("_positionType");
            EditorGUILayout.PropertyField(positionTypeProp);
            if ((PositionEditorType)positionTypeProp.enumValueIndex == PositionEditorType.ScreenUpperPosition)
            {
                EditorGUILayout.PropertyField(prop.FindPropertyRelative("_upperPosition"));
            }

            var sortinglayerSettingsProp = prop.FindPropertyRelative("_sortingLayerSettings");
            sortinglayerSettingsProp.isExpanded = EditorGUILayout.Foldout(sortinglayerSettingsProp.isExpanded, sortinglayerSettingsProp.displayName);
            if(sortinglayerSettingsProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                sortinglayerSettingsProp.arraySize = EditorGUILayout.IntField("Size", sortinglayerSettingsProp.arraySize);
                for (var i = 0; i < sortinglayerSettingsProp.arraySize; i++)
                {
                    _drawSortingLayerSetting(sortinglayerSettingsProp.GetArrayElementAtIndex(i));
                }
                EditorGUI.indentLevel--;
            }

            EditorGUI.indentLevel--;
        }

        // 各アクションごとにMeshRendererのSortingLayerNameを設定できるように表示する
        void _drawSortingLayerSetting(SerializedProperty childProp)
        {
            childProp.isExpanded = EditorGUILayout.Foldout(childProp.isExpanded, childProp.displayName);
            EditorGUI.indentLevel++;
            if (!childProp.isExpanded)
            {
                EditorGUI.indentLevel--;
                return;
            }
            EditorGUILayout.PropertyField(childProp.FindPropertyRelative("_meshRenderer"));
            var sortingLayerProp = childProp.FindPropertyRelative("_layerName");
            var layerNameList = new List<string>(_getSortingLayerNames());
            var selectIndex = layerNameList.FindIndex(name => name == sortingLayerProp.stringValue);
            if (selectIndex < 0) selectIndex = layerNameList.FindIndex(name => name == "Default");
            selectIndex = EditorGUILayout.Popup("Sorting Layer", selectIndex, layerNameList.ToArray());
            sortingLayerProp.stringValue = layerNameList[selectIndex];
            EditorGUILayout.PropertyField(childProp.FindPropertyRelative("_relativeSortingOrder"));
            EditorGUI.indentLevel--;
        }

        string[] _getSortingLayerNames()
        {
            var internalEditorUtilityType = typeof(InternalEditorUtility);
            var propertyInfo = internalEditorUtilityType.GetProperty("sortingLayerNames", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            return (string[])propertyInfo.GetValue(null, new object[0]);
        }

        // ActionViewのInspectorが描画されている間呼ばれ続ける
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("_debugLog"));
            _drawUnreversibleTransforms(serializedObject.FindProperty("_unreversibleTransforms"));
            var style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            for (var i = 1; i <= 5; i++)
            {
                EditorGUILayout.LabelField(_actionNameList[i - 1], style);
                _drawAsynchronousMoveParam("_action" + i.ToString() + "AsynchronousMoveParam");
                EditorGUILayout.Space();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}