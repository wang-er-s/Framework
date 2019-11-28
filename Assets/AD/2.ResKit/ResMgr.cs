using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AD.ResKit
{
    
    public static class ResMgr
    {
        public static Object Load(string assetPath)
        {
            return Resources.Load(assetPath);
        }

        public static T Load<T>(string assetPath) where T : Object
        {
            return Resources.Load<T>(assetPath);
        }
        
    }
}
