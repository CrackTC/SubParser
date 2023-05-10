using YamlDotNet.Serialization;

namespace top.cracktc.SubParser
{
    internal class ClashConfig
    {
        #region Properties
        [YamlMember(Alias = "port")]
        public int? Port { get; set; }

        [YamlMember(Alias = "socks-port")]
        public int? SocksPort { get; set; }

        [YamlMember(Alias = "redir-port")]
        public int? RedirPort { get; set; }

        [YamlMember(Alias = "tproxy-port")]
        public int? TproxyPort { get; set; }

        [YamlMember(Alias = "mixed-port")]
        public int? MixedPort { get; set; }

        [YamlMember(Alias = "authentication")]
        public List<string>? Authentication { get; set; }

        [YamlMember(Alias = "allow-lan")]
        public bool? AllowLan { get; set; }

        [YamlMember(Alias = "bind-address")]
        public string? BindAddress { get; set; }

        [YamlMember(Alias = "mode")]
        public string? Mode { get; set; }

        [YamlMember(Alias = "log-level")]
        public string? LogLevel { get; set; }

        [YamlMember(Alias = "ipv6")]
        public bool? Ipv6 { get; set; }

        [YamlMember(Alias = "external-controller")]
        public string? ExternalController { get; set; }

        [YamlMember(Alias = "external-ui")]
        public string? ExternalUi { get; set; }

        [YamlMember(Alias = "secret")]
        public string? Secret { get; set; }

        [YamlMember(Alias = "interface-name")]
        public string? InterfaceName { get; set; }

        [YamlMember(Alias = "routing-mark")]
        public int? RoutingMark { get; set; }

        [YamlMember(Alias = "hosts")]
        public Dictionary<string, string>? Hosts { get; set; }

        [YamlMember(Alias = "profile")]
        public ClashProfile? Profile { get; set; }

        [YamlMember(Alias = "dns")]
        public ClashDns? Dns { get; set; }

        [YamlMember(Alias = "nameserver-policy")]
        public Dictionary<string, string>? NameserverPolicy { get; set; }

        [YamlMember(Alias = "proxies")]
        public List<ClashProxy>? Proxies { get; set; }

        [YamlMember(Alias = "proxy-groups")]
        public List<ClashProxyGroup>? ProxyGroups { get; set; }

        [YamlMember(Alias = "proxy-providers")]
        public Dictionary<string, ClashProxyProvider>? ProxyProviders { get; set; }

        [YamlMember(Alias = "tunnels")]
        public List<ClashTunnel>? Tunnels { get; set; }

        [YamlMember(Alias = "rules")]
        public List<string>? Rules { get; set; }
        #endregion

        public ClashConfig SetDns(ClashDns? dns)
        {
            Dns = dns;
            return this;
        }

        public ClashConfig AddProxies(IEnumerable<ClashProxy>? proxies)
        {
            if (proxies is not null)
            {
                if (Proxies is null)
                    Proxies = new();

                Proxies.AddRange(proxies);
            }
            return this;
        }

        public ClashConfig AddProxyGroups(IEnumerable<ClashProxyGroup>? proxyGroups)
        {
            if (proxyGroups is not null)
            {
                if (ProxyGroups is null)
                    ProxyGroups = new();

                var proxyNames = (from proxy in Proxies select proxy.Name).ToList();
                var newProxyGroups = from proxyGroup in proxyGroups
                                     select new ClashProxyGroup
                                     {
                                         Name = proxyGroup.Name,
                                         Type = proxyGroup.Type,
                                         ProxyNames = proxyNames
                                     };
                ProxyGroups.AddRange(newProxyGroups);
            }
            return this;
        }

        public ClashConfig AddRules(IEnumerable<string>? rules)
        {
            if (rules is not null)
            {
                if (Rules is null)
                    Rules = new();

                Rules.InsertRange(0, rules);
            }
            return this;
        }
    }

    internal class ClashProxy
    {
        [YamlMember(Alias = "name")]
        public string? Name { get; set; }

        [YamlMember(Alias = "type")]
        public string? Type { get; set; }

        [YamlMember(Alias = "server")]
        public string? Server { get; set; }

        [YamlMember(Alias = "port")]
        public int? Port { get; set; }

        [YamlMember(Alias = "uuid")]
        public string? Uuid { get; set; }

        [YamlMember(Alias = "alterId")]
        public int? AlterId { get; set; }

        [YamlMember(Alias = "cipher")]
        public string? Cipher { get; set; }

        [YamlMember(Alias = "password")]
        public string? Password { get; set; }

        [YamlMember(Alias = "plugin")]
        public string? Plugin { get; set; }

        [YamlMember(Alias = "plugin-opts")]
        public ClashProxyPluginOpts? PluaginOpts { get; set; }

        [YamlMember(Alias = "udp")]
        public bool? Udp { get; set; }

        [YamlMember(Alias = "tls")]
        public bool? Tls { get; set; }

        [YamlMember(Alias = "skip-cert-verify")]
        public bool? SkipCertVerify { get; set; }

        [YamlMember(Alias = "servername")]
        public string? ServerName { get; set; }

        [YamlMember(Alias = "network")]
        public string? Network { get; set; }

        [YamlMember(Alias = "username")]
        public string? Username { get; set; }

        [YamlMember(Alias = "sni")]
        public string? Sni { get; set; }

        [YamlMember(Alias = "psk")]
        public string? Psk { get; set; }

        [YamlMember(Alias = "version")]
        public string? Version { get; set; }

        [YamlMember(Alias = "obfs")]
        public string? Obfs { get; set; }

        [YamlMember(Alias = "protocol")]
        public string? Protocol { get; set; }

        [YamlMember(Alias = "protocol-param")]
        public string? ProtocolParam { get; set; }

        [YamlMember(Alias = "alpn")]
        public List<string>? Alpn { get; set; }

        [YamlMember(Alias = "ws-opts")]
        public ClashProxyWsOpts? WsOpts { get; set; }

        [YamlMember(Alias = "h2-opts")]
        public ClashProxyH2Opts? H2Opts { get; set; }

        [YamlMember(Alias = "http-opts")]
        public ClashProxyHttpOpts? HttpOpts { get; set; }

        [YamlMember(Alias = "grpc-opts")]
        public ClashProxyGrpcOpts? GrpcOpts { get; set; }

        [YamlMember(Alias = "obfs-opts")]
        public ClashProxyObfsOpts? ObfsOpts { get; set; }
    }

    internal class ClashProxyObfsOpts
    {

        [YamlMember(Alias = "mode")]
        public string? Mode { get; set; }

        [YamlMember(Alias = "host")]
        public string? Host { get; set; }
    }

    internal class ClashProxyGrpcOpts
    {

        [YamlMember(Alias = "grpc-service-name")]
        public string? GrpcServiceName { get; set; }
    }

    internal class ClashProxyHttpOpts
    {

        [YamlMember(Alias = "method")]
        public string? Method { get; set; }

        [YamlMember(Alias = "path")]
        public List<string>? Path { get; set; }

        [YamlMember(Alias = "headers")]
        public Dictionary<string, dynamic>? Headers { get; set; }
    }

    internal class ClashProxyWsOpts
    {
        [YamlMember(Alias = "path")]
        public string? Path { get; set; }

        [YamlMember(Alias = "headers")]
        public Dictionary<string, dynamic>? Headers { get; set; }

        [YamlMember(Alias = "max-early-data")]
        public int? MaxEarlyData { get; set; }

        [YamlMember(Alias = "early-data-header-name")]
        public string? EarlyDataHeaderName { get; set; }
    }

    internal class ClashProxyH2Opts
    {
        [YamlMember(Alias = "path")]
        public string? Path { get; set; }

        [YamlMember(Alias = "host")]
        public List<string>? Host { get; set; }
    }

    internal class ClashProxyPluginOpts
    {
        [YamlMember(Alias = "mode")]
        public string? Mode { get; set; }

        [YamlMember(Alias = "host")]
        public string? Host { get; set; }

        [YamlMember(Alias = "skip-cert-verify")]
        public bool? SkipCertVerify { get; set; }

        [YamlMember(Alias = "tls")]
        public bool? Tls { get; set; }

        [YamlMember(Alias = "path")]
        public string? Path { get; set; }

        [YamlMember(Alias = "mux")]
        public bool? Mux { get; set; }

        [YamlMember(Alias = "headers")]
        public Dictionary<string, dynamic>? Headers { get; set; }
    }

    internal class ClashProxyGroup
    {
        [YamlMember(Alias = "name")]
        public string? Name { get; set; }

        [YamlMember(Alias = "type")]
        public string? Type { get; set; }

        [YamlMember(Alias = "proxies")]
        public List<string>? ProxyNames { get; set; }

        [YamlMember(Alias = "url")]
        public string? TestUrl { get; set; }

        [YamlMember(Alias = "interval")]
        public int? TestInterval { get; set; }

        [YamlMember(Alias = "routing-mark")]
        public int? RoutingMark { get; set; }

        [YamlMember(Alias = "use")]
        public List<string>? ProviderNames { get; set; }

        [YamlMember(Alias = "tolerance")]
        public int? Tolerance { get; set; }

        [YamlMember(Alias = "lazy")]
        public bool? Lazy { get; set; }

        [YamlMember(Alias = "strategy")]
        public string? Strategy { get; set; }

        [YamlMember(Alias = "disable-udp")]
        public bool? DisableUdp { get; set; }

        [YamlMember(Alias = "filter")]
        public string? Filter { get; set; }

        [YamlMember(Alias = "interface-name")]
        public string? InterfaceName { get; set; }
    }

    internal class ClashProfile
    {
        [YamlMember(Alias = "store-selected")]
        public bool? StoreSelected { get; set; }

        [YamlMember(Alias = "store-fake-ip")]
        public bool? StoreFakeIp { get; set; }
    }

    internal class ClashDns
    {
        [YamlMember(Alias = "enable")]
        public bool? Enable { get; set; }

        [YamlMember(Alias = "listen")]
        public string? Listen { get; set; }

        [YamlMember(Alias = "ipv6")]
        public bool? Ipv6 { get; set; }

        [YamlMember(Alias = "default-nameserver")]
        public List<string>? DefaultNameservers { get; set; }

        [YamlMember(Alias = "enhanced-mode")]
        public string? EnhancedMode { get; set; }

        [YamlMember(Alias = "fake-ip-range")]
        public string? FakeIpRange { get; set; }

        [YamlMember(Alias = "use-hosts")]
        public bool? UseHosts { get; set; }

        [YamlMember(Alias = "search-domains")]
        public List<string>? SearchDomains { get; set; }

        [YamlMember(Alias = "fake-ip-filter")]
        public List<string>? FakeIpFilter { get; set; }

        [YamlMember(Alias = "nameserver")]
        public List<string>? NameServer { get; set; }

        [YamlMember(Alias = "fallback")]
        public List<string>? Fallback { get; set; }

        [YamlMember(Alias = "fallback-filter")]
        public ClashDnsFallbackFilter? FallbackFilter { get; set; }
    }

    internal class ClashDnsFallbackFilter
    {
        [YamlMember(Alias = "geoip")]
        public bool? Geoip { get; set; }

        [YamlMember(Alias = "geoip-code")]
        public string? GeoipCode { get; set; }

        [YamlMember(Alias = "ipcidr")]
        public List<string>? Ipcidr { get; set; }

        [YamlMember(Alias = "domain")]
        public List<string>? Domain { get; set; }
    }

    internal class ClashProxyProvider
    {
        [YamlMember(Alias = "type")]
        public string? Type { get; set; }

        [YamlMember(Alias = "url")]
        public string? Url { get; set; }

        [YamlMember(Alias = "interval")]
        public int? Interval { get; set; }

        [YamlMember(Alias = "path")]
        public string? Path { get; set; }

        [YamlMember(Alias = "health-check")]
        public ClashProxyProviderHealthCheck? HealthCheck { get; set; }
    }

    internal class ClashProxyProviderHealthCheck
    {
        [YamlMember(Alias = "enable")]
        public bool? Enable { get; set; }

        [YamlMember(Alias = "lazy")]
        public bool? Lazy { get; set; }

        [YamlMember(Alias = "interval")]
        public int? Interval { get; set; }

        [YamlMember(Alias = "url")]
        public string? Url { get; set; }
    }

    internal class ClashTunnel
    {
        [YamlMember(Alias = "network")]
        public string? Network { get; set; }

        [YamlMember(Alias = "address")]
        public string? Address { get; set; }

        [YamlMember(Alias = "target")]
        public string? Target { get; set; }

        [YamlMember(Alias = "proxy")]
        public string? Proxy { get; set; }
    }
}
