using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace ZFramework
{
    public enum PoolObjectType
    {
        Fx_Coin,

        Fx_CoinX2,

        Fx_Item
    }

    public class ObjectPool
    {

        #region 单例
        private static readonly object locker = new object ();

        private static ObjectPool instance;

        public static ObjectPool GetInstance
        {
            get
            {
                lock ( locker )
                {
                    if ( instance == null )
                    {
                        lock ( locker )
                        {
                            instance = new ObjectPool ();
                        }
                    }
                }

                return instance;
            }
        }

        private ObjectPool ()
        {
            objectPoolDic = new Dictionary<PoolObjectType, Queue<GameObject>> ();
            baseObjectDic = new Dictionary<PoolObjectType, SimpleObjPoolItem> ();
        }
        #endregion

        public struct SimpleObjPoolItem
        {
            public GameObject obj;

            public Transform parent;

            public SimpleObjPoolItem ( GameObject obj, Transform parent )
            {
                this.obj    = obj;
                this.parent = parent;
            }
        }

        /// <summary>
        /// 存放物品类型和对象池的字典
        /// </summary>
        private Dictionary<PoolObjectType, Queue<GameObject>> objectPoolDic;

        /// <summary>
        /// 存放对象的模版
        /// </summary>
        private Dictionary<PoolObjectType, SimpleObjPoolItem> baseObjectDic;

        /// <summary>
        /// 初始化所需对象的对象池
        /// </summary>
        /// <param name="type">对象的类型</param>
        /// <param name="baseObj">物体</param>
        /// <param name="count">需要初始化的数量</param>
        /// <returns>是否初始化成功</returns>
        public bool Init ( PoolObjectType type, GameObject baseObj, Transform parent, int count )
        {
            if ( objectPoolDic.ContainsKey ( type ) )
            {
                Debug.Log ( "此类型已经存在，去获得吧！" );
                return false;
            }

            Queue<GameObject> tmpQueue = new Queue<GameObject> ();
            for ( int i = 0; i < count; i++ )
            {
                tmpQueue.Enqueue ( CreateObj ( new SimpleObjPoolItem ( baseObj, parent ) ) );
            }

            if ( tmpQueue.Count != count )
            {
                Debug.Log ( "没有生成足够的数量！" );
                return false;
            }

            objectPoolDic.Add ( type, tmpQueue );
            baseObjectDic.Add ( type, new SimpleObjPoolItem ( baseObj, parent ) );
            return true;
        }

        /// <summary>
        /// 获得物体
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public GameObject Pop ( PoolObjectType type )
        {
            if ( !objectPoolDic.ContainsKey ( type ) )
            {
                Debug.Log ( "此类型还没创建，快去初始化吧！" );
                return null;
            }

            GameObject tmpObj;
            if ( objectPoolDic[ type ].Count <= 0 )
            {
                tmpObj = CreateObj ( baseObjectDic[ type ] );
                tmpObj.SetActive ( true );
                return tmpObj;
            }

            tmpObj = objectPoolDic[ type ].Dequeue ();
            tmpObj.SetActive ( true );
            return tmpObj;
        }

        /// <summary>
        /// 存入物体
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        public void Push ( PoolObjectType type, GameObject obj )
        {
            if ( !objectPoolDic.ContainsKey ( type ) )
            {
                Debug.Log ( "此类型还没创建，快去初始化吧！" );
                return;
            }

            obj.Hide ();
            objectPoolDic[ type ].Enqueue ( obj );
        }

        /// <summary>
        /// 获得物品剩余数量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetRemainCount ( PoolObjectType type )
        {
            if ( !objectPoolDic.ContainsKey ( type ) )
            {
                Debug.Log ( "此类型还没创建，快去初始化吧！" );
                return -1;
            }

            return objectPoolDic[ type ].Count;
        }

        /// <summary>
        /// 清理对象池
        /// </summary>
        public void Clean ()
        {
            int i;
            foreach ( KeyValuePair<PoolObjectType, Queue<GameObject>> item in objectPoolDic )
            {
                int count = item.Value.Count;
                for ( i = 0; i < count; i++ )
                {
                    Object.DestroyImmediate ( item.Value.Dequeue () );
                }
            }

            objectPoolDic.Clear ();
            baseObjectDic.Clear ();
        }

        /// <summary>
        /// 创建一个物体并把active设为false
        /// </summary>
        /// <param name="obj"></param>
        private GameObject CreateObj ( SimpleObjPoolItem obj )
        {
            GameObject tmpObj;
            tmpObj = Object.Instantiate ( obj.obj, obj.parent );
            tmpObj.SetActive ( false );
            return tmpObj;
        }

        public void CleatSomeOne ( PoolObjectType type )
        {
            if ( !baseObjectDic.ContainsKey ( type ) ) return;
            if ( !objectPoolDic.ContainsKey ( type ) ) return;
            foreach ( GameObject gameObject in objectPoolDic[ type ] )
            {
                if ( gameObject ) gameObject.DestroySelf ();
            }

            objectPoolDic.Remove ( type );
            baseObjectDic.Remove ( type );
        }

        ~ObjectPool ()
        {
            instance = null;
        }

    }

}

