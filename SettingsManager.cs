using System;
using System.IO;
using Newtonsoft.Json;

namespace LocalLlmChatSsms
{
    /// <summary>
    /// Manages persistence of LLM configuration settings
    /// </summary>
    public class SettingsManager
    {
        private static SettingsManager _instance;
        private static readonly object _lock = new object();
        private readonly string _settingsPath;
        private LlmConfig _config;

        private SettingsManager()
        {
            // Store settings in user's AppData folder
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var settingsDir = Path.Combine(appDataPath, "LocalLlmChatSsms");

            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }

            _settingsPath = Path.Combine(settingsDir, "settings.json");
            _config = LoadSettings();
        }

        /// <summary>
        /// Gets the singleton instance
        /// </summary>
        public static SettingsManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SettingsManager();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets the current configuration
        /// </summary>
        public LlmConfig Config
        {
            get { return _config; }
        }

        /// <summary>
        /// Loads settings from disk or creates defaults
        /// </summary>
        private LlmConfig LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    var config = JsonConvert.DeserializeObject<LlmConfig>(json);

                    // Validate loaded config
                    if (config != null && !string.IsNullOrWhiteSpace(config.ApiUrl))
                    {
                        return config;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            // Return default config if load fails
            return new LlmConfig();
        }

        /// <summary>
        /// Saves current configuration to disk
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_config, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
                throw new Exception($"Failed to save settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates configuration and saves to disk
        /// </summary>
        public void UpdateSettings(LlmConfig newConfig)
        {
            if (newConfig == null)
                throw new ArgumentNullException(nameof(newConfig));

            _config = newConfig;
            SaveSettings();
        }

        /// <summary>
        /// Updates configuration with an action and saves
        /// </summary>
        public void UpdateSettings(Action<LlmConfig> updateAction)
        {
            if (updateAction == null)
                throw new ArgumentNullException(nameof(updateAction));

            updateAction(_config);
            SaveSettings();
        }

        /// <summary>
        /// Resets configuration to defaults
        /// </summary>
        public void ResetToDefaults()
        {
            _config = new LlmConfig();
            SaveSettings();
        }

        /// <summary>
        /// Gets the settings file path
        /// </summary>
        public string SettingsFilePath => _settingsPath;
    }
}
