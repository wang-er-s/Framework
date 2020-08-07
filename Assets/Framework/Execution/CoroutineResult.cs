using UnityEngine;
using System.Collections.Generic;
using Framework.Asynchronous;


namespace Framework.Execution
{
    public class CoroutineResult : AsyncResult, ICoroutinePromise
    {
        protected readonly List<Coroutine> Coroutines = new List<Coroutine>();

        public CoroutineResult() : base(true)
        {
        }

        public override bool Cancel()
        {
            if (this.IsDone)
                return false;

            this.CancellationRequested = true;
            foreach (Coroutine coroutine in this.Coroutines)
            {
                Executors.StopCoroutine(coroutine);
            }

            this.SetCancelled();
            return true;
        }

        public void AddCoroutine(Coroutine coroutine)
        {
            this.Coroutines.Add(coroutine);
        }
    }

    public class CoroutineResult<TResult> : AsyncResult<TResult>, ICoroutinePromise<TResult>
    {
        protected List<Coroutine> coroutines = new List<Coroutine>();

        public CoroutineResult() : base(true)
        {
        }

        public override bool Cancel()
        {
            if (this.IsDone)
                return false;

            this.CancellationRequested = true;
            foreach (Coroutine coroutine in this.coroutines)
            {
                Executors.StopCoroutine(coroutine);
            }

            this.SetCancelled();
            return true;
        }

        public void AddCoroutine(Coroutine coroutine)
        {
            this.coroutines.Add(coroutine);
        }
    }

    public class CoroutineProgressResult<TProgress> : ProgressResult<TProgress>, ICoroutineProgressPromise<TProgress>
    {
        protected readonly List<Coroutine> Coroutines = new List<Coroutine>();

        public CoroutineProgressResult() : base(true)
        {
        }

        public override bool Cancel()
        {
            if (this.IsDone)
                return false;

            this.CancellationRequested = true;
            foreach (Coroutine coroutine in this.Coroutines)
            {
                Executors.StopCoroutine(coroutine);
            }

            this.SetCancelled();
            return true;
        }

        public void AddCoroutine(Coroutine coroutine)
        {
            this.Coroutines.Add(coroutine);
        }
    }

    public class CoroutineProgressResult<TProgress, TResult> : ProgressResult<TProgress, TResult>,
        ICoroutineProgressPromise<TProgress, TResult>
    {
        protected readonly List<Coroutine> Coroutines = new List<Coroutine>();

        public CoroutineProgressResult() : base(true)
        {
        }

        public override bool Cancel()
        {
            if (this.IsDone)
                return false;

            this.CancellationRequested = true;
            foreach (Coroutine coroutine in this.Coroutines)
            {
                Executors.StopCoroutine(coroutine);
            }

            this.SetCancelled();
            return true;
        }

        public void AddCoroutine(Coroutine coroutine)
        {
            this.Coroutines.Add(coroutine);
        }
    }
}