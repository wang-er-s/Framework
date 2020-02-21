using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class MonoObjectPool<T> : Pool<T> where T : MonoBehaviour
    {

        private readonly Action<T> _onHideMethod;
        private readonly Action<T> _onShowMethod;
        private readonly Func<Object> _createMehtod;
        private Object _obj;

        public MonoObjectPool(Func<Object> createMethod,Action<T> onShowMethod = null, Action<T> onHideMethod = null, int initCount = 0)
        {
            _onHideMethod = onHideMethod;
            _createMehtod = createMethod;
            _onShowMethod = onShowMethod;
            _factory = new CustomFactory<T>(Create);
            for (int i = 0; i < initCount; i++)
            {
                _cacheStack.Push(_factory.Create());
            }
        }

        public override T Spawn()
        {
            var result = base.Spawn();
            _onShowMethod?.Invoke(result);
            result.Show();
            return result;
        }

        private T Create()
        {
            if (_obj == null)
                _obj = _createMehtod();
            return (Object.Instantiate(_obj) as GameObject)?.GetComponent<T>();
        }

        public override bool DeSpawn(T obj)
        {
            obj.Hide();
            _onHideMethod?.Invoke(obj);
            _cacheStack.Push(obj);
            return true;
        }
    }
}
