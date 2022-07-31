using UnityEngine;

namespace Framework
{
    public interface IInputDevice
    {
        bool CanUse();
        float GetAxis(string name);
        void Update();
    }
}