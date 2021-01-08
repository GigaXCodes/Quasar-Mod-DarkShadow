﻿using System.Net;

namespace Quasar.Common.DNS
{
    public class Host
    {
        /// <summary>
        /// Stores the hostname of the Host.
        /// </summary>
        /// <remarks>
        /// Can be an IPv4, IPv6 address or hostname.
        /// </remarks>
        public string Hostname { get; set; }

        /// <summary>
        /// Stores the IP address of host. 
        /// </summary>
        /// <remarks>
        /// Can be an IPv4 or IPv6 address.
        /// </remarks>
        public IPAddress IPv4Address { get; set; }

        /// <summary>
        /// Stores the IP address of host. 
        /// </summary>
        /// <remarks>
        /// Can be an IPv4 or IPv6 address.
        /// </remarks>
        public IPAddress IPv6Address { get; set; }

        /// <summary>
        /// Stores the port of the Host.
        /// </summary>
        public ushort Port { get; set; }

        public override string ToString()
        {
            return Hostname + ":" + Port;
        }
    }
}
