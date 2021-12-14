using UnityEngine;

namespace Framework
{
    public interface IInputDevice
    {
        bool CanUse();
        bool GetButton(string name);
        bool GetButtonDown(string name);
        bool GetButtonUp(string name);
        float GetAxis(string name, bool b);
    }
}