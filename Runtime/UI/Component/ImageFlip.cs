using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    [RequireComponent(typeof(RectTransform), typeof(Graphic)), DisallowMultipleComponent]
    public class ImageFlip : MonoBehaviour, IMeshModifier
    {
        //仿照NGUI的flip效果，添加了这四种枚举，对应四种翻转效果
        public enum FlipType
        {
            Nothing,
            Horizontally,
            Vertically,
            Both,
        }

        [SerializeField] private FlipType _type = FlipType.Nothing;

        public FlipType flipType
        {
            get => _type;
            set
            {
                _type = value;
                RefreshMesh();
            }
        }

        //当脚本第一次加载，或者值发生变化的时候才会调用(仅Editor下)
        protected void OnValidate()
        {
            RefreshMesh();
        }

        private void RefreshMesh()
        {
            GetComponent<Graphic>().SetVerticesDirty();
        }

        public void ModifyMesh(Mesh mesh)
        {
            
        }

        public void ModifyMesh(VertexHelper verts)
        {
            if(_type == FlipType.Nothing) return;
            RectTransform rt = transform as RectTransform;
            for (int i = 0; i < verts.currentVertCount; ++i)
            {
                UIVertex uiVertex = new UIVertex();
                verts.PopulateUIVertex(ref uiVertex, i);

                // Modify positions
                float uvX = uiVertex.position.x;
                float uvY = uiVertex.position.y;
                if (_type == FlipType.Horizontally || _type == FlipType.Both)
                {
                    uvX = 2 * rt.rect.center.x - uiVertex.position.x;
                }
                if (_type == FlipType.Vertically || _type == FlipType.Both)
                {
                    uvY = 2 * rt.rect.center.y - uiVertex.position.y;
                }
                uiVertex.position = new Vector3(uvX, uvY, uiVertex.position.z);
                // Apply
                verts.SetUIVertex(uiVertex, i);
            }
        }
    }
}