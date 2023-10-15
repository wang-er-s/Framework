using System.Collections;
using System.Collections.Generic;
using ScrollView;
using UnityEngine;

namespace Framework
{
    public class BindLoopGridView<TVm, TloopView> : BaseBind where TVm : ViewModel
        where TloopView : LoopGridItemView
    {
        private Window baseView;
        private Coroutine delayRefreshCoroutine;
        private LoopGrid loopGrid;
        private LoopGridItemPoolBase pool;
        private ObservableList<TVm> vms;

        public override void Init(object container)
        {
            base.Init(container);
            baseView = container as Window;
        }

        public void Reset(ObservableList<TVm> inVms, LoopGrid inLoopList)
        {
            loopGrid = inLoopList;
            vms = inVms;
            vms.AddListener(OnListChanged);
            pool = new LoopGridItemPool<TloopView>();
            pool.OnRecycleItem += OnRecycleItem;
            loopGrid.InitGridView(pool, inVms.Count, OnGetItemByIndex);
        }


        private void OnListChanged(List<TVm> inlist)
        {
            if (delayRefreshCoroutine == null)
                delayRefreshCoroutine = Executors.RunOnCoroutineReturn(DelayRefreshList());
        }

        private IEnumerator DelayRefreshList()
        {
            yield return null;
            loopGrid.SetListItemCount(vms.Count, false);
            loopGrid.RefreshAllShownItem();
            delayRefreshCoroutine = null;
        }


        private LoopGridItemView OnGetItemByIndex(LoopGrid inLoopGrid, int inIndex, int row, int column)
        {
            var view = inLoopGrid.NewListViewItem(out var go);
            view.SetGameObject(go);
            view.SetVm(vms[inIndex]);
            baseView.AddSubView(view);
            view.Visibility = true;
            return view;
        }

        private void OnRecycleItem(LoopGridItemView arg1, GameObject arg2)
        {
            baseView.RemoveSubView(arg1);
        }

        protected override void OnReset()
        {
            loopGrid.Clear();
        }

        protected override void OnClear()
        {
            if (delayRefreshCoroutine != null) Executors.StopCoroutine(delayRefreshCoroutine);
            loopGrid.SetListItemCount(0);
            loopGrid.RefreshAllShownItem();
            loopGrid.Clear();
            vms.RemoveListener(OnListChanged);
            pool.DestroyAllItem();
            pool.OnRecycleItem -= OnRecycleItem;
            delayRefreshCoroutine = null;
            baseView = null;
        }
    }
}