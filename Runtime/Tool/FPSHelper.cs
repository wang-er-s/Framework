using System.Text;
using UnityEngine;

namespace Framework
{
    /// <summary>
    /// 显示游戏帧率/占用内存等信息的助手类
    /// </summary>
    public class FPSHelper : MonoBehaviour
    {
        /// <summary>
        /// 计算的更新频率
        /// </summary>
        public float updateInterval = 0.5F;

        /// <summary>
        /// 用来保存时间间隔
        /// </summary>
        private float lastInterval;

        /// <summary>
        /// 记录帧数
        /// </summary>
        private int frames = 0;

        /// <summary>
        /// 记录帧率
        /// </summary>
        private float fps;

        #region 属性信息

        private Color ColorGreen = new Color(0, 1, 0);
        private Color ColorBlack = new Color(1, 1, 0);
        private Color ColorRed = new Color(1.0f, 0, 0);
        private Rect MemoryPosition = new Rect(Screen.width - 330, 0, 330, 300);
        private Rect GCBtnPosition = new Rect(Screen.width / 2 - 30, 0, 100, 50);
        private Rect FpsPosition = new Rect(100, 150, 300, 300);
        private StringBuilder stringBuilder;
        private float MBSize = 1024f * 1024f;

        #endregion

        void Start()
        {
            //Application.targetFrameRate=60;

            lastInterval = Time.realtimeSinceStartup;

            frames = 0;
            stringBuilder = new StringBuilder();
        }

        void OnGUI()
        {
            GUI.color = ColorGreen;
            GUI.skin.label.fontSize = 20;

            stringBuilder.Length = 0;
            //已经使用的内存
            stringBuilder.AppendFormat("MemoryAllocated:{0}\r\n",
                GetMemoryMB(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong()));
            //unity分配总内存
            stringBuilder.AppendFormat("MemoryReserved:{0}\r\n",
                GetMemoryMB(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong()));
            stringBuilder.AppendFormat("MemoryUnusedReserved:{0}\r\n",
                GetMemoryMB(UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong()));
            // 50mb以下属于正常值
            stringBuilder.AppendFormat("MonoHeapSize:{0}\r\n",
                GetMemoryMB(UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong()));
            stringBuilder.AppendFormat("MonoUsedSize:{0}",
                GetMemoryMB(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong()));
            GUI.Label(MemoryPosition, stringBuilder.ToString());

            if (GUI.Button(GCBtnPosition, "GC"))
            {
                CommonHelper.ClearMemory();
            }

            if (fps > 50)
            {
                GUI.color = ColorGreen;
            }
            else if (fps > 25)
            {
                GUI.color = ColorBlack;
            }
            else
            {
                GUI.color = ColorRed;
            }

            GUI.Label(FpsPosition, "FPS:" + fps.ToString("f2"));

        }

        private string GetMemoryMB(long curSize)
        {
            float mbSize = curSize / MBSize;
            return mbSize.ToString("f2") + "MB";
        }

        void Update()
        {
            ++frames;

            if (Time.realtimeSinceStartup > lastInterval + updateInterval)
            {
                fps = frames / (Time.realtimeSinceStartup - lastInterval);

                frames = 0;

                lastInterval = Time.realtimeSinceStartup;
            }
        }
    }
}