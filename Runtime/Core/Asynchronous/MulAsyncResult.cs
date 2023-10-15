using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Framework
{
    public interface IMulProgress
    {
        float Current { get; }
        float Total { get; }
    }
    
    public class MulAsyncResult : ProgressResult<float>
    {
        private RecyclableList<bool> progressFinishState;
        private RecyclableList<IAsyncResult> _allProgress;
        public override bool IsDone
        {
            get
            {
                if (isAllDone)
                {
                    RaiseOnProgressCallback(0);
                    return true;
                }
                return _allProgress.Count <= 0 || base.IsDone;
            }
        }

        private bool isAllDone = false;

        private MulAsyncResult()
        {
        }

        public static MulAsyncResult Create([CallerMemberName]string debugName = "", bool cancelable = true, bool isFromPool = false, params IAsyncResult[] allProgress)
        {
            var result = isFromPool ? ReferencePool.Allocate<MulAsyncResult>() : new MulAsyncResult();
            result._allProgress = RecyclableList<IAsyncResult>.Create();
            result.progressFinishState = RecyclableList<bool>.Create();
            result.OnCreate(debugName, cancelable, isFromPool);
            result.AddAsyncResult(allProgress);
            return result;
        }

        public void AddAsyncResult(IAsyncResult progressResult)
        {
            if (progressResult == null) return;
            _allProgress.Add(progressResult);
            progressFinishState.Add(progressResult.IsDone);
            SetSubProgressCb(progressResult);
            CheckAllFinish();
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
            if (progressResult.IsDone) return;
            progressResult.Callbackable().OnCallback(f => RaiseOnProgressCallback(0));
        }

        private bool CheckAllFinish()
        {
            for (int i = 0; i < _allProgress.Count; i++)
            {
                var progressResult = _allProgress[i];
                if (!progressFinishState[i] &&  progressResult.IsDone)
                {
                    progressFinishState[i] = true;
                }
            }
            
            for (var index = 0; index < _allProgress.Count; index++)
            {
                var progressResult = _allProgress[index];
                if (!progressResult.IsDone)
                {
                    isAllDone = false;
                    return false;
                }
            }
            isAllDone = true;
            return true;
        }

        protected override async void RaiseOnProgressCallback(float progress)
        {
            UpdateProgress();
            //延迟一帧 否则会比子任务提前完成
            if (CheckAllFinish())
            {
                await TimerComponent.Instance.WaitFrameAsync();
                SetResult();
            }
        }

        private void UpdateProgress()
        {
            float totalProgress = 0;
            for (var index = 0; index < _allProgress.Count; index++)
            {
                var progressResult = _allProgress[index];
                if (progressResult.IsDone)
                {
                    totalProgress += 1;
                }
            }

            Progress = totalProgress / _allProgress.Count;
        }

        public override void Clear()
        {
            base.Clear();
            foreach (var asyncResult in _allProgress)
            {
                ReferencePool.Free(asyncResult);
            }
            _allProgress.Dispose();
            _allProgress = null;
            progressFinishState.Dispose();
            progressFinishState = null;
            isAllDone = false;
        }
    }
    
    public class MulProgressResult : ProgressResult<float>
    {
        private RecyclableList<bool> progressFinishState;
        private RecyclableList<IProgressResult<float>> _allProgress;
        public override bool IsDone
        {
            get
            {
                if (isAllDone)
                {
                    RaiseFinish();
                    return true;
                }
                return _allProgress.Count <= 0 || base.IsDone;
            }
        }

        private bool isAllDone = false;

        private MulProgressResult()
        {
        }

        public static MulProgressResult Create([CallerMemberName]string debugName = "",bool cancelable = true,bool isFromPool = false, params IProgressResult<float>[] allProgress)
        {
            var result = isFromPool ? ReferencePool.Allocate<MulProgressResult>() : new MulProgressResult();
            result._allProgress = RecyclableList<IProgressResult<float>>.Create();
            result.progressFinishState = RecyclableList<bool>.Create();
            result.OnCreate(debugName, cancelable, isFromPool);
            result.AddAsyncResult(allProgress);
            return result;
        }

        public void AddAsyncResult(IProgressResult<float> progressResult)
        {
            if (progressResult == null) return;
            _allProgress.Add(progressResult);
            progressFinishState.Add(progressResult.IsDone);
            SetSubProgressCb(progressResult);
            CheckAllFinish();
        }

        public void AddAsyncResult(IEnumerable<IProgressResult<float>> progressResults)
        {
            foreach (var progressResult in progressResults)
            {
                AddAsyncResult(progressResult);
            }
        }

        private void SetSubProgressCb(IProgressResult<float> progressResult)
        {
            if (progressResult.IsDone) return;
            progressResult.Callbackable().OnProgressCallback((_ => RaiseOnProgressCallback(0)));
            progressResult.Callbackable().OnCallback(_ =>
            {
                if (CheckAllFinish())
                {
                    RaiseFinish();
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
            for (int i = 0; i < _allProgress.Count; i++)
            {
                var progressResult = _allProgress[i];
                if (!progressFinishState[i] && progressResult.IsDone)
                {
                    progressFinishState[i] = true;
                }
            }

            for (int i = 0; i < progressFinishState.Count; i++)
            {
                if (!progressFinishState[i])
                {
                    isAllDone = false;
                    return false;
                }
            }

            isAllDone = true;
            return true;
        }

        private async void RaiseFinish()
        {
            StringBuilder sb = null;
            foreach (var progressResult in _allProgress)
            {
                if (progressResult.Exception == null) continue;
                if (sb == null) sb = new StringBuilder();
                sb.AppendLine(progressResult.Exception.ToString());
            }

            //延迟一帧 否则会比子任务提前完成
            await TimerComponent.Instance.WaitFrameAsync();
            if (sb != null)
            {
                SetException(sb.ToString());
            }
            else
            {
                SetResult();
            }
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

        public override void Clear()
        {
            base.Clear();
            foreach (var asyncResult in _allProgress)
            {
                ReferencePool.Free(asyncResult);
            }

            progressFinishState.Dispose();
            progressFinishState = null;
            _allProgress.Dispose();
            _allProgress = null;
            isAllDone = false;
        }
    }
}