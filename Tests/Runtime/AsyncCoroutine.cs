using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Execution;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Tests
{
    public class AsyncCoroutineTest
    {
        private int mainThreadId;

        //IThreadPoolWorkItem, IAsyncResult,
        [UnityTest]
        public IEnumerator AwaitTest()
        {
            bool isComplete = false;
            AwaitTest1(() => isComplete = true);
            while (!isComplete)
            {
                yield return null;
            }
        }
        public async void AwaitTest1(Action complete)
        {
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
            float timer = Time.time;
            await new WaitForSeconds(0.1f);
            timer += 0.1f;
            Assert.IsTrue(Mathf.Abs(Time.time - timer) <= 0.1f);

            await Task.Delay(100);
            timer += 0.1f;
            Assert.IsTrue(Mathf.Abs(Time.time - timer) <= 0.1f);

            UnityWebRequest www = await UnityWebRequest.Get("http://www.baidu.com").SendWebRequest();
            
            Assert.IsFalse(www.isHttpError);
            Assert.IsFalse(www.isNetworkError);
            if (!www.isHttpError && !www.isNetworkError)
                Debug.Log(www.downloadHandler.text);
            timer = Time.time;
            
            //切换到另一个线程
            float result = await Calculate();
            timer += result / 1000;
            Assert.IsTrue(mainThreadId != Thread.CurrentThread.ManagedThreadId);
            Debug.Log($"async thread : {Thread.CurrentThread.ManagedThreadId}");

            //切换到主线程
            await new WaitForMainThread();
            Assert.IsTrue(mainThreadId == Thread.CurrentThread.ManagedThreadId);
            Debug.LogFormat("Switch to the main thread,Current Thread ID:{0}", Thread.CurrentThread.ManagedThreadId);

            await new WaitForSecondsRealtime(0.1f);
            timer += 0.1f;
            Assert.IsTrue(Mathf.Abs(Time.time - timer) <= 0.1f);

            await DoTask(5);
            timer += 0.1f;
            Assert.IsTrue(Mathf.Abs(Time.time - timer) <= 0.1f);
            complete();
        }
        
        IAsyncResult<float> Calculate()
        {
            return Executors.RunAsync(() =>
            {
                Assert.IsTrue(mainThreadId != Thread.CurrentThread.ManagedThreadId);
                float total = 0;
                for (int i = 0; i < 20; i++)
                {
                    try
                    {
                        Thread.Sleep(10);
                    }
                    catch (Exception) { }
                }
                total += 20 * 10;
                return total;
            });
        }
        
        IEnumerator DoTask(int n)
        {
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < n; i++)
            {
                yield return null;
            }
        }
    }
}