using System;
using System.Collections;
using System.Collections.Generic;

namespace Framework.Helper
{
    public class FilterEnumerator : IEnumerator
    {
        private readonly IEnumerator _enumerator;
        private readonly Predicate<object> _match;

        public FilterEnumerator(IEnumerator enumerator, Predicate<object> match)
        {
            _enumerator = enumerator;
            _match = match;
        }

        public object Current { get; private set; }

        public bool MoveNext()
        {
            while (_enumerator.MoveNext())
            {
                var current = _enumerator.Current;
                if (!_match(current))
                    continue;

                Current = current;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _enumerator.Reset();
        }
    }

    public class FilterEnumerator<T> : IEnumerator<T>
    {
        private IEnumerator<T> _enumerator;
        private Predicate<T> _match;

        public FilterEnumerator(IEnumerator<T> enumerator, Predicate<T> match)
        {
            Current = default;
            _enumerator = enumerator;
            _match = match;
        }

        public T Current { get; private set; }

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            while (_enumerator.MoveNext())
            {
                var current = _enumerator.Current;
                if (!_match(current))
                    continue;

                Current = current;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            _enumerator.Reset();
        }

        #region IDisposable Support

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                Reset();
                _enumerator = null;
                _match = null;
                _disposedValue = true;
            }
        }

        ~FilterEnumerator()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
