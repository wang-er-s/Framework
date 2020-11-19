using System;

namespace Framework.UI.Core
{
    public interface IAnimation
    {
        IAnimation OnStart(Action onStart);

        IAnimation OnEnd(Action onEnd);

        IAnimation Play();

    }
}