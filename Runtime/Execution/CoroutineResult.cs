/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using UnityEngine;
using System.Collections.Generic;


namespace Framework
{
    public class CoroutineResult : AsyncResult, ICoroutinePromise
    {
        protected List<Coroutine> Coroutines = new List<Coroutine>();


        public static CoroutineResult Create(bool cancelable = true)
        {
            CoroutineResult result = ReferencePool.Allocate<CoroutineResult>();
            result.Cancelable = cancelable;
            return result;
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

        public override void Clear()
        {
            base.Clear();
            Coroutines.Clear();
        }
    }

    public class CoroutineResult<TResult> : AsyncResult<TResult>, ICoroutinePromise<TResult>
    {
        protected List<Coroutine> coroutines = new List<Coroutine>();

        public static CoroutineResult<TResult> Create(bool cancelable = true)
        {
            var result = ReferencePool.Allocate<CoroutineResult<TResult>>();
            result.Cancelable = cancelable;
            return result;
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

        public override void Clear()
        {
            base.Clear();
            coroutines.Clear();
        }
    }

    public class CoroutineProgressResult<TProgress> : ProgressResult<TProgress>, ICoroutineProgressPromise<TProgress>
    {
        protected readonly List<Coroutine> Coroutines = new List<Coroutine>();

        public static CoroutineProgressResult<TProgress> Create(bool cancelable = true)
        {
            var result = ReferencePool.Allocate<CoroutineProgressResult<TProgress>>();
            result.Cancelable = cancelable;
            return result;
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

        public override void Clear()
        {
            base.Clear();
            Coroutines.Clear();
        }
    }

    public class CoroutineProgressResult<TProgress, TResult> : ProgressResult<TProgress, TResult>,
        ICoroutineProgressPromise<TProgress, TResult>
    {
        protected List<Coroutine> Coroutines = new List<Coroutine>();

        public static CoroutineProgressResult<TProgress, TResult> Create(bool cancelable = true)
        {
            var result = ReferencePool.Allocate<CoroutineProgressResult<TProgress, TResult>>();
            result.Cancelable = cancelable;
            return result;
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

        public override void Clear()
        {
            base.Clear();
            Coroutines.Clear();
        }
    }
}