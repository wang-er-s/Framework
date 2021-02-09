using System;
using Framework.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Runtime.UI.Component
{
    [UI("UI_AlertDialog")]
    public class AlertDialogView : View
    {
        public Text Title;

        public Text Message;

        public GameObject Content;

        public Button ConfirmButton;

        public Button NeutralButton;

        public Button CancelButton;

        public Button OutsideButton;

        public bool CanceledOnTouchOutside { get; set; }

        private View contentView;

        private AlertDialogVM vm;

        public override UILevel UILevel { get; } = UILevel.Pop;

        protected virtual void Button_OnClick(int which)
        {
            try
            {
                this.vm.OnClick(which);
            }
            catch (Exception) { }
            finally
            {
                this.Hide();
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