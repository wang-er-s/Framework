using System;

namespace Framework.Asynchronous
{
    public class ImmutableAsyncResult : AsyncResult
    {
        public ImmutableAsyncResult() : base(false)
        {
            this.SetResult(null);
        }

        public ImmutableAsyncResult(object result) : base(false)
        {
            this.SetResult(result);
        }

        public ImmutableAsyncResult(Exception exception) : base(false)
        {
            this.SetException(exception);
        }
    }

    public class ImmutableAsyncResult<TResult> : AsyncResult<TResult>
    {
        public ImmutableAsyncResult() : base(false)
        {
            this.SetResult(default);
        }

        public ImmutableAsyncResult(TResult result) : base(false)
        {
            this.SetResult(result);
        }

        public ImmutableAsyncResult(Exception exception) : base(false)
        {
            this.SetException(exception);
        }
    }
}
