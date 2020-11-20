using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Framework;
using Framework.Assets;
using Framework.Asynchronous;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Slider _slider;
    public Button _button;
    public Text _text;
    
    [Button]
    private async void CheckDownload()
    {
        var size = await Addressables.GetDownloadSizeAsync("dynamic");
        _text.text = $"{size / 1024}kb";
    }

    [Button]
    private async void Download()
    {
        var operation = Addressables.DownloadDependenciesAsync("dynamic");
        while (!operation.IsDone)
        {
            _slider.value = operation.PercentComplete;
            await Task.Yield();
        }
        _slider.value = 1;
    }

    [Button]
    private void LoadCube()
    {
        Addressables.InstantiateAsync("cube").Completed += handle =>
        {
            print("load cube");
            _text.text = handle.Result.name;
        };
    }

    private SpriteLoader Single;
    [Button]
    private async void LoadSingle()
    {
        Single = new SpriteLoader();
        var sp = await Single.LoadSprite("single");
        print(sp);
        _text.text = sp.name;
    }

    private SpriteLoader MulSprite;
    
    [Button]
    public async void LoadMul()
    {
        MulSprite = new SpriteLoader();
        var sp = await MulSprite.LoadSprite("sheet/sprite_sheet_0");
        print(sp);
        _text.text = sp.name;
    }

    private void Start()
    {
        AddButton(nameof(LoadSingle), LoadSingle);
        AddButton(nameof(LoadMul), LoadMul);
        AddButton(nameof(LoadCube), LoadCube);
        AddButton(nameof(CheckDownload), CheckDownload);
        AddButton(nameof(Download), Download);
    }

    private void AddButton(string text, Action action)
    {
        var btn = Instantiate(_button, _button.transform.parent);
        btn.GetComponentInChildren<Text>().text = text;
        btn.onClick.AddListener(() => action());
        btn.gameObject.SetActive(true);
    }
}
