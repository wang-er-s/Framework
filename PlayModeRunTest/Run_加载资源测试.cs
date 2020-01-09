using System.Collections;
using System.Collections.Generic;
using System.IO;
using AD;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class 测试加载资源
    {
        
        private string RandomFilePath()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
            return directoryInfo.GetFiles()[0].FullName;
        } 
        
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator AdTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
