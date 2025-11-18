using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LocalLlmChatSsms
{
    /// <summary>
    /// Utility functions for validation, security, and file operations
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Maximum file size allowed (1MB default)
        /// </summary>
        public const int MaxFileSize = 1024 * 1024;

        /// <summary>
        /// Validates a relative file path to prevent path traversal attacks
        /// </summary>
        /// <param name="path">The path to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateRelativePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            // Prevent null bytes
            if (path.Contains("\0"))
                return false;

            // Prevent absolute paths
            if (Path.IsPathRooted(path))
                return false;

            // Prevent path traversal
            if (path.Contains(".."))
                return false;

            // Prevent Windows-specific invalid characters
            char[] invalidChars = Path.GetInvalidPathChars();
            if (path.IndexOfAny(invalidChars) >= 0)
                return false;

            return true;
        }

        /// <summary>
        /// Validates file size
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="maxSize">Maximum size in bytes</param>
        /// <returns>True if size is acceptable</returns>
        public static bool ValidateFileSize(string filePath, int maxSize = MaxFileSize)
        {
            if (!File.Exists(filePath))
                return false;

            FileInfo fileInfo = new FileInfo(filePath);
            return fileInfo.Length <= maxSize;
        }

        /// <summary>
        /// Escapes HTML to prevent XSS attacks
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Escaped string</returns>
        public static string EscapeHtml(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\"", "&quot;")
                .Replace("'", "&#39;");
        }

        /// <summary>
        /// Trims conversation history to prevent context overflow
        /// </summary>
        /// <param name="maxMessages">Maximum number of messages to keep</param>
        /// <returns>Trimmed message count</returns>
        public static int TrimHistory<T>(System.Collections.ObjectModel.ObservableCollection<T> messages, int maxMessages)
        {
            if (messages == null || messages.Count <= maxMessages)
                return 0;

            int toRemove = messages.Count - maxMessages;
            for (int i = 0; i < toRemove; i++)
            {
                messages.RemoveAt(0);
            }

            return toRemove;
        }

        /// <summary>
        /// Extracts file suggestions from LLM response using special markdown syntax
        /// Format: ````file path="relative/path.ext"
        /// </summary>
        /// <param name="content">LLM response content</param>
        /// <returns>Array of file paths suggested</returns>
        public static string[] ExtractFileSuggestions(string content)
        {
            if (string.IsNullOrEmpty(content))
                return Array.Empty<string>();

            // Match ````file path="..."
            var regex = new Regex(@"````file\s+path=""([^""]+)""", RegexOptions.Multiline);
            var matches = regex.Matches(content);

            string[] suggestions = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                suggestions[i] = matches[i].Groups[1].Value;
            }

            return suggestions;
        }

        /// <summary>
        /// Safely reads a file with size validation
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="maxSize">Maximum allowed size</param>
        /// <returns>File content or error message</returns>
        public static string SafeReadFile(string filePath, int maxSize = MaxFileSize)
        {
            try
            {
                if (!File.Exists(filePath))
                    return $"Error: File not found: {filePath}";

                if (!ValidateFileSize(filePath, maxSize))
                    return $"Error: File too large (max {maxSize / 1024}KB): {filePath}";

                return File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                return $"Error reading file: {ex.Message}";
            }
        }

        /// <summary>
        /// Safely writes content to a file
        /// </summary>
        /// <param name="filePath">Path to file</param>
        /// <param name="content">Content to write</param>
        /// <returns>Success message or error</returns>
        public static string SafeWriteFile(string filePath, string content)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, content);
                return $"File written successfully: {filePath}";
            }
            catch (Exception ex)
            {
                return $"Error writing file: {ex.Message}";
            }
        }
    }
}
