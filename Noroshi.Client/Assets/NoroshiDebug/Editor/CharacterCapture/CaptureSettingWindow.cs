using UnityEngine;
using UnityEditor;
using System.Collections;
using UniRx;
using NoroshiDebug.CharacterCapture.Gizmo;
using NoroshiDebug.CharacterCapture.Editor.Character;
using NoroshiDebug.CharacterCapture.Editor.CreateTexture.Thumbnail;
using NoroshiDebug.CharacterCapture.Editor.CreateTexture.SkillButton;
using NoroshiDebug.CharacterCapture.Editor.FileIO;
using System.Collections.Generic;

namespace NoroshiDebug.CharacterCapture.Editor.Setting
{
    public class CaptureSettingWindow : EditorWindow
    {
        DrawCaptureGizmo _drawCaptureGizmo = null;
        CaptureCharacterSetting _captureCharacterSetting = new CaptureCharacterSetting();
        CreateThumbnailTexture _createThumbnailTexture = new CreateThumbnailTexture();
        CreateSkillButtonTexture _createSkillButtonTexture = new CreateSkillButtonTexture();
        FileAccess.CharacterSaveData _characterSaveData = null;

        Vector2 horizontalScrollPosition = Vector2.zero;
        CompositeDisposable _disposable = new CompositeDisposable();

        bool isCreate = false;
        
        [MenuItem("Capture/Setting")]
        static void Init()
        {
            var editorWindow = GetWindow(typeof(CaptureSettingWindow), false, "CaptureSetting");
            editorWindow.autoRepaintOnSceneChange = true;
            editorWindow.Show();
        }

        void _getDrawCaptureGizmo()
        {
            if (_drawCaptureGizmo != null) return;
            foreach(var obj in Resources.FindObjectsOfTypeAll(typeof(DrawCaptureGizmo)))
            {
                var path = AssetDatabase.GetAssetOrScenePath(obj);
                if (path.Contains(".unity"))
                {
                    _drawCaptureGizmo = obj as DrawCaptureGizmo;
                    _drawCaptureGizmo.Initialize();
                    break;
                }
            }
        }

        void _initCaptureThumbnail()
        {
            if (!_createThumbnailTexture.HasTexWidthObservers())
            {
                _createThumbnailTexture.GetOnChangeTexWidthObservable()
                .Subscribe(texWidth => 
                {
                    _drawCaptureGizmo.SetThumbnailGizmoWidth(texWidth);
                    SceneView.RepaintAll();
                })
                .AddTo(_disposable);
            }
            if (!_createThumbnailTexture.HasTexHeightObservers())
            {
                _createThumbnailTexture.GetOnChangeTexHeightObservable()
                .Subscribe(texHeight => 
                {
                    _drawCaptureGizmo.SetThumbnailGizmoHeight(texHeight);
                    SceneView.RepaintAll();
                })
                .AddTo(_disposable);
            }
            if (!_createThumbnailTexture.HasCreateTextureObservers())
            {
                _createThumbnailTexture.GetOnCreateTextureObservable()
                .Subscribe(skinID => _captureCharacterSetting.SetSkin(skinID))
                .AddTo(_disposable);
            }
        }

        void _initCaptureSkillButton()
        {
            if (!_createSkillButtonTexture.HasTexWidthObservers())
            {
                _createSkillButtonTexture.GetOnChangeTexWidthObservable()
                .Subscribe(texWidth => 
                {
                    _drawCaptureGizmo.SetSkillButtonGizmoWidth(texWidth);
                    SceneView.RepaintAll();
                })
                .AddTo(_disposable);
            }
            if (!_createSkillButtonTexture.HasTexHeightObservers())
            {
                _createSkillButtonTexture.GetOnChangeTexHeightObservable()
                .Subscribe(texHeight => 
                {
                    _drawCaptureGizmo.SetSkillButtonGizmoHeight(texHeight);
                    SceneView.RepaintAll();
                })
                .AddTo(_disposable);
            }
            if (!_createSkillButtonTexture.HasCreateTextureObservers())
            {
                _createSkillButtonTexture.GetOnCreateTextureObservable()
                .Subscribe(skinID => _captureCharacterSetting.SetSkin(skinID))
                .AddTo(_disposable);
            }
        }

        void _initCharacterSetting()
        {
            _initCaptureThumbnail();
            _initCaptureSkillButton();

            // キャラクターセーブデータの中身を初期化
            FileAccess.SetThumbnailData(_captureCharacterSetting.ThumbnailPosition,
                                        _captureCharacterSetting.ThumbnailScale);
            FileAccess.SetSkillButtonData(_captureCharacterSetting.SkillButtonPosition,
                                          _captureCharacterSetting.SkillButtonScale);
        }

        void Awake()
        {
            _getDrawCaptureGizmo();
            _initCharacterSetting();
        }
    	
    	// Update is called once per frame
    	void Update()
        {
            _createThumbnailTexture.SetRect(position);
            _createSkillButtonTexture.SetRect(position);
            _getDrawCaptureGizmo();
    	}

        void OnDestroy()
        {
            _createThumbnailTexture.Destroy();
            _createSkillButtonTexture.Destroy();
            _captureCharacterSetting.Destroy();
            _disposable.Dispose();
        }

        // EditorWindowにフォーカスが当たった際に呼ばれる
        void OnFocus()
        {
            // ここの時にデータが空になっている時があるので、空の際は再度作成する
            _getDrawCaptureGizmo();
            _initCharacterSetting();
        }

        IEnumerator _waitThumbnailPreview()
        {
            _captureCharacterSetting.SetSkin(0);
            yield return new WaitForSeconds(0.1f);
            _captureCharacterSetting.PreviewThumbnailDataSet();
            yield return new WaitForSeconds(0.1f);
        }

        IEnumerator _waitSkillButtonPreview()
        {
            _captureCharacterSetting.SetSkin(0);
            yield return new WaitForSeconds(0.1f);
            _captureCharacterSetting.PreviewSkillButtonDataSet();
            yield return new WaitForSeconds(0.1f);
        }

        // EditorWindow上のUI表示用
        void OnGUI()
        {
            horizontalScrollPosition = GUILayout.BeginScrollView(horizontalScrollPosition, GUIStyle.none);
            _captureCharacterSetting.TextureSizeSetting();

            GUILayout.Label("Thumbnail------------------");
            _captureCharacterSetting.DrawThumbnailData();
            _createThumbnailTexture.DrawTextSize();
            if (!isCreate)
            {
                if (GUILayout.Button("preview", GUILayout.Width(150)))
                {
                    isCreate = true;
                    _captureCharacterSetting.PreviewDataSet();
                    Observable.FromCoroutine(_waitThumbnailPreview)
                    .SelectMany(Observable.FromCoroutine(() => _createThumbnailTexture.Preview(_captureCharacterSetting.SkinCount)))
                    .Subscribe(
                        _ => {}, 
                        () => 
                        {
                            isCreate = false;
                            _captureCharacterSetting.SetSkin(0);
                            _captureCharacterSetting.PreviewDataSet();
                        }
                    )
                    .AddTo(_disposable);
                }
            }
            _createThumbnailTexture.DrawTexture();
            GUILayout.Label("---------------------------");

            EditorGUILayout.Space();

            GUILayout.Label("Skillbutton------------------");
            _captureCharacterSetting.DrawSkillButtonData();
            _createSkillButtonTexture.DrawTextSize();
            if (!isCreate)
            {
                if (GUILayout.Button("preview", GUILayout.Width(150)) && !isCreate)
                {
                    isCreate = true;
                    _captureCharacterSetting.PreviewDataSet();
                    Observable.FromCoroutine(_waitSkillButtonPreview)
                    .SelectMany(Observable.FromCoroutine(() => _createSkillButtonTexture.Preview(_captureCharacterSetting.SkinCount)))
                    .Subscribe(
                        _ => {}, 
                        () => 
                        {
                            isCreate = false;
                            _captureCharacterSetting.SetSkin(0);
                            _captureCharacterSetting.PreviewDataSet();
                        }
                    )
                    .AddTo(_disposable);
                }
            }
            _createSkillButtonTexture.DrawTexture();
            GUILayout.Label("---------------------------");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("SaveSystemData", GUILayout.Width(150)))
            {
                FileAccess.SaveSystemData(
                    _createThumbnailTexture.TexWidth, _createThumbnailTexture.TexHeight,
                    _createSkillButtonTexture.TexWidth, _createSkillButtonTexture.TexHeight,
                    _createSkillButtonTexture.SliceBottomValue,
                    _captureCharacterSetting.LoadPrefabPath,
                    _captureCharacterSetting.LoadObjectName);
            }

            if (GUILayout.Button("LoadSystemData", GUILayout.Width(150)))
            {
                var systemData = FileAccess.LoadSystemData();
                if (systemData != null)
                {
                    _createThumbnailTexture.SetTexWidth(systemData.ThumbnailTexWidth);
                    _createThumbnailTexture.SetTexHeight(systemData.ThumbnailTexHeight);
                    _createSkillButtonTexture.SetTexWidth(systemData.SkillButtonTexWidth);
                    _createSkillButtonTexture.SetTexHeight(systemData.SkillButtonTexHeight);
                    _createSkillButtonTexture.SetSliceBottomValue(systemData.SliceBottomValue);
                    _captureCharacterSetting.SetLoadPrefabPath(systemData.LoadPrefabPath);
                    _captureCharacterSetting.SetLoadObjectName(systemData.LoadObjectName);
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("SaveCharacterData", GUILayout.Width(150)))
            {
                // セーブ直前の値を保存対象とする
                FileAccess.SetThumbnailData(_captureCharacterSetting.ThumbnailPosition,
                                            _captureCharacterSetting.ThumbnailScale);
                FileAccess.SetSkillButtonData(_captureCharacterSetting.SkillButtonPosition,
                                              _captureCharacterSetting.SkillButtonScale);

                FileAccess.SaveCharacterData(_captureCharacterSetting.CharacterID);
            }

            if (GUILayout.Button("LoadCharacterData", GUILayout.Width(150)))
            {
                _characterSaveData = FileAccess.LoadCharacterData(_captureCharacterSetting.CharacterID);
                if (_characterSaveData != null)
                {
                    // 読み込んだ際はサムネイル用の座標とサイズを渡す
                    _captureCharacterSetting.SetThumbnailPosition(_characterSaveData.ThumbnailPosition);
                    _captureCharacterSetting.SetThumbnailScale(_characterSaveData.ThumbnailScale);
                    _captureCharacterSetting.SetSkillButtonPosition(_characterSaveData.SkillButtonPosition);
                    _captureCharacterSetting.SetSkillButtonScale(_characterSaveData.SkillButtonScale);
                    _captureCharacterSetting.PreviewDataSet();
                }
            }
            GUILayout.EndHorizontal();

            if (!isCreate)
            {
                if (GUILayout.Button("Capture Now Character", GUILayout.Width(200)))
                {
                    isCreate = true;
                    var characterSaveDataObservabla = new List<IObservable<Unit>>();

                    characterSaveDataObservabla.Add(Observable.FromCoroutine(_waitThumbnailPreview)
                    .SelectMany(Observable.FromCoroutine(() => _createThumbnailTexture.CaptureTexture(_captureCharacterSetting.SkinCount, _captureCharacterSetting.CharacterID, false))));
                    characterSaveDataObservabla.Add(Observable.FromCoroutine(_waitSkillButtonPreview)
                    .SelectMany(Observable.FromCoroutine(() => _createSkillButtonTexture.CaptureTexture(_captureCharacterSetting.SkinCount, _captureCharacterSetting.CharacterID, false))));
                    characterSaveDataObservabla.Concat().Subscribe(_ => 
                    {
                        isCreate = false;
                        _captureCharacterSetting.SetSkin(0);
                        _captureCharacterSetting.PreviewDataSet();
                    })
                    .AddTo(_disposable);
                }
            }

            GUILayout.EndScrollView();
        }
    }
}