using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Framework
{
    public sealed class Unit : Entity,IAwakeSystem<string>,IAwakeSystem<Transform>, IAwakeSystem<bool,string>, IAwakeSystem<bool,Transform>,IAwakeSystem<bool,bool,string>, IRendererUpdateSystem, IDestroySystem
    {
        private Transform transform;
        public Transform Transform
        {
            get => transform;
            set
            {
                if (transform == value)
                {
                    return;
                }

                if (transform != null)
                {
                    transform.gameObject.GetOrAddComponent<GoConnectedUnitId>().SetUnitId(0);
#if UNITY_EDITOR
                    transform.gameObject.GetOrAddComponent<EditorVisibleUnit>().SetUnit(null);
#endif
                }

                transform = value;
                if (transform != null)
                {
                    transform.gameObject.GetOrAddComponent<GoConnectedUnitId>().SetUnitId(Id);
#if UNITY_EDITOR
                    transform.gameObject.GetOrAddComponent<EditorVisibleUnit>().SetUnit(this);
#endif
                } 
            }
        }

        public GameObject GameObject
        {
            get => transform.gameObject;
            set
            {
                Transform = value.transform;
            }
        }
        
        public IAsyncResult<GameObject> LoadTransAsync { get; private set; }
        // 是否从transform同步位置
        private bool syncFromTrans;
        private bool isFromPool;
        
        private float3 position; //坐标
        
        [ShowInInspector]
        public float3 Position
        {
            get => position;
            set
            {
                position = value;
                if (!syncFromTrans && Transform != null)
                {
                    Transform.position = position;
                }
            }
        }

        public float3 Forward
        {
            get => math.mul(Rotation, math.forward());
            set => Rotation = quaternion.LookRotation(value, math.up());
        }

        private quaternion rotation;

        public quaternion Rotation
        {
            get => rotation;
            set
            {
                rotation = value;
                eulerAngle = rotation.EulerAngles();
                if (!syncFromTrans && Transform != null)
                {
                    Transform.rotation = rotation;
                }
            }
        }

        private float3 eulerAngle;

        [ShowInInspector]
        public float3 EulerAngle
        {
            get => eulerAngle;
            set
            {
                eulerAngle = value;
                rotation = quaternion.Euler(eulerAngle, math.RotationOrder.XYZ);
                if (!syncFromTrans && Transform != null)
                {
                    Transform.eulerAngles = eulerAngle;
                }
            }
        }

        private float3 scale;

        [ShowInInspector]
        public float3 Scale
        {
            get => scale;
            set
            {
                scale = value;
                if (!syncFromTrans && Transform != null)
                {
                    Transform.localScale = scale;
                }
            }
        }

        protected override string ViewName => $"{GetType().Name} ({Id})";

        public void Awake(bool fromPool, string path)
        {
            Awake(false, fromPool, path);
        }

        public void Awake(Transform go)
        {
            Awake(false, go);
        }

        public void Awake(bool syncFromTran,Transform trans)
        {
            syncFromTrans = syncFromTran;
            Transform = trans;
        }
        
        public void Awake(bool syncFromTran, bool isFromPool, string path)
        {
            syncFromTrans = syncFromTran;
            this.isFromPool = isFromPool;
            Load(path);
        }

        public void Awake(string path)
        {
            Awake(false,false, path);
        }
        
        public IAsyncResult<GameObject> Load(string path)
        {
            if (LoadTransAsync != null && !LoadTransAsync.IsDone)
            {
                LoadTransAsync.Cancel();
            }
            if (isFromPool)
            {
                LoadTransAsync = this.domain.GetComponent<PrefabPool>().Allocate(path);
                LoadTransAsync.Callbackable().OnCallback(OnAsyncLoadFinish);
            }
            else
            {
                LoadTransAsync = this.domain.GetComponent<ResComponent>().Instantiate(path);
                LoadTransAsync.Callbackable().OnCallback(OnAsyncLoadFinish);
            }

            return LoadTransAsync;
        }

        private async void OnAsyncLoadFinish(IAsyncResult<GameObject> asyncResult)
        {
            if (asyncResult.Exception != null)
            {
                Log.Error(asyncResult.Exception);
                return;
            }

            if (this.IsDisposed || asyncResult.IsCancelled)
            {
                Object.Destroy(asyncResult.Result);
            }

            Transform = asyncResult.Result.transform;
            Transform.position = position;
            Transform.rotation = rotation;
            Transform.forward = Forward;
        }

        public override string ToString()
        {
            if (Transform == null)
            {
                return base.ToString();
            }
            return $"{Transform.name}";
        }

        public void OnDestroy()
        {
            if (isFromPool)
            {
                DestroyGameObjectToPool();
            }
            else
            {
                DestroyGameObject();
            }
        }

        public void RenderUpdate(float deltaTime)
        {
            if(!syncFromTrans) return;
            if(Transform == null) return;
            if(!Transform.hasChanged) return;
            Position = Transform.position;
            Rotation = Transform.rotation;
            Scale = Transform.localScale;
            Transform.hasChanged = false;
        }

        public void DestroyGameObject()
        {
            if (transform != null)
            {
                Object.Destroy(transform.gameObject);
                transform = null;
            }
        }
        
        public void DestroyGameObjectToPool()
        {
            if (transform != null)
            {
                if(!domain.IsDisposed) domain.GetComponent<PrefabPool>().Free(transform.gameObject);
                transform = null;
            }
        }
    }
}