using UnityEditor;
using UnityEngine;

public class TextureProcessor : AssetPostprocessor
{
    private void OnPostprocessTexture(Texture2D texture)
    {
        if (!CommonAssetProcessor.FirstImport(assetImporter)) return;
        if (!CommonAssetProcessor.IsUI(assetPath))
        {
            // 判断图片尺寸是否是2的次方
            if (!Mathf.IsPowerOfTwo(texture.width) || !Mathf.IsPowerOfTwo(texture.height))
            {
                EditorUtility.DisplayDialog("注意", $"{assetPath} 长宽非2的次方，请及时修改", "好的");
                Debug.LogError($"{assetPath} 长宽非2的次方，请及时修改");
            }
        }
    }

    private void OnPreprocessTexture()
    {
        if (!CommonAssetProcessor.FirstImport(assetImporter)) return;
        TextureImporter importer = assetImporter as TextureImporter;
        FormatTexture(importer);
    }

    public static void FormatTexture(TextureImporter importer)
    {
        importer.isReadable = CommonAssetProcessor.ReadWrite(importer.assetPath);
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.anisoLevel = 1;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = CommonAssetProcessor.HasMipMap(importer.assetPath);
        if (CommonAssetProcessor.IsUI(importer.assetPath))
        {
            importer.mipmapEnabled = false;
            importer.textureType = TextureImporterType.Sprite;
            importer.npotScale = TextureImporterNPOTScale.None;
        }
        importer.npotScale = TextureImporterNPOTScale.None;
        TextureImporterPlatformSettings psAndroid = importer.GetPlatformTextureSettings("Android");
        TextureImporterPlatformSettings psIphone = importer.GetPlatformTextureSettings("iPhone");
        psAndroid.overridden = true;
        psIphone.overridden = true;
        if (importer.DoesSourceTextureHaveAlpha())
        {
            psAndroid.format = TextureImporterFormat.ASTC_6x6;
            psIphone.format = TextureImporterFormat.ASTC_6x6;
        }
        else
        {
            psAndroid.format = TextureImporterFormat.ASTC_6x6;
            psIphone.format = TextureImporterFormat.ASTC_6x6;
        }
        importer.SetPlatformTextureSettings(psAndroid);
        importer.SetPlatformTextureSettings(psIphone);
        importer.SaveAndReimport();
    }
}