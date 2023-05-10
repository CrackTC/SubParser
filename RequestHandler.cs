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

        private ClashDns? Dns { get; }
        private IEnumerable<ClashProxy>? Proxies { get; }
        private IEnumerable<string>? Rules { get; }
        private IEnumerable<ClashProxyGroup>? ProxyGroups { get; }

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

        public RequestHandler(ClashDns? dns, IEnumerable<ClashProxy>? proxies, IEnumerable<string>? rules, IEnumerable<ClashProxyGroup>? proxyGroups)
        {
            Client = new();
            Serializer = new SerializerBuilder().ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull).Build();
            Deserializer = new Deserializer();

            Dns = dns;
            Proxies = proxies;
            Rules = rules;
            ProxyGroups = proxyGroups;
        }
    }
}
