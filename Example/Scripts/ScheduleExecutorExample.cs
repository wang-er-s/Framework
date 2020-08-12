using System;
using Framework.Execution;
using UnityEngine;
using IAsyncResult = Framework.Asynchronous.IAsyncResult;

namespace Framework.Example
{
    public class ScheduleExecutorExample : MonoBehaviour
    {
        private void Start()
        {
            IScheduledExecutor scheduledExecutor = new ThreadScheduledExecutor();
            scheduledExecutor.Start();
            IAsyncResult asyncResult1 = null;
            asyncResult1 = scheduledExecutor.ScheduleAtFixedRate(time =>
            {
                print(time);
                if (time >= 5000)
                    asyncResult1.Cancel();
            }, () => print($"结束"), 1000, 500, 5000);
            
            
            scheduledExecutor = new CoroutineScheduledExecutor();
            scheduledExecutor.Start();
            IAsyncResult asyncResult2 = null;
            asyncResult2 = scheduledExecutor.ScheduleAtFixedRate(time =>
            {
                print(time);
                if (time >= 5000)
                    asyncResult2.Cancel();
            }, () => print($"结束"), 1000, 500, 5000);
        }
    }
}