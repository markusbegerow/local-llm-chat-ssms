using System.Windows.Media;

namespace LocalLlmChatSsms
{
    public class ChatMessageViewModel
    {
        public string Role { get; set; } = "";
        public string Content { get; set; } = "";
        public Brush Background { get; set; }
    }
}
