using System.Collections;
using System.Collections.Generic;
using SuperScrollView;
using UnityEngine;

namespace Framework
{
    public class BindLoopListView<TVm, TLoopView> : BaseBind where TVm : ViewModel
        where TLoopView : LoopListViewItem2
    {
        private Window baseView;
        private Coroutine delayRefreshCoroutine;
        private ObservableList<TVm> vms;
        private LoopListView2 loopListView;
        private GameObject template;

        public override void Init(object container)
        {
            base.Init(container);
            baseView = container as Window;
        }

        public void Reset(ObservableList<TVm> inVms, LoopListView2 loopListView2, GameObject item)
        {
            loopListView = loopListView2;
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
            if (!loopListView.IsInited)
            {
                loopListView.InitListView(vms.Count, OnGetItemByIndex,
                    () => baseView.Domain.GetComponent<UIComponent>()
                        .CreateViewWithGo<TLoopView>(null, Object.Instantiate(template)),
                    template.name);
            }

            delayRefreshCoroutine = null;
            loopListView.SetListItemCount(vms.Count);
            loopListView.RefreshAllShownItem();
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

        LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= vms.Count)
            {
                return null;
            }

            var vm = vms[index];
            LoopListViewItem2 item = listView.NewListViewItem(template.name);
            item.SetVm(vm);
            return item;
        }
    }
}