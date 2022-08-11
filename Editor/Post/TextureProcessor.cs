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

        if (IsPureTexture(texture))
        {
            if (texture.width > 4 || texture.height > 4)
            {
                EditorUtility.DisplayDialog("注意", $"{assetPath} 是纯色图片，大小需要是4*4，请及时修改", "好的");
                Debug.LogError($"{assetPath} 是纯色图片，大小需要是4*4，请及时修改");
            }
        }
        
        TextureImporter importer = assetImporter as TextureImporter;
        importer.isReadable = CommonAssetProcessor.ReadWrite(importer.assetPath);
        importer.SaveAndReimport();
    }

    private void OnPreprocessTexture()
    {
        if (!CommonAssetProcessor.FirstImport(assetImporter)) return;
        FormatTexture(assetImporter as TextureImporter);
    }

    public static void FormatTexture(TextureImporter importer)
    {
        importer.isReadable = true;
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

    private static bool IsPureTexture(Texture2D texture)
    {
        var color1 = texture.GetPixel(0, 0);
        bool isPure1 = true;
        foreach (var pixel in texture.GetPixels())
        {
            if (color1 != pixel)
            {
                isPure1 = false;
                break;
            }
        }
        return isPure1;
        
        // Create a temporary RenderTexture of the same size as the texture
        RenderTexture tmp = RenderTexture.GetTemporary(
            texture.width,
            texture.height,
            0,
            RenderTextureFormat.Default,
            RenderTextureReadWrite.Linear);

// Blit the pixels on texture to the RenderTexture
        Graphics.Blit(texture, tmp);

// Backup the currently set RenderTexture
        RenderTexture previous = RenderTexture.active;

// Set the current RenderTexture to the temporary one we created
        RenderTexture.active = tmp;

// Create a new readable Texture2D to copy the pixels to it
        Texture2D myTexture2D = new Texture2D(texture.width, texture.height);

// Copy the pixels from the RenderTexture to the new Texture
        myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
        myTexture2D.Apply();
        
        var color = myTexture2D.GetPixel(0, 0);
        bool isPure = true;
        foreach (var pixel in myTexture2D.GetPixels())
        {
            if (color != pixel)
            {
                isPure = false;
                break;
            }
        }
        
// Reset the active RenderTexture
        RenderTexture.active = previous;

// Release the temporary RenderTexture
        RenderTexture.ReleaseTemporary(tmp);

        return isPure;
    }
    
}