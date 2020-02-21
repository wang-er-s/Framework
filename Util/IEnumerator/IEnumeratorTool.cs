using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Framework;
using UnityEngine.UI;

public class IEnumeratorTool : MonoBehaviour
{
    /// <summary>
    /// 压入的action任务
    /// </summary>
    public class ActionTask
    {
        public Action willDoAction;
        public Action callBackAction;
    }

    /// <summary>
    /// 任务队列
    /// </summary>
    private static Queue<ActionTask> _actionTaskQueue = new Queue<ActionTask>();

    /// <summary>
    /// 执行任务
    /// </summary>
    /// <param name="action"></param>
    /// <param name="callBack"></param>
    public static void ExecAction(Action action, Action callBack = null)
    {
        var task = new ActionTask()
        {
            willDoAction = action,
            callBackAction = callBack
        };


        _actionTaskQueue.Enqueue(task);
    }


    /// <summary>
    /// 任务队列
    /// </summary>
    public static Queue<ActionTask> _actionTaskQueueImmediately = new Queue<ActionTask>();

    /// <summary>
    /// 立即执行
    /// </summary>
    public static void ExecActionImmediately(Action action, Action callBack = null)
    {
        var task = new ActionTask()
        {
            willDoAction = action,
            callBackAction = callBack
        };


        _actionTaskQueueImmediately.Enqueue(task);
    }

    static readonly Dictionary<int, IEnumerator> _iEnumeratorDictionary = new Dictionary<int, IEnumerator>();
    static readonly Dictionary<int, Coroutine> _coroutineDictionary = new Dictionary<int, Coroutine>();
    static readonly Queue<int> _iEnumeratorQueue = new Queue<int>();
    static int counter = -1;

    public new static int StartCoroutine(IEnumerator ie)
    {
        counter++;
        _iEnumeratorQueue.Enqueue(counter);
        _iEnumeratorDictionary[counter] = ie;
        return counter;
    }

    static Queue<int> _stopIEIdQueue = new Queue<int>();

    public static void StopCoroutine(int id)
    {
        _stopIEIdQueue.Enqueue(id);
    }

    private static bool _isStopAllCoroutine = false;

    /// <summary>
    /// 停止携程
    /// </summary>
    public static void StopAllCroutine()
    {
        _isStopAllCoroutine = true;
    }
    
    /// <summary>
    /// 等待一段时间后执行
    /// </summary>
    /// <param name="f"></param>
    /// <param name="action"></param>
    public static void WaitingForExec(float f, Action action)
    {
        StartCoroutine(IE_WaitingForExec(f, action));
    }

    private static IEnumerator IE_WaitingForExec(float f, Action action)
    {
        yield return new WaitForSecondsRealtime(f);
        if (action != null)
        {
            action();
        }
        yield break;
    }

    /// <summary>
    /// 主循环
    /// </summary>
    void Update()
    {
        //停止所有携程
        if (_isStopAllCoroutine)
        {
            StopAllCoroutines();
            _isStopAllCoroutine = false;
        }

        //优先停止携程
        while (_stopIEIdQueue.Count > 0)
        {
            var id = _stopIEIdQueue.Dequeue();
            Coroutine coroutine = null;
            if (_coroutineDictionary.TryGetValue(id, out coroutine))
            {

                base.StopCoroutine(coroutine);
                //
                _coroutineDictionary.Remove(id);
            }
            else
            {
                Debug.LogErrorFormat("此id协程不存在,无法停止:{0}", id);
            }
        }

        //携程循环
        while (_iEnumeratorQueue.Count > 0)
        {
            var id = _iEnumeratorQueue.Dequeue();
            //取出携程
            var ie = _iEnumeratorDictionary[id];
            _iEnumeratorDictionary.Remove(id);
            //执行携程
            var coroutine = base.StartCoroutine(ie);
            //存入coroutine
            _coroutineDictionary[id] = coroutine;
        }

        //主线程循环 立即执行
        while (_actionTaskQueueImmediately.Count > 0)
        {
            var task = _actionTaskQueueImmediately.Dequeue();
            task.willDoAction();
            if (task.callBackAction != null)
            {
                task.callBackAction();
            }
        }

        //主线程循环
        if (_actionTaskQueue.Count > 0)
        {
            var task = _actionTaskQueue.Dequeue();
            task.willDoAction();
            if (task.callBackAction != null)
            {
                task.callBackAction();
            }
        }
    }
}