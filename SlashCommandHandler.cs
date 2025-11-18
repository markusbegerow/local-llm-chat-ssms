using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LocalLlmChatSsms
{
    /// <summary>
    /// Handles slash commands for SQL operations and file management
    /// </summary>
    public class SlashCommandHandler
    {
        private readonly string _workingDirectory;

        public SlashCommandHandler(string workingDirectory = null, LlmConfig config = null)
        {
            _workingDirectory = workingDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // Don't cache config - always read fresh from SettingsManager
        }

        /// <summary>
        /// Gets a relative path from one path to another (compatible with .NET Framework 4.7.2)
        /// </summary>
        private static string GetRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

            Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
            Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

            if (fromUri.Scheme != toUri.Scheme) { return toPath; }

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            if (!Path.HasExtension(path) && !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return path + Path.DirectorySeparatorChar;
            }
            return path;
        }

        /// <summary>
        /// Processes a slash command and returns the result
        /// </summary>
        /// <param name="command">The full command string (including the /)</param>
        /// <returns>Result message or null if not a command</returns>
        public string ProcessCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command) || !command.StartsWith("/"))
                return null;

            var parts = command.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLower();
            var args = parts.Length > 1 ? parts[1] : "";

            switch (cmd)
            {
                case "/help":
                    return GetHelpText();

                case "/read":
                    return ReadFile(args);

                case "/list":
                    return ListDirectory(args);

                case "/search":
                    return SearchFiles(args);

                case "/write":
                    return PrepareWriteFile(args);

                case "/clear":
                    return "CLEAR_CONVERSATION"; // Special signal

                case "/config":
                    return GetConfigInfo();

                case "/info":
                    return GetInfoText();

                default:
                    return $"Unknown command: {cmd}. Type /help for available commands.";
            }
        }

        /// <summary>
        /// Checks if input is a slash command
        /// </summary>
        public static bool IsCommand(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.TrimStart().StartsWith("/");
        }

        private string GetHelpText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Available Slash Commands:");
            sb.AppendLine();
            sb.AppendLine("/help - Show this help message");
            sb.AppendLine("/read <file-path> - Read a SQL script or file into context");
            sb.AppendLine("/list [directory] - List files in a directory");
            sb.AppendLine("/search <pattern> - Search for files matching pattern");
            sb.AppendLine("/write <path> - Prepare to write content to a file");
            sb.AppendLine("/clear - Clear conversation history");
            sb.AppendLine("/config - Show current configuration");
            sb.AppendLine("/info - Show extension information");
            sb.AppendLine();
            sb.AppendLine("Examples:");
            sb.AppendLine("  /read script.sql");
            sb.AppendLine("  /list C:\\SQLScripts");
            sb.AppendLine("  /search *.sql");

            return sb.ToString();
        }

        private string ReadFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return "Error: Please specify a file path. Usage: /read <file-path>";

            try
            {
                // Resolve path
                string fullPath = Path.IsPathRooted(filePath)
                    ? filePath
                    : Path.Combine(_workingDirectory, filePath);

                // Validate
                if (!Utils.ValidateRelativePath(filePath) && !Path.IsPathRooted(filePath))
                    return "Error: Invalid file path.";

                if (!File.Exists(fullPath))
                    return $"Error: File not found: {fullPath}";

                if (!Utils.ValidateFileSize(fullPath))
                    return $"Error: File too large (max 1MB)";

                string content = File.ReadAllText(fullPath);
                return $"File: {Path.GetFileName(fullPath)}\n\n{content}";
            }
            catch (Exception ex)
            {
                return $"Error reading file: {ex.Message}";
            }
        }

        private string ListDirectory(string directoryPath)
        {
            try
            {
                string targetDir = string.IsNullOrWhiteSpace(directoryPath)
                    ? _workingDirectory
                    : directoryPath;

                if (!Directory.Exists(targetDir))
                    return $"Error: Directory not found: {targetDir}";

                var files = Directory.GetFiles(targetDir)
                    .Select(f => Path.GetFileName(f))
                    .OrderBy(f => f)
                    .Take(50); // Limit to 50 files

                var dirs = Directory.GetDirectories(targetDir)
                    .Select(d => Path.GetFileName(d))
                    .OrderBy(d => d)
                    .Take(20); // Limit to 20 directories

                var sb = new StringBuilder();
                sb.AppendLine($"Directory: {targetDir}");
                sb.AppendLine();

                if (dirs.Any())
                {
                    sb.AppendLine("Directories:");
                    foreach (var dir in dirs)
                        sb.AppendLine($"  [DIR] {dir}");
                    sb.AppendLine();
                }

                if (files.Any())
                {
                    sb.AppendLine("Files:");
                    foreach (var file in files)
                        sb.AppendLine($"  {file}");
                }

                if (!dirs.Any() && !files.Any())
                    sb.AppendLine("(empty directory)");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error listing directory: {ex.Message}";
            }
        }

        private string SearchFiles(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
                return "Error: Please specify a search pattern. Usage: /search <pattern>";

            try
            {
                var files = Directory.GetFiles(_workingDirectory, pattern, SearchOption.AllDirectories)
                    .Select(f => GetRelativePath(_workingDirectory, f))
                    .OrderBy(f => f)
                    .Take(30); // Limit to 30 results

                if (!files.Any())
                    return $"No files found matching: {pattern}";

                var sb = new StringBuilder();
                sb.AppendLine($"Files matching '{pattern}':");
                sb.AppendLine();

                foreach (var file in files)
                    sb.AppendLine($"  {file}");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Error searching files: {ex.Message}";
            }
        }

        private string PrepareWriteFile(string args)
        {
            if (string.IsNullOrWhiteSpace(args))
                return "Error: Please specify a file path. Usage: /write <path>";

            return $"Ready to write to: {args}\nPlease provide the content in your next message.";
        }

        private string GetConfigInfo()
        {
            // Always read fresh config from SettingsManager
            var config = SettingsManager.Instance.Config;

            var sb = new StringBuilder();
            sb.AppendLine("Current Configuration:");
            sb.AppendLine();
            sb.AppendLine("LLM Settings:");
            sb.AppendLine($"  Provider: {config.Provider}");
            sb.AppendLine($"  API URL: {config.ApiUrl}");
            sb.AppendLine($"  Model: {config.ModelName}");
            sb.AppendLine($"  Temperature: {config.Temperature}");
            sb.AppendLine($"  Max Tokens: {config.MaxTokens}");
            sb.AppendLine($"  Timeout: {config.TimeoutSeconds}s");
            sb.AppendLine($"  Max History: {config.MaxHistoryLength}");
            sb.AppendLine($"  Bearer Token: {(string.IsNullOrEmpty(config.BearerToken) ? "(not set)" : "***set***")}");
            sb.AppendLine();
            sb.AppendLine($"Working Directory: {_workingDirectory}");
            sb.AppendLine();
            sb.AppendLine("Tip: Change settings in Tools → Options → Local LLM Chat");

            return sb.ToString();
        }

        private string GetInfoText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Local LLM Chat for SSMS");
            sb.AppendLine();
            sb.AppendLine("Author: Markus Begerow");
            sb.AppendLine("Version: 1.0.0");
            sb.AppendLine("GitHub: https://github.com/markusbegerow/local-llm-chat-ssms");

            return sb.ToString();
        }
    }
}
