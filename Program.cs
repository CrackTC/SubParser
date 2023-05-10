using YamlDotNet.Serialization;

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

        private static ClashConfig GetCustomConfig()
        {
            string filePath = Environment.GetEnvironmentVariable("CONFIG_PATH") ?? "config.yml";
            return new Deserializer().Deserialize<ClashConfig>(File.ReadAllText(filePath));
        }

        private static void Main()
        {
            var config = GetCustomConfig();

            var dns = config.Dns;
            var proxies = config.Proxies;
            var rules = config.Rules;
            var proxyGroups = config.ProxyGroups;
            var handler = new RequestHandler(dns, proxies, rules, proxyGroups);

            string host = Environment.GetEnvironmentVariable("HOST") ?? "*";
            int port = GetPort();
            handler.Listen(host, port);
        }
    }
}
