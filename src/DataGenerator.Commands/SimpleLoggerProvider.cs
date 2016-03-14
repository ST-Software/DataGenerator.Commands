using Microsoft.Extensions.Logging;

namespace DataGenerator.Commands
{
    public class SimpleLoggerProvider : ILoggerProvider
    {
        private readonly ILogger _logger;

        public SimpleLoggerProvider(ILogger logger)
        {
            _logger = logger;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _logger;
        }

        public void Dispose()
        {
        }
    }
}