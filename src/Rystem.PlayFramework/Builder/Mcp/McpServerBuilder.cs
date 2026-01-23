namespace Rystem.PlayFramework
{
    /// <summary>
    /// Fluent builder for configuring an MCP server globally.
    /// Scenes decide which elements to use via UseMcpServer().
    /// </summary>
    internal sealed class McpServerBuilder : IMcpServerBuilder
    {
        private string? _httpUrl;
        private string? _command;
        private string[]? _commandArgs;
        private TimeSpan _timeout = TimeSpan.FromSeconds(30);

        public McpServerBuilder()
        {
        }

        public IMcpServerBuilder WithHttpServer(string serverUrl)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
                throw new ArgumentException("Server URL cannot be empty", nameof(serverUrl));

            _httpUrl = serverUrl;
            _command = null;
            _commandArgs = null;
            return this;
        }

        public IMcpServerBuilder WithCommand(string executable, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(executable))
                throw new ArgumentException("Executable cannot be empty", nameof(executable));

            _command = executable;
            _commandArgs = args;
            _httpUrl = null;
            return this;
        }

        public IMcpServerBuilder WithTimeout(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
                throw new ArgumentException("Timeout must be positive", nameof(timeout));

            _timeout = timeout;
            return this;
        }

        public McpServerConfiguration Build()
        {
            // Validate that either HTTP or Command is set
            if (string.IsNullOrWhiteSpace(_httpUrl) && string.IsNullOrWhiteSpace(_command))
                throw new InvalidOperationException("Either WithHttpServer or WithCommand must be called");

            // Validate that both are not set
            if (!string.IsNullOrWhiteSpace(_httpUrl) && !string.IsNullOrWhiteSpace(_command))
                throw new InvalidOperationException("Cannot set both HTTP server and command");

            return new McpServerConfiguration
            {
                HttpUrl = _httpUrl,
                Command = _command,
                CommandArgs = _commandArgs,
                Timeout = _timeout
            };
        }
    }
}
