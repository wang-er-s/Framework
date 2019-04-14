using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ZFramework;

public class BaseBoom : IObjPool
{

    private float length;
    private float timer;
    

    void Boom()
    {
        //TODO 爆炸
        ComplexObjectPool.Instance.Push( ComplexPoolObjectType.Boom,this);
    }

    public override IObjPool Init(params object[] value)
    {
        return this;
    }

    public override IObjPool OnShow(params object[] value)
    {
        transform.position = (Vector3) value[0];
        timer = (float) value[1];
        length = (float) value[2];
        gameObject.Show();
        StartCoroutine(InitBoom());
        return this;
    }

    public override IObjPool OnHide(params object[] value)
    {
        gameObject.Hide();
        return this;
    }

    IEnumerator InitBoom()
    {
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }
        Boom();
    }

    /// <summary>
    /// 立刻爆炸
    /// </summary>
    public void ImmdiBoom()
    {
        timer = 0;
    }

    public BaseBoom Create()
    {
        //ComplexObjectPool.Instance.( ComplexPoolObjectType.Boom,this);
    }

}
