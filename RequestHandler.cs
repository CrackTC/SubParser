using System.Net;
using System.Text;
using YamlDotNet.Serialization;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace top.cracktc.SubParser
{
    internal class RequestHandler
    {
        private HttpClient Client { get; init; }
        private ISerializer Serializer { get; }
        private IDeserializer Deserializer { get; }

        private void OnConfigChanged(object? state)
        {
            Console.WriteLine($"config changed");
            _CustomConfig = GetCustomConfig((string)state!);
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

        private static void SetupFileWatcher(string path, Action<string> changeCallback)
        {
            string absolutePath = Path.GetFullPath(path);

            PhysicalFileProvider fileProvider = new(Path.GetDirectoryName(absolutePath)!);

            fileProvider.UsePollingFileWatcher = true;

            var fileChangeToken = fileProvider.Watch(Path.GetFileName(absolutePath));

            Action<object?> callback = null!;
            callback = state =>
            {
                (var absolutePath, var fileProvider, var fileChangeToken) = ((string, PhysicalFileProvider, IChangeToken))state!;
                changeCallback(absolutePath);

                fileChangeToken = fileProvider.Watch(Path.GetFileName(absolutePath));
                fileChangeToken.RegisterChangeCallback(callback, (absolutePath, fileProvider, fileChangeToken));
            };

            fileChangeToken.RegisterChangeCallback(callback, (absolutePath, fileProvider, fileChangeToken));
        }

        public RequestHandler(string customConfigPath)
        {
            Client = new();
            Serializer = new SerializerBuilder().ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull).Build();
            Deserializer = new Deserializer();

            SetupFileWatcher(customConfigPath, OnConfigChanged);
        }

        ~RequestHandler()
        {
            Client.Dispose();
        }
    }
}
