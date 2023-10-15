using System;

namespace Framework
{
    public class CoroutineLock : IDisposable
    {
        private int type;
        private long key;
        private int level;

        public static CoroutineLock Create(int type, long k, int count)
        {
            CoroutineLock coroutineLock = ReferencePool.Allocate<CoroutineLock>();
            coroutineLock.type = type;
            coroutineLock.key = k;
            coroutineLock.level = count;
            return coroutineLock;
        }

        public void Dispose()
        {
            CoroutineLockComponent.Instance.RunNextCoroutine(this.type, this.key, this.level + 1);

            this.type = CoroutineLockType.None;
            this.key = 0;
            this.level = 0;

            ReferencePool.Free(this);
        }
    }
}