using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework
{

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class ViewportHandler : MonoBehaviour
    {
        #region FIELDS

        [ShowIf("@GetComponent<Camera>().orthographic")] [SerializeField]
        private float UnitsSize = 16; // size of your scene in unity units

        [ShowIf("@!GetComponent<Camera>().orthographic")] [SerializeField]
        private Resolution OriginResolution = new Resolution(1920, 1080);

        [SerializeField] private Constraint constraint = Constraint.Portrait;

        [SerializeField] [ShowIf("@!GetComponent<Camera>().orthographic")] [LabelText("距离相机的深度")]
        private float Depth;

        public static ViewportHandler Instance;
        private new Camera camera;
        private float originFov;

        #endregion

        #region PROPERTIES

        public float Width { get; private set; }

        public float Height { get; private set; }

        // helper points:
        public Vector3 BottomLeft { get; private set; }

        public Vector3 TopRight { get; private set; }

        #endregion

        #region METHODS

        private void Awake()
        {
            camera = GetComponent<Camera>();
            originFov = camera.fieldOfView;
            Instance = this;
            ComputeResolution();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                ComputeResolution();
#endif
        }

        private void ComputeResolution()
        {
            if (camera.orthographic)
            {
                float leftX = 0, rightX = 0, topY = 0, bottomY = 0;
                if (constraint == Constraint.Landscape)
                {
                    camera.orthographicSize = 1f / camera.aspect * UnitsSize / 2f;
                }
                else
                {
                    camera.orthographicSize = UnitsSize / 2f;
                }

                Height = 2f * camera.orthographicSize;
                Width = Height * camera.aspect;

                float cameraX = camera.transform.position.x;
                float cameraY = camera.transform.position.y;

                leftX = cameraX - Width / 2;
                rightX = cameraX + Width / 2;
                topY = cameraY + Height / 2;
                bottomY = cameraY - Height / 2;

                BottomLeft = new Vector3(leftX, bottomY, 0);
                TopRight = new Vector3(rightX, topY, 0);
            }
            else
            {
                float scale = 0;

                Width = Screen.width;
                Height = Screen.height;
                if (constraint == Constraint.Landscape)
                {
                    scale = Height / (OriginResolution.Height * 1.0f / OriginResolution.Width) / Width;
                }

                camera.fieldOfView = originFov * scale;
                float cameraZ = camera.transform.position.z;
                BottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, Depth) - new Vector3(0, 0, cameraZ));
                TopRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Depth)) -
                           new Vector3(0, 0, cameraZ);
            }
        }

        #endregion

        [Serializable]
        private enum Constraint
        {
            Landscape,
            Portrait
        }

        [Serializable]
        private struct Resolution
        {
            public int Height;
            public int Width;

            public Resolution(int height, int width)
            {
                Height = height;
                Width = width;
            }
        }
    }
}