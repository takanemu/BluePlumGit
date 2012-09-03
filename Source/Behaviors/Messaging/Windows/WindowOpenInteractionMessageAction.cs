
namespace BluePlumGit.Behaviors.Messaging.Windows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Livet.Behaviors.Messaging;
    using System.Windows;
    using Livet.Messaging;
    using BluePlumGit.Messaging.Windows;

    public class WindowOpenInteractionMessageAction : InteractionMessageAction<DependencyObject>
    {
        protected override void InvokeAction(InteractionMessage message)
        {
            if (!(message is WindowOpenMessage))
            {
                return;
            }
            WindowOpenMessage windowOpenMessage = (WindowOpenMessage)message;

            Window window = (Window)Activator.CreateInstance(windowOpenMessage.WindowType);   

            window.Owner = (Window)this.AssociatedObject;

            bool? result = window.ShowDialog();

            windowOpenMessage.Response = null;
        }
    }
}
