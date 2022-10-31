using System;
using System.IO;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class AlertDialog
    {

        public const int BUTTON_POSITIVE = 1;
        public const int BUTTON_NEGATIVE = -1;
        public const int BUTTON_NEUTRAL = 0;

        private static UIManager GetUIViewLocator()
        {
            return UIManager.Ins;
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="buttonText">The text shown in the only button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="clickCallback">A callback that should be executed after
        /// the dialog box is closed by the user.</param>
        /// <returns>A AlertDialog.</returns>
        public static async Task<AlertDialog> ShowMessage(
            string message,
            string title,
            string buttonText,
            Action<int> clickCallback)
        {
            return await ShowMessage(message, title, buttonText, null, null, false, clickCallback);
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="confirmButtonText">The text shown in the "confirm" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="neutralButtonText">The text shown in the "neutral" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="cancelButtonText">The text shown in the "cancel" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="canceledOnTouchOutside">Whether the dialog box is canceled when 
        /// touched outside the window's bounds. </param>
        /// <param name="clickCallback">A callback that should be executed after
        /// the dialog box is closed by the user. The callback method will get a boolean
        /// parameter indicating if the "confirm" button (true) or the "cancel" button
        /// (false) was pressed by the user.</param>
        /// <returns>A AlertDialog.</returns>
        [ItemCanBeNull]
        public static async Task<AlertDialog> ShowMessage(
            string message,
            string title,
            string confirmButtonText = null,
            string neutralButtonText = null,
            string cancelButtonText = null,
            bool canceledOnTouchOutside = true,
            Action<int> clickCallback = null)
        {
            AlertDialogVM viewModel = new AlertDialogVM();
            viewModel.Message.Value = message;
            viewModel.Title.Value = title;
            viewModel.ConfirmButtonText.Value = confirmButtonText;
            viewModel.NeutralButtonText.Value = neutralButtonText;
            viewModel.CancelButtonText.Value = cancelButtonText;
            viewModel.CanceledOnTouchOutside.Value = canceledOnTouchOutside;
            viewModel.Click = clickCallback;

            return await ShowMessage(viewModel);
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="contentView">The custom content view to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="confirmButtonText">The text shown in the "confirm" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="neutralButtonText">The text shown in the "neutral" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="cancelButtonText">The text shown in the "cancel" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="canceledOnTouchOutside">Whether the dialog box is canceled when 
        /// touched outside the window's bounds. </param>
        /// <param name="clickCallback">A callback that should be executed after
        /// the dialog box is closed by the user. The callback method will get a boolean
        /// parameter indicating if the "confirm" button (true) or the "cancel" button
        /// (false) was pressed by the user.</param>
        /// <returns>A AlertDialog.</returns>
        public static async Task<AlertDialogView> ShowMessage(
            View contentView,
            string title,
            string neutralButtonText,
            string cancelButtonText,
            string confirmButtonText,
            bool canceledOnTouchOutside,
            Action<int> clickCallback)
        {
            AlertDialogVM viewModel = new AlertDialogVM();
            viewModel.Title.Value = title;
            viewModel.ConfirmButtonText.Value = confirmButtonText;
            viewModel.NeutralButtonText.Value = neutralButtonText;
            viewModel.CancelButtonText.Value = cancelButtonText;
            viewModel.CanceledOnTouchOutside.Value = canceledOnTouchOutside;
            viewModel.Click = clickCallback;

            UIManager locator = GetUIViewLocator();
            AlertDialogView window = (await locator.OpenAsync<AlertDialogView>()) as AlertDialogView;
            AlertDialog dialog = new AlertDialog(window, contentView, viewModel);
            dialog.Show();
            return window;
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="viewName">The view name of the dialog box,if it is null, use the default view name</param>
        /// <param name="contentViewName">The custom content view name to be shown to the user.</param>
        /// <param name="viewModel">The view model of the dialog box</param>
        /// <returns>A AlertDialog.</returns>
        public static async Task<AlertDialog> ShowMessage(AlertDialogVM viewModel)
        {
            AlertDialogView view = null;
            try
            {
                UIManager locator = GetUIViewLocator();
                view = await locator.OpenAsync<AlertDialogView>() as AlertDialogView;
                AlertDialog dialog = new AlertDialog(view, null, viewModel);
                dialog.Show();
                return dialog;
            }
            catch (Exception e)
            {
                if (view != null)
                    UIManager.Ins.Close(view);
                Log.Error(e);
                throw;
            }
        }

        public AlertDialogView View { get; private set; }
        private View contentView;
        public AlertDialogVM ViewModel { get; private set; }

        public AlertDialog(AlertDialogView view, AlertDialogVM viewModel) : this(view, null, viewModel)
        {
        }

        public AlertDialog(AlertDialogView view, View contentView, AlertDialogVM viewModel)
        {
            this.View = view;
            this.contentView = contentView;
            this.ViewModel = viewModel;
        }

        public virtual object WaitForClosed()
        {
            return Executors.WaitWhile(() => !this.ViewModel.Closed);
        }

        public void Show()
        {
            this.View.SetVm(ViewModel);
            if (this.contentView != null)
                contentView.AddSubView(View);
            this.View.Show();
        }

        public void Cancel()
        {
            this.View.Cancel();
        }
    }

    [UI("AlertDialog")]
    public class AlertDialogView : View
    {
        [TransformPath("Panel/Title")] private Text Title;

        [TransformPath("Panel/Content")] private Text Message;

        [TransformPath("Panel/ButtonGroup/Confirm")]
        private Button ConfirmButton;

        [TransformPath("Panel/ButtonGroup/Neutral")]
        private Button NeutralButton;

        [TransformPath("Panel/ButtonGroup/Cancel")]
        private Button CancelButton;

        [TransformPath("Background")] private Button OutsideButton;

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
            catch (Exception)
            {
            }
            finally
            {
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

    public class AlertDialogVM : ViewModel
    {
        private int result;
        private Action<int> click;

        /// <summary>
        /// The title of the dialog box. This may be null.
        /// </summary>
        public ObservableProperty<string> Title = new ObservableProperty<string>();

        /// <summary>
        /// The message to be shown to the user.
        /// </summary>
        public ObservableProperty<string> Message = new ObservableProperty<string>();

        /// <summary>
        /// The text shown in the "confirm" button in the dialog box. 
        /// If left null, the button will be invisible.
        /// </summary>
        public ObservableProperty<string> ConfirmButtonText = new ObservableProperty<string>();

        /// <summary>
        /// The text shown in the "neutral" button in the dialog box. 
        /// If left null, the button will be invisible.
        /// </summary>
        public ObservableProperty<string> NeutralButtonText = new ObservableProperty<string>();

        /// <summary>
        /// The text shown in the "cancel" button in the dialog box. 
        /// If left null, the button will be invisible.
        /// </summary>
        public ObservableProperty<string> CancelButtonText = new ObservableProperty<string>();

        /// <summary>
        /// Whether the dialog box is canceled when 
        /// touched outside the window's bounds. 
        /// </summary>
        public ObservableProperty<bool> CanceledOnTouchOutside = new ObservableProperty<bool>();

        /// <summary>
        /// A callback that should be executed after
        /// the dialog box is closed by the user. The callback method will get a boolean
        /// parameter indicating if the "confirm" button (true) or the "cancel" button
        /// (false) was pressed by the user.
        /// </summary>
        public Action<int> Click;

        /// <summary>
        /// The dialog box has been closed.
        /// </summary>
        public ObservableProperty<bool> Closed = new ObservableProperty<bool>();

        /// <summary>
        /// result
        /// </summary>
        public virtual int Result
        {
            get { return this.result; }
        }

        public virtual void OnClick(int which)
        {
            try
            {
                this.result = which;
                this.Click?.Invoke(which);
            }
            catch (Exception)
            {
            }
            finally
            {
                this.Closed.Value = true;
            }
        }
    }
}