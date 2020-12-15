using System.Collections;
using Framework.Asynchronous;
using UnityEngine;

namespace Framework.Example
{
    public class AsyncResultCallbackExample : MonoBehaviour
    {

        void Start()
        {
            AsyncResult result = new AsyncResult();

            /* Register a callback */
            result.Callbackable().OnCallback((r) =>
            {
                if (r.Exception != null)
                {
                    Debug.LogFormat("The task is finished.IsDone:{0} Exception:{1}", r.IsDone, r.Exception);
                    return;
                }

                Debug.LogFormat("The task is finished. IsDone:{0} Result:{1}", r.IsDone, r.Result);
            });

            /* Start the task */
            this.StartCoroutine(DoTask(result));
        }

        /// <summary>
        /// Simulate a task.
        /// </summary>
        /// <returns>The task.</returns>
        /// <param name="promise">Promise.</param>
        protected IEnumerator DoTask(IPromise promise)
        {
            yield return new WaitForSeconds(0.5f);
            promise.SetResult();
        }

    }
}