using System.Collections;
using Framework.Execution;
using UnityEngine;

namespace Framework.Example
{
    public class ExecutorExample : MonoBehaviour
    {

        IEnumerator Start()
        {
            Executors.RunAsync(() =>
            {
                Debug.LogFormat("RunAsync ");
            });


            Executors.RunAsync(() =>
            {
                Executors.RunOnMainThread(() =>
                {
                    Debug.LogFormat("RunOnMainThread Time:{0} frame:{1}", Time.time, Time.frameCount);
                }, true);
            });

            Executors.RunOnMainThread(() =>
            {
                Debug.LogFormat("RunOnMainThread 2 Time:{0} frame:{1}", Time.time, Time.frameCount);
            }, false);

            Asynchronous.IAsyncResult result = Executors.RunOnCoroutine(DoRun());

            yield return result.WaitForDone();

            Debug.LogFormat("============finished=============");

        }

        IEnumerator DoRun()
        {
            for (int i = 0; i < 10; i++)
            {
                Debug.LogFormat("i = {0}", i);
                yield return null;
            }
        }
    }
}