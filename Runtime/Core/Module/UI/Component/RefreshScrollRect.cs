using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 
public class RefreshScrollRect : ScrollRect {
 
    //高度 往下拉是负数   往上拉是正数
    float f = -300f;
    //是否刷新
    bool isRef = false;
    //是否处于拖动
    bool isDrag = false;
    bool isUp = false;
    //如果满足刷新条件 执行的方法
    public Action PageUp;
    public Action PageDown;
    private RectTransform rect=>GetComponentInChildren<ContentSizeFitter>().GetComponent<RectTransform>();
 
    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(ScrollValueChanged);
    }
 
    /// <summary>
    /// 当ScrollRect被拖动时
    /// </summary>
    /// <param name="vector">被拖动的距离与Content的大小比例</param>
    void ScrollValueChanged(Vector2 vector)
    {
        //如果不拖动 当然不执行之下的代码
        if (!isDrag)
            return;
        //这个就是Content
        //如果拖动的距离大于给定的值
        if (f > rect.rect.height * vector.y)
        {
            isRef = true;
            isUp = true;
        }
        else if(-f+rect.rect.height<rect.rect.height * vector.y)
        {
            isRef = true;
            isUp = false;
        }
        else
        {
            isRef = false;
        }
    }
 
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        isDrag = true;
    }
 
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (isRef)
        {
            if (isUp)
            {
                PageUp?.Invoke();
            }
            else
            {
                PageDown?.Invoke();
            }
        }
        isRef = false;
        isDrag = false;
    }
 
}