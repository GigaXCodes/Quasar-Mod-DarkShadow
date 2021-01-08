using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Quasar.Common.DNS
{
    public class HostsManager
    {
        public bool IsEmpty => _hosts.Count == 0;

        private readonly Queue<Host> _hosts = new Queue<Host>();

        public HostsManager(List<Host> hosts)
        {
            foreach(var host in hosts)
                _hosts.Enqueue(host);
        }

        public Host GetNextHost()
        {
            var temp = _hosts.Dequeue();
            _hosts.Enqueue(temp); // add to the end of the queue

            temp.IPv4Address = ResolveHostname(temp);
            temp.IPv6Address = ResolveHostname(temp, true);

            return temp;
        }

        private static IPAddress ResolveHostname(Host host, bool IPv6 = false)
        {
            if (string.IsNullOrEmpty(host.Hostname)) return null;

            IPAddress ip;
            if (IPAddress.TryParse(host.Hostname, out ip))
            {
                if (ip.AddressFamily == AddressFamily.InterNetworkV6 && !Socket.OSSupportsIPv6)
                {
                    return null;
                }
                return ip;
            }

            //TODO: Custom DNS Resolver as Backup if blocked by ISP
            var ipAddresses = Dns.GetHostAddresses(host.Hostname);
            foreach (IPAddress ipAddress in ipAddresses)
            {
                switch (ipAddress.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        if (!IPv6)
                            return ipAddress;
                        break;
                    case AddressFamily.InterNetworkV6:
                        if (IPv6)
                            return ipAddress;
                        break;
                }
            }

            return ip;
        }
    }
}
