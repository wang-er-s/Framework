using System;

using Framework.Asynchronous;
using JetBrains.Annotations;
using IAsyncResult = Framework.Asynchronous.IAsyncResult;

namespace Framework.Execution
{
    public interface IScheduledExecutor : IDisposable
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
        IAsyncResult<TResult> Schedule<TResult>(Func<TResult> command, long delay);

        /// <summary>
        /// Creates and executes a task that becomes enabled after the given delay.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        IAsyncResult<TResult> Schedule<TResult>(Func<TResult> command, TimeSpan delay);

        /// <summary>
        /// Creates and executes a one-shot action that becomes enabled after the given delay.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delay">millisecond</param>
        /// <returns></returns>
        Asynchronous.IAsyncResult Schedule(Action command, long delay);

        /// <summary>
        /// Creates and executes a one-shot action that becomes enabled after the given delay.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        Asynchronous.IAsyncResult Schedule(Action command, TimeSpan delay);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given period; that is executions will commence after initialDelay then initialDelay+period, then initialDelay + 2 * period, and so on.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="initialDelay">millisecond</param>
        /// <param name="period">millisecond</param>
        /// <returns></returns>
        Asynchronous.IAsyncResult ScheduleAtFixedRate(Action command, long initialDelay, long period);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given period; that is executions will commence after initialDelay then initialDelay+period, then initialDelay + 2 * period, and so on.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="initialDelay"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        Asynchronous.IAsyncResult ScheduleAtFixedRate(Action command, TimeSpan initialDelay, TimeSpan period);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given period; that is executions will commence after initialDelay then initialDelay+period, then initialDelay + 2 * period, and so on.
        /// </summary>
        /// <param name="fixedUpdateCommand">call per fixed rate</param>
        /// <param name="endCommand">end call</param>
        /// <param name="initialDelay">millisecond</param>
        /// <param name="period">millisecond</param>
        /// <param name="duration">millisecond</param>
        /// <returns></returns>
        Asynchronous.IAsyncResult ScheduleAtFixedRate(Action<long> fixedUpdateCommand,
            [CanBeNull] Action endCommand = null, long? initialDelay = null, long? period = null, long? duration = null);

        /// <summary>
        /// Creates and executes a periodic action that becomes enabled first after the given initial delay, and subsequently with the given period; that is executions will commence after initialDelay then initialDelay+period, then initialDelay + 2 * period, and so on.
        /// </summary>
        /// <param name="fixedUpdateCommand">call per fixed rate</param>
        /// <param name="endCommand">end call</param>
        /// <param name="initialDelay"></param>
        /// <param name="period"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        Asynchronous.IAsyncResult ScheduleAtFixedRate(Action<long> fixedUpdateCommand,
            [CanBeNull] Action endCommand, TimeSpan? initialDelay, TimeSpan? period, TimeSpan? duration);

    }
}
