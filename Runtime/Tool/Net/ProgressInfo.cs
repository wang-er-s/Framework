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

using System;
using System.Globalization;
using Framework.Asynchronous;

namespace Framework.Net
{
    public enum UNIT
    {
        BYTE,
        KB,
        MB,
        GB
    }

    public class ProgressInfo : IMulProgress
    {
        private long _totalSize = 0;
        private long _completedSize = 0;

        private float _speed = 0f;
        private long _lastTime = -1;
        private long _lastValue = -1;
        private long _lastTime2 = -1;
        private long _lastValue2 = -1;

        public ProgressInfo() : this(0, 0)
        {
        }

        public ProgressInfo(long totalSize, long completedSize)
        {
            this._totalSize = totalSize;
            this._completedSize = completedSize;

            _lastTime = DateTime.UtcNow.Ticks / 10000;
            _lastValue = this._completedSize;

            _lastTime2 = _lastTime;
            _lastValue2 = _lastValue;
        }

        public long TotalSize
        {
            get => this._totalSize;
            set => this._totalSize = value;
        }
        public long CompletedSize
        {
            get => this._completedSize;
            set
            {
                this._completedSize = value;
                this.OnUpdate();
            }
        }

        public int TotalCount { get; set; } = 0;

        public int CompletedCount { get; set; } = 0;

        private void OnUpdate()
        {
            long now = DateTime.UtcNow.Ticks / 10000;

            if ((now - _lastTime) >= 1000)
            {
                _lastTime2 = _lastTime;
                _lastValue2 = _lastValue;

                this._lastTime = now;
                this._lastValue = this._completedSize;
            }

            float dt = (now - _lastTime2) / 1000f;
            _speed = (this._completedSize - this._lastValue2) / dt;
        }

        public virtual float Progress
        {
            get
            {
                if (this._totalSize <= 0)
                    return 0f;

                return this._completedSize / (float)this._totalSize;
            }
        }

        public virtual float GetTotalSize(UNIT unit = UNIT.BYTE)
        {
            switch (unit)
            {
                case UNIT.KB:
                    return this._totalSize / 1024f;
                case UNIT.MB:
                    return this._totalSize / 1048576f;
                case UNIT.GB:
                    return this._totalSize / 1073741824f;
                default:
                    return (float)this._totalSize;
            }
        }

        public virtual float GetCompletedSize(UNIT unit = UNIT.BYTE)
        {
            switch (unit)
            {
                case UNIT.KB:
                    return this._completedSize / 1024f;
                case UNIT.MB:
                    return this._completedSize / 1048576f;
                case UNIT.GB:
                    return this._completedSize / 1073741824f;
                default:
                    return (float)this._completedSize;
            }
        }

        public virtual float GetSpeed(UNIT unit = UNIT.BYTE)
        {
            switch (unit)
            {
                case UNIT.KB:
                    return _speed / 1024f;
                case UNIT.MB:
                    return _speed / 1048576f;
                case UNIT.GB:
                    return _speed / 1073741824f;
                default:
                    return _speed;
            }
        }

        public override string ToString()
        {
            return $"{GetCompletedSize()} / {GetTotalSize()} ";
        }

        public float Current => _completedSize;
        public float Total => _totalSize;
    }
}