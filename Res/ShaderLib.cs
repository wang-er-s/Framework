using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class ShaderLib : MonoSingleton<ShaderLib>
{
    private Dictionary<string, Shader> _shaderDic;
    public List<ShaderVariantCollection> svcs;
    public List<Shader> shaders;

    public Dictionary<string, Shader> ShaderDic
    {
        get { return _shaderDic; }
        set { _shaderDic = value; }
    }

    protected void Awake()
    {
        _shaderDic = new Dictionary<string, Shader>();
#if UNITY_EDITOR
        shaders = new List<Shader>();
#endif
    }

    public void AddShader(Shader shader)
    {
        if (!_shaderDic.ContainsKey(shader.name))
        {
            _shaderDic.Add(shader.name,shader);
#if UNITY_EDITOR
            shaders.Add(shader);
#endif
        }
    }

    public void RemoveShader(Shader shader)
    {
        if (_shaderDic.ContainsKey(shader.name))
        {
            _shaderDic.Remove(shader.name);
#if UNITY_EDITOR
            shaders.Remove(shader);
#endif
        }
    }

    public void AddRemoveShader(Shader shader, bool addRemove)
    {
        if(addRemove)
            AddShader(shader);
        else
            RemoveShader(shader);
    }

    public Shader GetShader(string shaderName)
    {
        Shader shader = null;
        _shaderDic.TryGetValue(shaderName, out shader);
        return shader;
    }

    public void AddRemoveSvc(ShaderVariantCollection svc,bool addRemove)
    {
        if (addRemove)
            svcs.Add(svc);
        else
            svcs.Remove(svc);
    }
    [ContextMenu("UpdateShaders")]
    public void UpdateFromResources()
    {
        Shader[] shaders = Resources.FindObjectsOfTypeAll<Shader>();
        foreach (var shader in shaders)
        {
            if(!_shaderDic.ContainsKey(shader.name))
                _shaderDic.Add(shader.name,shader);
        }
    }
}
