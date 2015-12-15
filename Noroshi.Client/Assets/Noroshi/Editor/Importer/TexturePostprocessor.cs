using UnityEngine;
using UnityEditor;

public sealed class TexturePostprocessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        TextureImporter textureImporter = assetImporter as TextureImporter;
        textureImporter.mipmapEnabled = false;
    }
}
