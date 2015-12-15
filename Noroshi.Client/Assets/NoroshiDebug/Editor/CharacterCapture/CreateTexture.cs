using System.Collections;
using UniLinq;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace NoroshiDebug.CharacterCapture.Editor.CreateTexture
{
    public abstract class CreateTexture
    {
        protected int _texWidth = 100;
        protected int _texHeight = 100;

        public int TexWidth { get { return _texWidth; } }
        public int TexHeight { get { return _texHeight; } }
        
        Camera _camera = Camera.main;
        Rect _position = new Rect();
        Texture2D[] _previewTextures = new Texture2D[5];
        
        Subject<int> _onChangeTexWidthSubject = new Subject<int>();
        Subject<int> _onChangeTexHeightSubject = new Subject<int>();
        protected Subject<int> _onCreateTextureSubject = new Subject<int>();

        public CreateTexture(){}
        
        public IObservable<int> GetOnChangeTexWidthObservable()
        {
            return _onChangeTexWidthSubject.AsObservable();
        }
        
        public IObservable<int> GetOnChangeTexHeightObservable()
        {
            return _onChangeTexHeightSubject.AsObservable();
        }
        
        public IObservable<int> GetOnCreateTextureObservable()
        {
            return _onCreateTextureSubject.AsObservable();
        }

        public void SetTexWidth(int texWidth)
        {
            _texWidth = texWidth;
            _onChangeTexWidthSubject.OnNext(_texWidth);
        }

        public void SetTexHeight(int texHeight)
        {
            _texHeight = texHeight;
            _onChangeTexHeightSubject.OnNext(_texHeight);
        }
        
        public void SetRect(Rect position)
        {
            _position.Set(position.x, position.y, Screen.width, Screen.height);
        }
        
        public IEnumerator Preview(int texCount)
        {
            if (_texWidth == 0 || _texHeight == 0 || texCount == 0) yield break;

            if (_previewTextures == null) _previewTextures = new Texture2D[5];
            
            for(var i = 0; i < texCount; i++)
            {
                _onCreateTextureSubject.OnNext(i);
                // ここにコルーチンを挟んでちゃんとカメラにSkinを切り替えた状態を表示させる
                yield return new WaitForSeconds(0.3f);
                _previewTextures[i] = _createPreviewTexture(_texWidth, _texHeight);
            }
        }

        public void DrawTexture()
        {
            GUILayout.BeginHorizontal();
            for(var i = 0; i < _previewTextures.Count(tex => tex != null); i++)
            {
                var texRect = GUILayoutUtility.GetRect(0, 0, GUILayout.Width(_texWidth), GUILayout.Height(_texHeight));
                EditorGUI.DrawPreviewTexture(texRect, _previewTextures[i]);
                EditorGUILayout.Space();
            }
            GUILayout.EndHorizontal();
        }
        
        public virtual void DrawTextSize()
        {
            GUILayout.BeginHorizontal(GUILayout.Width(100));
            GUILayout.Label("texWidth");
            var width = EditorGUILayout.IntField(_texWidth, GUILayout.Width(100));
            if (_texWidth != width)
            {
                _texWidth = width;
                _onChangeTexWidthSubject.OnNext(_texWidth);
            }
            
            GUILayout.Label("texHeight");
            var height = EditorGUILayout.IntField(_texHeight, GUILayout.Width(100));
            if (_texHeight != height)
            {
                _texHeight = height;
                _onChangeTexHeightSubject.OnNext(_texHeight);
            }
            GUILayout.EndHorizontal();
        }

        Texture2D _createPreviewTexture(int texWidth, int texHeight)
        {
            var renderTex = new RenderTexture((int)_position.width, (int)_position.height, (int)RenderTextureFormat.ARGB32);
            _camera.targetTexture = renderTex;
            _camera.Render();
            RenderTexture.active = renderTex;
            
            var tex2D = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
            var center2D = new Vector2(_position.width / 2, _position.height / 2);
            var texPosition = new Vector2(center2D.x - (texWidth / 2), center2D.y - (texHeight / 2));
            tex2D.ReadPixels(new Rect(texPosition.x, texPosition.y, texWidth, texHeight), 0, 0);
            _camera.targetTexture = null;

            CopyTexture(tex2D, texWidth, texHeight);
            
            var textureBytes = tex2D.EncodeToPNG();
            var previewTex = new Texture2D(1, 1);
            previewTex.LoadImage(textureBytes);
            
            _camera.targetTexture = null;
            RenderTexture.active = null;
            renderTex.Release();
            Resources.UnloadUnusedAssets();
            
            return previewTex;
        }

        public abstract IEnumerator CaptureTexture(int texCount, int characterID, bool isTest);

        protected void _saveCaptureTexture(int texWidth, int texHeight, string savePath, string fileName)
        {
            var renderTex = new RenderTexture((int)_position.width, (int)_position.height, (int)RenderTextureFormat.ARGB32);
            _camera.targetTexture = renderTex;
            _camera.Render();
            RenderTexture.active = renderTex;
            
            var tex2D = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false);
            var center2D = new Vector2(_position.width / 2, _position.height / 2);
            var texPosition = new Vector2(center2D.x - (texWidth / 2), center2D.y - (texHeight / 2));
            tex2D.ReadPixels(new Rect(texPosition.x, texPosition.y, texWidth, texHeight), 0, 0);
            tex2D.Apply();
            _camera.targetTexture = null;
            CopyTexture(tex2D, texWidth, texHeight);

            var textureBytes = tex2D.EncodeToPNG();
            if (textureBytes != null)
            {
                try
                {
                    System.IO.File.WriteAllBytes(savePath + "/" + fileName + ".png", textureBytes);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
            tex2D = null;
            _camera.targetTexture = null;
            RenderTexture.active = null;
            renderTex.Release();
            Resources.UnloadUnusedAssets();
        }
        
        public bool HasCreateTextureObservers()
        {
            return _onCreateTextureSubject.HasObservers;
        }
        
        public bool HasTexWidthObservers()
        {
            return _onChangeTexWidthSubject.HasObservers;
        }
        
        public bool HasTexHeightObservers()
        {
            return _onChangeTexHeightSubject.HasObservers;
        }
        
        public void Dispose()
        {
            if (HasTexWidthObservers()) _onChangeTexWidthSubject.Dispose();
            if (HasTexHeightObservers()) _onChangeTexHeightSubject.Dispose();
            if (HasCreateTextureObservers()) _onCreateTextureSubject.Dispose();
        }

        public void Destroy()
        {
            if (_previewTextures != null)
            {
                for(var i = 0; i < _previewTextures.Length; i++)
                {
                    if (_previewTextures[i] != null)
                    {
                        _previewTextures[i] = null;
                    }
                }
                _previewTextures = null;
            }
        }

        public abstract void CopyTexture(Texture2D tex2D, int texWidth, int texHeight);
    }
}