using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Framework.BaseUtil;
using Framework.Util;
using UnityEngine;

namespace Framework
{
    public class PathIdProfile : Singleton<PathIdProfile>
    {
        public PathIdProfile()
        {
        }
        
        public static readonly string fileName = "pathid.info";
        private bool validated = false;
        private Dictionary<string,string> ids = new Dictionary<string, string>(); //path ->id
        private bool dirty = false;
        private bool updating = false;
        private SHA1 sha1 = null;
        
        private Dictionary<string, string> toRemove = null;
        
        private Dictionary<string,string> id2Path = new Dictionary<string, string>();//id->path

        private static readonly char[] sep = new[] {':'};

        public void Clear()
        {
            validated = false;
            ids.Clear();
            id2Path.Clear();
            dirty = false;
            updating = false;
            sha1 = null;
        }

        public void Load()
        {
            Clear();
            string filePath = FileUtils.GetFileReadFullPath(BundleConfig.RootPath+ fileName);
            Import(filePath);
#if UNITY_EDITOR
            if (null != BundleConfig.TrdPath)
            {
                filePath = FileUtils.GetFileReadPath(BundleConfig.TrdPath + fileName, false);
                Import(filePath);
            }
#endif
            if (validated&&AppEnv.ResVerbose)
            {
                foreach (var kvp in ids)
                {
                    id2Path.Add(kvp.Value,kvp.Key+BundleConfig.bundleFileExt);
                }
            }
        }

        public bool IsDirty()
        {
            if (dirty)
                return true;
            if (null != toRemove && toRemove.Count > 0)
                return true;
            return false;
        }

        public void MarkUpdating(bool validated)
        {
            this.validated = validated;
            updating = true;
            toRemove = new Dictionary<string, string>(ids);
            sha1 = SHA1.Create();
        }

        public void FinishUpdating()
        {
            if (null != sha1)
            {
                sha1.Dispose();
                sha1 = null;
            }
        }

        public string GetPathId(string path)
        {
            if (!validated)
                return path;
            if (ids.ContainsKey(path))
            {
                if (updating)
                {
                    if (toRemove.ContainsKey(path))
                        toRemove.Remove(path);
                }
                return ids[path];
            }

            if (updating)
            {
                string pathId = GenPathId(path);
                ids.Add(path,pathId);
                dirty = true;
                return pathId;
            }

            return null;
        }

        public string GetPath(string pathId)
        {
            if (!validated)
                return pathId;
            if (id2Path.ContainsKey(pathId))
                return id2Path[pathId];
            return pathId;
        }

        private string GenPathId(string path)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(path);
            var hash = sha1.ComputeHash(bytes);
            string name = BitConverter.ToString(hash).Replace("-", "").ToLower();

            string dir = name.Substring(0, 2);//是有前2位作为目录
            string file = name.Substring(2);
            return $"{dir}/{file}";
        }
        
        public void Import(string file)
        {
            try
            {
                using (var stream = FileUtils.OpenFile(file))
                {
                    if (null != stream)
                    {
                        using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                        {
                            while (sr.Peek() > -1)
                            {
                                string line = sr.ReadLine();
                                if (string.IsNullOrEmpty(line))
                                    continue;
                                string[] parts = line.Split(sep);
                                if(parts.Length!=2)
                                    continue;
                                if(!ids.ContainsKey(parts[0]))
                                {
                                    ids.Add(parts[0],parts[1]);
                                }
                            }
                        }
                    }
                }

                validated = ids.Count > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
        
        public void Export(string file)
        {
            foreach (var kvp in toRemove)
            {
                ids.Remove(kvp.Key);
            }
            StreamWriter sw = File.CreateText(file);
            foreach (var kvp in ids)
            {
                sw.WriteLine($"{kvp.Key}:{kvp.Value}");
            }
            sw.Close();
        }
    }
}