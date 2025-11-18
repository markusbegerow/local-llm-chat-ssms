using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LocalLlmChatSsms
{
    public class LlmClient
    {
        private readonly HttpClient _http;
        private readonly LlmConfig _config;

        public LlmClient(LlmConfig config = null)
        {
            _config = config ?? new LlmConfig();
            _http = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds)
            };

            // Add bearer token if provided
            if (!string.IsNullOrEmpty(_config.BearerToken))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.BearerToken);
            }
        }

        public LlmConfig Config => _config;

        public async Task<string> ChatAsync(IList<(string role, string content)> messages, CancellationToken cancellationToken = default)
        {
            try
            {
                // Create payload based on provider type
                object payload;

                if (_config.Provider == ApiProvider.Ollama)
                {
                    // Ollama format
                    payload = new
                    {
                        model = _config.ModelName,
                        messages = messages.Select(m => new { role = m.role, content = m.content }).ToArray(),
                        stream = false,
                        options = new
                        {
                            temperature = _config.Temperature,
                            num_predict = _config.MaxTokens
                        }
                    };
                }
                else
                {
                    // OpenAI-compatible format (LM Studio, OpenAI, Custom)
                    payload = new
                    {
                        model = _config.ModelName,
                        messages = messages.Select(m => new { role = m.role, content = m.content }).ToArray(),
                        temperature = _config.Temperature,
                        max_tokens = _config.MaxTokens,
                        stream = false
                    };
                }

                var json = JsonConvert.SerializeObject(payload);
                var baseUrl = _config.ApiUrl.TrimEnd('/');

                // Choose endpoint based on provider
                string endpoint;
                switch (_config.Provider)
                {
                    case ApiProvider.Ollama:
                        endpoint = $"{baseUrl}/api/chat";
                        break;
                    case ApiProvider.LmStudio:
                    case ApiProvider.OpenAiCompatible:
                    case ApiProvider.Custom:
                        // These providers expect the full URL to be provided in ApiUrl
                        endpoint = baseUrl;
                        break;
                    default:
                        endpoint = $"{baseUrl}/api/chat";
                        break;
                }

                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                using (var response = await _http.PostAsync(endpoint, content, cancellationToken))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        throw new HttpRequestException(
                            $"LLM API request failed with status {response.StatusCode}: {errorContent}");
                    }

                    var respJson = await response.Content.ReadAsStringAsync();

                    // Parse response based on provider
                    var root = JObject.Parse(respJson);

                    // Try OpenAI-compatible format first (choices[0].message.content)
                    if (root["choices"] != null && root["choices"].HasValues)
                    {
                        var choice = root["choices"][0];
                        if (choice["message"] != null && choice["message"]["content"] != null)
                        {
                            return choice["message"]["content"].ToString();
                        }
                    }

                    // Try Ollama format (message.content)
                    if (root["message"] != null && root["message"]["content"] != null)
                    {
                        return root["message"]["content"].ToString();
                    }

                    // Try Ollama legacy format (response)
                    if (root["response"] != null)
                    {
                        return root["response"].ToString();
                    }

                    // Fallback: return raw JSON
                    return respJson;
                }
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException($"Request timed out after {_config.TimeoutSeconds} seconds");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Network error: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Failed to parse LLM response: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update configuration dynamically
        /// </summary>
        public void UpdateConfig(Action<LlmConfig> configAction)
        {
            configAction?.Invoke(_config);

            // Update timeout
            _http.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);

            // Update bearer token
            if (!string.IsNullOrEmpty(_config.BearerToken))
            {
                _http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _config.BearerToken);
            }
            else
            {
                _http.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
}
