
namespace BluePlumGit.Messaging.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using BluePlumGit.Entitys;
    using Livet.Messaging;
    using BluePlumGit.Enums;

    public class WindowOpenMessage : ResponsiveInteractionMessage<RepositoryEntity>
    {
        public WindowOpenMessage()
        {
        }

        public WindowOpenMessage(string messageKey)
            : base(messageKey)
        {
        }

        public WindowTypeEnum WindowType
        {
            get { return (WindowTypeEnum)GetValue(WindowTypeProperty); }
            set { SetValue(WindowTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WindowType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowTypeProperty =
            DependencyProperty.Register("WindowType", typeof(WindowTypeEnum), typeof(WindowOpenMessage), new UIPropertyMetadata(null));

        /// <summary>
        /// 派生クラスでは必ずオーバーライドしてください。Freezableオブジェクトとして必要な実装です。<br/>
        /// 通常このメソッドは、自身の新しいインスタンスを返すように実装します。
        /// </summary>
        /// <returns>自身の新しいインスタンス</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new WindowOpenMessage(MessageKey);
        }

    }
}
