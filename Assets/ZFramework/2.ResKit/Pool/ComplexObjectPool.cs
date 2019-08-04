using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace SF
{

    public enum ComplexPoolObjectType
    {
        Boom,
    }

    public struct ComplexObjPoolItem
    {
        public ComplexPoolObjectType Type;

        public string Path;

        public GameObject GameObject;

        public ComplexObjPoolItem ( ComplexPoolObjectType type, string path )
        {
            Type       = type;
            Path       = path;
            GameObject = Resources.Load<GameObject> ( path );
        }
    }

    public abstract class IObjPool : MonoBehaviour
    {
        public abstract IObjPool Init ( params object[] value );

        public abstract IObjPool OnShow ( params object[] value );

        public abstract IObjPool OnHide ( params object[] value );
    }

    public class ComplexObjectPool : Singleton<ComplexObjectPool>
    {

        private ComplexObjectPool ()
        {

            objectPoolDic = new Dictionary<ComplexPoolObjectType, Queue<IObjPool>> ();

            baseObjectDic = new Dictionary<ComplexPoolObjectType, ComplexObjPoolItem> ();

        }

        /// <summary>
        /// 存放物品类型和对象池的字典
        /// </summary>
        private Dictionary<ComplexPoolObjectType, Queue<IObjPool>> objectPoolDic;

        /// <summary>
        /// 存放对象的模版
        /// </summary>
        private Dictionary<ComplexPoolObjectType, ComplexObjPoolItem> baseObjectDic;

        /// <summary>
        /// 初始化所需对象的对象池
        /// </summary>
        /// <param name="type">对象的类型</param>
        /// <param name="baseObj">物体</param>
        /// <param name="count">需要初始化的数量</param>
        /// <returns>是否初始化成功</returns>
        public bool Init ( ComplexPoolObjectType type, ComplexObjPoolItem poolItem, int count, params object[] value )
        {
            if ( objectPoolDic.ContainsKey ( type ) )
            {
                if ( objectPoolDic[ type ].Count < count )
                {
                    for ( int i = 0; i < count - objectPoolDic[ type ].Count; i++ )
                    {
                        objectPoolDic[ type ].Enqueue ( CreateObj ( type, value ).GetComponent<IObjPool> () );
                    }
                }

                return true;
            }

            Queue<IObjPool> tmpQueue = new Queue<IObjPool> ();
            for ( int i = 0; i < count; i++ )
            {
                tmpQueue.Enqueue ( CreateObj ( type, value ) );
            }

            if ( tmpQueue.Count != count )
            {
                Debug.Log ( "没有生成足够的数量！" );
                return false;
            }

            objectPoolDic.Add ( type, tmpQueue );
            if ( !baseObjectDic.ContainsKey ( type ) )
                baseObjectDic.Add ( type, poolItem );
            return true;
        }

        /// <summary>
        /// 获得物体
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IObjPool Pop ( ComplexPoolObjectType type, params object[] value )
        {
            if ( !baseObjectDic.ContainsKey ( type ) )
            {
                Debug.Log ( "此类型还没创建，快去初始化吧！" );
                return null;
            }

            if ( objectPoolDic[ type ].Count <= 0 )
            {
                return CreateObj ( type, value ).OnShow ( value );
            }

            return objectPoolDic[ type ].Dequeue ().OnShow ( value );
        }

        /// <summary>
        /// 存入物体
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        public void Push ( ComplexPoolObjectType type, IObjPool obj, params object[] value )
        {
            if ( !objectPoolDic.ContainsKey ( type ) )
            {
                Debug.Log ( "此类型还没创建，快去初始化吧！" );
                return;
            }

            obj.OnHide ( value );
            objectPoolDic[ type ].Enqueue ( obj );
        }

        /// <summary>
        /// 获得物品剩余数量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public int GetRemainCount ( ComplexPoolObjectType type )
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
            foreach ( KeyValuePair<ComplexPoolObjectType, Queue<IObjPool>> item in objectPoolDic )
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
        /// 创建一个物体并初始化
        /// </summary>
        private IObjPool CreateObj ( ComplexPoolObjectType type, params object[] value )
        {
            IObjPool temp = Object.Instantiate ( baseObjectDic[ type ].GameObject ).GetComponent<IObjPool> ();
            temp.Init ( value );
            return temp;
        }


    }
}

