using System.Collections.Generic;
using UnityEngine;

namespace Framework.Editor.AssetsChecker
{
    public class RuleDataTable
    {
        private ForDictionary<string, List<string>> datas = new ForDictionary<string, List<string>>();
        public RuleDataTable(params string[] cols)
        {
            foreach (var col in cols)
            {
                if (datas.ContainsKey(col))
                {
                    Debug.LogError("相同的行名" + col);
                    return;
                }
                datas.Add(col, new List<string>());
            }
        }

        public void AddRow(params object[] objs)
        {
            if (objs.Length < datas.Count)
            {
                Debug.LogError("数据数量不符合");
                return;
            }
            for (int i = 0; i < objs.Length; i++)
            {
                datas.GetByIndex(i).Add(objs[i].ToString());
            }
        }
    }
}