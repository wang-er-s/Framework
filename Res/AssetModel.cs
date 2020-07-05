using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class AssetModel
    {
        #region Var
        public readonly string ModelRes;
        //public string ModelRes => modelRes;

        public GameObject gameObj { get; protected set; }
        public Transform trans { get; protected set;}
        private ulong gcbid;
        private static string EMPTY_RES = string.Empty;
        public bool IsLoaded
        {
            get { return gameObj != null; }
        }
        #endregion
        public AssetModel(string modelRes)
        {
            ModelRes = modelRes??EMPTY_RES;
        }
        
        public void Load(Action onLoaded = null, bool doneOnUpdate = false)
        {
            if (string.IsNullOrEmpty(ModelRes))
            {
                gameObj = new GameObject();
                trans = gameObj.transform;
                onLoaded?.Invoke();
                return;
            }
            gcbid = GameObjPool.Ins.GetGameObj(ModelRes, (go, cbId) =>
            {
                gcbid = 0;
                if (gameObj == null)
                {
                    var fileName = Path.GetFileName(ModelRes);
                    gameObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    gameObj.name = $"error: {fileName}";
                }
                if (null != gameObj)
                {
                    trans = gameObj.transform;
                }

                onLoaded?.Invoke();
            },doneOnUpdate);
        }
        public void LoadSync()
        {
            if (string.IsNullOrEmpty(ModelRes))
            {
                gameObj = new GameObject();
                trans = gameObj.transform;
                return;
            }

            gameObj = GameObjPool.Ins.GetGameObj(ModelRes);
            trans = gameObj == null ? null : gameObj.transform;
        }
        public virtual void Release(bool reserve = true)
        {
            trans = null;
            if (null != gameObj)
            {
                if (reserve&&null!=GameObjPool.Ins&&!string.IsNullOrEmpty(ModelRes))
                {
                    GameObjPool.Ins.UnuseGameObj(this.ModelRes, gameObj);
                }
                else
                {
                    GameObject.Destroy(gameObj);
                    BundleMgr.Instance.ReleaseAsset(ModelRes,true,1);
                }
                gameObj = null;
            }
            if (gcbid > 0)
            {
                if(null!=GameObjPool.Ins)
                    GameObjPool.Ins.CancelUngotGameObj(gcbid, reserve);
                gcbid = 0;
            }
        }
    }
}
