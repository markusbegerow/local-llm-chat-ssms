using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace LocalLlmChatSsms
{
    [Guid("9362f942-17f5-4c80-8aab-6fa332fa488f")]
    public class ChatWindow : ToolWindowPane
    {
        public ChatWindow() : base(null)
        {
            this.Caption = "Local LLM Chat";
            this.Content = new ChatWindowControl();
        }
    }
}
