using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;
using System.Collections;
using Framework;

public static class ClassExtension
{
    public static void Example()
    {
        var simpleClass = new object();
        if (simpleClass.IsNull()) // simpleClass == null
        {
            // do sth
        }
        else if (simpleClass.IsNotNull()) // simpleClasss != null
        {
            // do sth
        }
    }

    public static bool IsNull<T>(this T selfObj) where T : class
    {
        return null == selfObj;
    }

    public static bool IsNotNull<T>(this T selfObj) where T : class
    {
        return null != selfObj;
    }
}

public static class GenericExtension
{
    public static void Example()
    {
        var typeName = GenericExtension.GetTypeName<string>();
        Debug.Log(typeName);
    }

    public static string GetTypeName<T>()
    {
        return typeof(T).ToString();
    }
}

public static class IEnumerableExtension
{
    public static void Example()
    {
        // Array
        var testArray = new int[] {1, 2, 3};
        testArray.ForEach(number => Debug.Log(number));

        // IEnumerable<T>
        IEnumerable<int> testIenumerable = new List<int> {1, 2, 3};
        testIenumerable.ForEach(number => Debug.Log(number));
        new Dictionary<string, string>().ForEach(
            keyValue => Debug.Log(string.Format("key:{0},value:{1}", keyValue.Key, keyValue.Value)));

        // testList
        var testList = new List<int> {1, 2, 3};
        testList.ForEach(number => Debug.Log(number));
        testList.ForEachReverse(number => Debug.Log(number));

        // merge
        var dictionary1 = new Dictionary<string, string> {{"1", "2"}};
        var dictionary2 = new Dictionary<string, string> {{"3", "4"}};
        var dictionary3 = dictionary1.Merge(dictionary2);
        dictionary3.ForEach(pair => Debug.LogFormat("{0}:{1}", pair.Key, pair.Value));
    }

    #region Array Extension

    /// <summary>
    /// Fors the each.
    /// </summary>
    /// <returns>The each.</returns>
    /// <param name="selfArray">Self array.</param>
    /// <param name="action">Action.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static T[] ForEach<T>(this T[] selfArray, Action<T> action)
    {
        Array.ForEach(selfArray, action);
        return selfArray;
    }

    /// <summary>
    /// Fors the each.
    /// </summary>
    /// <returns>The each.</returns>
    /// <param name="selfArray">Self array.</param>
    /// <param name="action">Action.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> selfArray, Action<T> action)
    {
        if (action == null) throw new ArgumentException();
        foreach (var item in selfArray)
        {
            action(item);
        }
        return selfArray;
    }

    /// <summary>
    /// 给数组添加数组
    /// </summary>
    /// <param name="arr"></param>
    /// <param name="selfArray"></param>
    /// <param name="plusArray"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T[] AddRange<T>(this T[] arr, ref T[] selfArray, T[] plusArray)
    {
        T[] array = new T[selfArray.Length + plusArray.Length];
        for (int i = 0; i < selfArray.Length; i++)
        {
            array[i] = selfArray[i];
        }
        for (int i = 0; i < plusArray.Length; i++)
        {
            array[i + selfArray.Length] = plusArray[i];
        }
        selfArray = array;
        return selfArray;
    }


    /// <summary>
    /// 减去数组指定数量
    /// </summary>
    /// <param name="arr"></param>
    /// <param name="slefArray"></param>
    /// <param name="length"></param>
    /// <param name="startIndex">如果startIndex=-1，表示去除行尾的length个</param>
    /// <returns></returns>
    public static T[] SubRange<T>(this T[] arr, ref T[] slefArray, int length, int startIndex = -1)
    {
        if (startIndex >= arr.Length || (startIndex + length) > arr.Length) return arr;
        if (startIndex == -1 && length > arr.Length) return arr;
        if (startIndex == -1) startIndex = arr.Length - length;
        T[] array = new T[arr.Length - length];
        for (int i = 0; i < arr.Length; i++)
        {
            if (i >= startIndex && i < startIndex + length) continue;
            if (i >= startIndex + length)
            {
                array[i - length] = arr[i];
            }
            else
            {
                array[i] = arr[i];
            }
        }
        slefArray = array;
        return arr;
    }

    public static T GetRandomItem<T>(this T[] arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Length - 1)];
    }

    public static T First<T>(this T[] arr)
    {
        return arr[0];
    }

    public static T Last<T>(this T[] arr)
    {
        return arr[arr.Length - 1];
    }

    public static bool Contains<T>(this T[] arr, T val) where T : IEquatable<T>
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(arr[i], val))
                return true;
        }
        return false;
    }

    #endregion

    #region List Extension

    /// <summary>
    /// Fors the each reverse.
    /// </summary>
    /// <returns>The each reverse.</returns>
    /// <param name="selfList">Self list.</param>
    /// <param name="action">Action.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static List<T> ForEachReverse<T>(this List<T> selfList, Action<T> action)
    {
        if (action == null) throw new ArgumentException();
        for (var i = selfList.Count - 1; i >= 0; --i)
            action(selfList[i]);
        return selfList;
    }

    /// <summary>
    /// Fors the each reverse.
    /// </summary>
    /// <returns>The each reverse.</returns>
    /// <param name="selfList">Self list.</param>
    /// <param name="action">Action.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static List<T> ForEachReverse<T>(this List<T> selfList, Action<T, int> action)
    {
        if (action == null) throw new ArgumentException();
        for (var i = selfList.Count - 1; i >= 0; --i)
            action(selfList[i], i);
        return selfList;
    }

    /// <summary>
    /// 遍历列表
    /// </summary>
    /// <typeparam name="T">列表类型</typeparam>
    /// <param name="list">目标表</param>
    /// <param name="action">行为</param>
    public static void ForEach<T>(this List<T> list, Action<int, T> action)
    {
        for (var i = 0; i < list.Count; i++)
        {
            action(i, list[i]);
        }
    }

    public static T GetRandomItem<T>(this IList<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public static T GetRandomItem<T>(this IList<T> list, float min, float max)
    {
        if (min > 1 || max > 1) throw new Exception("min max muse less or equal 1");
        int minIndex = (int)(min * list.Count);
        int maxIndex = (int)(max * list.Count);
        return list[UnityEngine.Random.Range(minIndex, maxIndex)];
    }
    
    public static int GetRandomIndex(this ICollection list)
    {
        return UnityEngine.Random.Range(0, list.Count);
    }

    public static int GetRandomIndex(this ICollection list, float min, float max)
    {
        if (min > 1 || max > 1) throw new Exception("min max muse less or equal 1");
        int minIndex = (int) (min * list.Count);
        int maxIndex = (int) (max * list.Count);
        return UnityEngine.Random.Range(minIndex, maxIndex);
    }

    public static T Last<T>(this IList<T> list)
    {
        return list[list.Count - 1];
    }

    public static T First<T>(this IList<T> list)
    {
        return list[0];
    }

    public static T RemoveLast<T>(this IList<T> list)
    {
        T result = list.Last();
        list.RemoveAt(list.Count - 1);
        return result;
    }
    
    public static void Swap<T>(this IList<T> list, int i, int j)
    {
        var temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
    
    public static RecyclableList<T> RandomSort<T>(this IList<T> list)
    {
        RecyclableList<T> result = RecyclableList<T>.Create(list, list.Count);
        System.Random rnd = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            result.Swap(k, n);
        }
        return result;
    }    /// <summary>
    /// 根据权值来获取索引
    /// </summary>
    /// <param name="powers"></param>
    /// <returns></returns>
    public static int GetRandomWithPower(this List<int> powers)
    {
        var sum = 0;
        foreach (var power in powers)
        {
            sum += power;
        }
        var randomNum = UnityEngine.Random.Range(0, sum);
        var currentSum = 0;
        for (var i = 0; i < powers.Count; i++)
        {
            var nextSum = currentSum + powers[i];
            if (randomNum >= currentSum && randomNum <= nextSum)
            {
                return i;
            }
            currentSum = nextSum;
        }
        Debug.LogError("权值范围计算错误！");
        return -1;
    }

    /// <summary>
    /// 拷贝到
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="begin"></param>
    /// <param name="end">不包含end</param>
    public static void CopyTo<T>(this List<T> from, ref List<T> to, int begin = 0, int end = -1)
    {
        if (begin < 0) begin = 0;
        else if (begin >= from.Count) begin = from.Count;
        int endIndex = 0;
        if (end == -1) endIndex = from.Count;
        else if (end > from.Count) endIndex = from.Count;
        else endIndex = end;
        to = new List<T>(endIndex - begin);
        for (var i = begin; i < endIndex; i++)
        {
            to.Add(from[i]);
        }
    }

    public static void CopyTo<TBase, TChild>(this List<TChild> from, ref List<TBase> to, int begin = 0,
        int end = -1)
        where TBase : class where TChild : TBase
    {
        if (begin < 0) begin = 0;
        else if (begin >= from.Count) begin = from.Count;
        int endIndex = 0;
        if (end == -1) endIndex = from.Count;
        else if (end > from.Count) endIndex = from.Count;
        else endIndex = end;
        to = new List<TBase>(endIndex - begin);
        for (var i = begin; i < endIndex; i++)
        {
            to.Add(from[i]);
        }
    }

    /// <summary>
    /// 将List转为Array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="selfList"></param>
    /// <returns></returns>
    public static T[] ToArraySavely<T>(this List<T> selfList)
    {
        var res = new T[selfList.Count];
        for (var i = 0; i < selfList.Count; i++)
        {
            res[i] = selfList[i];
        }
        return res;
    }

    /// <summary>
    /// 尝试获取，如果没有该数则返回null
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="selfList"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static T TryGet<T>(this IList<T> selfList, int index)
    {
        return selfList.Count > index ? selfList[index] : default(T);
    }

    public static bool TryRemove<T>(this IList<T> self, T value)
    {
        var index = self.IndexOf(value);
        if (index == -1) return false;
        self.RemoveAt(index);
        return true;
    }

    public static bool TryAdd<T>(this IList<T> selfList, T value)
    {
        if (!selfList.Contains(value))
        {
            selfList.Add(value);
            return true;
        }
        return false;
    }

    #endregion

    #region Dictionary Extension

    /// <summary>
    /// Merge the specified dictionary and dictionaries.
    /// </summary>
    /// <returns>The merge.</returns>
    /// <param name="dictionary">Dictionary.</param>
    /// <param name="dictionaries">Dictionaries.</param>
    /// <typeparam name="TKey">The 1st type parameter.</typeparam>
    /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
        params Dictionary<TKey, TValue>[] dictionaries)
    {
        return dictionaries.Aggregate(dictionary,
            (current, dict) =>
                current.Union(dict).ToDictionary(kv => kv.Key, kv => kv.Value));
    }

    /// <summary>
    /// 根据权值获取值，Key为值，Value为权值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="powersDict"></param>
    /// <returns></returns>
    public static T GetRandomWithPower<T>(this Dictionary<T, int> powersDict)
    {
        var keys = new List<T>();
        var values = new List<int>();
        foreach (var key in powersDict.Keys)
        {
            keys.Add(key);
            values.Add(powersDict[key]);
        }
        var finalKeyIndex = values.GetRandomWithPower();
        return keys[finalKeyIndex];
    }

    /// <summary>
    /// 遍历
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dict"></param>
    /// <param name="action"></param>
    public static void ForEach<K, V>(this Dictionary<K, V> dict, Action<K, V> action)
    {
        var dictE = dict.GetEnumerator();
        while (dictE.MoveNext())
        {
            var current = dictE.Current;
            action(current.Key, current.Value);
        }
        dictE.Dispose();
    }

    /// <summary>
    /// 向其中添加新的词典
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="dict"></param>
    /// <param name="addInDict"></param>
    /// <param name="isOverride"></param>
    public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> addInDict,
        bool isOverride = false)
    {
        var dictE = addInDict.GetEnumerator();
        while (dictE.MoveNext())
        {
            var current = dictE.Current;
            if (dict.ContainsKey(current.Key))
            {
                if (isOverride)
                    dict[current.Key] = current.Value;
                continue;
            }
            dict.Add(current.Key, current.Value);
        }
        dictE.Dispose();
    }

    #endregion
}

/// <summary>
/// 各种文件的读写复制操作,主要是对System.IO的一些封装
/// </summary>
public static class IOExtension
{
    public static void Example()
    {
        var testDir = Application.persistentDataPath.CombinePath("TestFolder");
        testDir.CreateDirIfNotExists();
        Debug.Log(Directory.Exists(testDir));
        testDir.DeleteDirIfExists();
        Debug.Log(Directory.Exists(testDir));
        var testFile = testDir.CombinePath("test.txt");
        testDir.CreateDirIfNotExists();
        File.Create(testFile);
        testFile.DeleteFileIfExists();
    }

    /// <summary>
    /// 创建新的文件夹,如果存在则不创建
    /// </summary>
    public static string CreateDirIfNotExists(this string dirFullPath)
    {
        if (!Directory.Exists(dirFullPath))
        {
            Directory.CreateDirectory(dirFullPath);
        }
        return dirFullPath;
    }

    /// <summary>
    /// 删除文件夹，如果存在
    /// </summary>
    public static void DeleteDirIfExists(this string dirFullPath)
    {
        if (Directory.Exists(dirFullPath))
        {
            Directory.Delete(dirFullPath, true);
        }
    }

    /// <summary>
    /// 清空 Dir,如果存在。
    /// </summary>
    public static void EmptyDirIfExists(this string dirFullPath)
    {
        if (Directory.Exists(dirFullPath))
        {
            Directory.Delete(dirFullPath, true);
        }
        Directory.CreateDirectory(dirFullPath);
    }

    /// <summary>
    /// 删除文件 如果存在
    /// </summary>
    /// <param name="fileFullPath"></param>
    /// <returns> True if exists</returns>
    public static bool DeleteFileIfExists(this string fileFullPath)
    {
        if (File.Exists(fileFullPath))
        {
            File.Delete(fileFullPath);
            return true;
        }
        return false;
    }

    public static string CombinePath(this string selfPath, string toCombinePath)
    {
        return Path.Combine(selfPath, toCombinePath);
    }

    #region 未经过测试

    /// <summary>
    /// 保存文本
    /// </summary>
    /// <param name="text"></param>
    /// <param name="path"></param>
    public static void SaveText(this string text, string path)
    {
        path.DeleteFileIfExists();
        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (var sr = new StreamWriter(fs))
            {
                sr.Write(text); //开始写入值
            }
        }
    }

    /// <summary>
    /// 读取文本
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static string ReadText(this FileInfo file)
    {
        return ReadText(file.FullName);
    }

    /// <summary>
    /// 读取文本
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static string ReadText(this string fileFullPath)
    {
        var result = string.Empty;
        using (var fs = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read))
        {
            using (var sr = new StreamReader(fs))
            {
                result = sr.ReadToEnd();
            }
        }
        return result;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="path"></param>
    public static void OpenFolder(string path)
    {
#if UNITY_STANDALONE_OSX
            System.Diagnostics.Process.Start("open", path);
#elif UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start("explorer.exe", path);
#endif
    }
#endif

    /// <summary>
    /// 获取文件夹名
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetDirectoryName(string fileName)
    {
        fileName = IOExtension.MakePathStandard(fileName);
        return fileName.Substring(0, fileName.LastIndexOf('/'));
    }

    /// <summary>
    /// 获取文件名
    /// </summary>
    /// <param name="path"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string GetFileName(string path, char separator = '/')
    {
        path = IOExtension.MakePathStandard(path);
        return path.Substring(path.LastIndexOf(separator) + 1);
    }

    /// <summary>
    /// 获取不带后缀的文件名
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string GetFileNameWithoutExtension(string fileName, char separator = '/')
    {
        return GetFilePathWithoutExtension(GetFileName(fileName, separator));
    }

    /// <summary>
    /// 获取不带后缀的文件路径
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetFilePathWithoutExtension(string fileName)
    {
        if (fileName.Contains("."))
            return fileName.Substring(0, fileName.LastIndexOf('.'));
        return fileName;
    }

    /// <summary>
    /// 获取streamingAssetsPath
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetStreamPath(string fileName)
    {
        string str = Application.streamingAssetsPath + "/" + fileName;
        if (Application.platform != RuntimePlatform.Android)
        {
            str = "file://" + str;
        }
        return str;
    }

    /// <summary>
    /// 工程根目录
    /// </summary>
    public static string projectPath
    {
        get
        {
            DirectoryInfo directory = new DirectoryInfo(Application.dataPath);
            return MakePathStandard(directory.Parent.FullName);
        }
    }


    public static void CopyTo(this DirectoryInfo srcPath, string destPath)
    {
        Directory.CreateDirectory(destPath);
        foreach (var srcInfo in srcPath.GetDirectories("*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory($"{destPath}{srcInfo.FullName[srcPath.FullName.Length..]}");
        }
        
        foreach (var srcInfo in srcPath.GetFiles("*", SearchOption.AllDirectories))
        {
            File.Copy(srcInfo.FullName, $"{destPath}{srcInfo.FullName[srcPath.FullName.Length..]}", true);
        }
    }    /// <summary>
    /// 使目录存在,Path可以是目录名必须是文件名
    /// </summary>
    /// <param name="path"></param>
    public static void MakeFileDirectoryExist(string path)
    {
        string root = Path.GetDirectoryName(path);
        if (!Directory.Exists(root))
        {
            Directory.CreateDirectory(root);
        }
    }

    /// <summary>
    /// 使目录存在
    /// </summary>
    /// <param name="path"></param>
    public static void MakeDirectoryExist(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 结合目录
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    public static string Combine(params string[] paths)
    {
        string result = "";
        foreach (string path in paths)
        {
            result = Path.Combine(result, path);
        }
        result = MakePathStandard(result);
        return result;
    }

    /// <summary>
    /// 获取父文件夹
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetPathParentFolder(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        return Path.GetDirectoryName(path);
    }

    /// <summary>
    /// 将绝对路径转换为相对于Asset的路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ConvertAbstractToAssetPath(string path)
    {
        path = MakePathStandard(path);
        return MakePathStandard(path.Replace(projectPath + "/", ""));
    }

    /// <summary>
    /// 将绝对路径转换为相对于Asset的路径且去除后缀
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ConvertAbstractToAssetPathWithoutExtension(string path)
    {
        return IOExtension.GetFilePathWithoutExtension(ConvertAbstractToAssetPath(path));
    }

    /// <summary>
    /// 将相对于Asset的路径转换为绝对路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ConvertAssetPathToAbstractPath(string path)
    {
        path = MakePathStandard(path);
        return Combine(projectPath, path);
    }

    /// <summary>
    /// 将相对于Asset的路径转换为绝对路径且去除后缀
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ConvertAssetPathToAbstractPathWithoutExtension(string path)
    {
        return IOExtension.GetFilePathWithoutExtension(ConvertAssetPathToAbstractPath(path));
    }

    /// <summary>
    /// 使路径标准化，去除空格并将所有'\'转换为'/'
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string MakePathStandard(string path)
    {
        return path.Trim().Replace("\\", "/");
    }

    /// <summary>
    /// 去除‘..’用路径替换
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string Normalize(string path)
    {
        var normalized = path;
        normalized = Regex.Replace(normalized, @"/\./", "/");
        if (normalized.Contains(".."))
        {
            var list = new List<string>();
            var paths = normalized.Split('/');
            foreach (var name in paths)
            {
                // 首位是".."无法处理的
                if (name.Equals("..") && list.Count > 0)
                    list.RemoveAt(list.Count - 1);
                else
                    list.Add(name);
            }
            normalized = list.Join("/");
        }
        if (path.Contains("\\"))
        {
            normalized = normalized.Replace("\\", "/");
        }
        return normalized;
    }

    /// <summary>
    /// 根据文件夹路径获取文件夹下所有的文件
    /// </summary>
    /// <param name="dirABSPath"></param>
    /// <param name="isRecursive"></param>
    /// <param name="pattern"></param>
    /// <param name="filter">true会被剔除</param>
    /// <returns></returns>
    public static List<string> DirGetSubFiles(this string dirABSPath, bool isRecursive = true,string pattern = "*", Func<string, bool> filter = null)
    {
        var pathList = new List<string>();
        var di = new DirectoryInfo(dirABSPath);
        if (!di.Exists)
        {
            return pathList;
        }

        pathList.AddRange(di
            .GetFiles(pattern, isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
            .Select((f) => f.FullName));
        if (filter != null)
        {
            for (int i = 0; i < pathList.Count; i++)
            {
                if (filter(pathList[i]))
                {
                    pathList.RemoveAt(i);
                    i--;
                }
            }
        }
        return pathList;
    }

    /// <summary>
    /// 根据文件夹路径获取文件夹下所有文件夹
    /// </summary>
    /// <param name="dirABSPath"></param>
    /// <param name="isRecursive"></param>
    /// <param name="pattern"></param>
    /// <param name="filter">true会被剔除</param>
    /// <returns></returns>
    public static List<string> DirGetSubDirs(this string dirABSPath, bool isRecursive = true, string pattern = "*", Func<string, bool> filter = null)
    {
        var pathList = new List<string>();
        var di = new DirectoryInfo(dirABSPath);
        if (!di.Exists)
        {
            return pathList;
        }
        var dirs = di.GetDirectories(pattern , isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        pathList.AddRange(dirs.Select((dir) => dir.FullName));
        if (filter != null)
        {
            for (int i = 0; i < pathList.Count; i++)
            {
                if (filter(pathList[i]))
                {
                    pathList.RemoveAt(i);
                    i--;
                }
            }
        }
        return pathList;
    }

    public static string GetFileExtendName(this string absOrAssetsPath)
    {
        var lastIndex = absOrAssetsPath.LastIndexOf(".");
        if (lastIndex >= 0)
        {
            return absOrAssetsPath.Substring(lastIndex);
        }
        return string.Empty;
    }

    public static string GetLastDirName(this string absOrAssetsPath)
    {
        var name = absOrAssetsPath.Replace("\\", "/");
        var dirs = name.Split('/');
        return absOrAssetsPath.EndsWith("/") ? dirs[dirs.Length - 2] : dirs[dirs.Length - 1];
    }

    #endregion
}

public static class OOPExtension
{
    private interface ExampleInterface
    {
    }

    public static void Example()
    {
        if (typeof(OOPExtension).ImplementsInterface<ExampleInterface>())
        {
        }
        if (new object().ImplementsInterface<ExampleInterface>())
        {
        }
    }

    /// <summary>
    /// Determines whether the type implements the specified interface
    /// and is not an interface itself.
    /// </summary>
    /// <returns><c>true</c>, if interface was implementsed, <c>false</c> otherwise.</returns>
    /// <param name="type">Type.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static bool ImplementsInterface<T>(this Type type)
    {
        return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
    }

    /// <summary>
    /// Determines whether the type implements the specified interface
    /// and is not an interface itself.
    /// </summary>
    /// <returns><c>true</c>, if interface was implementsed, <c>false</c> otherwise.</returns>
    /// <param name="type">Type.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public static bool ImplementsInterface<T>(this object obj)
    {
        var type = obj.GetType();
        return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
    }
}

public class AssemblyManager
{
    /// <summary>
    /// 编辑器默认的程序集Assembly-CSharp.dll
    /// </summary>
    private static Assembly defaultCSharpAssembly;

    /// <summary>
    /// 程序集缓存
    /// </summary>
    private static Dictionary<string, Assembly> assemblyCache = new Dictionary<string, Assembly>();


    /// <summary>
    /// 获取编辑器默认的程序集Assembly-CSharp.dll
    /// </summary>
    public static Assembly DefaultCSharpAssembly
    {
        get
        {
            //如果已经找到，直接返回
            if (defaultCSharpAssembly != null)
                return defaultCSharpAssembly;

            //从当前加载的程序包中寻找，如果找到，则直接记录并返回
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assem in assems)
            {
                //所有本地代码都编译到Assembly-CSharp中
                if (assem.GetName().Name == "Assembly-CSharp")
                {
                    //保存到列表并返回
                    defaultCSharpAssembly = assem;
                    break;
                }
            }
            return defaultCSharpAssembly;
        }
    }

    /// <summary>
    /// 获取Assembly
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Assembly GetAssembly(string name)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.FullName.StartsWith("JetBrains")) continue;
            if (assembly.GetName().Name.Equals(name))
            {
                return assembly;
            }
        }
        return null;
    }

    /// <summary>
    /// 根据名称获取程序集中的类型
    /// </summary>
    /// <param name="assembly">程序集名称，例如：SSJJ，Start</param>
    /// <param name="typeName">类型名称，必须包含完整的命名空间，例如：SSJJ.Render.BulletRail</param>
    /// <returns>类型</returns>
    public static Type GetAssemblyType(string assembly, string typeName)
    {
        Type t;
        if (Application.platform == RuntimePlatform.Android || Application.isEditor)
            t = GetAssembly(assembly).GetType(typeName);
        //其他平台使用默认程序集中的类型
        else
            t = DefaultCSharpAssembly.GetType(typeName);
        return t;
    }

    /// <summary>
    /// 通过类的完整名称来获得
    /// </summary>
    /// <param name="typeFullName"></param>
    /// <returns></returns>
    public static Type GetAssemblyType(string typeFullName)
    {
        var pointPos = typeFullName.LastIndexOf(".", StringComparison.Ordinal);
        string assemblyName = null;
        string typeName = null;
        if (pointPos > 0)
        {
            assemblyName = typeFullName.Substring(0, pointPos);
            typeName = typeFullName.Substring(pointPos);
        }
        else
        {
            typeName = typeFullName;
        }
        var orgType = assemblyName == null
            ? GetDefaultAssemblyType(typeName)
            : GetAssemblyType(assemblyName, typeName);
        return orgType;
    }

    /// <summary>
    /// 获取默认的程序集
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    public static Type GetDefaultAssemblyType(string typeName)
    {
        return DefaultCSharpAssembly.GetType(typeName);
    }

    public static Type[] GetTypeList(string assemblyName)
    {
        return GetAssembly(assemblyName).GetTypes();
    }
}

public static class ReflectionExtension
{
    public static void Example()
    {
        var selfType = ReflectionExtension.GetAssemblyCSharp().GetType("AD.ReflectionExtension");
        Debug.Log(selfType);
    }

    public static Assembly GetAssemblyCSharp()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in assemblies)
        {
            if (a.FullName.StartsWith("Assembly-CSharp,"))
            {
                return a;
            }
        }
        Debug.LogError(">>>>>>>Error: Can\'t find Assembly-CSharp.dll");
        return null;
    }

    public static Assembly GetAssemblyCSharpEditor()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in assemblies)
        {
            if (a.FullName.StartsWith("Assembly-CSharp-Editor,"))
            {
                return a;
            }
        }
        Debug.LogError(">>>>>>>Error: Can\'t find Assembly-CSharp-Editor.dll");
        return null;
    }

    /// <summary>
    /// 通过反射方式调用函数
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="methodName">方法名</param>
    /// <param name="args">参数</param>
    /// <returns></returns>
    public static object InvokeByReflect(this object obj, string methodName, params object[] args)
    {
        var methodInfo = obj.GetType().GetMethod(methodName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return methodInfo == null ? null : methodInfo.Invoke(obj, args);
    }

    /// <summary>
    /// 通过反射方式获取域值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fieldName">域名</param>
    /// <returns></returns>
    public static object GetFieldByReflect(this object obj, string fieldName)
    {
        var fieldInfo = obj.GetType().GetField(fieldName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return fieldInfo == null ? null : fieldInfo.GetValue(obj);
    }

    public static void SetFieldByReflect(this object obj, string fieldName, object value)
    {
        var fieldInfo = obj.GetType().GetField(fieldName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo != null)
        {
            fieldInfo.SetValue(obj, value);
        }
        else
        {
            Debug.LogError($"{obj.GetType().Name} 找不到字段 {fieldName}");
        }
    }

    /// <summary>
    /// 通过反射方式获取属性
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="fieldName">属性名</param>
    /// <returns></returns>
    public static object GetPropertyByReflect(this object obj, string propertyName, object[] index = null)
    {
        var propertyInfo = obj.GetType().GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        return propertyInfo == null ? null : propertyInfo.GetValue(obj, index);
    }

    public static void SetPropertyByReflect(this object obj, string propertyName, object value)
    {
        var propertyInfo = obj.GetType().GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (propertyInfo != null)
        {
            propertyInfo.SetValue(obj, value);
        }
        else
        {
            Debug.Log($"{obj.GetType().Name} 找不到字段 {propertyName}");
        }
    }

    /// <summary>
    /// 拥有特性
    /// </summary>
    /// <returns></returns>
    public static bool HasAttribute(this PropertyInfo prop, Type attributeType, bool inherit)
    {
        return prop.GetCustomAttributes(attributeType, inherit).Length > 0;
    }

    /// <summary>
    /// 拥有特性
    /// </summary>
    /// <returns></returns>
    public static bool HasAttribute(this FieldInfo field, Type attributeType, bool inherit)
    {
        return field.GetCustomAttributes(attributeType, inherit).Length > 0;
    }

    /// <summary>
    /// 拥有特性
    /// </summary>
    /// <returns></returns>
    public static bool HasAttribute(this Type type, Type attributeType, bool inherit)
    {
        return type.GetCustomAttributes(attributeType, inherit).Length > 0;
    }

    /// <summary>
    /// 拥有特性
    /// </summary>
    /// <returns></returns>
    public static bool HasAttribute(this MethodInfo method, Type attributeType, bool inherit)
    {
        return method.GetCustomAttributes(attributeType, inherit).Length > 0;
    }
    
    public static bool IsOverride(this MethodInfo methodInfo)
    {
        return (methodInfo.GetBaseDefinition().DeclaringType != methodInfo.DeclaringType);
    } 

    /// <summary>
    /// 获取第一个特性
    /// </summary>
    public static T GetFirstAttribute<T>(this MethodInfo method, bool inherit) where T : Attribute
    {
        var attrs = (T[]) method.GetCustomAttributes(typeof(T), inherit);
        if (attrs != null && attrs.Length > 0)
            return attrs[0];
        return null;
    }

    /// <summary>
    /// 获取第一个特性
    /// </summary>
    public static T GetFirstAttribute<T>(this FieldInfo field, bool inherit) where T : Attribute
    {
        var attrs = (T[]) field.GetCustomAttributes(typeof(T), inherit);
        if (attrs != null && attrs.Length > 0)
            return attrs[0];
        return null;
    }

    /// <summary>
    /// 获取第一个特性
    /// </summary>
    public static T GetFirstAttribute<T>(this PropertyInfo prop, bool inherit) where T : Attribute
    {
        var attrs = (T[]) prop.GetCustomAttributes(typeof(T), inherit);
        if (attrs != null && attrs.Length > 0)
            return attrs[0];
        return null;
    }

    /// <summary>
    /// 获取第一个特性
    /// </summary>
    public static T GetFirstAttribute<T>(this Type type, bool inherit) where T : Attribute
    {
        var attrs = (T[]) type.GetCustomAttributes(typeof(T), inherit);
        if (attrs != null && attrs.Length > 0)
            return attrs[0];
        return null;
    }
}

public static class TypeEx
{
    /// <summary>
    /// 获取默认值
    /// </summary>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public static object DefaultForType(this Type targetType)
    {
        return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
    }

    /// <summary>
    /// GenericTypeDefinition means  List<>  or SomeClass<>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericTypeDefinition">genericTypeDefinition</param>
    /// <returns></returns>
    public static bool IsSubclassOfGenericTypeDefinition(this Type type, Type genericTypeDefinition)
    {
        if (!genericTypeDefinition.IsGenericTypeDefinition)
            return false;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
            return true;
        Type baseType = type.BaseType;
        if (baseType != null && baseType != typeof(object))
        {
            if (IsSubclassOfGenericTypeDefinition(baseType, genericTypeDefinition))
                return true;
        }
        foreach (Type t in type.GetInterfaces())
        {
            if (IsSubclassOfGenericTypeDefinition(t, genericTypeDefinition))
                return true;
        }
        return false;
    }
}

/// <summary>
/// 通过编写方法并且添加属性可以做到转换至String 如：
/// 
/// <example>
/// [ToString]
/// public static string ConvertToString(TestObj obj)
/// </example>
///
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ToString : Attribute
{
}

/// <summary>
/// 通过编写方法并且添加属性可以做到转换至String 如：
/// 
/// <example>
/// [FromString]
/// public static TestObj ConvertFromString(string str)
/// </example>
///
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class FromString : Attribute
{
}

public static class StringExtension
{
    public static void Example()
    {
        var emptyStr = string.Empty;
        Debug.Log(emptyStr.IsNotNullAndEmpty());
        Debug.Log(emptyStr.IsNullOrEmpty());
        emptyStr = emptyStr.Append("appended").Append("1").ToString();
        Debug.Log(emptyStr);
        Debug.Log(emptyStr.IsNullOrEmpty());
    }

    /// <summary>
    /// Check Whether string is null or empty
    /// </summary>
    /// <param name="selfStr"></param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string selfStr)
    {
        return string.IsNullOrEmpty(selfStr);
    }

    /// <summary>
    /// Check Whether string is null or empty
    /// </summary>
    /// <param name="selfStr"></param>
    /// <returns></returns>
    public static bool IsNotNullAndEmpty(this string selfStr)
    {
        return !string.IsNullOrEmpty(selfStr);
    }

    /// <summary>
    /// Check Whether string trim is null or empty
    /// </summary>
    /// <param name="selfStr"></param>
    /// <returns></returns>
    public static bool IsTrimNotNullAndEmpty(this string selfStr)
    {
        return !string.IsNullOrEmpty(selfStr.Trim());
    }

    /// <summary>
    /// 避免每次都用.
    /// </summary>
    private static readonly char[] mCachedSplitCharArray = {'.'};

    public static string[] Split(this string selfStr, char splitSymbol)
    {
        mCachedSplitCharArray[0] = splitSymbol;
        return selfStr.Split(mCachedSplitCharArray);
    }

    public static string UppercaseFirst(this string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    public static string LowercaseFirst(this string str)
    {
        return char.ToLower(str[0]) + str.Substring(1);
    }

    public static string ToUnixLineEndings(this string str)
    {
        return str.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    public static string ToCSV(this string[] values)
    {
        return string.Join(
            ", ",
            values.Where(value => !string.IsNullOrEmpty(value)).Select(value => value.Trim()).ToArray()
        );
    }

    public static string[] ArrayFromCSV(this string values)
    {
        return values.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(value => value.Trim())
            .ToArray();
    }

    public static string ToSpacedCamelCase(this string text)
    {
        var sb = new StringBuilder(text.Length * 2);
        sb.Append(char.ToUpper(text[0]));
        for (var i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
            {
                sb.Append(' ');
            }
            sb.Append(text[i]);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 有点不安全,编译器不会帮你排查错误。
    /// </summary>
    /// <param name="selfStr"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string FillFormat(this string selfStr, params object[] args)
    {
        return string.Format(selfStr, args);
    }

    public static StringBuilder Append(this string selfStr, string toAppend)
    {
        return new StringBuilder(selfStr).Append(toAppend);
    }

    public static string AddPrefix(this string selfStr, string toPrefix)
    {
        return new StringBuilder(toPrefix).Append(selfStr).ToString();
    }

    public static StringBuilder AppendFormat(this string selfStr, string toAppend, params object[] args)
    {
        return new StringBuilder(selfStr).AppendFormat(toAppend, args);
    }

    public static string LastWord(this string selfUrl)
    {
        return selfUrl.Split('/').Last();
    }

    public static int ToInt(this string selfStr, int defaulValue = 0)
    {
        var retValue = defaulValue;
        return int.TryParse(selfStr, out retValue) ? retValue : defaulValue;
    }

    public static DateTime ToDateTime(this string selfStr, DateTime defaultValue = default(DateTime))
    {
        var retValue = defaultValue;
        return DateTime.TryParse(selfStr, out retValue) ? retValue : defaultValue;
    }

    public static float ToFloat(this string selfStr, float defaulValue = 0)
    {
        var retValue = defaulValue;
        return float.TryParse(selfStr, out retValue) ? retValue : defaulValue;
    }

    private const char Spriter1 = ',';
    private const char Spriter2 = ':';

    private const char FBracket1 = '(';
    private const char BBracket1 = ')';

    public static T GetValue<T>(this string value)
    {
        return string.IsNullOrEmpty(value)
            ? default(T)
            : value.TryGetValue((T) typeof(T).DefaultForType());
    }

    /// <summary>
    /// 从字符串中获取值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T TryGetValue<T>(this string value, T defultValue)
    {
        if (string.IsNullOrEmpty(value))
        {
            return default(T);
        }
        return (T) TryGetValue(value, typeof(T), defultValue);
    }

    public static object GetValue(this string value, Type type)
    {
        return value.TryGetValue(type, type.DefaultForType());
    }

    /// <summary>
    /// 从字符串中获取值
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object TryGetValue(this string value, Type type, object defultValue)
    {
        try
        {
            if (type == null) return "";
            if (string.IsNullOrEmpty(value))
            {
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            }
            if (type == typeof(string))
            {
                return value;
            }
            if (type == typeof(int))
            {
                return Convert.ToInt32(Convert.ToDouble(value));
            }
            if (type == typeof(float))
            {
                return float.Parse(value);
            }
            if (type == typeof(byte))
            {
                return Convert.ToByte(Convert.ToDouble(value));
            }
            if (type == typeof(sbyte))
            {
                return Convert.ToSByte(Convert.ToDouble(value));
            }
            if (type == typeof(uint))
            {
                return Convert.ToUInt32(Convert.ToDouble(value));
            }
            if (type == typeof(short))
            {
                return Convert.ToInt16(Convert.ToDouble(value));
            }
            if (type == typeof(long))
            {
                return Convert.ToInt64(Convert.ToDouble(value));
            }
            if (type == typeof(ushort))
            {
                return Convert.ToUInt16(Convert.ToDouble(value));
            }
            if (type == typeof(ulong))
            {
                return Convert.ToUInt64(Convert.ToDouble(value));
            }
            if (type == typeof(double))
            {
                return double.Parse(value);
            }
            if (type == typeof(bool))
            {
                return bool.Parse(value);
            }
            if (type.BaseType == typeof(Enum))
            {
                return GetValue(value, Enum.GetUnderlyingType(type));
            }
            if (type == typeof(Vector2))
            {
                Vector2 vector;
                ParseVector2(value, out vector);
                return vector;
            }
            if (type == typeof(Vector3))
            {
                Vector3 vector;
                ParseVector3(value, out vector);
                //Debug.LogError(vector.ToString());
                return vector;
            }
            if (type == typeof(Vector4))
            {
                Vector4 vector;
                ParseVector4(value, out vector);
                return vector;
            }
            if (type == typeof(Quaternion))
            {
                Quaternion quaternion;
                ParseQuaternion(value, out quaternion);
                return quaternion;
            }
            if (type == typeof(Color))
            {
                Color color;
                ParseColor(value, out color);
                return color;
            }
            object constructor;
            object genericArgument;
            //词典
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
            {
                Type[] genericArguments = type.GetGenericArguments();
                Dictionary<string, string> dictionary = ParseMap(value, Spriter2, Spriter1);
                constructor = type.GetConstructor(Type.EmptyTypes).Invoke(null);
                foreach (KeyValuePair<string, string> pair in dictionary)
                {
                    var genericArgument1 = GetValue(pair.Key, genericArguments[0]);
                    genericArgument = GetValue(pair.Value, genericArguments[1]);
                    type.GetMethod("Add").Invoke(constructor, new[] {genericArgument1, genericArgument});
                }
                return constructor;
            }

            //列表
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
            {
                var type2 = type.GetGenericArguments()[0];
                var list = ParseList(value, Spriter1);
                constructor = Activator.CreateInstance(type);
                foreach (var str in list)
                {
                    genericArgument = GetValue(str, type2);
                    Debug.Log(str + "  " + type2.Name);
                    type.GetMethod("Add").Invoke(constructor, new[] {genericArgument});
                }
                return constructor;
            }
            if (type == typeof(ArrayList))
            {
                return value.GetValue<List<string>>() ?? defultValue;
            }
            if (type == typeof(Hashtable))
            {
                return value.GetValue<Dictionary<string, string>>() ?? defultValue;
            }

            //数组
            if (type.IsArray)
            {
                var elementType = Type.GetType(
                    type.FullName.Replace("[]", string.Empty));
                var elStr = value.Split(Spriter1);
                var array = Array.CreateInstance(elementType, elStr.Length);
                for (var i = 0; i < elStr.Length; i++)
                {
                    array.SetValue(elStr[i].GetValue(elementType), i);
                }
                return array;
            }
            if (CanConvertFromString(type))
            {
                return ParseFromStringableObject(value, type);
            }
            Debug.LogWarning("字符转换 没有适合的转换类型，返回默认值");
            return defultValue != type.DefaultForType() ? defultValue : type.DefaultForType();
        }
        catch (Exception)
        {
            Debug.LogWarning("字符转换 解析失败，返回默认值");
            return type.DefaultForType();
        }
    }

    #region FromString

    /// <summary>
    /// 解析颜色
    /// </summary>
    /// <param name="_inputString"></param>
    /// <param name="result"></param>
    /// <param name="colorSpriter"></param>
    /// <returns></returns>
    public static bool ParseColor(string _inputString, out Color result, char colorSpriter = ',')
    {
        string str = _inputString.Trim();
        str = str.Replace(FBracket1.ToString(), "");
        str = str.Replace(BBracket1.ToString(), "");
        result = Color.clear;
        if (str.Length < 9)
        {
            return false;
        }
        try
        {
            var strArray = str.Split(colorSpriter);
            if (strArray.Length != 4)
            {
                return false;
            }
            result = new Color(float.Parse(strArray[0]) / 255f, float.Parse(strArray[1]) / 255f,
                float.Parse(strArray[2]) / 255f, float.Parse(strArray[3]) / 255f);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 解析列表
    /// </summary>
    /// <param name="strList"></param>
    /// <param name="listSpriter"></param>
    /// <returns></returns>
    public static List<string> ParseList(this string strList, char listSpriter = ',')
    {
        var list = new List<string>();
        if (!string.IsNullOrEmpty(strList))
        {
            var str = strList.Trim();
            if (string.IsNullOrEmpty(strList))
            {
                return list;
            }
            var strArray = str.Split(listSpriter);
            list.AddRange(from str2 in strArray where !string.IsNullOrEmpty(str2) select str2.Trim());
        }
        return list;
    }

    /// <summary>
    /// 解析词典
    /// </summary>
    /// <param name="strMap"></param>
    /// <param name="keyValueSpriter"></param>
    /// <param name="mapSpriter"></param>
    /// <returns></returns>
    public static Dictionary<string, string> ParseMap(this string strMap, char keyValueSpriter = ':',
        char mapSpriter = ',')
    {
        var dictionary = new Dictionary<string, string>();
        if (!string.IsNullOrEmpty(strMap))
        {
            var strArray = strMap.Split(mapSpriter);
            foreach (var str in strArray)
            {
                if (!string.IsNullOrEmpty(str))
                {
                    var strArray2 = str.Split(keyValueSpriter);
                    if ((strArray2.Length == 2) && !dictionary.ContainsKey(strArray2[0]))
                    {
                        dictionary.Add(strArray2[0].Trim(), strArray2[1].Trim());
                    }
                }
            }
        }
        return dictionary;
    }

    /// <summary>
    /// 解析四维向量
    /// </summary>
    /// <param name="_inputString"></param>
    /// <param name="result"></param>
    /// <param name="vectorSpriter"></param>
    /// <returns></returns>
    public static bool ParseVector4(string _inputString, out Vector4 result, char vectorSpriter = ',')
    {
        var str = _inputString.Trim();
        str = str.Replace(FBracket1.ToString(), "");
        str = str.Replace(BBracket1.ToString(), "");
        result = new Vector4();
        try
        {
            var strArray = str.Split(vectorSpriter);
            if (strArray.Length != 4)
            {
                return false;
            }
            result.x = float.Parse(strArray[0]);
            result.y = float.Parse(strArray[1]);
            result.z = float.Parse(strArray[2]);
            result.w = float.Parse(strArray[3]);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    /// <summary>
    /// 解析四元数
    /// </summary>
    /// <param name="_inputString"></param>
    /// <param name="result"></param>
    /// <param name="spriter"></param>
    /// <returns></returns>
    public static bool ParseQuaternion(string _inputString, out Quaternion result, char spriter = ',')
    {
        var vec = new Vector4();
        var flag = ParseVector4(_inputString, out vec, spriter);
        result = new Quaternion(vec.x, vec.y, vec.z, vec.w);
        return flag;
    }

    /// <summary>
    /// 解析三维向量
    /// </summary>
    /// <param name="_inputString"></param>
    /// <param name="result"></param>
    /// <param name="spriter"></param>
    /// <returns></returns>
    public static bool ParseVector3(string _inputString, out Vector3 result, char spriter = ',')
    {
        var str = _inputString.Trim();
        str = str.Replace(FBracket1.ToString(), "");
        str = str.Replace(BBracket1.ToString(), "");
        result = new Vector3();
        try
        {
            var strArray = str.Split(spriter);
            if (strArray.Length != 3)
            {
                return false;
            }
            result.x = float.Parse(strArray[0]);
            result.y = float.Parse(strArray[1]);
            result.z = float.Parse(strArray[2]);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    /// <summary>
    /// 解析二维向量
    /// </summary>
    /// <param name="_inputString"></param>
    /// <param name="result"></param>
    /// <param name="spriter"></param>
    /// <returns></returns>
    public static bool ParseVector2(string _inputString, out Vector2 result, char spriter = ',')
    {
        var str = _inputString.Trim();
        str = str.Replace(FBracket1.ToString(), "");
        str = str.Replace(BBracket1.ToString(), "");
        result = new Vector2();
        try
        {
            var strArray = str.Split(spriter);
            if (strArray.Length != 2)
            {
                return false;
            }
            result.x = float.Parse(strArray[0]);
            result.y = float.Parse(strArray[1]);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
    }

    /// <summary>
    /// 解析可解析对象
    /// </summary>
    /// <param name="str"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object ParseFromStringableObject(string str, Type type)
    {
        var methodInfos = type.GetMethods();
        MethodInfo info = null;
        foreach (var method in methodInfos)
        {
            if (info != null) break;
            var attrs = method.GetCustomAttributes(false);
            if (attrs.OfType<FromString>().Any())
            {
                info = method;
            }
        }
        return info.Invoke(null, new object[1] {str});
    }

    #endregion FromString

    /// <summary>
    /// 从“？~？”的字符串中获取随机数
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static float GetRandom(this string str)
    {
        var strs = str.Split('~');
        var num1 = strs[0].GetValue<float>();
        var num2 = strs[1].GetValue<float>();
        return str.Length == 1
            ? num1
            : UnityEngine.Random.Range(Mathf.Min(num1, num2), Mathf.Max(num1, num2));
    }

    /// <summary>
    /// 将值转化为字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ConverToString(this object value)
    {
        //Debug.logger.Log("ConverToString " + Spriter1 + "  "+ Spriter2);
        if (value == null) return string.Empty;
        var type = value.GetType();
        if (type == typeof(Vector3))
        {
            return FBracket1.ToString() + ((Vector3) value).x + Spriter1 + ((Vector3) value).y +
                   Spriter1 + ((Vector3) value).z + BBracket1;
        }
        if (type == typeof(Vector2))
        {
            return FBracket1.ToString() + ((Vector2) value).x + Spriter1 + ((Vector2) value).y +
                   BBracket1;
        }
        if (type == typeof(Vector4))
        {
            return FBracket1.ToString() + ((Vector4) value).x + Spriter1 + ((Vector4) value).y +
                   Spriter1 + ((Vector4) value).z + Spriter1 + ((Vector4) value).w +
                   BBracket1;
        }
        if (type == typeof(Quaternion))
        {
            return FBracket1.ToString() + ((Quaternion) value).x + Spriter1 + ((Quaternion) value).y +
                   Spriter1 + ((Quaternion) value).z + Spriter1 + ((Quaternion) value).w +
                   BBracket1;
        }
        if (type == typeof(Color))
        {
            return FBracket1.ToString() + ((Color) value).r + Spriter1 + ((Color) value).g +
                   Spriter1 + ((Color) value).b + BBracket1;
        }
        if (type.BaseType == typeof(Enum))
        {
            return Enum.GetName(type, value);
        }
        if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
        {
            var Count = (int) type.GetProperty("Count").GetValue(value, null);
            if (Count == 0) return string.Empty;
            var getIe = type.GetMethod("GetEnumerator");
            var enumerator = getIe.Invoke(value, new object[0]);
            var enumeratorType = enumerator.GetType();
            var moveToNextMethod = enumeratorType.GetMethod("MoveNext");
            var current = enumeratorType.GetProperty("Current");
            var stringBuilder = new StringBuilder();
            while (enumerator != null && (bool) moveToNextMethod.Invoke(enumerator, new object[0]))
            {
                stringBuilder.Append(Spriter1 + ConverToString(current.GetValue(enumerator, null)));
            }
            return stringBuilder.ToString().ReplaceFirst(Spriter1.ToString(), "");
        }
        if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>)))
        {
            var pairKey = type.GetProperty("Key").GetValue(value, null);
            var pairValue = type.GetProperty("Value").GetValue(value, null);
            var keyStr = ConverToString(pairKey);
            var valueStr = ConverToString(pairValue);
            return keyStr + Spriter2 + valueStr;
        }
        if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(List<>)))
        {
            var Count = (int) type.GetProperty("Count").GetValue(value, null);
            if (Count == 0) return String.Empty;
            var mget = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);
            var stringBuilder = new StringBuilder();
            object item;
            string itemStr;
            for (var i = 0; i < Count - 1; i++)
            {
                item = mget.Invoke(value, new object[] {i});
                itemStr = item.ConverToString();
                stringBuilder.Append(itemStr + Spriter1);
            }
            item = mget.Invoke(value, new object[] {Count - 1});
            itemStr = item.ConverToString();
            stringBuilder.Append(itemStr);
            return stringBuilder.ToString();
        }
        if (type == typeof(ArrayList))
        {
            var builder = new StringBuilder();
            var arrayList = value as ArrayList;
            for (var i = 0; i < arrayList.Count - 1; i++)
            {
                builder.Append(arrayList[i].ConverToString()).Append(",");
            }
            builder.Append(arrayList[arrayList.Count - 1].ConverToString());
            return builder.ToString();
        }
        if (type == typeof(Hashtable))
        {
            var builder = new StringBuilder();
            var table = value as Hashtable;
            var e = table.Keys.GetEnumerator();
            while (e.MoveNext())
            {
                var tableKey = e.Current.ConverToString();
                var tableValue = table[e.Current].ConverToString();
                builder.Append(tableKey).Append(Spriter2).Append(tableValue).Append(Spriter1);
            }
            builder.Remove(builder.Length - 2, 1);
            return builder.ToString();
        }
        if (type.IsArray)
        {
            var stringBuilder = new StringBuilder();
            var array = value as Array;
            if (array.Length > 0)
            {
                stringBuilder.Append(ConverToString(array.GetValue(0)));
                for (var i = 1; i < array.Length; i++)
                {
                    stringBuilder.Append(Spriter1.ToString());
                    stringBuilder.Append(ConverToString(array.GetValue(i)));
                }
                return stringBuilder.ToString();
            }
            return string.Empty;
        }
        if (CanConvertToString(type))
        {
            return ToStringableObjectConvertToString(value, type);
        }
        return value.ToString();
    }

    #region ToString

    public static string Vector2ToString(Vector2 value)
    {
        return FBracket1.ToString() + value.x + Spriter1 + value.y +
               BBracket1;
    }

    public static string Vector3ToString(Vector3 value)
    {
        return FBracket1.ToString() + value.x + Spriter1 + value.y +
               Spriter1 + value.z + BBracket1;
    }

    public static string Vector4ToString(Vector4 value)
    {
        return FBracket1.ToString() + value.x + Spriter1 + value.y +
               Spriter1 + value.z + Spriter1 + value.w +
               BBracket1;
    }

    public static string ColorToString(Color value)
    {
        return FBracket1.ToString() + value.r + Spriter1 + value.g +
               Spriter1 + value.b + BBracket1;
    }

    public static string QuaternionToString(Quaternion value)
    {
        return FBracket1.ToString() + value.x + Spriter1 + value.y +
               Spriter1 + value.z + Spriter1 + value.w +
               BBracket1;
    }

    /// <summary>
    /// 将列表转换至字符串
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string ListConvertToString<T>(this List<T> value, char split1 = ',')
    {
        var type = value.GetType();
        var Count = (int) type.GetProperty("Count").GetValue(value, null);
        if (Count == 0) return String.Empty;
        var mget = type.GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public);
        var stringBuilder = new StringBuilder();
        object item;
        string itemStr;
        for (var i = 0; i < Count - 1; i++)
        {
            item = mget.Invoke(value, new object[] {i});
            itemStr = item.ConverToString();
            stringBuilder.Append(itemStr + split1);
        }
        item = mget.Invoke(value, new object[] {Count - 1});
        itemStr = item.ConverToString();
        stringBuilder.Append(itemStr);
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 数组转换至字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="split1"></param>
    /// <returns></returns>
    public static string ArrConvertToString(this Array value, char split1 = ',')
    {
        var stringBuilder = new StringBuilder();
        var array = value;
        if (array.Length > 0)
        {
            stringBuilder.Append(ConverToString(array.GetValue(0)));
            for (var i = 1; i < array.Length; i++)
            {
                stringBuilder.Append(split1.ToString());
                stringBuilder.Append(ConverToString(array.GetValue(i)));
            }
            return stringBuilder.ToString();
        }
        return string.Empty;
    }

    /// <summary>
    /// 将键值对转换至字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="split1"></param>
    /// <returns></returns>
    public static string KVPConvertToString<K, V>(this KeyValuePair<K, V> value, char split1 = ':')
    {
        var type = value.GetType();
        var pairKey = type.GetProperty("Key").GetValue(value, null);
        var pairValue = type.GetProperty("Value").GetValue(value, null);
        var keyStr = ConverToString(pairKey);
        var valueStr = ConverToString(pairValue);
        return keyStr + Spriter2 + valueStr;
    }

    /// <summary>
    /// 将Dictionary转换至字符串
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="value"></param>
    /// <param name="split1"></param>
    /// <param name="split2"></param>
    /// <returns></returns>
    public static string DictConvertToString<K, V>(this Dictionary<K, V> value, char split1 = ',',
        char split2 = ':')
    {
        var type = value.GetType();
        var Count = (int) type.GetProperty("Count").GetValue(value, null);
        if (Count == 0) return String.Empty;
        var getIe = type.GetMethod("GetEnumerator");
        var enumerator = getIe.Invoke(value, new object[0]);
        var enumeratorType = enumerator.GetType();
        var moveToNextMethod = enumeratorType.GetMethod("MoveNext");
        var current = enumeratorType.GetProperty("Current");
        var stringBuilder = new StringBuilder();
        while (enumerator != null && (bool) moveToNextMethod.Invoke(enumerator, new object[0]))
        {
            stringBuilder.Append(split1 + ConverToString(current.GetValue(enumerator, null)));
        }
        return stringBuilder.ToString().ReplaceFirst(split1.ToString(), "");
    }

    /// <summary>
    /// 将可转换至字符串的对象转换为字符串
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ToStringableObjectConvertToString(this object obj, Type type)
    {
        var methodInfos = type.GetMethods();
        MethodInfo info = null;
        foreach (var method in methodInfos)
        {
            if (info != null) break;
            var attrs = method.GetCustomAttributes(false);
            if (attrs.OfType<ToString>().Any())
            {
                info = method;
            }
        }
        return info.Invoke(null, new object[1] {obj}) as string;
    }

    #endregion ToString

    //可转换类型列表
    public static readonly List<Type> convertableTypes = new List<Type>
    {
        typeof(int),
        typeof(string),
        typeof(float),
        typeof(double),
        typeof(byte),
        typeof(long),
        typeof(bool),
        typeof(short),
        typeof(uint),
        typeof(ulong),
        typeof(ushort),
        typeof(sbyte),
        typeof(Vector3),
        typeof(Vector2),
        typeof(Vector4),
        typeof(Quaternion),
        typeof(Color),
        typeof(Dictionary<,>),
        typeof(KeyValuePair<,>),
        typeof(List<>),
        typeof(Enum),
        typeof(Array)
    };

    /// <summary>
    /// 通过文本获取类型：
    /// 注意！解析嵌套多泛型类型时会出现问题！
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static Type GetTypeByString(this string str)
    {
        str = str.Trim();
        switch (str)
        {
            case "int":
                return typeof(int);
            case "float":
                return typeof(float);
            case "string":
                return typeof(string);
            case "double":
                return typeof(double);
            case "byte":
                return typeof(byte);
            case "bool":
                return typeof(bool);
            case "short":
                return typeof(short);
            case "uint":
                return typeof(uint);
            case "ushort":
                return typeof(ushort);
            case "sbyte":
                return typeof(sbyte);
            case "Vector3":
                return typeof(Vector3);
            case "Vector2":
                return typeof(Vector2);
            case "Vector4":
                return typeof(Vector4);
            case "Quaternion":
                return typeof(Quaternion);
            case "Color":
                return typeof(Color);
        }
        if (str.StartsWith("List"))
        {
            var genType =
                str.Substring(str.IndexOf('<') + 1, str.IndexOf('>') - str.LastIndexOf('<') - 1).GetTypeByString();
            return Type.GetType("System.Collections.Generic.List`1[[" + genType.FullName + ", " +
                                genType.Assembly.FullName + "]], " + typeof(List<>).Assembly.FullName);
        }
        if (str.StartsWith("Dictionary"))
        {
            var typeNames =
                str.Substring(str.IndexOf('<') + 1, str.IndexOf('>') - str.LastIndexOf('<') - 1).Split(',');
            var type1 = typeNames[0].Trim().GetTypeByString();
            var type2 = typeNames[1].Trim().GetTypeByString();
            var typeStr = "System.Collections.Generic.Dictionary`2[[" + type1.FullName + ", " +
                          type1.Assembly.FullName + "]" +
                          ",[" + type2.FullName + ", " + type2.Assembly.FullName + "]], " +
                          typeof(Dictionary<,>).Assembly.FullName;
            return Type.GetType(typeStr);
        }

        //仅支持内置类型,支持多维数组
        if (str.Contains("[") && str.Contains("]"))
        {
            var itemTypeStr = str.Substring(0, str.IndexOf('['));
            var bracketStr =
                str.Substring(str.IndexOf('['), str.LastIndexOf(']') - str.IndexOf('[') + 1);
            var itemType = itemTypeStr.GetTypeByString();
            var typeStr = itemType.FullName + bracketStr;
            return Type.GetType(typeStr);
        }
        return Type.GetType(str);
    }

    /// <summary>
    /// 是否为可转换字符串的类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsConvertableType(this Type type)
    {
        return CanConvertFromString(type) && CanConvertToString(type) || convertableTypes.Contains(type);
    }

    /// <summary>
    /// 是否可以从String中转换出来
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool CanConvertFromString(this Type type)
    {
        var methodInfos = type.GetMethods();
        return methodInfos.Select(method => method.GetCustomAttributes(false))
            .Any(attrs => attrs.OfType<FromString>().Any());
    }

    public static bool IsStruct(this Type type)
    {
        return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
    }

    /// <summary>
    /// 是否可以转换为String
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool CanConvertToString(this Type type)
    {
        var methodInfos = type.GetMethods();
        return methodInfos.SelectMany(method => method.GetCustomAttributes(false)).OfType<ToString>().Any();
    }

    /// <summary>
    /// 替换第一个匹配值
    /// </summary>
    /// <param name="input"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    /// <param name="startAt"></param>
    /// <returns></returns>
    public static string ReplaceFirst(this string input, string oldValue, string newValue, int startAt = 0)
    {
        var index = input.IndexOf(oldValue, startAt, StringComparison.Ordinal);
        if (index < 0)
        {
            return input;
        }
        return (input.Substring(0, index) + newValue + input.Substring(index + oldValue.Length));
    }

    /// <summary>
    /// 是否存在中文字符
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool HasChinese(this string input)
    {
        return Regex.IsMatch(input, @"[\u4e00-\u9fa5]");
    }

    /// <summary>
    /// 是否存在空格
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool HasSpace(this string input)
    {
        return input.Contains(" ");
    }

    /// <summary>
    /// 将一系列的对象转换为字符串并且以符号分割
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="sp"></param>
    /// <returns></returns>
    public static string Join<T>(this IEnumerable<T> source, string sp)
    {
        var result = new StringBuilder();
        var first = true;
        foreach (T item in source)
        {
            if (first)
            {
                first = false;
                result.Append(item.ConverToString());
            }
            else
            {
                result.Append(sp).Append(item.ConverToString());
            }
        }
        return result.ToString();
    }

    /// <summary>
    /// 扩展方法来判断字符串是否为空
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool IsNullOrEmptyR(this string str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    /// 删除特定字符
    /// </summary>
    /// <param name="str"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static string RemoveString(this string str, params string[] targets)
    {
        return targets.Aggregate(str, (current, t) => current.Replace(t, string.Empty));
    }

    /// <summary>
    /// 拆分并去除空格
    /// </summary>
    /// <param name="str"></param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string[] SplitAndTrim(this string str, params char[] separator)
    {
        var res = str.Split(separator);
        for (var i = 0; i < res.Length; i++)
        {
            res[i] = res[i].Trim();
        }
        return res;
    }

    /// <summary>
    /// 查找在两个字符串中间的字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="front"></param>
    /// <param name="behined"></param>
    /// <returns></returns>
    public static string FindBetween(this string str, string front, string behined)
    {
        var startIndex = str.IndexOf(front) + front.Length;
        var endIndex = str.IndexOf(behined);
        if (startIndex < 0 || endIndex < 0)
            return str;
        return str.Substring(startIndex, endIndex - startIndex);
    }

    /// <summary>
    /// 查找在字符后面的字符串
    /// </summary>
    /// <param name="str"></param>
    /// <param name="front"></param>
    /// <returns></returns>
    public static string FindAfter(this string str, string front)
    {
        var startIndex = str.IndexOf(front) + front.Length;
        return startIndex < 0 ? str : str.Substring(startIndex);
    }
}