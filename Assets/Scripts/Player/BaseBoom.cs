using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ZFramework;

public class BaseBoom : IObjPool
{

    private float length;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override IObjPool Init(params object[] value)
    {
        return this;
    }

    public override IObjPool OnShow(params object[] value)
    {
        return this;
    }

    public override IObjPool OnHide(params object[] value)
    {
        return this;
    }
}
