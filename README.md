# Local LLM Chat for SSMS

<div align="center">

![SSMS](https://img.shields.io/badge/SSMS-Extension-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![.NET](https://img.shields.io/badge/.NET%20Framework-4.7.2-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Ollama](https://img.shields.io/badge/Ollama-Compatible-000000?style=for-the-badge&logo=ollama&logoColor=white)
![LM Studio](https://img.shields.io/badge/LM%20Studio-Supported-FF6B6B?style=for-the-badge)
![License](https://img.shields.io/badge/License-GPLv2-blue?style=for-the-badge)

**A powerful SSMS extension for chatting with local Large Language Models directly in SQL Server Management Studio—completely private and secure**

[Features](#features) • [Installation](#installation) • [Getting Started](#getting-started) • [Configuration](#configuration) • [Commands](#commands)

</div>

---

## Features

### 🤖 Local AI Chat for SQL Server
- **Privacy-First**: All data stays on your machine—no cloud APIs required
- **Multiple LLM Support**: Works with Ollama, LM Studio, OpenAI-compatible endpoints, and custom APIs
- **SQL Server Expertise**: Chat with AI assistants specialized in T-SQL, database design, and optimization
- **Persistent Conversations**: Chat history is maintained throughout your session
- **Selectable Responses**: Copy and paste AI responses easily

### 🔧 Integrated Chat Window
- **Native SSMS Integration**: Access via View → Other Windows → Local LLM Chat
- **Simple UI**: Clean, distraction-free chat interface
- **Instant Access**: No need to switch between applications
- **Keyboard Shortcuts**: Send messages with Ctrl+Enter

### 📁 Workspace Integration
- **Read Files**: Load SQL scripts and files into the conversation context
- **List Directories**: Browse your script folders directly from chat
- **Search Files**: Find files using glob patterns (e.g., `*.sql`)
- **Working Directory**: Easily manage files in your documents folder

### ⚡ Slash Commands
Built-in slash commands for quick actions:
- `/help` - Display all available commands
- `/info` - Show extension information (author, version, GitHub)
- `/config` - Show current LLM configuration (API URL, model, settings)
- `/read <file-path>` - Read a SQL script or file into context
- `/list [directory]` - List files in a directory
- `/search <pattern>` - Search for files with patterns
- `/write <path>` - Prepare to write content to a file
- `/clear` - Clear conversation history

## Installation

### From VSIX File
1. Download the `.vsix` file from the [releases page](https://github.com/markusbegerow/local-llm-chat-ssms/releases)
2. Double-click the `.vsix` file to install
3. Restart SQL Server Management Studio
4. Access via **View → Other Windows → Local LLM Chat**

### From Source
```bash
git clone https://github.com/markusbegerow/local-llm-chat-ssms.git
cd local-llm-chat-ssms
# Open local-llm-chat-ssms.sln in Visual Studio
# Build the solution (F6)
# The VSIX will be in bin\Debug or bin\Release
```

### Uninstallation
Run the included `uninstall.bat` file or uninstall via **Extensions → Manage Extensions** in SSMS.

## Getting Started

### 1. Set Up Your Local LLM

**Option A: Ollama** (Recommended for local use)
```bash
# Install Ollama from https://ollama.ai
ollama pull llama3
ollama serve
```
- Default URL: `http://localhost:11434`
- Provider: Select "Ollama"

**Option B: LM Studio**
1. Download from [lmstudio.ai](https://lmstudio.ai)
2. Load a model (e.g., Llama 3, Mistral)
3. Start the local server (default: `http://localhost:1234/v1/chat/completions`)
4. Provider: Select "LM Studio"

**Option C: Custom/OpenAI-compatible API**
- Use any OpenAI-compatible API endpoint
- Provider: Select "Custom" or "OpenAI Compatible"
- Set full endpoint URL (e.g., `https://your-api.com/v1/chat/completions`)

### 2. Configure the Extension

1. Open SSMS
2. Go to **Tools → Options → Local LLM Chat → General**
3. Configure your settings:
   - **API Provider**: Select your LLM provider (Ollama, LM Studio, OpenAI Compatible, or Custom)
   - **API URL**: Enter your API endpoint URL
   - **Model Name**: Enter the model to use (e.g., `llama3`, `gpt-4o`)
   - **Temperature**: Control randomness (0.0 = focused, 2.0 = creative)
   - **Bearer Token**: Optional authentication token (if required)
   - **System Prompt**: Customize the AI's behavior
   - **Advanced Settings**: Max tokens, timeout, history length

4. Click **OK** to save

### 3. Start Chatting

1. Open **View → Other Windows → Local LLM Chat**
2. Type your question or command
3. Press **Send** or **Ctrl+Enter**
4. The AI will respond based on your configuration!

## Configuration

### Settings Location
**Tools → Options → Local LLM Chat → General**

### Available Settings

| Setting | Default | Description |
|---------|---------|-------------|
| **API Provider** | Ollama | The LLM provider to use (Ollama, LM Studio, OpenAI Compatible, Custom) |
| **API URL** | `http://localhost:11434` | Base URL for the LLM API endpoint |
| **Model Name** | `llama3` | Name of the model to use |
| **Temperature** | `0.7` | Controls randomness in responses (0.0-2.0) |
| **Max Tokens** | `2048` | Maximum number of tokens in response |
| **Timeout** | `120` seconds | Request timeout in seconds |
| **Max History Length** | `50` | Maximum number of messages to keep in history |
| **System Prompt** | SQL Server assistant | System prompt that defines AI behavior |
| **Bearer Token** | (empty) | Optional bearer token for API authentication |

### Configuration Examples

**Ollama (Local)**
```
Provider: Ollama
API URL: http://localhost:11434
Model: llama3
Temperature: 0.7
```

**LM Studio (Local)**
```
Provider: LM Studio
API URL: http://localhost:1234/v1/chat/completions
Model: llama-3-8b
Temperature: 0.7
```

**Custom/OpenAI-compatible API**
```
Provider: Custom
API URL: https://your-api.com/v1/chat/completions
Model: gpt-4o
Temperature: 0.7
Bearer Token: your-api-key-here
```

## Commands

### Slash Commands (In-Chat)

Type these commands in the chat window:

| Command | Description | Example |
|---------|-------------|---------|
| `/help` | Show all available commands | `/help` |
| `/info` | Show extension information | `/info` |
| `/config` | Display current configuration | `/config` |
| `/read <file-path>` | Read a SQL script or file | `/read scripts\backup.sql` |
| `/list [directory]` | List files in directory | `/list C:\SQLScripts` |
| `/search <pattern>` | Search for files | `/search *.sql` |
| `/write <path>` | Prepare to write to file | `/write test.sql` |
| `/clear` | Clear conversation history | `/clear` |

## Usage Examples

### Ask SQL Questions
```
How do I optimize a query with multiple LEFT JOINs?
```

### Analyze SQL Scripts
```
/read backup_script.sql
Can you review this backup script and suggest improvements?
```

### Generate SQL Code
```
Create a stored procedure that archives old orders to a history table
```

### Get Configuration Info
```
/config
```
Displays:
```
Current Configuration:

LLM Settings:
  Provider: Ollama
  API URL: http://localhost:11434
  Model: llama3
  Temperature: 0.7
  Max Tokens: 2048
  Timeout: 120s
  Max History: 50
  Bearer Token: (not set)

Working Directory: C:\Users\YourName\Documents

Tip: Change settings in Tools → Options → Local LLM Chat
```

### Search SQL Scripts
```
/search *.sql
/list C:\SQLScripts
```

## Troubleshooting

### Connection Issues
- Verify your LLM server is running:
  - Ollama: `curl http://localhost:11434/api/tags`
  - LM Studio: Check that the server is started in LM Studio
- Check the API URL in **Tools → Options → Local LLM Chat**
- For custom APIs, verify the full endpoint URL includes `/v1/chat/completions` or the correct path

### "Method Not Allowed" Error
- Make sure your **API Provider** matches your actual LLM service
- For OpenAI-compatible APIs, use "Custom" or "OpenAI Compatible" provider
- Verify the API URL is complete (e.g., `http://localhost:1234/v1/chat/completions`)

### Chat Not Responding
- Check your model is loaded in the LLM server
- Increase the **Timeout** setting in options
- Verify network connectivity to the API endpoint

### Settings Not Saving
- Close and reopen the chat window after changing settings
- Check that settings are saved in **Tools → Options**
- Restart SSMS if settings don't apply

## Requirements

- **SSMS**: Version 19.0 or higher (supports 18.x, 19.x, 20+)
- **.NET Framework**: 4.7.2 or higher
- **Local LLM**: Ollama, LM Studio, or compatible OpenAI API server
- **Architecture**: 64-bit (amd64)

## Development

### Building from Source

```bash
# Clone repository
git clone https://github.com/markusbegerow/local-llm-chat-ssms.git
cd local-llm-chat-ssms

# Open in Visual Studio
start local-llm-chat-ssms.sln

# Build solution (F6)
# Output: bin\Debug\local-llm-chat-ssms.vsix or bin\Release\local-llm-chat-ssms.vsix
```

### Project Structure

```
local-llm-chat-ssms/
├── ChatMessageViewModel.cs           # View model for chat messages
├── LlmConfig.cs                       # Configuration model
├── LocalLlmChatSsmsPackage.cs        # VSIX package entry point
├── LocalLlmChatSsmsPackage.vsct      # VS command table (menu definitions)
├── LocalLlmChatWindow.cs             # Tool window definition
├── LocalLlmChatWindowControl.xaml    # Chat UI (XAML)
├── LocalLlmChatWindowControl.xaml.cs # Chat UI code-behind
├── LocalLlmChatWindowCommand.cs      # Command to open chat window
├── OllamaClient.cs                    # LLM API client
├── OptionsPage.cs                     # Tools → Options page
├── SettingsManager.cs                 # Settings persistence
├── SlashCommandHandler.cs             # Slash command processor
├── Utils.cs                           # Utility functions
├── source.extension.vsixmanifest     # VSIX manifest
├── local-llm-chat-ssms.csproj        # Project file
├── local-llm-chat-ssms.sln           # Solution file
├── uninstall.bat                      # Uninstaller script
└── README.md                          # This file
```

## Recommended Models

### SQL Server Tasks
- **Llama 3.1 8B**: Fast, good for general SQL questions
- **CodeLlama 13B**: Specialized for code generation
- **Qwen 2.5 Coder**: Excellent code understanding
- **DeepSeek Coder**: Strong at complex database logic

### General Tasks
- **Llama 3.1**: Best all-around performance
- **Mistral 7B**: Fast and efficient
- **Phi-3**: Compact but capable

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the GNU General Public License v2.0 (GPLv2) - see the [LICENSE](LICENSE) file for details.

This means:
- ✅ You can freely use, modify, and distribute this software
- ✅ You must provide source code when distributing
- ✅ Any derivative works must also be licensed under GPLv2
- ✅ No warranty is provided

## Acknowledgments

- Thanks to the Ollama team for making local LLMs accessible
- LM Studio for providing an excellent local inference platform
- The Visual Studio extensibility team for comprehensive documentation

## 🙋‍♂️ Get Involved

If you encounter any issues or have questions:
- 🐛 [Report bugs](https://github.com/markusbegerow/local-llm-chat-ssms/issues)
- 💡 [Request features](https://github.com/markusbegerow/local-llm-chat-ssms/issues)
- ⭐ Star the repo if you find it useful!

## ☕ Support the Project

If you like this project, support further development with a repost or coffee:

<a href="https://www.linkedin.com/sharing/share-offsite/?url=https://github.com/MarkusBegerow/local-llm-chat-ssms" target="_blank">
  <img src="https://img.shields.io/badge/💼-Share%20on%20LinkedIn-blue" />
</a>

[![Buy Me a Coffee](https://img.shields.io/badge/☕-Buy%20me%20a%20coffee-yellow)](https://paypal.me/MarkusBegerow?country.x=DE&locale.x=de_DE)

## 📬 Contact

- 🧑‍💻 [Markus Begerow](https://linkedin.com/in/markusbegerow)
- 💾 [GitHub](https://github.com/markusbegerow)
- ✉️ [Twitter](https://x.com/markusbegerow)

---

**Privacy Notice**: This extension operates entirely locally by default. No data is sent to external servers unless you explicitly configure it to use a remote API endpoint.
