using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Execution;
using UnityEngine;

namespace Framework.UI.Core.Bind
{
    public class BindLoopViewList<TVm, TView> : BaseBind where TVm : ViewModel where TView : View , new()
    {
        private Dictionary<Transform, View> itemTrans2View = new Dictionary<Transform, View>();
        private ObservableList<TVm> itemsVm;
        private LoopScrollRect loopScrollRect;
        private Type viewType;
        
        public BindLoopViewList()  : base(null)
        {
            viewType = typeof(TView);
        }

        public void SetViewType(Type type)
        {
            viewType = type;
        }
        
        public void Reset(ObservableList<TVm> items, LoopScrollRect loopScrollRect)
        {
            itemsVm = items;
            this.loopScrollRect = loopScrollRect;
            items.AddListener(OnListChanged);
            loopScrollRect.totalCount = items.Count;
            loopScrollRect.OnItemShow += OnItemChanged;
            loopScrollRect.RefillCells();
        }

        private void OnItemChanged(Transform itemTrans, int index)
        {
            if (!itemTrans2View.TryGetValue(itemTrans, out var item))
            {
                item = ReflectionHelper.CreateInstance(viewType) as View;
                itemTrans2View[itemTrans] = item;
                item.SetGameObject(itemTrans.gameObject);
            }
            item.SetVm(itemsVm[index]);
        }

        private Coroutine delayRefreshCoroutine;
        private void OnListChanged(List<TVm> list)
        {
            //加一个延迟协程，防止一阵内多次修改list，造成频繁计算，一帧内修改完后，下一帧再计算一次就可以了
            if (delayRefreshCoroutine == null)
            {
                delayRefreshCoroutine = Executors.RunOnCoroutineReturn(DelayRefreshList());
            }
        }
        
        IEnumerator DelayRefreshList()
        {
            yield return null;
            loopScrollRect.totalCount = itemsVm.Count;
            loopScrollRect.RefreshCells();
            delayRefreshCoroutine = null;
        }

        public override void ClearView()
        {
            loopScrollRect.OnItemShow -= OnItemChanged;
            itemTrans2View.Clear();
        }

        public override void Clear()
        {
            itemsVm.RemoveListener(OnListChanged);
        }
    }
}