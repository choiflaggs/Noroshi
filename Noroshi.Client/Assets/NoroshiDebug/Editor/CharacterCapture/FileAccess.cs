using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UniLinq;

namespace NoroshiDebug.CharacterCapture.Editor.FileIO
{
    public class FileAccess
    {
        const string SYSTEM_DATA_PATH = "/NoroshiDebug/CaptureData/SystemData/SystemData.txt";
        const string CHARACTER_SAVE_DATA_PATH = "/NoroshiDebug/CaptureData/CharacterData/";

        public class SystemSaveData
        {
            public int ThumbnailTexWidth { get; set; }
            public int ThumbnailTexHeight { get; set; }
            public int SkillButtonTexWidth { get; set; }
            public int SkillButtonTexHeight { get; set; }
            public int SliceBottomValue { get; set; }
            public string LoadPrefabPath { get; set; }
            public string LoadObjectName { get; set; }

            public override string ToString ()
            {
                return string.Format ("{0},{1},{2},{3},{4},{5},{6}", 
                                      ThumbnailTexWidth, ThumbnailTexHeight, 
                                      SkillButtonTexWidth, SkillButtonTexHeight, 
                                      SliceBottomValue,
                                      LoadPrefabPath,
                                      LoadObjectName);
            }
        }

        public class CharacterSaveData
        {
            public int CharacterID { get; set; }
            public Vector3 ThumbnailPosition { get; set; }
            public Vector3 ThumbnailScale { get; set; }
            public Vector3 SkillButtonPosition { get; set; }
            public Vector3 SkillButtonScale { get; set; }

            public override string ToString ()
            {
                return string.Format ("{0}\n{1},{2},{3}\n{4},{5},{6}\n{7},{8},{9}\n{10},{11},{12}",
                                      CharacterID, 
                                      ThumbnailPosition.x, ThumbnailPosition.y, ThumbnailPosition.z, 
                                      ThumbnailScale.x, ThumbnailScale.y, ThumbnailScale.z,
                                      SkillButtonPosition.x, SkillButtonPosition.y, SkillButtonPosition.z,
                                      SkillButtonScale.x, SkillButtonScale.y, SkillButtonScale.z);
            }
        }

        static CharacterSaveData _characterSaveData = new CharacterSaveData();

        public static void SetThumbnailPosition(Vector3 position)
        {
            _characterSaveData.ThumbnailPosition = position;
        }

        public static void SetThumbnailScale(Vector3 scale)
        {
            _characterSaveData.ThumbnailScale = scale;
        }

        public static void SetThumbnailData(Vector3 position, Vector3 scale)
        {
            SetThumbnailPosition(position);
            SetThumbnailScale(scale);
        }

        public static void SetSkillButtonPosition(Vector3 position)
        {
            _characterSaveData.SkillButtonPosition = position;
        }

        public static void SetSkillButtonScale(Vector3 scale)
        {
            _characterSaveData.SkillButtonScale = scale;
        }

        public static void SetSkillButtonData(Vector3 position, Vector3 scale)
        {
            SetSkillButtonPosition(position);
            SetSkillButtonScale(scale);
        }

        static Vector3 _convertStringToVector3(string data)
        {
            var dataList = data.Split(',');
            return new Vector3(float.Parse(dataList[0]), float.Parse(dataList[1]), float.Parse(dataList[2]));
        }

        static bool _isDirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        static bool _isFileExists(string path)
        {
            return File.Exists(path);
        }

        static bool _displayDialog(string title, string message, int buttonCount)
        {
            switch (buttonCount)
            {
            case 1:
                return EditorUtility.DisplayDialog(title, message, "OK");
            case 2:
                return EditorUtility.DisplayDialog(title, message, "OK", "Cancel");
            default:
                throw new System.ArgumentException();
            }
        }

        static string _getSystemDataPath()
        {
            return Application.dataPath + SYSTEM_DATA_PATH;
        }

        static string _getCharacterSaveData(int characterID, bool isSearchDirectory)
        {
            return isSearchDirectory ? Application.dataPath + CHARACTER_SAVE_DATA_PATH :
                Application.dataPath + CHARACTER_SAVE_DATA_PATH + characterID.ToString() + ".txt";
        }

        public static CharacterSaveData[] LoadAllCharacterData()
        {
            var loadPath = _getCharacterSaveData(0, true);
            if (!_isDirectoryExists(loadPath))
            {
                FileAccess._displayDialog("Capture", loadPath + "\n対象のディレクトリが見つかりません。", 1);
                return null;
            }

            var di = new DirectoryInfo(loadPath);
            var allFile = di.GetFiles("*.txt").Select(file => int.Parse(Path.GetFileNameWithoutExtension(file.Name)));

            return allFile.Select(characterID => 
            {
                try
                {
                    var characterSaveData = FileAccess.LoadCharacterData(characterID);
                    return characterSaveData;
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
                return null;
            })
            .OrderBy(characterSaveData => characterSaveData.CharacterID).ToArray();
        }

        public static void SaveCharacterData(int characterID)
        {
            var savePath = _getCharacterSaveData(characterID, false);
            var isOk = true;
            if (_isFileExists(savePath))
            {
                isOk = _displayDialog("SaveCharacterData", savePath + "\n対象のファイルがすでに存在しています。 \n上書きしますか？", 2);
            }
            if (!string.IsNullOrEmpty(savePath) && isOk)
            {
                _characterSaveData.CharacterID = characterID;

                try
                {
                    File.WriteAllText(savePath, _characterSaveData.ToString(), System.Text.Encoding.Unicode);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static CharacterSaveData LoadCharacterData(int characterID = 0)
        {
            var loadPath = _getCharacterSaveData(characterID, false);
            if (!_isFileExists(loadPath))
            {
                _displayDialog("LoadCharacterData", loadPath + "\n対象のファイルが見つかりませんでした。", 1);
                return null;
            }

            if (!string.IsNullOrEmpty(loadPath))
            {
                try
                {
                    var dataList = File.ReadAllText(loadPath).Split('\n');

                    var characterSaveData = new CharacterSaveData()
                    {
                        CharacterID = int.Parse(dataList[0]),
                        ThumbnailPosition = _convertStringToVector3(dataList[1]),
                        ThumbnailScale = _convertStringToVector3(dataList[2]),
                        SkillButtonPosition = _convertStringToVector3(dataList[3]),
                        SkillButtonScale = _convertStringToVector3(dataList[4]),
                    };
                    return characterSaveData;
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return null;
        }

        public static void SaveSystemData(int thumbnailTexWidth, int thumbnailTexHeight,
                                   int skillButtonTexWidth, int skillbuttonTexHeight,
                                   int sliceBottomValue,
                                   string loadPrefabPath, string loadObjectName)
        {
            var savePath = _getSystemDataPath();

            var isOk = true;
            if (_isFileExists(savePath))
            {
                isOk = _displayDialog("SaveSystemData", savePath + "\n対象のファイルがすでに存在しています。 \n上書きしますか？", 2);
            }

            if (!string.IsNullOrEmpty(savePath) && isOk)
            {
                var systemSaveData = new SystemSaveData()
                {
                    ThumbnailTexWidth = thumbnailTexWidth,
                    ThumbnailTexHeight = thumbnailTexHeight,
                    SkillButtonTexWidth = skillButtonTexWidth,
                    SkillButtonTexHeight = skillbuttonTexHeight,
                    SliceBottomValue = sliceBottomValue,
                    LoadPrefabPath = loadPrefabPath,
                    LoadObjectName = loadObjectName,
                };
                
                try
                {
                    File.WriteAllText(savePath, systemSaveData.ToString(), System.Text.Encoding.Unicode);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        
        public static SystemSaveData LoadSystemData()
        {
            var loadPath = _getSystemDataPath();
            if (!_isFileExists(loadPath))
            {
                _displayDialog("LoadSystemData", loadPath + "\n対象のファイルが見つかりませんでした。", 1);
                return null;
            }

            if (!string.IsNullOrEmpty(loadPath))
            {
                try
                {
                    var data = File.ReadAllText(loadPath, System.Text.Encoding.Unicode);
                    var dataList = data.Split(',');
                    var systemSaveData = new SystemSaveData()
                    {
                        ThumbnailTexWidth = int.Parse(dataList[0]),
                        ThumbnailTexHeight = int.Parse(dataList[1]),
                        SkillButtonTexWidth = int.Parse(dataList[2]),
                        SkillButtonTexHeight = int.Parse(dataList[3]),
                        SliceBottomValue = int.Parse(dataList[4]),
                        LoadPrefabPath = dataList[5],
                        LoadObjectName = dataList[6],
                    };
                    return systemSaveData;
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return null;
        }
    }
}