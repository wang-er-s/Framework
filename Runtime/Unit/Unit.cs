using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Framework
{
    public sealed class Unit : Entity, IAwakeSystem<bool>, IAwakeSystem<bool,string>, IAwakeSystem<bool,Transform>, IRendererUpdateSystem
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
                    transform.gameObject.GetOrAddComponent<GoConnectedUnitId>().SetUnitId(parent.Id);
#if UNITY_EDITOR
                    transform.gameObject.GetOrAddComponent<EditorVisibleUnit>().SetUnit(parent as Unit);
#endif
                } 
            }
        }
        // 是否从transform同步位置
        private bool syncFromTrans;
        
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

        public void Awake(bool syncFromTran,string path)
        {
            Awake(syncFromTran);
            this.domain.GetComponent<PrefabPool>().Allocate(path).Callbackable().OnCallback((r) =>
            {
                if (r.Exception != null)
                {
                    Log.Error(r.Exception);
                    return;
                }
                if (this.IsDisposed)
                {
                    Object.Destroy(r.Result);
                }
                Awake(r.Result.transform);
            });
        }

        public void Awake(bool syncFromTran,Transform trans)
        {
            Awake(syncFromTran);
            Transform = trans;
        }

        public void Awake(bool syncFromTran)
        {
            syncFromTrans = syncFromTran;
        }

        public override string ToString()
        {
            if (Transform == null)
            {
                return base.ToString();
            }
            return $"{Transform.name}\n {base.ToString()}";
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
                domain.GetComponent<PrefabPool>().Free(transform.gameObject);
            }
        }
    }
}