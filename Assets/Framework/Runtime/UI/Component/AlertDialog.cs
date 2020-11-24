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

        public const int BUTTON_POSITIVE = -1;
        public const int BUTTON_NEGATIVE = -2;
        public const int BUTTON_NEUTRAL = -3;

        private const string DEFAULT_VIEW_LOCATOR_KEY = "_DEFAULT_VIEW_LOCATOR";
        private const string DEFAULT_VIEW_NAME = "AlertDialog";

        private static string viewName;
        public static string ViewName
        {
            get => string.IsNullOrEmpty(viewName) ? DEFAULT_VIEW_NAME : viewName;
            set => viewName = value;
        }

        private static IViewLocator GetUIViewLocator()
        {
            ApplicationContext context = Context.GetApplicationContext();
            IViewLocator locator = context.GetService<IViewLocator>();
            return locator;
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="buttonText">The text shown in the only button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user.</param>
        /// <returns>A AlertDialog.</returns>
        public static async Task<AlertDialog> ShowMessage(
            string message,
            string title,
            string buttonText,
            Action<int> afterHideCallback)
        {
            return await ShowMessage(message, title, buttonText, null, null, false, afterHideCallback);
        }

        /// <summary>
        /// Displays information to the user.
        /// </summary>
        /// <param name="message">The message to be shown to the user.</param>
        /// <param name="title">The title of the dialog box. This may be null.</param>
        /// <param name="confirmButtonText">The text shown in the "confirm" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="cancelButtonText">The text shown in the "cancel" button
        /// in the dialog box. If left null, the button will be invisible.</param>
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user. The callback method will get a boolean
        /// parameter indicating if the "confirm" button (true) or the "cancel" button
        /// (false) was pressed by the user.</param>
        /// <returns>A AlertDialog.</returns>
        public static async Task<AlertDialog> ShowMessage(
            string message,
            string title,
            string confirmButtonText,
            string cancelButtonText,
            Action<int> afterHideCallback)
        {
            return await ShowMessage(message, title, confirmButtonText, null, cancelButtonText, false, afterHideCallback);
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
        /// <param name="afterHideCallback">A callback that should be executed after
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
            Action<int> afterHideCallback = null)
        {
            AlertDialogVM viewModel = new AlertDialogVM();
            viewModel.Message.Value = message;
            viewModel.Title.Value = title;
            viewModel.ConfirmButtonText.Value = confirmButtonText;
            viewModel.NeutralButtonText.Value = neutralButtonText;
            viewModel.CancelButtonText.Value = cancelButtonText;
            viewModel.CanceledOnTouchOutside.Value = canceledOnTouchOutside;
            viewModel.Click = afterHideCallback;

            return await ShowMessage(ViewName, viewModel);
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
        /// <param name="afterHideCallback">A callback that should be executed after
        /// the dialog box is closed by the user. The callback method will get a boolean
        /// parameter indicating if the "confirm" button (true) or the "cancel" button
        /// (false) was pressed by the user.</param>
        /// <returns>A AlertDialog.</returns>
        public static async Task<AlertDialogView> ShowMessage(
            View contentView,
            string title,
            string confirmButtonText,
            string neutralButtonText,
            string cancelButtonText,
            bool canceledOnTouchOutside,
            Action<int> afterHideCallback)
        {
            AlertDialogVM viewModel = new AlertDialogVM();
            viewModel.Title.Value = title;
            viewModel.ConfirmButtonText.Value = confirmButtonText;
            viewModel.NeutralButtonText.Value = neutralButtonText;
            viewModel.CancelButtonText.Value = cancelButtonText;
            viewModel.CanceledOnTouchOutside.Value = canceledOnTouchOutside;
            viewModel.Click = afterHideCallback;
            
            IViewLocator locator = GetUIViewLocator();
            AlertDialogView window = await locator.LoadViewAsync<AlertDialogView>(ViewName);
            if (window == null)
            {
                Log.Warning($"Not found the dialog window named \"{viewModel}\".");

                throw new FileNotFoundException($"Not found the dialog window named \"{viewName}\".");
            }

            AlertDialog dialog = new AlertDialog(window, contentView, viewModel);
            dialog.Show();
            return window;
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="viewModel">The view model of the dialog box</param>
        /// <returns>A AlertDialog.</returns>
        public static async Task<AlertDialog> ShowMessage(AlertDialogVM viewModel)
        {
            return await ShowMessage(ViewName, viewModel);
        }

        /// <summary>
        /// Displays information to the user. 
        /// </summary>
        /// <param name="viewName">The view name of the dialog box,if it is null, use the default view name</param>
        /// <param name="contentViewName">The custom content view name to be shown to the user.</param>
        /// <param name="viewModel">The view model of the dialog box</param>
        /// <returns>A AlertDialog.</returns>
        public static async Task<AlertDialog> ShowMessage(string viewName, AlertDialogVM viewModel)
        {
            AlertDialogView view = null;
            try
            {
                if (string.IsNullOrEmpty(viewName))
                    viewName = ViewName;

                IViewLocator locator = GetUIViewLocator();
                view = await locator.LoadViewAsync<AlertDialogView>(viewName);
                if (view == null)
                {
                    Log.Warning($"Not found the dialog window named \"{viewName}\".");

                    throw new FileNotFoundException($"Not found the dialog window named \"{viewName}\".");
                }
                
                AlertDialog dialog = new AlertDialog(view, null, viewModel);
                dialog.Show();
                return dialog;
            }
            catch (Exception e)
            {
                if (view != null)
                    view.Destroy();
                Log.Error(e);
                throw;
            }
        }

        private AlertDialogView view;
        private View contentView;
        private AlertDialogVM viewModel;

        public AlertDialog(AlertDialogView view, AlertDialogVM viewModel) : this(view, null, viewModel)
        {
        }

        public AlertDialog(AlertDialogView view, View contentView, AlertDialogVM viewModel)
        {
            this.view = view;
            this.contentView = contentView;
            this.viewModel = viewModel;
        }

        public virtual object WaitForClosed()
        {
            return Executors.WaitWhile(() => !this.viewModel.Closed);
        }

        public void Show()
        {
            this.view.SetVm(viewModel);
            if (this.contentView != null)
                contentView.AddSubView(view);
            this.view.Show();
        }

        public void Cancel()
        {
            this.view.Cancel();
        }
    }
}