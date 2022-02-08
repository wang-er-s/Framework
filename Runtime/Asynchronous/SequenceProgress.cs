using System;
using System.Collections.Generic;

namespace Framework.Asynchronous
{
    public class SequenceProgress : ProgressResult<float>
    {
        protected Queue<Func<IProgressResult<float>>> progressQueue = new Queue<Func<IProgressResult<float>>>();
        private IProgressResult<float> currentProgress;
        private int finishProgress;

        public override bool IsDone => currentProgress == null || base.IsDone;

        public SequenceProgress(params Func<IProgressResult<float>>[] allProgress) : this(false, allProgress)
        {
        }

        public SequenceProgress(IProgressResult<float> progress, params Func<IProgressResult<float>>[] allProgress) : this(false, allProgress)
        {
            currentProgress = progress;
            SetSubProgressCb(currentProgress);
            AddAsyncResult(allProgress);
        }

        public SequenceProgress(bool cancelable, params Func<IProgressResult<float>>[] allProgress) : base(cancelable)
        {
            AddAsyncResult(allProgress);
        }

        public void AddAsyncResult(Func<IProgressResult<float>> progressResult)
        {
            if(progressResult == null) return;
            progressQueue.Enqueue(progressResult);
            if (currentProgress == null)
            {
                SetNextProgress();
            }
        }

        private void SetNextProgress()
        {
            if (progressQueue.Count > 0)
            {
                currentProgress = progressQueue.Dequeue().Invoke();
                SetSubProgressCb(currentProgress);
            }
            else
            {
                currentProgress = null;
            }
        }

        public void AddAsyncResult(IEnumerable<Func<IProgressResult<float>>> progressResults)
        {
            foreach (var progressResult in progressResults)
            {
                AddAsyncResult(progressResult);
            }
        }

        private void SetSubProgressCb(IProgressResult<float> progressResult)
        {
            progressResult.Callbackable().OnProgressCallback((progress => RaiseOnProgressCallback(0)));
            progressResult.Callbackable().OnCallback(progress =>
            {
                finishProgress++;
                if (!CheckAllFinish())
                {
                    SetNextProgress();
                }
            });
        }

        protected override void RaiseOnProgressCallback(float progress)
        {
            UpdateProgress();
            base.RaiseOnProgressCallback(Progress);
        }

        private bool CheckAllFinish()
        {
            RaiseOnProgressCallback(0);
            if (currentProgress.IsDone && progressQueue.Count <= 0)
            {
                //延迟一帧 否则会比子任务提前完成
                Timer.RegisterFrame(() => SetResult());
                return true;
            }
            return false;
        }

        private void UpdateProgress()
        {
            float totalProgress = finishProgress + currentProgress.Progress;
            // 1 是当前正在执行的progress
            Progress = totalProgress / (finishProgress + progressQueue.Count + 1);
        }
    }
}