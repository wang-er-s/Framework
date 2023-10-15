using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Framework
{
    public static class MathHelper
    {
        private struct MinAndMax
        {
            public int min;
            public int max;

            public MinAndMax(int min, int max)
            {
                this.min = min;
                this.max = max;
            }
        }

        /// <summary>
        /// 根据概率得到随机的索引 索引是从0开始
        /// 传入的是百分之的概率 50代表百分之50
        /// [20,30,50] 总和需要是100
        /// </summary>
        public static int GetRandomIndex(params int[] muls)
        {
            List<MinAndMax> tempList = new List<MinAndMax>(muls.Length);
            for (int i = 0; i < muls.Length; i++)
            {
                int min = 0, max = 0;
                for (int j = 0; j < i; j++)
                {
                    min += muls[j];
                }

                max = min + muls[i];
                tempList.Add(new MinAndMax(min, max));
            }

            int rand = Random.Range(1, 101);
            foreach (MinAndMax item in tempList)
            {
                if (rand >= item.min && rand <= item.max)
                    return tempList.IndexOf(item);
            }

            return -1;
        }

        /// <summary>
        /// 线性贝塞尔曲线，根据T值，计算贝塞尔曲线上面相对应的点
        /// </summary>
        /// <param name="t"></param>T值
        /// <param name="p0"></param>起始点
        /// <param name="p1"></param>控制点
        /// <returns></returns>根据T值计算出来的贝赛尔曲线点
        private static Vector3 CalculateLineBezierPoint(float t, Vector3 p0, Vector3 p1)
        {
            float u = 1 - t;
            Vector3 p = u * p0;
            p += t * p1;
            return p;
        }

        /// <summary>
        /// 二次贝塞尔曲线，根据T值，计算贝塞尔曲线上面相对应的点
        /// </summary>
        /// <param name="t"></param>T值
        /// <param name="p0"></param>起始点
        /// <param name="p1"></param>控制点
        /// <param name="p2"></param>目标点
        /// <returns></returns>根据T值计算出来的贝赛尔曲线点
        private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            return p;
        }

        /// <summary>
        /// 三次贝塞尔曲线，根据T值，计算贝塞尔曲线上面相对应的点
        /// </summary>
        /// <param name="t">插量值</param>
        /// <param name="p0">起点</param>
        /// <param name="p1">控制点1</param>
        /// <param name="p2">控制点2</param>
        /// <param name="p3">尾点</param>
        /// <returns></returns>
        private static Vector3 CalculateThreePowerBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float ttt = tt * t;
            float uuu = uu * u;

            Vector3 p = uuu * p0;
            p += 3 * t * uu * p1;
            p += 3 * tt * u * p2;
            p += ttt * p3;

            return p;
        }


        /// <summary>
        /// 获取存储贝塞尔曲线点的数组
        /// </summary>
        /// <param name="startPoint"></param>起始点
        /// <param name="controlPoint"></param>控制点
        /// <param name="endPoint"></param>目标点
        /// <param name="segmentNum"></param>采样点的数量
        /// <returns></returns>存储贝塞尔曲线点的数组
        public static RecyclableList<Vector3> GetLineBezierList(Vector3 startPoint, Vector3 endPoint,
            int segmentNum = 10)
        {
            RecyclableList<Vector3> path = RecyclableList<Vector3>.Create();
            path.Add(startPoint);
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                Vector3 pixel = CalculateLineBezierPoint(t, startPoint, endPoint);
                path.Add(pixel);
            }

            return path;
        }

        /// <summary>
        /// 获取存储的二次贝塞尔曲线点的数组
        /// </summary>
        /// <param name="startPoint"></param>起始点
        /// <param name="controlPoint"></param>控制点
        /// <param name="endPoint"></param>目标点
        /// <param name="segmentNum"></param>采样点的数量
        /// <returns></returns>存储贝塞尔曲线点的数组
        public static RecyclableList<Vector3> GetTwoBezierList(Vector3 startPoint, Vector3 controlPoint,
            Vector3 endPoint, int segmentNum = 10)
        {
            RecyclableList<Vector3> path = RecyclableList<Vector3>.Create();
            path.Add(startPoint);
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                Vector3 pixel = CalculateCubicBezierPoint(t, startPoint,
                    controlPoint, endPoint);
                path.Add(pixel);
            }

            return path;
        }

        /// <summary>
        /// 获取存储的三次贝塞尔曲线点的数组
        /// </summary>
        /// <param name="startPoint"></param>起始点
        /// <param name="controlPoint1"></param>控制点1
        /// <param name="controlPoint2"></param>控制点2
        /// <param name="endPoint"></param>目标点
        /// <param name="segmentNum"></param>采样点的数量
        /// <returns></returns>存储贝塞尔曲线点的数组
        public static RecyclableList<Vector3> GetThreePowerBezierList(Vector3 startPoint, Vector3 controlPoint1,
            Vector3 controlPoint2, Vector3 endPoint, int segmentNum = 10)
        {
            RecyclableList<Vector3> path = RecyclableList<Vector3>.Create();
            path.Add(startPoint);
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                Vector3 pixel = CalculateThreePowerBezierPoint(t, startPoint,
                    controlPoint1, controlPoint2, endPoint);
                path.Add(pixel);
            }

            return path;
        }
    }
}