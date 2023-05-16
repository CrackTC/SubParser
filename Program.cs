namespace top.cracktc.SubParser
{

    internal class Program
    {
        private static int GetPort()
        {
            const int defaultPort = 4447;
            string? portString = Environment.GetEnvironmentVariable("PORT");

            _ = int.TryParse(portString, out int port);
            return (port is > 0 and < 65536) ? port : defaultPort;
        }

        private static void Main()
        {
            string customConfigPath = Environment.GetEnvironmentVariable("CONFIG_PATH") ?? "./config/config.yml";
            var handler = new RequestHandler(customConfigPath);

            string host = Environment.GetEnvironmentVariable("HOST") ?? "*";
            int port = GetPort();
            handler.Listen(host, port);
        }
    }
}
