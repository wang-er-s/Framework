using System;
using System.IO;
using System.Threading.Tasks;
using Framework.Asynchronous;
using Framework.Contexts;
using Framework.Execution;
using Framework.UI.Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Framework.Runtime.UI.Component
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
        private AlertDialogVM viewModel;

        public AlertDialog(AlertDialogView view, AlertDialogVM viewModel) : this(view, null, viewModel)
        {
        }

        public AlertDialog(AlertDialogView view, View contentView, AlertDialogVM viewModel)
        {
            this.View = view;
            this.contentView = contentView;
            this.viewModel = viewModel;
        }

        public virtual object WaitForClosed()
        {
            return Executors.WaitWhile(() => !this.viewModel.Closed);
        }

        public void Show()
        {
            this.View.SetVm(viewModel);
            if (this.contentView != null)
                contentView.AddSubView(View);
            this.View.Show();
        }

        public void Cancel()
        {
            this.View.Cancel();
        }
    }
}