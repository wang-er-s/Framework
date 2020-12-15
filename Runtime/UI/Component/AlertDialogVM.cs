using System;
using Framework.UI.Core;
using Framework.UI.Core.Bind;

namespace Framework.Runtime.UI.Component
{
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
            catch (Exception) { }
            finally
            {
                this.Closed.Value = true;
            }
        }
    }
}