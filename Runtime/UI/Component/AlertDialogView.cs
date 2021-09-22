using System;
using DG.Tweening;
using DG.Tweening.Core;
using Framework.Asynchronous;
using Framework.UI.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Runtime.UI.Component
{
    [UI("AlertDialog")]
    public class AlertDialogView : View
    {
        [TransformPath("Panel/Title")]
        private Text Title;

        [TransformPath("Panel/Content")]
        private Text Message;

        [TransformPath("Panel/ButtonGroup/Confirm")]
        private Button ConfirmButton;

        [TransformPath("Panel/ButtonGroup/Neutral")]
        private Button NeutralButton;

        [TransformPath("Panel/ButtonGroup/Cancel")]
        private Button CancelButton;

        [TransformPath("Background")]
        private Button OutsideButton;

        public bool CanceledOnTouchOutside { get; set; }

        private View contentView;
        
        private AlertDialogVM vm;

        private ABSAnimationComponent tween;

        protected override void Start()
        {
            tween = Go.GetComponentInChildren<ABSAnimationComponent>();
        }

        protected override void OnShow()
        {
            if (tween)
                tween.DOPlay();
        }

        public override UILevel UILevel { get; } = UILevel.Pop;

        protected virtual async void Button_OnClick(int which)
        {
            try
            {
                this.vm.OnClick(which);
            }
            catch (Exception) { }
            finally
            {
                if (tween)
                {
                    tween.DOPlayBackwards();
                    await new WaitForSeconds(tween.tween.Duration());
                }
                this.Close();
            }
        }

        public virtual void Cancel()
        {
            this.Button_OnClick(AlertDialog.BUTTON_NEGATIVE);
        }

        protected override void OnVmChange()
        {
            vm = ViewModel as AlertDialogVM;
            if (this.Message != null)
            {
                if (!string.IsNullOrEmpty(vm.Message))
                {
                    this.Message.gameObject.SetActive(true);
                    this.Message.text = this.vm.Message;
                }
                else
                    this.Message.gameObject.SetActive(false);
            }

            if (this.Title != null)
            {
                if (!string.IsNullOrEmpty(vm.Title))
                {
                    this.Title.gameObject.SetActive(true);
                    this.Title.text = this.vm.Title;
                }
                else
                    this.Title.gameObject.SetActive(false);
            }

            if (this.ConfirmButton != null)
            {
                if (!string.IsNullOrEmpty(vm.ConfirmButtonText))
                {
                    this.ConfirmButton.gameObject.SetActive(true);
                    this.ConfirmButton.onClick.AddListener(() => { this.Button_OnClick(AlertDialog.BUTTON_POSITIVE); });
                    Text text = this.ConfirmButton.GetComponentInChildren<Text>();
                    if (text != null)
                        text.text = this.vm.ConfirmButtonText;
                }
                else
                {
                    this.ConfirmButton.gameObject.SetActive(false);
                }
            }

            if (this.CancelButton != null)
            {
                if (!string.IsNullOrEmpty(this.vm.CancelButtonText))
                {
                    this.CancelButton.gameObject.SetActive(true);
                    this.CancelButton.onClick.AddListener(() => { this.Button_OnClick(AlertDialog.BUTTON_NEGATIVE); });
                    Text text = this.CancelButton.GetComponentInChildren<Text>();
                    if (text != null)
                        text.text = this.vm.CancelButtonText;
                }
                else
                {
                    this.CancelButton.gameObject.SetActive(false);
                }
            }

            if (this.NeutralButton != null)
            {
                if (!string.IsNullOrEmpty(this.vm.NeutralButtonText))
                {
                    this.NeutralButton.gameObject.SetActive(true);
                    this.NeutralButton.onClick.AddListener(() => { this.Button_OnClick(AlertDialog.BUTTON_NEUTRAL); });
                    Text text = this.NeutralButton.GetComponentInChildren<Text>();
                    if (text != null)
                        text.text = this.vm.NeutralButtonText;
                }
                else
                {
                    this.NeutralButton.gameObject.SetActive(false);
                }
            }

            this.CanceledOnTouchOutside = this.vm.CanceledOnTouchOutside;
            if (this.OutsideButton != null && this.CanceledOnTouchOutside)
            {
                this.OutsideButton.gameObject.SetActive(true);
                this.OutsideButton.interactable = true;
                this.OutsideButton.onClick.AddListener(() => { this.Button_OnClick(AlertDialog.BUTTON_NEGATIVE); });
            }
        }

        public override bool IsSingle { get; } = false;
        
    }
}