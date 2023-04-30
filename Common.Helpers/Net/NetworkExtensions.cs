﻿using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Collections.Generic;

namespace Common.Helpers.Net
{
    public class NetworkExtensions
    {
        public static IEnumerable<string> GetAllLocalIPv4()
        {
            var types = new List<NetworkInterfaceType>() { NetworkInterfaceType.Ethernet, NetworkInterfaceType.Wireless80211 };
            return GetLocalIPv4Addresses(types);
        }

        private static IEnumerable<string> GetLocalIPv4Addresses(IEnumerable<NetworkInterfaceType> types)
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(x => x.OperationalStatus == OperationalStatus.Up && types.Contains(x.NetworkInterfaceType))
                .SelectMany(x => x.GetIPProperties().UnicastAddresses)
                .Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(x => x.Address.ToString());
        }
    }
}
