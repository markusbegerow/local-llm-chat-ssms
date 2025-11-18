using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LocalLlmChatSsms
{
    public partial class ChatWindowControl : UserControl
    {
        private readonly ObservableCollection<ChatMessageViewModel> _messages;
        private readonly SlashCommandHandler _commandHandler;
        private CancellationTokenSource _cancellationTokenSource;

        public ChatWindowControl()
        {
            InitializeComponent();

            // Initialize messages collection
            _messages = new ObservableCollection<ChatMessageViewModel>();
            MessagesList.ItemsSource = _messages;

            // Initialize command handler (will read config dynamically)
            _commandHandler = new SlashCommandHandler(null, null);

            // Add system message on startup
            AddMessage("system", "Local LLM Chat ready! Type /help for available commands.");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var text = InputBox.Text?.Trim();
            if (string.IsNullOrEmpty(text))
                return;

            // Check for slash commands
            if (SlashCommandHandler.IsCommand(text))
            {
                InputBox.Clear();
                AddMessage("user", text);

                var result = _commandHandler.ProcessCommand(text);

                if (result == "CLEAR_CONVERSATION")
                {
                    ClearConversation();
                    AddMessage("system", "Conversation cleared.");
                }
                else if (result != null)
                {
                    AddMessage("system", result);
                }

                return;
            }

            // Normal chat flow
            AddMessage("user", text);
            InputBox.Clear();

            // Disable send button during request
            SendButton.IsEnabled = false;

            try
            {
                // Cancel any existing request
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                // Get fresh config from SettingsManager
                var config = SettingsManager.Instance.Config;

                // Trim history to prevent context overflow
                Utils.TrimHistory(_messages, config.MaxHistoryLength);

                // Build conversation history
                var history = _messages
                    .Where(m => m.Role.ToLower() != "system" || m.Content.StartsWith("Local LLM") == false)
                    .Select(m => (m.Role.ToLower(), m.Content))
                    .ToList();

                // Add system prompt if not present
                if (!history.Any(m => m.Item1 == "system"))
                {
                    history.Insert(0, ("system", config.SystemPrompt));
                }

                // Show loading indicator
                var loadingMsg = AddMessage("assistant", "Thinking...");

                // Create new LLM client with fresh config to ensure latest settings are used
                var llmClient = new LlmClient(config);

                // Call LLM
                var response = await llmClient.ChatAsync(history, _cancellationTokenSource.Token);

                // Remove loading indicator
                _messages.Remove(loadingMsg);

                // Add actual response
                AddMessage("assistant", response);

                // Auto-scroll to bottom
                ScrollToBottom();
            }
            catch (OperationCanceledException)
            {
                AddMessage("system", "Request cancelled.");
            }
            catch (TimeoutException ex)
            {
                AddMessage("system", $"Timeout: {ex.Message}");
            }
            catch (Exception ex)
            {
                AddMessage("system", $"Error: {ex.Message}");
            }
            finally
            {
                SendButton.IsEnabled = true;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private ChatMessageViewModel AddMessage(string role, string content)
        {
            var bg = role switch
            {
                "user" => new SolidColorBrush(Color.FromRgb(220, 240, 255)),
                "assistant" => new SolidColorBrush(Color.FromRgb(235, 235, 235)),
                "system" => new SolidColorBrush(Color.FromRgb(255, 240, 220)),
                _ => new SolidColorBrush(Color.FromRgb(245, 245, 245))
            };

            var message = new ChatMessageViewModel
            {
                Role = role,
                Content = content,
                Background = bg
            };

            _messages.Add(message);
            return message;
        }

        private void ClearConversation()
        {
            _messages.Clear();
        }

        private void ScrollToBottom()
        {
            // Auto-scroll logic would go here if ScrollViewer is named
            // For now, WPF will handle it automatically with the ItemsControl
        }

        // Handle Enter key in InputBox
        private void InputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter &&
                !System.Windows.Input.Keyboard.Modifiers.HasFlag(System.Windows.Input.ModifierKeys.Shift))
            {
                e.Handled = true;
                SendButton_Click(sender, e);
            }
        }
    }
}
