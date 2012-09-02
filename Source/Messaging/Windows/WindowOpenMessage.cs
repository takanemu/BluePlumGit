using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Livet.Messaging;

namespace BluePlumGit.Messaging.Windows
{
    public class WindowOpenMessage : ResponsiveInteractionMessage<string[]>
    {
        public WindowOpenMessage()
        {
        }

        public WindowOpenMessage(string messageKey)
            : base(messageKey)
        {
        }
    }
}
