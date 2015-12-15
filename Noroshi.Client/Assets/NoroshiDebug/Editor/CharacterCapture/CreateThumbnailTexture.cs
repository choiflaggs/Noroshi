using UnityEngine;
using System.Collections;

namespace NoroshiDebug.CharacterCapture.Editor.CreateTexture.Thumbnail
{
    public class CreateThumbnailTexture : CreateTexture
    {
        public CreateThumbnailTexture() : base()
        {
            _texWidth = 116;
            _texHeight = 116;
        }

        public override IEnumerator CaptureTexture (int texCount, int characterID, bool isTest)
        {
            if(_texWidth == 0 || _texHeight == 0 || texCount == 0) yield break;

            var savePath = isTest ? 
                            Application.dataPath + "/NoroshiDebug/CaptureData/TestCapture" : 
                            Application.dataPath + "/Noroshi/Resources/Character/" + characterID.ToString();
            
            for(var i = 0; i < texCount; i++)
            {
                _onCreateTextureSubject.OnNext(i);
                // ここにコルーチンを挟んでちゃんとカメラにSkinを切り替えた状態を表示させる
                yield return new WaitForSeconds(0.3f);
                var fileName = isTest ? "thumb_" + characterID.ToString() + "_" + i.ToString() : "thumb_" + (i + 1).ToString();
                _saveCaptureTexture(_texWidth, _texHeight, savePath, fileName);
            }
        }

        public override void CopyTexture(Texture2D tex2D, int texWidth, int texHeight)
        {
            for(var y = 0; y < texHeight; y++)
            {
                for(var x = 0; x < texWidth; x++)
                {
                    var pixelColor = tex2D.GetPixel(x, y);
                    pixelColor = new Color(pixelColor.r, pixelColor.g, pixelColor.b, pixelColor.a);
                    tex2D.SetPixel(x, y, pixelColor);
                }
            }
        }
    }
}