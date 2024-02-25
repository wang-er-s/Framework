using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;

namespace Framework
{
    public class BindLoopGridView<TVm, TLoopView> : BaseBind where TVm : ViewModel
        where TLoopView : LoopGridViewItem
    {
        private Window baseView;
        private Coroutine delayRefreshCoroutine;
        private ObservableList<TVm> vms;
        private LoopGridView loopGridView;
        private GameObject template;

        public override void Init(object container)
        {
            base.Init(container);
            baseView = container as Window;
        }

        public void Reset(ObservableList<TVm> inVms, LoopGridView loopGridView, GameObject item)
        {
            this.loopGridView = loopGridView;
            template = item;
            vms = inVms;
            vms.AddListener(OnListChanged);
        }

        private void OnListChanged(List<TVm> inlist)
        {
            if (delayRefreshCoroutine == null)
                delayRefreshCoroutine = Executors.RunOnCoroutineReturn(DelayRefreshList());
        }

        private IEnumerator DelayRefreshList()
        {
            yield return null;
            if (!loopGridView.IsInited)
            {
                loopGridView.InitGridView(vms.Count, OnGetItemByRowColumn,
                    () => baseView.Domain.GetComponent<UIComponent>()
                        .CreateViewWithGo<TLoopView>(null, Object.Instantiate(template)),
                    template.name);
            }

            delayRefreshCoroutine = null;
            loopGridView.SetListItemCount(vms.Count);
            loopGridView.RefreshAllShownItem();
        }

        protected override void OnReset()
        {
        }

        protected override void OnClear()
        {
            if (delayRefreshCoroutine != null) Executors.StopCoroutine(delayRefreshCoroutine);
            vms?.RemoveListener(OnListChanged);
            vms = null;
            delayRefreshCoroutine = null;
            baseView = null;
        }

        LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index, int row, int column)
        {
            if (index < 0 || index >= vms.Count)
            {
                return null;
            }

            var vm = vms[index];
            var item = loopGridView.NewListViewItem(template.name);
            item.SetVm(vm);
            return item;
        }
    }
}