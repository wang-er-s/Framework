using UnityEditor;
using UnityEngine;

public class TextureProcessor : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        if (!CommonAssetProcessor.FirstImport(assetImporter)) return;
        FormatTexture(assetImporter as TextureImporter);
    }

    public static void FormatTexture(TextureImporter importer)
    {
        importer.isReadable = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.anisoLevel = 1;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = true;
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
            psAndroid.format = TextureImporterFormat.ASTC_4x4;
            psIphone.format = TextureImporterFormat.ASTC_4x4;
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