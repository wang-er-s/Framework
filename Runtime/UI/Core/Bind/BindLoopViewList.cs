using System.Collections;
using System.Collections.Generic;
using Framework.Execution;
using UnityEngine;

namespace Framework.UI.Core.Bind
{
    public class BindLoopViewList<TVm, TView> : BaseBind where TVm : ViewModel where TView : View , new()
    {
        private Dictionary<Transform, TView> itemTrans2View = new Dictionary<Transform, TView>();
        private ObservableList<TVm> itemsVm;
        private LoopScrollRect loopScrollRect;
        
        public BindLoopViewList()  : base(null)
        {
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
                item = new TView();
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

        public override void ClearModel()
        {
            itemsVm.RemoveListener(OnListChanged);
        }
    }
}