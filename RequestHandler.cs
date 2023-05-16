using System.Net;
using System.Text;
using YamlDotNet.Serialization;

namespace top.cracktc.SubParser
{
    internal class RequestHandler
    {
        private HttpClient Client { get; init; }
        private ISerializer Serializer { get; }
        private IDeserializer Deserializer { get; }

        private void OnConfigChanged(object source, FileSystemEventArgs e)
        {
            Console.WriteLine($"config changed: {e.FullPath}");
            _CustomConfig = GetCustomConfig(e.FullPath);
        }
        private ClashConfig? _CustomConfig;

        private ClashDns? Dns => _CustomConfig?.Dns;
        private IEnumerable<ClashProxy>? Proxies => _CustomConfig?.Proxies;
        private IEnumerable<string>? Rules => _CustomConfig?.Rules;
        private IEnumerable<ClashProxyGroup>? ProxyGroups => _CustomConfig?.ProxyGroups;

        private static void SendMethodNotAllowed(HttpListenerResponse response)
        {
            response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            response.AddHeader("Allow", "GET");
            response.Close();
        }

        private static void SendUsage(HttpListenerResponse response)
        {
            response.OutputStream.Write(Encoding.UTF8.GetBytes("usage: /?url=<url>"));
            response.Close();
        }

        private async Task<ClashConfig> GetConfig(string url) => Deserializer.Deserialize<ClashConfig>(await Client.GetStringAsync(url));

        private async void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            if (request.HttpMethod != HttpMethod.Get.Method)
            {
                SendMethodNotAllowed(response);
                return;
            }

            string? url = request.QueryString["url"];
            if (url is null)
            {
                SendUsage(response);
                return;
            }

            try
            {
                var config = (await GetConfig(url))
                                .SetDns(Dns)
                                .AddProxies(Proxies)
                                .AddProxyGroups(ProxyGroups)
                                .AddRules(Rules);

                response.ContentType = "text/plain; charset=utf-8";
                response.OutputStream.Write(Encoding.UTF8.GetBytes(Serializer.Serialize(config)));
            }
            catch (Exception e)
            {
                response.OutputStream.Write(Encoding.UTF8.GetBytes(e.Message));
#if DEBUG
                throw;
#endif
            }
            finally
            {
                response.Close();
            }
        }

        public void Listen(string host, int port)
        {
            var listener = new HttpListener();

            listener.Prefixes.Add($"http://{host}:{port}/");
            listener.Prefixes.ToList().ForEach(prefix => Console.WriteLine($"listening at {prefix}"));
            listener.Start();

            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;
                var response = context.Response;

                Console.WriteLine(context.Request.Url?.ToString());
                Handle(request, response);
            }
        }

        private static ClashConfig GetCustomConfig(string path)
            => new Deserializer().Deserialize<ClashConfig>(File.ReadAllText(path));

        public RequestHandler(string customConfigPath)
        {
            Client = new();
            Serializer = new SerializerBuilder().ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull).Build();
            Deserializer = new Deserializer();

            var watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(customConfigPath) ?? throw new ArgumentException("invalid path"),
                Filter = Path.GetFileName(customConfigPath),
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };
            watcher.Changed += OnConfigChanged;

            _CustomConfig = GetCustomConfig(customConfigPath);
        }
    }
}
