using System.Collections.Generic;
using System.Text;
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

        public RecyclableList<string> GetDataByRow(int row)
        {
            RecyclableList<string> result = RecyclableList<string>.Create();
            datas.ForeachValue(list => result.Add(list[row]));
            return result;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            datas.ForeachKey(key => sb.Append($"{key}\t"));
            sb.AppendLine();
            for (int i = 0; i < DataColumnCount; i++)
            {
                using (var data = GetDataByRow(i))
                {
                    foreach (var str in data)
                    {
                        sb.Append($"{str}\t");
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public int DataColumnCount => datas.Count <= 0 ? 0 : datas.GetByIndex(0).Count;
    }
}