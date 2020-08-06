using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Execution;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;

namespace Tests
{
    public class AsyncCoroutineTest
    {
        //IThreadPoolWorkItem, IAsyncResult,
        [Test]
        public async void AwaitTest()
        {
            await new WaitForSeconds(2f);
            Debug.Log("WaitForSeconds  End");

            await Task.Delay(1000);
            Debug.Log("Delay  End");

            UnityWebRequest www = await UnityWebRequest.Get("http://www.baidu.com").SendWebRequest();
            
            if (!www.isHttpError && !www.isNetworkError)
                Debug.Log(www.downloadHandler.text);

            int result = await Calculate();
            Debug.LogFormat("Calculate Result = {0} Calculate Task End,Current Thread ID:{1}", result, Thread.CurrentThread.ManagedThreadId);

            await new WaitForMainThread();
            Debug.LogFormat("Switch to the main thread,Current Thread ID:{0}", Thread.CurrentThread.ManagedThreadId);

            await new WaitForSecondsRealtime(1f);
            Debug.Log("WaitForSecondsRealtime  End");

            await DoTask(5);
            Debug.Log("DoTask End");
        }
        
        IAsyncResult<int> Calculate()
        {
            return Executors.RunAsync(() =>
            {
                Debug.LogFormat("Calculate Task ThreadId:{0}", Thread.CurrentThread.ManagedThreadId);
                int total = 0;
                for (int i = 0; i < 20; i++)
                {
                    total += i;
                    try
                    {
                        Thread.Sleep(100);
                    }
                    catch (Exception) { }
                }
                return total;
            });
        }
        
        IEnumerator DoTask(int n)
        {
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < n; i++)
            {
                yield return null;
            }
        }
    }
}