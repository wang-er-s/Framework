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

namespace Framework
{
    public interface ITimerExecutor : IDisposable
    {
        /// <summary>
        /// Start the service.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the service.
        /// </summary>
        void Stop();

        /// <summary>
        /// Creates and executes a task that becomes enabled after the given delay.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        IAsyncResult<TResult> Delay<TResult>(Func<TResult> command, long delay);

        /// <summary>
        /// Creates and executes a task that becomes enabled after the given delay.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        IAsyncResult<TResult> Delay<TResult>(Func<TResult> command, TimeSpan delay);

        /// <summary>
        /// Creates and executes a one-shot action that becomes enabled after the given delay.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        IAsyncResult Delay(Action command, long delay);

        /// <summary>
        /// Creates and executes a one-shot action that becomes enabled after the given delay.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        IAsyncResult Delay(Action command, TimeSpan delay);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given period; that is executions will commence after initialDelay then initialDelay+period, then initialDelay + 2 * period, and so on.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="initialDelay">millisecond</param>
        /// <param name="period">millisecond</param>
        /// <returns></returns>
        IAsyncResult FixedRate(Action command, long initialDelay, long period);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given period; that is executions will commence after initialDelay then initialDelay+period, then initialDelay + 2 * period, and so on.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="initialDelay"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        IAsyncResult FixedRate(Action command, TimeSpan initialDelay, TimeSpan period);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given period; that is executions will commence after initialDelay then initialDelay+period, then initialDelay + 2 * period, and so on.
        /// </summary>
        /// <param name="fixedUpdateCommand">call per fixed rate</param>
        /// <param name="endCommand">end call</param>
        /// <param name="initialDelay">millisecond</param>
        /// <param name="period">millisecond</param>
        /// <param name="duration">millisecond</param>
        /// <returns></returns>
        IAsyncResult FixedRateAtDuration(Action<long> fixedUpdateCommand, Action endCommand = null, long? initialDelay = null, long? period = null, long? duration = null);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given period; that is executions will commence after initialDelay then initialDelay+period, then initialDelay + 2 * period, and so on.
        /// </summary>
        /// <param name="fixedUpdateCommand">call per fixed rate</param>
        /// <param name="endCommand">end call</param>
        /// <param name="initialDelay"></param>
        /// <param name="period"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        IAsyncResult FixedRateAtDuration(Action<long> fixedUpdateCommand, Action endCommand, TimeSpan? initialDelay, TimeSpan? period, TimeSpan? duration);

    }
}
