using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniLinq;
using NoroshiDebug.CharacterCapture.Editor.Character;
using NoroshiDebug.CharacterCapture.Editor.CreateTexture.Thumbnail;
using NoroshiDebug.CharacterCapture.Editor.CreateTexture.SkillButton;
using NoroshiDebug.CharacterCapture.Editor.FileIO;
using NoroshiDebug.CharacterCapture.Gizmo;

namespace NoroshiDebug.CharacterCapture.Editor.Capture
{
    public class CaptureCharacter
    {
        // UnityEditor上から動作させる(本番用)
        [MenuItem("Capture/Character/Start")]
        static void _captureStart()
        {
            _captureCharacter(false, false);
        }
        // UnityEditor上から動作させる(テスト用)
        [MenuItem("Capture/Character/TestStart")]
        static void _captureTestStart()
        {
            _captureCharacter(true, false);
        }
        // コマンドライン上から起動する(本番用)
        static void _captureBootCommandLineStart()
        {
            _captureCharacter(false, true);
        }
        // コマンドライン上から起動する(テスト用)
        static void _captureBootCommandLineTestStart()
        {
            _captureCharacter(true, true);
        }

        static void _captureCharacter(bool isTest, bool isCommandLine)
        {
            var disposables = new CompositeDisposable();
            var systemData = _getSystemData();
            var characterSaveDataList = _getCharacterDataList();
            var characterSetting = new CaptureCharacterSetting();
            var createThumbnail = new CreateThumbnailTexture();
            var createSkillButton = new CreateSkillButtonTexture();
            
            createThumbnail.SetTexWidth(systemData.ThumbnailTexWidth);
            createThumbnail.SetTexHeight(systemData.ThumbnailTexHeight);
            createThumbnail.SetRect(new Rect(0, 0, systemData.ThumbnailTexWidth, systemData.ThumbnailTexHeight));

            createSkillButton.SetTexWidth(systemData.SkillButtonTexWidth);
            createSkillButton.SetTexHeight(systemData.SkillButtonTexHeight);
            createSkillButton.SetSliceBottomValue(systemData.SliceBottomValue);
            createSkillButton.SetRect(new Rect(0, 0, systemData.SkillButtonTexWidth, systemData.SkillButtonTexHeight));

            characterSetting.SetLoadPrefabPath(systemData.LoadPrefabPath);
            characterSetting.SetLoadObjectName(systemData.LoadObjectName);

            if (!createThumbnail.HasCreateTextureObservers())
            {
                createThumbnail.GetOnCreateTextureObservable()
                    .Subscribe(skinID => characterSetting.SetSkin(skinID)).AddTo(disposables);
            }

            if (!createSkillButton.HasCreateTextureObservers())
            {
                createSkillButton.GetOnCreateTextureObservable()
                    .Subscribe(skinID => characterSetting.SetSkin(skinID)).AddTo(disposables);
            }

            var characterSaveDataObservabla = new List<IObservable<Unit>>();
            for(int i = 0; i < characterSaveDataList.Count; i++)
            {
                characterSaveDataObservabla.Add(Observable.FromCoroutine(() => _waitThumbnailPreview(characterSetting, characterSaveDataList))
                    .SelectMany(Observable.FromCoroutine(() => createThumbnail.CaptureTexture(characterSetting.SkinCount,
                                                                                          characterSetting.CharacterID,
                                                                                          isTest)))
                    .SelectMany(Observable.FromCoroutine(() => _waitSkillButtonPreview(characterSetting)))
                    .SelectMany(Observable.FromCoroutine(() => createSkillButton.CaptureTexture(characterSetting.SkinCount,
                                                                                          characterSetting.CharacterID,
                                                                                          isTest))));
            }
            characterSaveDataObservabla.Concat().Subscribe(_ => 
            {
                characterSetting.Destroy();
                createThumbnail.Destroy();
                createSkillButton.Destroy();
            },
            () => 
            {
                createThumbnail.Dispose();
                createSkillButton.Dispose();
                Debug.Log("Finish");
                // コマンドライン上から起動した場合はここでUnityを終了させる
                // コマンド上で-quitをすると非同期部分がスキップされるため
                if (isCommandLine) EditorApplication.Exit(0);
            });
        }
        
        static IEnumerator _waitThumbnailPreview(CaptureCharacterSetting characterSetting, List<FileAccess.CharacterSaveData> characterSaveDataList)
        {
            // ループ内で引数が変更されているが実際にここに来るとリストの最後のデータしか入ってこないので、ここでリストから取り出すようにする
            var characterSaveData = characterSaveDataList.FirstOrDefault(characterSave => characterSave != null);
            characterSetting.SetCharacterID(characterSaveData.CharacterID);
            characterSetting.SetThumbnailPosition(characterSaveData.ThumbnailPosition);
            characterSetting.SetThumbnailScale(characterSaveData.ThumbnailScale);
            characterSetting.SetSkillButtonPosition(characterSaveData.SkillButtonPosition);
            characterSetting.SetSkillButtonScale(characterSaveData.SkillButtonScale);

            characterSaveDataList.RemoveAt(0);
            
            characterSetting.LoadCharacter();
            
            characterSetting.SetSkin(0);
            yield return new WaitForSeconds(0.1f);
            characterSetting.PreviewThumbnailDataSet();
            yield return new WaitForSeconds(0.1f);
        }
        
        static IEnumerator _waitSkillButtonPreview(CaptureCharacterSetting characterSetting)
        {
            characterSetting.SetSkin(0);
            yield return new WaitForSeconds(0.1f);
            characterSetting.PreviewSkillButtonDataSet();
            yield return new WaitForSeconds(0.1f);
        }

        static FileAccess.SystemSaveData _getSystemData()
        {
            return FileAccess.LoadSystemData();
        }

        static List<FileAccess.CharacterSaveData> _getCharacterDataList()
        {
            return new List<FileAccess.CharacterSaveData>(FileAccess.LoadAllCharacterData());
        }
    }
}