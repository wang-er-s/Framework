using System.Collections.Generic;

namespace Framework.Asynchronous
{
    public class MulProgressResult : AsyncResult, IProgressResult<float>
    {
        public float Progress { get; private set;}
        private ProgressCallbackable<float> _callbackable;
        private List<IProgressResult<float>> _allProgress = new List<IProgressResult<float>>();

        public MulProgressResult(params IProgressResult<float>[] allProgress) : this(false, allProgress)
        {
        }

        public MulProgressResult(bool cancelable, params IProgressResult<float>[] allProgress) : base(cancelable)
        {
            AddProgress(allProgress);
        }

        public void AddProgress(IProgressResult<float> progressResult)
        {
            _allProgress.Add(progressResult);
            SetSubProgressCb(progressResult);
        }

        public void AddProgress(IEnumerable<IProgressResult<float>> progressResults)
        {
            foreach (var progressResult in progressResults)
            {
                AddProgress(progressResult);
            }
        }

        private void SetSubProgressCb(IProgressResult<float> progressResult)
        {
            progressResult.Callbackable().OnProgressCallback(f => RaiseOnProgressCallback(0));
            progressResult.Callbackable().OnCallback(f => RaiseOnProgressCallback(0));
        }

        protected virtual void RaiseOnProgressCallback(float progress)
        {
            UpdateProgress();
            _callbackable?.RaiseOnProgressCallback(Progress);
            if(Progress >= 1)
                SetResult();
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
                else
                {
                    totalProgress += progressResult.Progress;
                }
            }
            Progress = totalProgress / _allProgress.Count;
        }
        
        public new virtual IProgressCallbackable<float> Callbackable()
        {
            lock (Lock)
            {
                return this._callbackable ?? (this._callbackable = new ProgressCallbackable<float>(this));
            }
        }
    }
}