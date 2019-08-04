using SF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPool : MonoBehaviour
{
    private static MonoObjectPool<TestPool> pool = new MonoObjectPool<TestPool>(Create);

    public static TestPool Get()
    {
        return pool.Spawn();
    }

    private static Object Create()
    {
        return Resources.Load("Sphere");
    }
}
