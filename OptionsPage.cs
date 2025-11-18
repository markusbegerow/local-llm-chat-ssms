using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;

namespace LocalLlmChatSsms
{
    /// <summary>
    /// Options page for Local LLM Chat settings in Tools -> Options
    /// </summary>
    public class OptionsPage : DialogPage
    {
        private const string GeneralCategory = "General";
        private const string AdvancedCategory = "Advanced";
        private const string AuthenticationCategory = "Authentication";


        [Category(GeneralCategory)]
        [DisplayName("API Provider")]
        [Description("The LLM API provider to use (Ollama, LM Studio, OpenAI Compatible, or Custom)")]
        [DefaultValue(ApiProvider.Ollama)]
        public ApiProvider Provider { get; set; } = ApiProvider.Ollama;

        [Category(GeneralCategory)]
        [DisplayName("API URL")]
        [Description("The base URL for the LLM API endpoint (e.g., http://localhost:11434)")]
        [DefaultValue("http://localhost:11434")]
        public string ApiUrl { get; set; } = "http://localhost:11434";

        [Category(GeneralCategory)]
        [DisplayName("Model Name")]
        [Description("The name of the model to use (e.g., llama3, codellama, mistral)")]
        [DefaultValue("llama3")]
        public string ModelName { get; set; } = "llama3";

        [Category(GeneralCategory)]
        [DisplayName("Temperature")]
        [Description("Controls randomness in responses. Lower values (0.0) are more focused, higher values (2.0) are more creative.")]
        [DefaultValue(0.7)]
        public double Temperature { get; set; } = 0.7;

        [Category(AdvancedCategory)]
        [DisplayName("Max Tokens")]
        [Description("Maximum number of tokens in the response")]
        [DefaultValue(2048)]
        public int MaxTokens { get; set; } = 2048;

        [Category(AdvancedCategory)]
        [DisplayName("Timeout (seconds)")]
        [Description("Request timeout in seconds")]
        [DefaultValue(120)]
        public int TimeoutSeconds { get; set; } = 120;

        [Category(AdvancedCategory)]
        [DisplayName("Max History Length")]
        [Description("Maximum number of messages to keep in conversation history")]
        [DefaultValue(50)]
        public int MaxHistoryLength { get; set; } = 50;

        [Category(GeneralCategory)]
        [DisplayName("System Prompt")]
        [Description("The system prompt that defines the assistant's behavior and expertise")]
        [DefaultValue("You are a helpful SQL Server and T-SQL assistant inside SSMS. You provide expert advice on SQL Server, T-SQL queries, database design, performance optimization, and troubleshooting.")]
        public string SystemPrompt { get; set; } =
            "You are a helpful SQL Server and T-SQL assistant inside SSMS. " +
            "You provide expert advice on SQL Server, T-SQL queries, database design, " +
            "performance optimization, and troubleshooting.";

        [Category(AuthenticationCategory)]
        [DisplayName("Bearer Token")]
        [Description("Optional bearer token for API authentication (leave empty if not required)")]
        [DefaultValue("")]
        [PasswordPropertyText(true)]
        public string BearerToken { get; set; } = "";

        /// <summary>
        /// Called when settings are saved
        /// </summary>
        public override void SaveSettingsToStorage()
        {
            base.SaveSettingsToStorage();

            try
            {
                // Also save to SettingsManager so the runtime sees the changes
                var config = new LlmConfig
                {
                    Provider = this.Provider,
                    ApiUrl = this.ApiUrl,
                    ModelName = this.ModelName,
                    Temperature = this.Temperature,
                    MaxTokens = this.MaxTokens,
                    TimeoutSeconds = this.TimeoutSeconds,
                    MaxHistoryLength = this.MaxHistoryLength,
                    SystemPrompt = this.SystemPrompt,
                    BearerToken = this.BearerToken
                };
                SettingsManager.Instance.UpdateSettings(config);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving to SettingsManager: {ex.Message}");
            }
        }

        /// <summary>
        /// Called when settings are loaded
        /// </summary>
        public override void LoadSettingsFromStorage()
        {
            base.LoadSettingsFromStorage();

            try
            {
                // If this is the first time, initialize from SettingsManager
                if (string.IsNullOrEmpty(this.ApiUrl))
                {
                    var config = SettingsManager.Instance.Config;
                    this.Provider = config.Provider;
                    this.ApiUrl = config.ApiUrl;
                    this.ModelName = config.ModelName;
                    this.Temperature = config.Temperature;
                    this.MaxTokens = config.MaxTokens;
                    this.TimeoutSeconds = config.TimeoutSeconds;
                    this.MaxHistoryLength = config.MaxHistoryLength;
                    this.SystemPrompt = config.SystemPrompt;
                    this.BearerToken = config.BearerToken;
                }
            }
            catch
            {
                // Ignore errors on load
            }
        }
    }
}
