using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Interactivity;
using System.Windows;
using System.Windows.Input;

namespace Livet.Behaviors
{
    /// <summary>
    /// WindowのClose処理をキャンセルし、キャンセルした事をCallback通知可能なビヘイビアです。
    /// </summary>
    public class WindowCloseCancelBehavior : Behavior<Window>
    {
        Action<object> _callbackMethodAction;

        /// <summary>
        /// このWindowを閉じることが可能かどうかを取得、または設定します。このプロパティがfalseを示す場合、WindowClose処理はキャンセルされます。デフォルト値はtrueです。
        /// </summary>
        public bool CanClose
        {
            get { return (bool)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CanClose.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(bool), typeof(WindowCloseCancelBehavior), new UIPropertyMetadata(true));

        /// <summary>
        /// Windowを閉じる処理がこのビヘイビアによってキャンセルされた場合に実行するコマンドを取得、または設定します。
        /// </summary>
        public ICommand CloseCanceledCallbackCommand
        {
            get { return (ICommand)GetValue(CloseCanceledCallbackCommandProperty); }
            set { SetValue(CloseCanceledCallbackCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCanceledCallbackCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCanceledCallbackCommandProperty =
            DependencyProperty.Register("CloseCanceledCallbackCommand", typeof(ICommand), typeof(WindowCloseCancelBehavior), new UIPropertyMetadata(null));

        /// <summary>
        /// Windowを閉じる処理がこのビヘイビアによってキャンセルされた場合に実行するメソッドを持つオブジェクトを取得、または設定します。
        /// </summary>
        public object CloseCanceledCallbackMethodTarget
        {
            get { return (object)GetValue(CloseCanceledCallbackMethodTargetProperty); }
            set { SetValue(CloseCanceledCallbackMethodTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCanceledCallbackMethodTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCanceledCallbackMethodTargetProperty =
            DependencyProperty.Register("CloseCanceledCallbackMethodTarget", typeof(object), typeof(WindowCloseCancelBehavior), new UIPropertyMetadata(null, CloseCanceledCallbackMethodChanged));

        /// <summary>
        /// Windowを閉じる処理がこのビヘイビアによってキャンセルされた場合に実行するメソッドの名前を取得、または設定します。
        /// </summary>
        public string CloseCanceledCallbackMethodName
        {
            get { return (string)GetValue(CloseCanceledCallbackMethodNameProperty); }
            set { SetValue(CloseCanceledCallbackMethodNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCanceledCallbackMethodName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCanceledCallbackMethodNameProperty =
            DependencyProperty.Register("CloseCanceledCallbackMethodName", typeof(string), typeof(WindowCloseCancelBehavior), new UIPropertyMetadata(null, CloseCanceledCallbackMethodChanged));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += (sender, e) =>
                {
                    if (!CanClose)
                    {
                        if (CloseCanceledCallbackCommand != null && CloseCanceledCallbackCommand.CanExecute(null))
                        {
                            CloseCanceledCallbackCommand.Execute(null);
                        }

                        if (_callbackMethodAction == null)
                        {
                            UpdateMethodInfo();
                        }

                        if (_callbackMethodAction == null && CloseCanceledCallbackMethodTarget != null)
                        {
                            _callbackMethodAction(CloseCanceledCallbackMethodTarget);
                        }

                        e.Cancel = true;
                    }
                };
        }

        private void UpdateMethodInfo()
        {
            if(CloseCanceledCallbackMethodTarget == null || string.IsNullOrEmpty(CloseCanceledCallbackMethodName)) return;

            _callbackMethodAction = LambdaCreator.CreateActionLambda<object>(CloseCanceledCallbackMethodTarget.GetType(), CloseCanceledCallbackMethodName, false);
        }

        private static void CloseCanceledCallbackMethodChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var thisReference = (WindowCloseCancelBehavior)sender;

            if (thisReference == null) return;

            thisReference.UpdateMethodInfo();
        }
    }
}
