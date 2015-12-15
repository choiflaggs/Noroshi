using UnityEngine;
using UnityEditor;
using UniRx;
using System.Collections.Generic;

namespace NoroshiDebug.CharacterCapture.Editor.Character
{
    public class CaptureCharacterSetting
    {
        const string  OBUJECT_EXTENSION = ".prefab";
        const string SKIN_NAME = "step";
        // 読み込み時のパスにも利用する
        int _characterID = 1001;
        Vector3 _thumbnailPosition = Vector3.zero;
        Vector3 _thumbnailScale = Vector3.one;
        Vector3 _skillButtonPostion = Vector3.zero;
        Vector3 _skillButtonScale = Vector3.one;
        // 
        string _loadPrefabPath = "Assets/Noroshi/Resources/UICharacter/";
        string _loadObjectName = "Character";

        GameObject _character = null;
        SkeletonAnimation _skeletonAnimation = null;
        List<string> _skinNameList = new List<string>();

        bool _isFocusThumbnail = true;

        public int CharacterID { get { return _characterID; } }
        public Vector3 ThumbnailPosition { get { return _thumbnailPosition; } }
        public Vector3 ThumbnailScale { get { return _thumbnailScale; } }
        public Vector3 SkillButtonPosition { get { return _skillButtonPostion; } }
        public Vector3 SkillButtonScale { get { return _skillButtonScale; } }
        public int SkinCount { get { return _skinNameList.Count; } }
        public string LoadPrefabPath { get { return _loadPrefabPath; } }
        public string LoadObjectName { get { return _loadObjectName; } }

        public void SetSkin(int skinID)
        {
            _skeletonAnimation.Skeleton.SetSkin(_skinNameList[skinID]);
        }

        public void SetThumbnailPosition(Vector3 position)
        {
            _thumbnailPosition = position;
            if (_isFocusThumbnail && _character != null) _character.transform.position = _thumbnailPosition;
        }

        public void SetThumbnailScale(Vector3 scale)
        {
            _thumbnailScale = scale;
            if (_isFocusThumbnail && _character != null) _character.transform.localScale = _thumbnailScale;
        }

        public void SetSkillButtonPosition(Vector3 position)
        {
            _skillButtonPostion = position;
            if (!_isFocusThumbnail && _character != null) _character.transform.position = _skillButtonPostion;
        }

        public void SetSkillButtonScale(Vector3 scale)
        {
            _skillButtonScale = scale;
            if (!_isFocusThumbnail && _character != null) _character.transform.localScale = _skillButtonScale;
        }

        public void SetLoadPrefabPath(string loadPrefabPath)
        {
            _loadPrefabPath = loadPrefabPath;
        }

        public void SetLoadObjectName(string loadObjectName)
        {
            _loadObjectName = loadObjectName;
        }

        public void SetCharacterID(int characterID)
        {
            _characterID = characterID;
        }

        public void PreviewThumbnailDataSet()
        {
            if (_character != null)
            {
                _isFocusThumbnail = true;
                _character.transform.position = _thumbnailPosition;
                _character.transform.localScale = _thumbnailScale;
            }
        }

        public void PreviewSkillButtonDataSet()
        {
            if (_character != null)
            {
                _isFocusThumbnail = false;
                _character.transform.position = _skillButtonPostion;
                _character.transform.localScale = _skillButtonScale;
            }
        }

        public void PreviewDataSet()
        {
            if (_isFocusThumbnail)
            {
                PreviewThumbnailDataSet();
            }
            else
            {
                PreviewSkillButtonDataSet();
            }
        }

        public void TextureSizeSetting()
        {
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            GUILayout.Label("CharacterID");
            _characterID = EditorGUILayout.IntField(_characterID, GUILayout.Width(100));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            GUILayout.Label("Load Directory");
            _loadPrefabPath = EditorGUILayout.TextField(_loadPrefabPath, GUILayout.Width(150));
            if (GUILayout.Button("Select", GUILayout.Width(100)))
            {
                var path = EditorUtility.OpenFolderPanel("LoadCharacter", Application.dataPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    int index = path.IndexOf("Assets");
                    path = path.Remove(0, index);
                    _loadPrefabPath = path + "/";
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            GUILayout.Label("Load PrefabName");
            _loadObjectName = EditorGUILayout.TextField(_loadObjectName, GUILayout.Width(150));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            GUILayout.Label("Character");
            _character = EditorGUILayout.ObjectField(_character, typeof(GameObject), true) as GameObject;
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(GUILayout.Width(300));
            GUILayout.Label("SkeletonAnimation");
            _skeletonAnimation = EditorGUILayout.ObjectField(_skeletonAnimation, typeof(SkeletonAnimation), true) as SkeletonAnimation;
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            if (GUILayout.Button("LoadCharacter", GUILayout.Width(150)))
            {
                _loadCharacter();
            }

            if (GUILayout.Button("RemoveCharacter", GUILayout.Width(150)))
            {
                Destroy();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            var focusThumbnail = EditorGUILayout.ToggleLeft("Focus Thumbnail", _isFocusThumbnail);
            if (_isFocusThumbnail != focusThumbnail)
            {
                _isFocusThumbnail = focusThumbnail;
                PreviewDataSet();
            }
            GUILayout.EndHorizontal();
        }

        public void DrawThumbnailData()
        {
            EditorGUI.BeginDisabledGroup(!_isFocusThumbnail);
            var position = EditorGUILayout.Vector3Field("ThumbnailPosition", (_character != null && _isFocusThumbnail) ? _character.transform.position : _thumbnailPosition);
            if (!_thumbnailPosition.Equals(position) && _isFocusThumbnail)
            {
                _thumbnailPosition = position;
                if (_character != null) _character.transform.position = _thumbnailPosition;
            }
            
            var scale = EditorGUILayout.Vector3Field("ThumbnailScale", (_character != null && _isFocusThumbnail) ? _character.transform.localScale : _thumbnailScale);
            if (!_thumbnailScale.Equals(scale) && _isFocusThumbnail)
            {
                _thumbnailScale = scale;
                if (_character != null) _character.transform.localScale = _thumbnailScale;
            }
            EditorGUI.EndDisabledGroup();
        }

        public void DrawSkillButtonData()
        {
            EditorGUI.BeginDisabledGroup(_isFocusThumbnail);
            var position = EditorGUILayout.Vector3Field("SkillButtonPosition", (_character != null && !_isFocusThumbnail) ? _character.transform.position : _skillButtonPostion);
            if (!_skillButtonPostion.Equals(position) && !_isFocusThumbnail)
            {
                _skillButtonPostion = position;
                if (_character != null) _character.transform.position = _skillButtonPostion;
            }
            
            var scale = EditorGUILayout.Vector3Field("SkillButtonScale", (_character != null && !_isFocusThumbnail) ? _character.transform.localScale : _skillButtonScale);
            if (!_skillButtonScale.Equals(scale) && !_isFocusThumbnail)
            {
                _skillButtonScale = scale;
                if (_character != null) _character.transform.localScale = _skillButtonScale;
            }
            EditorGUI.EndDisabledGroup();
        }

        public void LoadCharacter()
        {
            _loadCharacter();
        }

        void _loadCharacter()
        {
            if (_character != null) return;
            string path = _loadPrefabPath + _characterID.ToString() + "/" + _loadObjectName + OBUJECT_EXTENSION;
            var characterObj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (_isFocusThumbnail)
            {
                _character = GameObject.Instantiate(characterObj, _thumbnailPosition, Quaternion.identity) as GameObject;
                _character.transform.localScale = _thumbnailScale;
            }
            else
            {
                _character = GameObject.Instantiate(characterObj, _skillButtonPostion, Quaternion.identity) as GameObject;
                _character.transform.localScale = _skillButtonScale;
            }
            _getSkinNames();
            characterObj = null;
            Resources.UnloadUnusedAssets();
        }

        void _getSkinNames()
        {
            _skeletonAnimation = _character.GetComponent<SkeletonAnimation>();
            var stepCnt = 1;
            _skinNameList.Clear();
            while(_skeletonAnimation.Skeleton.Data.FindSkin(SKIN_NAME + stepCnt) != null)
            {
                _skinNameList.Add(SKIN_NAME + stepCnt);
                stepCnt++;
            }
        }

        public void Destroy()
        {
            if (_character == null) return;
            if (_isFocusThumbnail)
            {
                _thumbnailPosition = _character.transform.position;
                _thumbnailScale = _character.transform.localScale;
            }
            else
            {
                _skillButtonPostion = _character.transform.position;
                _skillButtonScale = _character.transform.localScale;
            }
            GameObject.DestroyImmediate(_character);
            _character = null;
        }
    }
}