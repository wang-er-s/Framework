using System;
using System.Threading;
using Framework.Execution;
using UnityEngine;
using IAsyncResult = Framework.Asynchronous.IAsyncResult;

namespace Framework.Example
{
    public class ScheduleExecutorExample : MonoBehaviour
    {
        private void Start()
        {
            ITimerExecutor timerExecutor = new ThreadTimerExecutor();
            timerExecutor.Start();
            IAsyncResult asyncResult1 = null;
            asyncResult1 = timerExecutor.FixedRateAtDuration(time =>
            {
                print($"time={time}  thread id ={Thread.CurrentThread.ManagedThreadId}");
                if (time >= 5000)
                    asyncResult1.Cancel();
            }, () => print($"结束"), 1000, 500, 5000);
            
            
            timerExecutor = new CoroutineTimerExecutor();
            timerExecutor.Start();
            IAsyncResult asyncResult2 = null;
            asyncResult2 = timerExecutor.FixedRateAtDuration(time =>
            {
                print($"time={time}  thread id ={Thread.CurrentThread.ManagedThreadId}");
                if (time >= 5000)
                    asyncResult2.Cancel();
            }, () => print($"结束"), 1000, 500, 5000);
        }
    }
}