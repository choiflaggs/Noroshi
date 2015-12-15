using UnityEngine;
using UnityEditor;
using System.Collections;
using Noroshi.BattleScene;
using Noroshi.BattleScene.MonoBehaviours;
using Noroshi.Grid;

namespace NoroshiDebug.PickupCharacterEdit.Editor.Window
{
    public class PickupCharacterEditWindow : EditorWindow
    {
        const string RESOURCES_PATH = "Assets/Noroshi/Resources/Character/{0}/Character.prefab";
        const string SKIN_NAME = "step";

        int _characterID = 0;
        GameObject _character = null;
        SerializedObject _serializeObject = null;
        Vector2 _scrollPos = Vector2.zero;
        bool _isDirectionLeft = false;
        bool _isPreviewStory = true;
        SkeletonAnimation _skeletonAnimation = null;

        [MenuItem("PickupCharacter/Edit")]
        static void Init()
        {
            var editorWindow = GetWindow(typeof(PickupCharacterEditWindow), false, "PickupCharacterEditWindow");
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.Show();
        }

        void OnDestroy()
        {
            if (_serializeObject != null) _serializeObject.Dispose();
            _deleteCharacter();
        }

        void _loadCharacterPrefab()
        {
            _deleteCharacter();
            var path = string.Format(RESOURCES_PATH, _characterID);
            var characterObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            if (characterObj == null)
            {
                EditorUtility.DisplayDialog("Error", "Not Found CharacterID = " + _characterID.ToString(), "OK");
                return;
            }
            _character = PrefabUtility.InstantiatePrefab(characterObj) as GameObject;
            characterObj = null;
            if (_character.GetComponent<CharacterView>() == null)
            {
                EditorUtility.DisplayDialog("Error", "Missing Component : CharacterView", "OK");
                _deleteCharacter();
            }

            Resources.UnloadUnusedAssets();
        }

        void _drawCharacterViewData()
        {
            if (_character == null) return;
            _serializeObject = new SerializedObject(_character.GetComponent<CharacterView>());
            _serializeObject.Update();

            var scaleForUIProp = _serializeObject.FindProperty("_scaleForUI");
            scaleForUIProp.floatValue = EditorGUILayout.FloatField(scaleForUIProp.displayName, scaleForUIProp.floatValue);

            _character.transform.localScale = new Vector3(scaleForUIProp.floatValue, scaleForUIProp.floatValue, 1);

            var centerPositionForUIStoryProp = _serializeObject.FindProperty("_centerPositionForUIWithStory");
            EditorGUILayout.PropertyField(centerPositionForUIStoryProp);

            var centerPositionForUIResultProp = _serializeObject.FindProperty("_centerPositionForUIWithResult");
            EditorGUILayout.PropertyField(centerPositionForUIResultProp);

            _setPreviewPosition(_isPreviewStory ? centerPositionForUIStoryProp : centerPositionForUIResultProp);

            _character.transform.eulerAngles = new Vector3(0, _isDirectionLeft ? 0 : 180, 0);

            _serializeObject.ApplyModifiedProperties();
        }

        void _setPreviewPosition(SerializedProperty positionProp)
        {
            _character.SetActive(true);
            _character.transform.position = new Vector3(-positionProp.vector2Value.x * (_isDirectionLeft ? 1 : -1),
                                                        -positionProp.vector2Value.y,
                                                        0);
        }

        void _setSpine()
        {
            if (_character == null) return;
            _skeletonAnimation = _character.GetComponent<SkeletonAnimation>();
            var stepCnt = 1;
            bool haveStepSkin = false;
            while(_skeletonAnimation.Skeleton.Data.FindSkin(SKIN_NAME + stepCnt) != null)
            {
                haveStepSkin = true;
                stepCnt++;
            }
            _skeletonAnimation.skeleton.SetSkin(haveStepSkin ? (SKIN_NAME + (stepCnt - 1)) : "default");
            _changeSkeletonAnimation();
        }

        void _changeSkeletonAnimation()
        {
            if (_skeletonAnimation == null) return;
            _skeletonAnimation.state.SetAnimation(0, _isPreviewStory ? Constant.IDLE_ANIMATION_NAME : Constant.WIN_ANIMATION_NAME, false)
            .Time = _isPreviewStory ? 0 : 100;
            _skeletonAnimation.Update(0);
            foreach (var chilsSkeleton in _character.GetComponentsInChildren<SkeletonAnimation>())
            {
                chilsSkeleton.state.SetAnimation(0, _isPreviewStory ? Constant.IDLE_ANIMATION_NAME : Constant.WIN_ANIMATION_NAME, false)
                .Time = _isPreviewStory ? 0 : 100;
                chilsSkeleton.Update(0);
            }
        }

        void _deleteCharacter()
        {
            if (_character == null) return;
            _skeletonAnimation = null;
            DestroyImmediate(_character);
            _character = null;
        }

        void _applyPrefab()
        {
            // Prefabに保存しないのでリセット--
            _character.transform.position = Vector3.zero;
            _character.transform.rotation = Quaternion.identity;
            _character.transform.localScale = Vector3.one;
            //---

            var originalPrefab = PrefabUtility.GetPrefabParent(_character);
            if (originalPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Not Found Prefab", "OK");
                return;
            }
            PrefabUtility.ReplacePrefab(_character, originalPrefab);
        }
        
        // EditorWindow上のUI表示用
        void OnGUI()
        {
            _characterID = EditorGUILayout.IntField("CharacterID", _characterID);
            GUILayout.BeginVertical();
            if (GUILayout.Button("Load", GUILayout.Width(100)))
            {
                _loadCharacterPrefab();
                _setSpine();
            }
            
            if (GUILayout.Button("Delete", GUILayout.Width(100)))
            {
                _deleteCharacter();
            }
            _isDirectionLeft = EditorGUILayout.Toggle("Direction Left", _isDirectionLeft);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Preview Change", GUILayout.Width(100)))
            {
                _isPreviewStory = !_isPreviewStory;
                _changeSkeletonAnimation();
                _character.SetActive(false);
            }
            GUILayout.Label(_isPreviewStory ? "Story" : "Result");
            GUILayout.EndHorizontal();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            _drawCharacterViewData();
            GUILayout.EndScrollView();
            
            if (GUILayout.Button("Apply", GUILayout.Width(100)))
            {
                _applyPrefab();
            }
            
            var style = new GUIStyle();
            style.fontSize = 25;
            style.fontStyle = FontStyle.Bold;
            GUILayout.Label("Apply後はシーンのセーブを忘れずに！", style);

            GUILayout.EndVertical();
        }
        //_transform.localPosition = new Vector3(-centerPositionForUI.x , -centerPositionForUI.y, _transform.localPosition.z);
    }
}