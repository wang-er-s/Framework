using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.CommonHelper
{
    public class CoroutineHelper : MonoBehaviour
    {
        /// <summary>
        /// 任务队列
        /// </summary>
        private static readonly Queue<Action> actionTaskQueue = new Queue<Action>();
        private static readonly Dictionary<int, IEnumerator> iEnumeratorDictionary = new Dictionary<int, IEnumerator>();
        private static readonly Dictionary<int, Coroutine> coroutineDictionary = new Dictionary<int, Coroutine>();
        private static readonly Queue<int> iEnumeratorQueue = new Queue<int>();
        private static readonly Queue<int> stopIeIdQueue = new Queue<int>();
        private static int counter = -1;
    
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callBack"></param>
        public static void ExecAction(Action action)
        {
            actionTaskQueue.Enqueue(action);
        }

        /// <summary>
        /// 任务队列
        /// </summary>
        private static readonly Queue<Action> actionTaskQueueImmediately = new Queue<Action>();

        /// <summary>
        /// 立即执行
        /// </summary>
        public static void ExecActionImmediately(Action action)
        {
            actionTaskQueueImmediately.Enqueue(action);
        }

   
        public new static int StartCoroutine(IEnumerator ie)
        {
            counter++;
            iEnumeratorQueue.Enqueue(counter);
            iEnumeratorDictionary[counter] = ie;
            return counter;
        }
    
        public static void StopCoroutine(int id)
        {
            stopIeIdQueue.Enqueue(id);
        }

        private static bool isStopAllCoroutine;

        /// <summary>
        /// 停止携程
        /// </summary>
        public static void StopAllCoroutine()
        {
            isStopAllCoroutine = true;
        }

        /// <summary>
        /// 等待一段时间后执行
        /// </summary>
        /// <param name="f"></param>
        /// <param name="action"></param>
        public static void DelayAction(float f, Action action)
        {
            StartCoroutine(IE_WaitingForExec(f, action));
        }

        public static IEnumerator DelayAction(Action action, int delayFrames = 1)
        {
            for (int i = 0; i < delayFrames; i++)
            {
                yield return null;
            }

            action();
        }

        private static IEnumerator IE_WaitingForExec(float f, Action action)
        {
            yield return new WaitForSecondsRealtime(f);
            action?.Invoke();
        }

        /// <summary>
        /// 主循环
        /// </summary>
        void Update()
        {
            //停止所有携程
            if (isStopAllCoroutine)
            {
                StopAllCoroutines();
                isStopAllCoroutine = false;
            }

            //优先停止携程
            while (stopIeIdQueue.Count > 0)
            {
                var id = stopIeIdQueue.Dequeue();
                if (!coroutineDictionary.TryGetValue(id, out var coroutine)) continue;
                base.StopCoroutine(coroutine);
                coroutineDictionary.Remove(id);
            }

            //携程循环
            while (iEnumeratorQueue.Count > 0)
            {
                var id = iEnumeratorQueue.Dequeue();
                //取出携程
                var ie = iEnumeratorDictionary[id];
                iEnumeratorDictionary.Remove(id);
                //执行携程
                var coroutine = base.StartCoroutine(ie);
                //存入coroutine
                coroutineDictionary[id] = coroutine;
            }

            //主线程循环 立即执行
            while (actionTaskQueueImmediately.Count > 0)
            {
                var task = actionTaskQueueImmediately.Dequeue();
                task?.Invoke();
            }

            //主线程循环
            if (actionTaskQueue.Count > 0)
            {
                var task = actionTaskQueue.Dequeue();
                task?.Invoke();
            }
        }
    }
}