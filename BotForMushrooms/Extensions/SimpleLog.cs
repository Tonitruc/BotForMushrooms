namespace BotForMushrooms.Extensions
{
    public class SimpleLog
    {
        private readonly ILogger<SimpleLog> _logger;

        public SimpleLog(ILogger<SimpleLog> logger)
        {
            _logger = logger;
        }

        public void DoSomething()
        {
            _logger.LogDebug("This is a debug message.");
            _logger.LogInformation("This is an info message.");
        }
    }
}
