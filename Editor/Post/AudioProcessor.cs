using System;
using UnityEditor;
using UnityEngine;

public class AudioProcessor : AssetPostprocessor
{
    private void OnPostprocessAudio(AudioClip audio)
    {
        if (!CommonAssetProcessor.FirstImport(assetImporter)) return;
       FormatAudio(assetImporter as AudioImporter, audio);
    }

    public static void FormatAudio(AudioImporter importer, AudioClip audioClip = null)
    {
        importer.forceToMono = true;
        if (audioClip == null)
            audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(importer.assetPath);
        if (audioClip == null)
        {
            throw new Exception($"Audio Clip {importer.assetPath} 不存在");
        }
        var audioImporterSampleSettingsAndroid = importer.GetOverrideSampleSettings("Android");
        var audioImporterSampleSettingsios = importer.GetOverrideSampleSettings("Android");

        // 大于10s 表示 BGM/环境音,音效不会弄这么长
        if (audioClip.length >= 10)
        {
            //播放音频的时候流式加载，好处是文件不占用内存，坏处是加载的时候对IO、CPU都会有开销。即使没有加载音频文件，也会占有一个200KB的空间。
            // Vorbis / MP3: 有压缩，比PCM质量有下降，配合 Quality 值进行压缩。适合中等长度声音。

            audioImporterSampleSettingsios.loadType = AudioClipLoadType.Streaming;
            audioImporterSampleSettingsios.compressionFormat = AudioCompressionFormat.Vorbis;
            audioImporterSampleSettingsios.quality = 65f;

            audioImporterSampleSettingsAndroid.loadType = AudioClipLoadType.Streaming;
            audioImporterSampleSettingsAndroid.compressionFormat = AudioCompressionFormat.Vorbis;
            audioImporterSampleSettingsAndroid.quality = 65f;

            importer.loadInBackground = false;
        }
        // 3-10s 表示中断音效
        if (audioClip.length >= 2 && audioClip.length < 10)
        {
            audioImporterSampleSettingsios.loadType = AudioClipLoadType.CompressedInMemory;
            audioImporterSampleSettingsAndroid.compressionFormat = AudioCompressionFormat.Vorbis;
            audioImporterSampleSettingsios.quality = 70f;

            audioImporterSampleSettingsAndroid.loadType = AudioClipLoadType.CompressedInMemory;
            audioImporterSampleSettingsAndroid.compressionFormat = AudioCompressionFormat.Vorbis;
            audioImporterSampleSettingsAndroid.quality = 70f;

            importer.loadInBackground = false;
        }
        // 段音效
        if (audioClip.length < 2)
        {
            importer.loadInBackground = true;

            audioImporterSampleSettingsios.loadType = AudioClipLoadType.DecompressOnLoad;
            audioImporterSampleSettingsios.compressionFormat = AudioCompressionFormat.ADPCM;

            audioImporterSampleSettingsAndroid.loadType = AudioClipLoadType.DecompressOnLoad;
            audioImporterSampleSettingsAndroid.compressionFormat = AudioCompressionFormat.Vorbis;
        }

        //关闭预加载
        importer.preloadAudioData = false;

        importer.SetOverrideSampleSettings(BuildTargetGroup.iOS.ToString(), audioImporterSampleSettingsios);
        importer.SetOverrideSampleSettings(BuildTargetGroup.Android.ToString(), audioImporterSampleSettingsAndroid);


        importer.SetOverrideSampleSettings("Android", audioImporterSampleSettingsAndroid);
        importer.SetOverrideSampleSettings("iPhone", audioImporterSampleSettingsAndroid);
        
        importer.SaveAndReimport();
    }
}