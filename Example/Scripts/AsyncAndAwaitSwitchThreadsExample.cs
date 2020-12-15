using System.Threading;
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine;

namespace Framework.Example
{
    public class AsyncAndAwaitSwitchThreadsExample : MonoBehaviour
    {
        async void Start()
        {
            //Unity Thread
            Debug.LogFormat("1. ThreadID:{0}",Thread.CurrentThread.ManagedThreadId);

            await new WaitForBackgroundThread();

            //Background Thread
            Debug.LogFormat("2.After the WaitForBackgroundThread.ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);

            await new WaitForMainThread();

            //Unity Thread
            Debug.LogFormat("3.After the WaitForMainThread.ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);

            await Task.Delay(3000).ConfigureAwait(false);

            //Background Thread
            Debug.LogFormat("4.After the Task.Delay.ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);

            await new WaitForSeconds(1f);

            //Unity Thread
            Debug.LogFormat("5.After the WaitForSeconds.ThreadID:{0}", Thread.CurrentThread.ManagedThreadId);
        }
    }
}