using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nine
{
    public class MonoObjectPool<T> : Pool<T> where T : MonoBehaviour
    {

        private readonly Action<T> mOnHideMethod;
        private readonly Action<T> mOnShowMethod;
        private readonly Func<Object> mCreateMehtod;
        private Object obj;

        public MonoObjectPool(Func<Object> createMethod,Action<T> onShowMethod = null, Action<T> onHideMethod = null, int initCount = 0)
        {
            mOnHideMethod = onHideMethod;
            mCreateMehtod = createMethod;
            mOnShowMethod = onShowMethod;
            mFactory = new CustomFactory<T>(Create);
            for (int i = 0; i < initCount; i++)
            {
                mCacheStack.Push(mFactory.Create());
            }
        }

        public override T Spawn()
        {
            var result = base.Spawn();
            mOnShowMethod?.Invoke(result);
            result.Show();
            return result;
        }

        private T Create()
        {
            if (obj == null)
                obj = mCreateMehtod();
            return ((Object.Instantiate(obj)) as GameObject).GetComponent<T>();
        }

        public override bool DeSpawn(T obj)
        {
            obj.Hide();
            mOnHideMethod?.Invoke(obj);
            mCacheStack.Push(obj);
            return true;
        }
    }
}
