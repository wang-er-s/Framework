using System;
using Framework;
using UnityEngine;

public class GameLoop : MonoSingleton<GameLoop>
{
    public event Action OnUpdate;
    public event Action OnFixedUpdate;

    private void Update()
    {
        OnUpdate?.Invoke();
    }

    private void FixedUpdate()
    {
        OnFixedUpdate?.Invoke();
    }
}