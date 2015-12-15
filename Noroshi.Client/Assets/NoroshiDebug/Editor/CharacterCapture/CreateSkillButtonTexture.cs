using UnityEngine;
using UnityEditor;
using System.Collections;

namespace NoroshiDebug.CharacterCapture.Editor.CreateTexture.SkillButton
{
    public class CreateSkillButtonTexture : CreateTexture
    {
        int _sliceBottomValue = 30;
        public int SliceBottomValue { get { return _sliceBottomValue; } }

        public CreateSkillButtonTexture() : base()
        {
            _texWidth = 101;
            _texHeight = 88;
        }

        public void SetSliceBottomValue(int sliceBottomValue)
        {
            _sliceBottomValue = sliceBottomValue;
        }
        
        public override IEnumerator CaptureTexture (int texCount, int characterID, bool isTest)
        {
            if(_texWidth == 0 || _texHeight == 0 || texCount == 0) yield break;

            var savePath = isTest ? 
                           Application.dataPath + "/NoroshiDebug/CaptureData/TestCapture" : 
                           Application.dataPath + "/Noroshi/Resources/Character/" + characterID.ToString() + "/Battle";
            
            for(var i = 0; i < texCount; i++)
            {
                _onCreateTextureSubject.OnNext(i);
                // ここにコルーチンを挟んでちゃんとカメラにSkinを切り替えた状態を表示させる
                yield return new WaitForSeconds(0.3f);
                var fileName = isTest ? "skillbutton_" + characterID.ToString() + "_" + i.ToString() : "skillbutton_" + (i + 1).ToString();
                _saveCaptureTexture(_texWidth, _texHeight, savePath, fileName);
            }
        }

        public override void DrawTextSize()
        {
            base.DrawTextSize ();
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            GUILayout.Label("SliceBottomValue");
            _sliceBottomValue = EditorGUILayout.IntField(_sliceBottomValue, GUILayout.Width(100));
            GUILayout.EndHorizontal();
        }

        public override void CopyTexture(Texture2D tex2D, int texWidth, int texHeight)
        {
            var start = new Vector2(texWidth, 0);
            var end = new Vector2(texWidth - _sliceBottomValue, texHeight);
            for(var y = 0; y < texHeight; y++)
            {
                for(var x = 0; x < texWidth; x++)
                {
                    var lerp = Vector2.Lerp(end, start, (float)y / (float)texHeight);
                    var pixelColor = tex2D.GetPixel(x, y);
                    if (lerp.x > x)
                    {
                        pixelColor = new Color(pixelColor.r, pixelColor.g, pixelColor.b, pixelColor.a);
                    }
                    else
                    {
                        pixelColor = new Color(1, 1, 1, 0);
                    }
                    tex2D.SetPixel(x, y, pixelColor);
                }
            }
        }
    }
}
