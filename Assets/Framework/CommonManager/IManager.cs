using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.CommonManager
{
    public interface IManager
    {
        void Init(params object[] para);
        void Update(float deltaTime);
    }
}