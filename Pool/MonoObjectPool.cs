using System;
using Framework.Pool.Factory;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Pool
{
    public class MonoObjectPool<T> : Pool<T> where T : MonoBehaviour
    {

        private readonly Action<T> _onHideMethod;
        private readonly Action<T> _onShowMethod;
        private readonly Func<Object> _createMethod;
        private Object _obj;

        public MonoObjectPool(Func<Object> createMethod, Action<T> onShowMethod = null, Action<T> onHideMethod = null,
            int initCount = 0, int? maxCount = null)
        {
            _onHideMethod = onHideMethod;
            _createMethod = createMethod;
            _onShowMethod = onShowMethod;
            MaxCount = maxCount ?? MaxCount;
            Factory = new CustomFactory<T>(Create);
            for (int i = 0; i < initCount; i++)
            {
                CacheStack.Push(Factory.Create());
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
                _obj = _createMethod();
            return (Object.Instantiate(_obj) as GameObject)?.GetComponent<T>();
        }

        public override bool DeSpawn(T obj)
        {
            obj.Hide();
            _onHideMethod?.Invoke(obj);
            CacheStack.Push(obj);
            return true;
        }
    }
}
