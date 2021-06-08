using System.Collections.Generic;
using Framework.Execution;

namespace Framework.Asynchronous
{
    public class MulAsyncResult : AsyncResult
    {
        public float Progress { get; private set;}
        private Callbackable _callbackable;
        private List<IAsyncResult> _allProgress = new List<IAsyncResult>();

        public MulAsyncResult(params IAsyncResult[] allProgress) : this(false, allProgress)
        {
        }

        public MulAsyncResult(bool cancelable, params IAsyncResult[] allProgress) : base(cancelable)
        {
            AddAsyncResult(allProgress);
        }

        public void AddAsyncResult(IAsyncResult progressResult)
        {
            _allProgress.Add(progressResult);
            SetSubProgressCb(progressResult);
        }

        public void AddAsyncResult(IEnumerable<IAsyncResult> progressResults)
        {
            foreach (var progressResult in progressResults)
            {
                AddAsyncResult(progressResult);
            }
        }

        private void SetSubProgressCb(IAsyncResult progressResult)
        {
            progressResult.Callbackable().OnCallback(f => RaiseOnProgressCallback(0));
        }

        protected virtual void RaiseOnProgressCallback(float progress)
        {
            UpdateProgress();
            //延迟一帧 否则会比子任务提前完成
            if (Progress >= 1)
            {
                GameLoop.Ins.Delay(() => SetResult());
            }
        }

        protected override void RaiseOnCallback()
        {
            base.RaiseOnCallback();
            _callbackable?.RaiseOnCallback();
        }

        private void UpdateProgress()
        {
            float totalProgress = 0;
            foreach (var progressResult in _allProgress)
            {
                if (progressResult.IsDone)
                {
                    totalProgress += 1;
                }
            }
            Progress = totalProgress / _allProgress.Count;
        }
        
        public override ICallbackable Callbackable()
        {
            lock (Lock)
            {
                return this._callbackable ?? (this._callbackable = new Callbackable(this));
            }
        }
    }
}