using System;

namespace LocalLlmChatSsms
{
    /// <summary>
    /// Configuration settings for LLM communication
    /// </summary>
    public class LlmConfig
    {
        /// <summary>
        /// API endpoint URL (default: Ollama local endpoint)
        /// </summary>
        public string ApiUrl { get; set; } = "http://localhost:11434";

        /// <summary>
        /// Model name to use (default: llama3)
        /// </summary>
        public string ModelName { get; set; } = "llama3";

        /// <summary>
        /// Temperature for response generation (0.0 - 2.0, default: 0.7)
        /// </summary>
        public double Temperature { get; set; } = 0.7;

        /// <summary>
        /// Maximum tokens in response (default: 2048)
        /// </summary>
        public int MaxTokens { get; set; } = 2048;

        /// <summary>
        /// Request timeout in seconds (default: 120)
        /// </summary>
        public int TimeoutSeconds { get; set; } = 120;

        /// <summary>
        /// Maximum conversation history length (default: 50)
        /// </summary>
        public int MaxHistoryLength { get; set; } = 50;

        /// <summary>
        /// System prompt for SQL Server assistance
        /// </summary>
        public string SystemPrompt { get; set; } =
            "You are a helpful SQL Server and T-SQL assistant inside SSMS. " +
            "You provide expert advice on SQL Server, T-SQL queries, database design, " +
            "performance optimization, and troubleshooting.";

        /// <summary>
        /// API provider type
        /// </summary>
        public ApiProvider Provider { get; set; } = ApiProvider.Ollama;

        /// <summary>
        /// Optional bearer token for authentication
        /// </summary>
        public string BearerToken { get; set; } = "";
    }

    public enum ApiProvider
    {
        Ollama,
        LmStudio,
        OpenAiCompatible,
        Custom
    }
}
