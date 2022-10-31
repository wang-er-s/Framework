using System;

namespace Framework
{
    public interface IAnimation
    {
        IAnimation OnStart(Action onStart);

        IAnimation OnEnd(Action onEnd);

        IAnimation Play();

    }
}